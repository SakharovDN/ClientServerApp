namespace Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using _EventArgs_;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private WebSocketServer _server;
        private static ConcurrentDictionary<Guid, WsConnection> _connections;
        private static List<WsClient> _clients;

        #endregion

        #region Constructors

        public WsServer(IPEndPoint listenAddress)
        {
            _server = new WebSocketServer(listenAddress.Address, listenAddress.Port, false);
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
            _clients = new List<WsClient>();
            WsConnection.ConnectionStateChanged += HandleConnectionStateChanged;
            WsClient.ClientConnectionStateChanged += HandleClientConnectionStateChanged;
        }

        #endregion

        #region Methods

        public void Start()
        {
            _server.AddWebSocketService<WsConnection>("/");
            _server.Start();
        }

        public void Stop()
        {
            WsConnection[] connections = _connections.Select(item => item.Value).ToArray();

            foreach (WsConnection connection in connections)
            {
                connection.Close();
            }

            _server?.Stop();
            _server = null;

            _connections.Clear();
        }

        public static bool ClientExists(string clientName)
        {
            return _clients.Any(client => clientName == client.ClientName);
        }

        internal static void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
        {
            if (args.Connected)
            {
                _connections.TryAdd(args.Id, args.Connection);
            }
            else
            {
                _connections.TryRemove(args.Id, out WsConnection _);
            }
        }

        internal static void HandleClientConnectionStateChanged(object sender, ClientConnectionStateChangedEventArgs args)
        {
            if (args.Connected)
            {
                _clients.Add(args.Client);
            }
            else
            {
                _clients.Remove(args.Client);
            }

            string clientState = args.Connected ? "подключен" : "отключен";
            string message = $"Клиент '{args.ClientName}' {clientState}.";
            Console.WriteLine(message);
        }

        #endregion
    }
}
