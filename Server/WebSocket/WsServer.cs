namespace Server.WebSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Settings;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;
        private readonly ConcurrentDictionary<string, WsConnection> _connections;
        private readonly ConfigSettings _configSettings;

        #endregion

        #region Events

        public event EventHandler<ConnectionRequestReceivedEventArgs> ConnectionRequestReceived;

        public event EventHandler<DisconnectionRequestReceivedEventArgs> DisconnectionRequestReceived;

        public event EventHandler<MessageRequestReceivedEventArgs> MessageRequestReceived;

        public event EventHandler<EventLogsRequestReceivedEventArgs> EventLogsRequestReceived;

        public event EventHandler<ChatHistoryRequestReceivedEventArgs> ChatHistoryRequestReceived;

        #endregion

        #region Constructors

        public WsServer(ConfigSettings configSettings)
        {
            _configSettings = configSettings;
            _listenAddress = new IPEndPoint(IPAddress.Any, _configSettings.Port);
            _connections = new ConcurrentDictionary<string, WsConnection>();
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
                    connection.AddServer(this);
                    connection.SetInactivityTimeout(_configSettings.InactivityTimeoutInterval);
                    connection.RequestReceived += HandleRequest;
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

        internal void AddConnection(WsConnection connection)
        {
            _connections.TryAdd(connection.ID, connection);
        }

        internal void FreeConnection(string connectionId)
        {
            _connections.TryRemove(connectionId, out WsConnection _);
        }

        private void HandleRequest(object sender, RequestReceivedEventArgs args)
        {
            if (!_connections.TryGetValue(args.ConnectionId, out WsConnection connection))
            {
                return;
            }

            var container = JsonConvert.DeserializeObject<MessageContainer>(args.Request);

            if (container == null)
            {
                return;
            }

            switch (container.Type)
            {
                case MessageTypes.ConnectionRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) is ConnectionRequest connectionRequest))
                    {
                        return;
                    }

                    ConnectionRequestReceived?.Invoke(
                        sender,
                        new ConnectionRequestReceivedEventArgs(connectionRequest.ClientName, Send, SendBroadcast));
                    break;

                case MessageTypes.DisconnectionRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(DisconnectionRequest)) is DisconnectionRequest disconnectionRequest))
                    {
                        return;
                    }

                    DisconnectionRequestReceived?.Invoke(
                        sender,
                        new DisconnectionRequestReceivedEventArgs(disconnectionRequest.ClientName, Send, SendBroadcast));
                    break;

                case MessageTypes.MessageRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(MessageRequest)) is MessageRequest messageRequest))
                    {
                        return;
                    }

                    MessageRequestReceived?.Invoke(
                        sender,
                        new MessageRequestReceivedEventArgs(
                            messageRequest.Body,
                            messageRequest.Source,
                            messageRequest.Target,
                            Send,
                            SendTo,
                            SendBroadcast));
                    break;

                case MessageTypes.EventLogsRequest:
                    EventLogsRequestReceived?.Invoke(sender, new EventLogsRequestReceivedEventArgs(Send));
                    break;

                case MessageTypes.ChatHistoryRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(ChatHistoryRequest)) is ChatHistoryRequest chatHistoryRequest))
                    {
                        return;
                    }

                    ChatHistoryRequestReceived?.Invoke(sender, new ChatHistoryRequestReceivedEventArgs(chatHistoryRequest.Participants, Send));
                    break;
            }
        }

        private void Send(object sender, MessageContainer container)
        {
            var connection = sender as WsConnection;
            connection?.Send(container);
        }

        private void SendTo(object sender, MessageContainer container, int targetId)
        {
            var connection = sender as WsConnection;
            string targetConnectionId =
                (from connectionItem in _connections.Values where connectionItem.ClientId == targetId select connectionItem.ID).FirstOrDefault();
            connection?.SendTo(container, targetConnectionId);
        }

        private void SendBroadcast(object sender, MessageContainer container)
        {
            var connection = sender as WsConnection;
            connection?.Broadcast(container);
        }

        #endregion
    }
}
