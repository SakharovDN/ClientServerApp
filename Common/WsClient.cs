namespace Common
{
    using System;
    using System.Collections.Concurrent;

    using Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WebSocket _socket;

        #endregion

        #region Properties

        public string Name { get; set; }

        public Guid Id { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public event EventHandler<MessageReceivedEventArgs> ClientMessageReceived;

        #endregion

        #region Constructors

        public WsClient()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            Id = Guid.NewGuid();
        }

        #endregion

        #region Methods

        public void Connect(string address, string port)
        {
            if (IsConnected)
            {
                Disconnect();
            }

            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.ConnectAsync();
        }

        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }

            if (IsConnected)
            {
                SignOut();
                _socket.Close();
                _socket.OnOpen -= OnOpen;
                _socket.OnClose -= OnClose;
                _socket.OnMessage -= OnMessage;
                _socket = null;
                Name = string.Empty;
            }
        }

        public void Send(string message)
        {
            _sendQueue.Enqueue(new MessageRequest(this, message).GetContainer());
            SendImpl();
        }

        public void SignIn(string clientName)
        {
            Name = clientName;
            _sendQueue.Enqueue(new ConnectionRequest(this).GetContainer());
            SendImpl();
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(this, true));
        }

        public void SignOut()
        {
            _sendQueue.Enqueue(new DisconnectionRequest(this).GetContainer());
            SendImpl();
        }

        public void RequestClientsList()
        {
            string serializedMessages = JsonConvert.SerializeObject(new ClientsListRequest().GetContainer(), _settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        public void Receive(string message)
        {
            ClientMessageReceived?.Invoke(this, new MessageReceivedEventArgs(Name, message));
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(this, false));
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
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            ClientMessageHandler.HandleMessage(e.Data, this);
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(this, false));
        }

        private void OnOpen(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(this, true));
        }

        #endregion
    }
}
