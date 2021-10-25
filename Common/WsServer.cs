namespace Common
{
    using System.Net;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private WebSocketServer _server;
        private readonly IPEndPoint _listenAddress;

        #endregion

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            _listenAddress = listenAddress;
        }

        #endregion

        #region Methods

        public void Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
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
