namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        public MessageHandler MessageHandler;

        private KeepAlive _keepAlive;
        private readonly JsonSerializerSettings _settings;
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

        public int Id { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<MessageContainerReceivedEventArgs> MessageContainerReceived;

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        #endregion

        #region Constructors

        public WsClient()
        {
            MessageHandler = new MessageHandler();
            MessageContainerReceived += MessageHandler.HandleMessageContainer;
            MessageHandler.ConnectionResponseReceived += HandleConnectionResponseReceived;
            MessageHandler.DisconnectionResponseReceived += HandleDisconnectionResponseReceived;
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

            _socket = new WebSocket($"ws://{address}:{port}/Connection");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
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
            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket.OnError -= OnError;
            _socket = null;
        }

        public void LogIn(string clientName)
        {
            Name = clientName;
            Send(new ConnectionRequest(Name).GetContainer());
        }

        public void LogOut()
        {
            Send(new DisconnectionRequest(Name).GetContainer());
        }

        public void RequestEventLogs()
        {
            Send(new EventLogsRequest().GetContainer());
        }

        public void SendMessage(string message, string target)
        {
            Send(new MessageRequest(message, Name, target).GetContainer());
        }

        public void RequestChatHistory(string targetName)
        {
            var participants = new List<string>
            {
                Name,
                targetName
            };
            Send(new ChatHistoryRequest(participants).GetContainer());
        }

        public void Ping()
        {
            try
            {
                _socket.Ping();
            }
            catch
            {
                Disconnect();
            }
        }

        private void Send(MessageContainer container)
        {
            if (!IsConnected)
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(container, _settings);
            _socket.Send(serializedMessages);
        }

        private void HandleConnectionResponseReceived(object sender, ConnectionResponseReceivedEventArgs args)
        {
            Id = args.ClientId;
            _keepAlive = new KeepAlive(this, args.KeepAliveInterval);
            _keepAlive.Start();
        }

        private void HandleDisconnectionResponseReceived(object sender, DisconnectionResponseReceivedEventArgs args)
        {
            Disconnect();
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            throw e.Exception;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                _keepAlive.ResetPingResponseCounter();
                return;
            }

            MessageContainerReceived?.Invoke(this, new MessageContainerReceivedEventArgs(e.Data));
        }

        private void OnOpen(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(true));
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            _keepAlive.Stop();
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(false));
        }

        #endregion
    }
}
