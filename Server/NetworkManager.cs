namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Common;
    using Common.Messages;

    using NLog;

    using Services;

    using Storage;

    using WebSocket;

    public class NetworkManager
    {
        #region Fields

        private readonly WsServer _wsServer;
        private readonly InternalStorage _storage;
        private readonly ClientService _clientService;
        private readonly MessageService _messageService;
        private readonly ConfigSettings _configSetting;
        private readonly Logger _logger;

        #endregion

        #region Constructors

        public NetworkManager()
        {
            _configSetting = ConfigSettings.ReadConfigFile();
            _wsServer = new WsServer(_configSetting);
            _wsServer.ConnectionRequestReceived += HandleConnectionRequestReceived;
            _wsServer.DisconnectionRequestReceived += HandleDisconnectionRequestReceived;
            _wsServer.MessageRequestReceived += HandleMessageRequestReceived;
            _wsServer.EventLogsRequestReceived += HandleEventLogsRequestReceived;
            _wsServer.ChatHistoryRequestReceived += HandleChatHistoryRequestReceived;
            _storage = new InternalStorage(_configSetting.DbServerName);
            _clientService = new ClientService(_storage);
            _messageService = new MessageService(_storage);
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

        private void HandleConnectionRequestReceived(object sender, ConnectionRequestReceivedEventArgs args)
        {
            var connectionResponse = new ConnectionResponse();

            if (_clientService.ClientIsConnected(args.ClientName))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{args.ClientName}' is already connected.";
            }
            else
            {
                connectionResponse.Result = ResultCodes.Ok;
                connectionResponse.ConnectedClients = _clientService.ConnectedClients;
                connectionResponse.ClientId = _storage.ClientContext.GetClientId(args.ClientName);
            }

            args.Send(sender, connectionResponse.GetContainer());

            if (connectionResponse.Result == ResultCodes.Failure)
            {
                return;
            }

            if (sender is WsConnection connection)
            {
                connection.ClientId = connectionResponse.ClientId;
            }

            _clientService.AddClient(args.ClientName);
            args.SendBroadcast(sender, new ConnectionStateChangedEcho(args.ClientName, true).GetContainer());
        }

        private void HandleDisconnectionRequestReceived(object sender, DisconnectionRequestReceivedEventArgs args)
        {
            if (!_clientService.ClientIsConnected(args.ClientName))
            {
                return;
            }

            _clientService.RemoveClient(args.ClientName);
            args.Send(sender, new DisconnectionResponse().GetContainer());
            args.SendBroadcast(sender, new ConnectionStateChangedEcho(args.ClientName, false).GetContainer());
        }

        private void HandleMessageRequestReceived(object sender, MessageRequestReceivedEventArgs args)
        {
            var message = new Message
            {
                Body = args.Body,
                Source = args.Source,
                Target = args.Target,
                Timestamp = DateTime.Now
            };
            _messageService.AddNewMessage(message);
            var messageBroadcast = new MessageBroadcast(message);

            if (args.Target == "Common")
            {
                args.SendBroadcast(sender, messageBroadcast.GetContainer());
            }
            else
            {
                args.Send(sender, messageBroadcast.GetContainer());
                int targetId = _storage.ClientContext.GetClientId(args.Target);
                args.SendTo(sender, messageBroadcast.GetContainer(), targetId);
            }
        }

        private void HandleEventLogsRequestReceived(object sender, EventLogsRequestReceivedEventArgs args)
        {
            DataTable eventLogs = _storage.EventLogContext.GetEventLogs();
            args.Send(sender, new EventLogsResponse(eventLogs).GetContainer());
        }

        private void HandleChatHistoryRequestReceived(object sender, ChatHistoryRequestReceivedEventArgs args)
        {
            List<Message> chatHistory = args.Participants.Contains("Common")
                                            ? _storage.MessageContext.GetCommonChatHistory()
                                            : _storage.MessageContext.GetChatHistory(args.Participants);
            args.Send(sender, new ChatHistoryResponse(chatHistory).GetContainer());
        }

        #endregion
    }
}
