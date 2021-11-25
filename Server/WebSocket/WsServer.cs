namespace Server.WebSocket
{
    using System.Net;

    using Services;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;
        private readonly ConfigSettings _configSettings;
        private readonly MessageService _messageService;
        private readonly ClientService _clientService;
        private readonly EventLogService _eventLogService;

        #endregion

        #region Constructors

        public WsServer(ConfigSettings configSettings)
        {
            _configSettings = configSettings;
            _listenAddress = new IPEndPoint(IPAddress.Any, _configSettings.Port);
            _messageService = new MessageService();
            _clientService = new ClientService(configSettings.DbConnection);
            _eventLogService = new EventLogService(configSettings.DbConnection);
        }

        #endregion

        #region Methods

        public void Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
            _server.AddWebSocketService<WsConnection>(
                "/Connection",
                connection =>
                {
                    connection.SetSettings(_configSettings.InactivityTimeoutInterval, _messageService, _clientService, _eventLogService);
                });
            _server.Start();
        }

        public void Stop()
        {
            _server?.Stop();
            _messageService.Stop();
            _clientService.Stop();
            _eventLogService.Stop();
            _server = null;
        }

        #endregion
    }
}
