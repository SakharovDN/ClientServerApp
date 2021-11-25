namespace Server.WebSocket
{
    using System.Net;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;
        private readonly ConfigSettings _configSettings;

        #endregion

        #region Constructors

        public WsServer(ConfigSettings configSettings)
        {
            _configSettings = configSettings;
            _listenAddress = new IPEndPoint(IPAddress.Any, _configSettings.Port);
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
                    connection.SetConfigSettings(_configSettings);
                });
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
