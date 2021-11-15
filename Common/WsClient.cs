namespace Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Text.RegularExpressions;

    using Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WebSocket _socket;
        private WebSocket _chatSocket;
        private string _name;

        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                if (value.IsNullOrEmpty())
                {
                    throw new Exception("Введите имя, чтобы авторизироваться");
                }

                if (value.Length > 10)
                {
                    throw new Exception("Имя может содержать максимум 10 символов");
                }

                var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                _name = regex.Replace(value, @" ").Trim();
            }
        }

        public Guid Id { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

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

            _socket = new WebSocket($"ws://{address}:{port}/Connection");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.OnError += OnError;
            _socket.ConnectAsync();
            _chatSocket = new WebSocket($"ws://{address}:{port}/CommonChat");
            _chatSocket.OnMessage += OnMessage;
            _chatSocket.OnError += OnError;
            _chatSocket.ConnectAsync();
        }

        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }

            if (!IsConnected)
            {
                return;
            }

            _socket.Close();
            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket = null;
            _chatSocket.Close();
            _chatSocket.OnMessage -= OnMessage;
            _chatSocket = null;
        }

        public void SignIn(string clientName)
        {
            Name = clientName;
            _sendQueue.Enqueue(new ConnectionRequest(this).GetContainer());
            SendImpl();
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

        public void RequestEventLogs()
        {
            string serializedMessages = JsonConvert.SerializeObject(new EventLogsRequest().GetContainer(), _settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        public void Send(string message)
        {
            string serializedMessages = JsonConvert.SerializeObject(new MessageRequest(this, message).GetContainer(), _settings);
            _chatSocket.SendAsync(serializedMessages, SendCompleted);
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

        private static void OnError(object sender, ErrorEventArgs e)
        {
            throw e.Exception;
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
