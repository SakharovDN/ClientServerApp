namespace Server.WebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Common;
    using Common.EventLog;

    using Services;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields
        
        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;

        #endregion

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            ClientService.Clients = new Dictionary<Guid, WsClient>();
            _listenAddress = listenAddress;
        }

        #endregion

        #region Methods

        public void Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
            _server.AddWebSocketService<WsConnection>("/Connection");
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
