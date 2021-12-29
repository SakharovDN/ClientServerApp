namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using NLog;

    using WebSocketSharp;

    using Logger = NLog.Logger;

    public class WsClient
    {
        #region Constants

        private const int MIN_PORT = 8000;
        private const int MAX_PORT = 65535;
        private const string PATTERN = @"[ ]{2,}";
        private const string REPLACEMENT = @" ";

        #endregion

        #region Fields

        public ResponseQueue ResponseQueue;
        private readonly JsonSerializerSettings _settings;
        private WebSocket _socket;
        private string _name;
        private string _ipAddress;
        private string _port;
        private readonly Logger _logger;

        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                if (value.IsNullOrEmpty())
                {
                    throw new Exception("Enter your name to log in.");
                }

                if (value.Length > 10)
                {
                    throw new Exception("The name can contain a maximum of 10 characters.");
                }

                var regex = new Regex(PATTERN, RegexOptions.None);
                _name = regex.Replace(value, REPLACEMENT).Trim();
            }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (!IPAddress.TryParse(value, out _))
                {
                    throw new Exception("The IP address was entered incorrectly.");
                }

                _ipAddress = value;
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                if (!int.TryParse(value, out int portNumber) || portNumber < MIN_PORT || portNumber > MAX_PORT)
                {
                    throw new Exception("Port entered incorrectly.");
                }

                _port = value;
            }
        }

        public string Id { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<MessageContainerReceivedEventArgs> MessageContainerReceived;

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        #endregion

        #region Constructors

        public WsClient()
        {
            ResponseQueue = new ResponseQueue();
            MessageContainerReceived += ResponseQueue.EnqueueResponse;
            ResponseQueue.ConnectionResponseReceived += HandleConnectionResponseReceived;
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Methods

        public void Connect()
        {
            if (IsConnected)
            {
                Disconnect();
            }

            _socket = new WebSocket($"ws://{IpAddress}:{Port}/Connection");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.OnError += OnError;
            _socket.Connect();
            ResponseQueue.Start();
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

            ResponseQueue.Stop();
            _socket.Close();
            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket.OnError -= OnError;
            _socket = null;
        }

        public void LogIn()
        {
            Send(new ConnectionRequest(Name).GetContainer());
        }

        public void RequestEventLogs()
        {
            Send(new EventLogsRequest().GetContainer());
        }

        public void RequestChatHistory(string chatId)
        {
            Send(new ChatHistoryRequest(chatId).GetContainer());
        }

        public void SendMessage(string chatId, string body)
        {
            Send(new MessageRequest(body, Id, chatId).GetContainer());
        }

        public void RequestGroupCreation(string groupTitle, ObservableCollection<Client> selectedClients)
        {
            List<string> clientIds = selectedClients.Select(client => client.Id.ToString()).ToList();
            clientIds.Add(Id);
            Send(new GroupCreationRequest(groupTitle, clientIds, Id).GetContainer());
        }

        public void RequestChatInfo(string chatId)
        {
            Send(new ChatInfoRequest(chatId).GetContainer());
        }

        public void RequestGroupLeaving(string chatId)
        {
            Send(new GroupLeavingRequest(chatId, Id).GetContainer());
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
            if (args.Result == ResultCodes.Failure)
            {
                Disconnect();
                return;
            }

            Id = args.ClientId;
            RequestChatList();
        }

        private void RequestChatList()
        {
            Send(new ChatListRequest(Id).GetContainer());
        }

        private void OnError(object sender, ErrorEventArgs args)
        {
            _logger.Error(args.Exception);
            throw args.Exception;
        }

        private void OnMessage(object sender, MessageEventArgs args)
        {
            MessageContainerReceived?.Invoke(this, new MessageContainerReceivedEventArgs(args.Data));
        }

        private void OnOpen(object sender, EventArgs args)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(true));
        }

        private void OnClose(object sender, CloseEventArgs args)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(false));
        }

        #endregion
    }
}
