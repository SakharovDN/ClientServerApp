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

    using Requests;

    using Settings;

    using WebSocketSharp.Server;

    public class WsServer
    {
        #region Fields

        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;
        private readonly ConcurrentDictionary<string, WsConnection> _connections;
        private readonly ConfigSettings _configSettings;
        private readonly RequestQueue _requestQueue;

        #endregion

        #region Events

        public event EventHandler<ConnectionRequestReceivedEventArgs> ConnectionRequestReceived;

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionClosed;

        public event EventHandler<MessageRequestReceivedEventArgs> MessageRequestReceived;

        public event EventHandler EventLogsRequestReceived;

        public event EventHandler<ChatHistoryRequestReceivedEventArgs> ChatHistoryRequestReceived;

        public event EventHandler<GroupCreationRequestReceivedEventArgs> GroupCreationRequestReceived;

        public event EventHandler<ChatListRequestReceivedEventArgs> ChatListRequestReceived;

        #endregion

        #region Constructors

        public WsServer(ConfigSettings configSettings)
        {
            _configSettings = configSettings;
            _listenAddress = new IPEndPoint(IPAddress.Any, _configSettings.Port);
            _connections = new ConcurrentDictionary<string, WsConnection>();
            _requestQueue = new RequestQueue();
            _requestQueue.RequestDequeued += HandleRequest;
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
                    connection.RequestReceived += _requestQueue.EnqueueRequest;
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

        public void Send(object sender, MessageContainer container)
        {
            if (sender is WsConnection connection)
            {
                connection.Send(container);
            }
        }

        public void SendTo(object sender, MessageContainer container, string targetId)
        {
            if (!(sender is WsConnection connection))
            {
                return;
            }

            string targetConnectionId = _connections.Values.Where(connectionItem => connectionItem.ClientId == targetId)
                                                    .Select(connectionItem => connectionItem.ID).FirstOrDefault();

            if (targetConnectionId != null)
            {
                connection.SendTo(container, targetConnectionId);
            }
        }

        public void SendBroadcast(MessageContainer container)
        {
            foreach (WsConnection connection in _connections.Values)
            {
                connection.Send(container);
            }
        }

        internal void AddConnection(WsConnection connection)
        {
            _connections.TryAdd(connection.ID, connection);
        }

        internal void ReleaseConnection(string connectionId)
        {
            if (_connections.TryRemove(connectionId, out WsConnection connection))
            {
                ConnectionClosed?.Invoke(connection, new ConnectionStateChangedEventArgs(connection.ClientId, false));
            }
        }

        private void HandleRequest(object sender, RequestDequeuedEventArgs args)
        {
            if (!_connections.TryGetValue(args.ConnectionId, out WsConnection senderConnection))
            {
                return;
            }

            var container = JsonConvert.DeserializeObject<MessageContainer>(args.SerializedRequest);

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

                    ConnectionRequestReceived?.Invoke(senderConnection, new ConnectionRequestReceivedEventArgs(connectionRequest.ClientName));
                    break;

                case MessageTypes.MessageRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(MessageRequest)) is MessageRequest messageRequest))
                    {
                        return;
                    }

                    MessageRequestReceived?.Invoke(
                        senderConnection,
                        new MessageRequestReceivedEventArgs(messageRequest.Body, messageRequest.SourceId, messageRequest.ChatId));
                    break;
                case MessageTypes.EventLogsRequest:
                    EventLogsRequestReceived?.Invoke(senderConnection, EventArgs.Empty);
                    break;

                case MessageTypes.ChatHistoryRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(ChatHistoryRequest)) is ChatHistoryRequest chatHistoryRequest))
                    {
                        return;
                    }

                    ChatHistoryRequestReceived?.Invoke(
                        senderConnection,
                        new ChatHistoryRequestReceivedEventArgs(chatHistoryRequest.ChatId));
                    break;

                case MessageTypes.GroupCreationRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(GroupCreationRequest)) is GroupCreationRequest groupCreationRequest))
                    {
                        return;
                    }

                    GroupCreationRequestReceived?.Invoke(
                        senderConnection,
                        new GroupCreationRequestReceivedEventArgs(
                            groupCreationRequest.GroupTitle,
                            groupCreationRequest.ClientIds,
                            groupCreationRequest.CreatorId));
                    break;

                case MessageTypes.ChatListRequest:
                    if (!(((JObject)container.Payload).ToObject(typeof(ChatListRequest)) is ChatListRequest chatListRequest))
                    {
                        return;
                    }

                    ChatListRequestReceived?.Invoke(
                        senderConnection,
                        new ChatListRequestReceivedEventArgs(chatListRequest.ClientId));
                    break;
            }
        }

        #endregion
    }
}
