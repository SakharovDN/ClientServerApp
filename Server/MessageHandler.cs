namespace Server
{
    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Services;

    using WebSocket;

    public class MessageHandler
    {
        #region Fields

        private readonly WsConnection _connection;
        private readonly MessageService _messageService;
        private readonly ClientService _clientService;
        private readonly EventLogService _eventLogService;

        #endregion

        #region Constructors

        public MessageHandler(WsConnection connection, MessageService messageService, ClientService clientService, EventLogService eventLogService)
        {
            _connection = connection;
            _connection.MessageContainerReceived += HandleMessageContainer;
            _messageService = messageService;
            _clientService = clientService;
            _eventLogService = eventLogService;
        }

        #endregion

        #region Methods

        public void Send(MessageContainer container)
        {
            _connection.Send(container);
        }

        public void SendBroadcast(MessageContainer container)
        {
            _connection.Broadcast(container);
        }

        private void HandleMessageContainer(object sender, MessageContainerReceivedEventArgs e)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(e.MessageContainer);

            if (container == null)
            {
                return;
            }

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
        }

        private void HandleDisconnectionRequest(MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(DisconnectionRequest)) is DisconnectionRequest disconnectionRequest))
            {
                return;
            }

            _clientService.EnqueueDisconnectionRequest(new Request(disconnectionRequest, this));
        }

        private void HandleConnectionRequest(MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) is ConnectionRequest connectionRequest))
            {
                return;
            }

            _clientService.EnqueueConnectionRequest(new Request(connectionRequest, this));
        }

        private void HandleMessageRequest(MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(MessageRequest)) is MessageRequest messageRequest))
            {
                return;
            }

            _messageService.EnqueueMessageRequest(new Request(messageRequest, this));
        }

        private void HandleEventLogsRequest()
        {
            _eventLogService.ConstructEventLogsResponse();
        }

        #endregion
    }
}
