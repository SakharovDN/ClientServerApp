namespace Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Text.RegularExpressions;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        public MessageHandler MessageHandler;

        private readonly JsonSerializerSettings _settings;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WebSocket _socket;
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
                    throw new Exception("Enter your name to log in");
                }

                if (value.Length > 10)
                {
                    throw new Exception("The name can contain a maximum of 10 characters");
                }

                var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                _name = regex.Replace(value, @" ").Trim();
            }
        }

        public Guid Id { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<MessageContainerReceivedEventArgs> MessageContainerReceived;

        #endregion

        #region Constructors

        public WsClient()
        {
            MessageHandler = new MessageHandler(this);
            MessageHandler.DisconnectionResponseReceived += HandleDisconnectionResponseReceived;
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
            _socket.OnMessage += OnMessage;
            _socket.OnError += OnError;
            _socket.EmitOnPing = true;
            _socket.Connect();
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
            _socket.OnMessage -= OnMessage;
            _socket = null;
        }

        public void LogIn(string clientName)
        {
            Name = clientName;
            _sendQueue.Enqueue(new ConnectionRequest(Name).GetContainer());
            SendImpl();
        }

        public void LogOut()
        {
            _sendQueue.Enqueue(new DisconnectionRequest(Name).GetContainer());
            SendImpl();
        }

        public void RequestEventLogs()
        {
            _sendQueue.Enqueue(new EventLogsRequest().GetContainer());
            SendImpl();
        }

        public void Send(string message)
        {
            _sendQueue.Enqueue(new MessageRequest(Name, message).GetContainer());
            SendImpl();
        }

        private void HandleDisconnectionResponseReceived(object sender, DisconnectionResponseReceivedEventArgs e)
        {
            Disconnect();
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
            _socket.Send(serializedMessages);
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            throw e.Exception;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                _socket.Ping();
                return;
            }

            MessageContainerReceived?.Invoke(this, new MessageContainerReceivedEventArgs(e.Data));
        }

        #endregion
    }
}
