namespace Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Timers;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Storage.Client;
    using Storage.EventLog;

    using WebSocket;

    public class MessageHandler
    {
        #region Fields

        private readonly ClientContext _clientContext;
        private readonly EventLogContext _eventLogContext;
        private readonly WsConnection _connection;
        private readonly ConcurrentQueue<MessageContainer> _handlingQueue;
        private readonly Timer _handlingTimer;
        private bool _messageIsHandling;

        #endregion

        #region Constructors

        public MessageHandler(WsConnection connection, string dbConnection)
        {
            _clientContext = new ClientContext(dbConnection);
            _eventLogContext = new EventLogContext(dbConnection);
            _connection = connection;
            _connection.ConnectionClosed += StopHandlingTimer;
            _connection.MessageContainerReceived += EnqueueMessageContainer;
            _handlingQueue = new ConcurrentQueue<MessageContainer>();
            _messageIsHandling = false;
            _handlingTimer = new Timer(100);
            _handlingTimer.Elapsed += HandleMessageContainer;
            _handlingTimer.Start();
        }

        #endregion

        #region Methods

        private void StopHandlingTimer(object sender, EventArgs e)
        {
            _handlingTimer.Stop();
        }

        private void EnqueueMessageContainer(object sender, MessageContainerReceivedEventArgs e)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(e.MessageContainer);

            if (container == null)
            {
                return;
            }

            _handlingQueue.Enqueue(container);
        }

        private void HandleMessageContainer(object sender, ElapsedEventArgs e)
        {
            if (!_handlingQueue.TryDequeue(out MessageContainer container) || _messageIsHandling)
            {
                return;
            }

            _messageIsHandling = true;

            switch (container.Type)
            {
                case MessageTypes.ConnectionRequest:
                    HandleConnectionRequest(container);
                    break;

                case MessageTypes.DisconnectionRequest:
                    HandleDisconnectionRequest(container);
                    break;

                case MessageTypes.MessageRequest:
                    HandleMessageRequest(container);
                    break;

                case MessageTypes.EventLogsRequest:
                    HandleEventLogsRequest();
                    break;
            }

            _messageIsHandling = false;
        }

        private void HandleMessageRequest(MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(MessageRequest)) is MessageRequest messageRequest))
            {
                return;
            }

            MessageContainer messageBroadcast = new MessageBroadcast(messageRequest.Message, messageRequest.SenderName).GetContainer();
            _connection.Broadcast(messageBroadcast);
        }

        private void HandleEventLogsRequest()
        {
            DataTable eventLogs = _eventLogContext.GetEventLogs();
            _connection.Send(new EventLogsResponse(eventLogs).GetContainer());
        }

        private void HandleDisconnectionRequest(MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(DisconnectionRequest)) is DisconnectionRequest disconnectionRequest))
            {
                return;
            }

            if (!_clientContext.ClientIsConnected(disconnectionRequest.ClientName))
            {
                return;
            }

            _clientContext.ChangeConnectionStatus(disconnectionRequest.ClientName);
            _eventLogContext.AddEventLogToDt($"Client {disconnectionRequest.ClientName} is disconnected");
            _connection.Broadcast(new ConnectionStateChangedEcho(disconnectionRequest.ClientName, false).GetContainer());
            _connection.Send(new DisconnectionResponse().GetContainer());
        }

        private void HandleConnectionRequest(MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) is ConnectionRequest connectionRequest))
            {
                return;
            }

            var connectionResponse = new ConnectionResponse
            {
                Result = ResultCodes.Ok
            };

            if (_clientContext.ClientIsConnected(connectionRequest.ClientName))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{connectionRequest.ClientName}' is already connected.";
                connectionResponse.ConnectedClients = null;
            }

            if (connectionResponse.Result == ResultCodes.Ok)
            {
                connectionResponse.ConnectedClients = _clientContext.GetConnectedClients();

                if (!_clientContext.ClientExists(connectionRequest.ClientName))
                {
                    _clientContext.AddNewClientToDt(connectionRequest.ClientName);
                }
                else
                {
                    _clientContext.ChangeConnectionStatus(connectionRequest.ClientName);
                }

                _eventLogContext.AddEventLogToDt($"Client {connectionRequest.ClientName} is connected");
                _connection.Broadcast(new ConnectionStateChangedEcho(connectionRequest.ClientName, true).GetContainer());
            }

            _connection.Send(connectionResponse.GetContainer());
        }

        #endregion
    }
}
