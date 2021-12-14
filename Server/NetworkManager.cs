namespace Server
{
    using System;
    using System.Collections.Generic;

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
            _storage = new InternalStorage(GetDbConnectionString(_configSetting.DbServerName));
            _chatService = new ChatService(_storage);
            _groupService = new GroupService(_storage);
            _clientService = new ClientService(_storage);
            _messageService = new MessageService(_storage);
            _eventLogService = new EventLogService(_storage);
            _logger = LogManager.GetCurrentClassLogger();
            _wsServer.ConnectionRequestReceived += _clientService.HandleConnectionRequest;
            _clientService.ConnectionRequestHandled += SendConnectionResponse;
            _wsServer.ConnectionClosed += HandleConnectionClosed;
            _wsServer.MessageRequestReceived += _messageService.HandleMessageRequest;
            _messageService.MessageRequestHandled += SendBroadcast;
            _chatService.NewChatCreated += SendBroadcast;
            _wsServer.ChatHistoryRequestReceived += _chatService.HandleChatHistoryRequest;
            _chatService.ChatHistoryRequestHandled += SendChatHistoryResponse;
            _wsServer.GroupCreationRequestReceived += _groupService.HandleGroupCreationRequest;
            _wsServer.EventLogsRequestReceived += SendEventLogsResponse;
            _messageService.ChatNotExists += _chatService.CreateNewChat;
            _groupService.ChatNotExists += _chatService.CreateNewChat;
            _messageService.MessageAddedToDb += _chatService.UpdateChatRecord;
            _wsServer.GroupListRequestReceived += _groupService.HandleGroupListRequest;
            _groupService.GroupListRequestHandled += Send;
            _wsServer.ChatListRequestReceived += _chatService.HandleChatListRequest;
            _chatService.ChatListRequestHandled += Send;
        }

        #endregion

        #region Methods

        public void Start()
        {
            try
            {
                _wsServer.Start();
                _logger.Info("Server started successfully");
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.Source);
            }
        }

        private string GetDbConnectionString(string dbServerName)
        {
            return $"Data Source={dbServerName};Initial Catalog=ClientServerApp;Integrated Security=True;MultipleActiveResultSets=True";
        }

        private void HandleConnectionClosed(object sender, EventArgs args)
        {
            var connection = sender as WsConnection;
            Client client = _clientService.GetClientById(connection?.ClientId);

            if (client == null)
            {
                return;
            }

            if (!_clientService.ClientIsConnected(client.Id.ToString()))
            {
                return;
            }

            _clientService.SetClientDisconnected(client);
            _wsServer.SendBroadcast(new ConnectionStateChangedBroadcast(client, false).GetContainer());
        }

        private void SendConnectionResponse(object sender, ConnectionRequestHandledEventArgs args)
        {
            if (args.ConnectionResponse.Result == ResultCodes.Ok)
            {
                if (sender is WsConnection connection)
                {
                    connection.ClientId = args.Client.Id.ToString();
                }

                args.ConnectionResponse.KeepAliveInterval = _configSetting.InactivityTimeoutInterval / 2;
                _wsServer.SendBroadcast(new ConnectionStateChangedBroadcast(args.Client, true).GetContainer());
            }

            _wsServer.Send(sender, args.ConnectionResponse.GetContainer());
        }

        private void SendChatHistoryResponse(object sender, ChatHistoryRequestHandledEventArgs args)
        {
            _wsServer.Send(sender, args.ChatHistoryResponse);
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
                    List<string> clientIds = _groupService.GetClientIds(args.Chat.TargetId);

                    if (clientIds != null)
                    {
                        foreach (string clientId in clientIds)
                        {
                            _wsServer.SendTo(sender, args.Response, clientId);
                        }
                    }

                    break;
            }
        }

        private void SendEventLogsResponse(object sender, EventArgs args)
        {
            _wsServer.Send(sender, new EventLogsResponse(_eventLogService.GetEventLogs()).GetContainer());
        }

        #endregion
    }
}
