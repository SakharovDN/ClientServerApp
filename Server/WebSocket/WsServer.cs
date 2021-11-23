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
        private readonly int _inactivityTimeoutInterval;
        private ClientService _clientService;
        private EventLogService _eventLogService;

        #endregion

        #region Constructors

        public WsServer(ConfigSettings configSettings)
        {
            _listenAddress = new IPEndPoint(IPAddress.Any, configSettings.Port);
            _inactivityTimeoutInterval = configSettings.InactivityTimeoutInterval;
            _clientService = new ClientService();
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
                    connection.SetInactivityTimeoutInterval(_inactivityTimeoutInterval);
                });
            _server.AddWebSocketService<WsChat>("/CommonChat");
            _server.Start();
        }

        public void Stop()
        {
            _server?.Stop();
            _server = null;
        }

        #endregion
    }
}
