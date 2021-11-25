namespace Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Timers;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MessageHandler
    {
        #region Fields

        private readonly WsClient _client;
        private readonly ConcurrentQueue<MessageContainer> _handlingQueue;
        private readonly Timer _handlingTimer;
        private bool _messageIsHandling;

        #endregion

        #region Events

        public event EventHandler<EventLogsReceivedEventArgs> EventLogsReceived;

        public event EventHandler<ConnectionResponseReceivedEventArgs> ConnectionResponseReceived;

        public event EventHandler<DisconnectionResponseReceivedEventArgs> DisconnectionResponseReceived;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<ConnectionStateChangedEchoReceivedEventArgs> ConnectionStateChangedEchoReceived;

        #endregion

        #region Constructors

        public MessageHandler(WsClient client)
        {
            _client = client;
            _client.ClientClosed += StopHandlingTimer;
            _client.MessageContainerReceived += EnqueueMessageContainer;
            _handlingQueue = new ConcurrentQueue<MessageContainer>();
            _messageIsHandling = false;
            _handlingTimer = new Timer(100);
            _handlingTimer.Elapsed += HandleMessageContainer;
            _handlingTimer.Start();
        }

        #endregion

        #region Methods

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
                case MessageTypes.ConnectionResponse:
                    HandleConnectionResponse(container);
                    break;

                case MessageTypes.DisconnectionResponse:
                    HandleDisconnectionResponse(container);
                    break;

                case MessageTypes.MessageBroadcast:
                    HandleMessageBroadcast(container);
                    break;

                case MessageTypes.EventLogsResponse:
                    HandleEventLogsResponse(container);
                    break;

                case MessageTypes.ConnectionStateChangedEcho:
                    HandleConnectionStateChangedEcho(container);
                    break;
            }

            _messageIsHandling = false;
        }

        private void StopHandlingTimer(object sender, EventArgs e)
        {
            _handlingTimer.Stop();
        }

        private void HandleDisconnectionResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(DisconnectionResponse)) is DisconnectionResponse disconnectionResponse)
            {
                DisconnectionResponseReceived?.Invoke(null, new DisconnectionResponseReceivedEventArgs());
            }
        }

        private void HandleConnectionStateChangedEcho(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionStateChangedEcho)) is ConnectionStateChangedEcho connectionStateChangedEcho)
            {
                ConnectionStateChangedEchoReceived?.Invoke(
                    null,
                    new ConnectionStateChangedEchoReceivedEventArgs(connectionStateChangedEcho.ClientName, connectionStateChangedEcho.IsConnected));
            }
        }

        private void HandleEventLogsResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(EventLogsResponse)) is EventLogsResponse eventLogsResponse)
            {
                EventLogsReceived?.Invoke(null, new EventLogsReceivedEventArgs(eventLogsResponse.EventLogs));
            }
        }

        private void HandleMessageBroadcast(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) is MessageBroadcast messageBroadcast)
            {
                MessageReceived?.Invoke(null, new MessageReceivedEventArgs(messageBroadcast.SenderName, messageBroadcast.Message));
            }
        }

        private void HandleConnectionResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) is ConnectionResponse connectionResponse)
            {
                ConnectionResponseReceived?.Invoke(
                    null,
                    new ConnectionResponseReceivedEventArgs(
                        connectionResponse.Result,
                        connectionResponse.Reason,
                        connectionResponse.ConnectedClients));
            }
        }

        #endregion
    }
}
