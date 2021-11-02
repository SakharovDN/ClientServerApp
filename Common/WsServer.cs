namespace Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Messages;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;

        private WebSocketServer _server;

        #endregion

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            MessageHandler.MessageReceived += HandleMessageReceived;
            MessageHandler.ConnectionStateChanged += ConnectionStateChanged;
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
        }

        #endregion

        #region Methods

        public void Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
            _server.AddWebSocketService<WsConnection>(
                "/",
                connection =>
                {
                    connection.AddServer(this);
                });
            _server.Start();
        }

        public void Stop()
        {
            _server?.Stop();
            _server = null;
            WsConnection[] connections = _connections.Select(item => item.Value).ToArray();

            foreach (WsConnection connection in connections)
            {
                connection.Close();
            }

            _connections.Clear();
        }

        public void Send(string message)
        {
            MessageContainer messageBroadcast = new MessageBroadcast(message).GetContainer();

            foreach (KeyValuePair<Guid, WsConnection> connection in _connections)
            {
                connection.Value.Send(messageBroadcast);
            }
        }

        internal void AddConnection(WsConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
        }

        internal void FreeConnection(Guid connectionId)
        {
            _connections.TryRemove(connectionId, out WsConnection _);
        }

        private void ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.Connected ? "подключён" : "отключён";
            string message = $"Клиент {e.ClientName} {clientState}";
            Send(message);
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = $"{e.ClientName}: {e.Message}";
            Send(message);
        }

        #endregion
    }
}
