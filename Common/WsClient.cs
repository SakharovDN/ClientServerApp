namespace Common
{
    using System;
    using System.Collections.Concurrent;

    using Messages;

    using Newtonsoft.Json;

    using Services;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WebSocket _socket;

        #endregion

        #region Properties

        public string Login { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Constructors

        public WsClient()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
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
                _socket.CloseAsync();
            }

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket = null;
            Login = string.Empty;
        }

        public void SignIn(string login)
        {
            Login = login;
            _sendQueue.Enqueue(new ConnectionRequest(Login).GetContainer());
            SendImpl();
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, false));
                ClientService.Remove(this);
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
            MessageHandler.HandleMessage(e.Data, this);
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, false));
            ClientService.Remove(this);
        }

        private void OnOpen(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Login, true));
            ClientService.Add(this);
        }

        #endregion
    }
}
