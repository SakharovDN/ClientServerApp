namespace Server
{
    using System;

    using Common;
    using Common.Messages;

    using NLog;

    using Services;

    using Settings;

    using Storage;

    using WebSocket;

    public class NetworkManager
    {
        #region Fields

        private readonly WsServer _wsServer;
        private readonly InternalStorage _storage;
        private readonly IClientService _clientService;
        private readonly IMessageService _messageService;
        private readonly IChatService _chatService;
        private readonly IGroupService _groupService;
        private readonly EventLogService _eventLogService;
        private readonly ConfigSettings _configSetting;
        private readonly SettingsManager _settingsManager;
        private readonly Logger _logger;

        #endregion

        #region Constructors

        public NetworkManager()
        {
            _settingsManager = new SettingsManager();
            _configSetting = _settingsManager.GetConfigSettings();
            _wsServer = new WsServer(_configSetting);
            _storage = new InternalStorage(_settingsManager.GetDbConnectionString());
            _chatService = new ChatService(_storage);
            _groupService = new GroupService(_storage);
            _clientService = new ClientService(_storage);
            _messageService = new MessageService(_storage);
            _eventLogService = new EventLogService(_storage);
            _logger = LogManager.GetCurrentClassLogger();
            _wsServer.ConnectionRequestReceived += _clientService.HandleConnectionRequest;
            _wsServer.MessageRequestReceived += _messageService.HandleMessageRequest;
            _wsServer.ChatHistoryRequestReceived += _chatService.HandleChatHistoryRequest;
            _wsServer.GroupCreationRequestReceived += _groupService.HandleGroupCreationRequest;
            _wsServer.GroupLeavingRequestReceived += _groupService.HandleGroupLeavingRequest;
            _wsServer.EventLogsRequestReceived += _eventLogService.HandleEventLogsRequest;
            _wsServer.ChatListRequestReceived += _chatService.HandleChatListRequest;
            _wsServer.ChatInfoRequestReceived += _chatService.HandleChatInfoRequest;
            _messageService.ChatNotExists += _chatService.CreateNewChat;
            _groupService.ChatNotExists += _chatService.CreateNewChat;
            _messageService.MessageAddedToDb += _chatService.UpdateChatRecord;
            _clientService.ConnectionRequestHandled += Send;
            _chatService.ChatHistoryRequestHandled += Send;
            _eventLogService.EventLogRequestHandled += Send;
            _chatService.ChatListRequestHandled += Send;
            _groupService.GroupCreationRequestHandled += Send;
            _chatService.ChatInfoRequestHandled += Send;
            _groupService.GroupLeavingRequestHandled += Send;
            _messageService.MessageRequestHandled += SendBroadcast;
            _chatService.NewChatCreated += SendBroadcast;
            _clientService.ClientConnected += SendConnectionStateChangedBroadcast;
            _wsServer.ConnectionClosed += SendConnectionStateChangedBroadcast;
            _wsServer.ConnectionClosed += _clientService.SetClientDisconnected;
        }

        #endregion

        #region Methods

        public void Start()
        {
            try
            {
                _wsServer.Start();
                _logger.Info("Server started successfully");
                Console.WriteLine("Server started successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.Source);
            }
        }

        public void Stop()
        {
            try
            {
                _wsServer.Stop();
                _logger.Info("Server stopped successfully");
                Console.WriteLine("Server stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.Source);
            }
        }

        private void SendConnectionStateChangedBroadcast(object sender, ConnectionStateChangedEventArgs args)
        {
            if (!(sender is WsConnection connection))
            {
                return;
            }

            if (string.IsNullOrEmpty(connection.ClientId))
            {
                return;
            }

            Client client = _clientService.GetClientById(connection.ClientId);

            if (client == null)
            {
                return;
            }

            _wsServer.SendBroadcast(new ConnectionStateChangedBroadcast(client, args.IsConnected).GetContainer());
        }

        private void Send(object sender, RequestHandledEventArgs args)
        {
            _wsServer.Send(sender, args.Response);
        }

        private void SendBroadcast(object sender, RequestHandledEventArgs args)
        {
            switch (args.Chat.Type)
            {
                case ChatTypes.Common:
                    _wsServer.SendBroadcast(args.Response);
                    break;
                case ChatTypes.Private:
                    _wsServer.SendTo(sender, args.Response, args.Chat.TargetId);
                    _wsServer.SendTo(sender, args.Response, args.Chat.SourceId);
                    break;
                    case ChatTypes.Group:
                    foreach (string clientId in args.ClientIds)
                    {
                        _wsServer.SendTo(sender, args.Response, clientId);
                    }

                    break;
            }
        }

        #endregion
    }
}
