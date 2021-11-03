namespace Common
{
    using System;
    using System.Collections.Concurrent;

    using Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsConnection : WebSocketBehavior
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private WsServer _server;

        #endregion

        #region Properties

        public Guid Id { get; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Constructors

        public WsConnection()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            Id = Guid.NewGuid();
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        #endregion

        #region Methods

        public void AddServer(WsServer server)
        {
            _server = server;
        }

        public void Send(MessageContainer container)
        {
            if (!IsConnected)
            {
                return;
            }

            _sendQueue.Enqueue(container);
            SendImpl();
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        protected override void OnOpen()
        {
            _server.AddConnection(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _server.FreeConnection(Id);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            MessageHandler.HandleMessage(e.Data, _server, this);
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                _server.FreeConnection(Id);
                Context.WebSocket.CloseAsync();
                return;
            }

            SendImpl();
        }

        private void SendImpl()
        {
            if (!IsConnected)
            {
                return;
            }

            if (!_sendQueue.TryDequeue(out MessageContainer message))
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(message, _settings);
            SendAsync(serializedMessages, SendCompleted);
        }

        #endregion
    }
}
