namespace Server
{
    using System;
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
            _storage = new InternalStorage(_configSetting.DbServerName);
            _clientService = new ClientService(_storage);
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
            var connectionResponse = new ConnectionResponse
            {
                Result = ResultCodes.Ok,
                ConnectedClients = _clientService.ConnectedClients
            };

            if (_clientService.ClientIsConnected(args.ClientName))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{args.ClientName}' is already connected.";
                connectionResponse.ConnectedClients = null;
            }

            args.Send(sender, connectionResponse.GetContainer());

            if (connectionResponse.Result == ResultCodes.Failure)
            {
                return;
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
            args.SendBroadcast(sender, new ConnectionStateChangedEcho(args.ClientName, false).GetContainer());
        }

        private void HandleMessageRequestReceived(object sender, MessageRequestReceivedEventArgs args)
        {
            var messageBroadcast = new MessageBroadcast(args.Message, args.SenderName);
            args.SendBroadcast(sender, messageBroadcast.GetContainer());
        }

        private void HandleEventLogsRequestReceived(object sender, EventLogsRequestReceivedEventArgs args)
        {
            DataTable eventLogs = _storage.EventLogContext.GetEventLogs();
            args.Send(sender, new EventLogsResponse(eventLogs).GetContainer());
        }

        #endregion
    }
}
