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

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WsServer _server;

        #endregion

        #region Properties

        public Guid Id { get; }

        public string Login { get; set; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Constructors

        public WsConnection()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            Id = Guid.NewGuid();
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
            _server.ReleaseConnection(Id);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (!e.IsText)
            {
                return;
            }

            var message = JsonConvert.DeserializeObject<MessageContainer>(e.Data);
            _server.HandleMessage(Id, message);
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

            var settings = new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           };
            string serializedMessages = JsonConvert.SerializeObject(message, settings);
            SendAsync(serializedMessages, SendCompleted);
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                _server.ReleaseConnection(Id);
                Context.WebSocket.CloseAsync();

                return;
            }

            SendImpl();
        }

        #endregion
    }
}
