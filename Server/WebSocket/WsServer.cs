namespace Server.WebSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Common;
    using Common.EventLog;
    using Common.Messages;

    using Services;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        public readonly ClientService ClientService;
        private readonly EventLogContext _eventLogContext;
        private readonly IPEndPoint _listenAddress;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;
        private WebSocketServer _server;

        #endregion

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            MessageService.MessageReceived += HandleMessageReceived;
            MessageService.ConnectionStateChanged += HandleConnectionStateChanged;
            ClientService = new ClientService();
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
            _eventLogContext = new EventLogContext();
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

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.Connected ? "подключён" : "отключён";
            string message = $"Клиент {e.Client.Name} {clientState}";

            if (e.Connected)
            {
                ClientService.Add(e.Client);
            }
            else
            {
                ClientService.Remove(e.Client);
            }

            Send(message);
            SendClientList();
            _eventLogContext.ConnectionEventLog(message);
        }

        private void SendClientList()
        {
            MessageContainer clientsListResponse = new ClientsListResponse(ClientService.Clients).GetContainer();

            foreach (KeyValuePair<Guid, WsConnection> connection in _connections)
            {
                connection.Value.Send(clientsListResponse);
            }
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = $"{e.ClientName}: {e.Message}";
            Send(message);
        }

        #endregion
    }
}
