namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Data;

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
        private readonly ClientService _clientService;
        private readonly MessageService _messageService;
        private readonly ChatService _chatService;
        private readonly ConfigSettings _configSetting;
        private readonly SettingsManager _settingsManager;
        private readonly Logger _logger;

        #endregion

        #region Constructors

        public NetworkManager()
        {
            _settingsManager = new SettingsManager();
            _configSetting = _settingsManager.ReadConfigFile();
            _wsServer = new WsServer(_configSetting);
            _wsServer.ConnectionRequestReceived += HandleConnectionRequestReceived;
            _wsServer.ConnectionClosed += HandleConnectionClosed;
            _wsServer.MessageRequestReceived += HandleMessageRequestReceived;
            _wsServer.EventLogsRequestReceived += HandleEventLogsRequestReceived;
            _wsServer.ChatHistoryRequestReceived += HandleChatHistoryRequestReceived;
            _storage = new InternalStorage(_configSetting.DbServerName);
            _clientService = new ClientService(_storage);
            _messageService = new MessageService(_storage);
            _chatService = new ChatService(_storage);
            _logger = LogManager.GetCurrentClassLogger();
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

        private void HandleConnectionClosed(object sender, ConnectionClosedEventArgs args)
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
            args.SendBroadcast(new ConnectionStateChangedBroadcast(client, false).GetContainer());
        }

        private void HandleConnectionRequestReceived(object sender, ConnectionRequestReceivedEventArgs args)
        {
            var client = new Client
            {
                Name = args.ClientName
            };

            if (!_storage.ClientContext.ClientExists(client.Name))
            {
                client.Id = Guid.NewGuid();
                _clientService.CreateNewClient(client);
            }
            else
            {
                client = _clientService.GetClientByName(client.Name);
            }

            var connectionResponse = new ConnectionResponse();

            if (_clientService.ClientIsConnected(client.Id.ToString()))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{client.Name}' is already connected.";
            }
            else
            {
                connectionResponse.Result = ResultCodes.Ok;
                connectionResponse.ClientId = client.Id.ToString();
                connectionResponse.AvailableChats = _chatService.GetAvailableChats(client.Id.ToString());
                connectionResponse.ConnectedClients = _clientService.ConnectedClients;
                connectionResponse.KeepAliveInterval = _configSetting.InactivityTimeoutInterval / 2;
            }

            args.Send(sender, connectionResponse.GetContainer());

            if (connectionResponse.Result == ResultCodes.Failure)
            {
                return;
            }

            if (sender is WsConnection connection)
            {
                connection.ClientId = client.Id.ToString();
            }

            _clientService.SetClientConnected(client);
            args.SendBroadcast(new ConnectionStateChangedBroadcast(client, true).GetContainer());
        }

        private void HandleChatHistoryRequestReceived(object sender, ChatHistoryRequestReceivedEventArgs args)
        {
            List<Message> chatHistory = _messageService.GetChatHistory(args.ChatId);
            args.Send(sender, new ChatHistoryResponse(chatHistory).GetContainer());
        }

        private void HandleMessageRequestReceived(object sender, MessageRequestReceivedEventArgs args)
        {
            DateTime timestamp = DateTime.Now;
            Chat chat = args.Chat;

            if (chat.Id == Guid.Empty)
            {
                chat = new Chat
                {
                    Id = Guid.NewGuid(),
                    Type = ChatTypes.Private,
                    SourceId = args.SourceId,
                    TargetId = args.Chat.TargetId,
                    LastMessageTimestamp = timestamp,
                    MessageAmount = 0
                };
                _chatService.CreateNewChat(chat);
                chat.TargetName = _clientService.GetClientById(chat.TargetId).Name;
                args.Send(sender, new ChatCreatedBroadcast(chat).GetContainer());
                chat.TargetName = _clientService.GetClientById(chat.SourceId).Name;
                args.SendTo(sender, new ChatCreatedBroadcast(chat).GetContainer(), chat.TargetId);
            }

            var message = new Message
            {
                Body = args.Body,
                ChatId = chat.Id.ToString(),
                SourceId = args.SourceId,
                SourceName = _clientService.GetClientById(args.SourceId).Name,
                Timestamp = timestamp
            };
            _messageService.AddNewMessage(message);
            _chatService.UpdateRecord(chat.Id.ToString(), timestamp);

            switch (chat.Type)
            {
                case ChatTypes.Common:
                {
                    var messageBroadcast = new MessageBroadcast(message);
                    args.SendBroadcast(messageBroadcast.GetContainer());
                    break;
                }

                case ChatTypes.Private:
                {
                    var messageBroadcast = new MessageBroadcast(message);
                    args.SendTo(sender, messageBroadcast.GetContainer(), chat.TargetId);
                    args.SendTo(sender, messageBroadcast.GetContainer(), chat.SourceId);
                    break;
                }
            }
        }

        private void HandleEventLogsRequestReceived(object sender, EventLogsRequestReceivedEventArgs args)
        {
            DataTable eventLogs = _storage.EventLogContext.GetEventLogs();
            args.Send(sender, new EventLogsResponse(eventLogs).GetContainer());
        }

        #endregion
    }
}
