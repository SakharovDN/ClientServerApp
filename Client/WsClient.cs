namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;

    using Group = Common.Group;

    public class WsClient
    {
        #region Constants

        private const string PATTERN = @"[ ]{2,}";
        private const string REPLACEMENT = @" ";

        #endregion

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

                var regex = new Regex(PATTERN, RegexOptions.None);
                _name = regex.Replace(value, REPLACEMENT).Trim();
            }
        }

        public string Id { get; set; }

        public List<Group> Groups { get; set; }

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
            MessageHandler.GroupListResponseReceived += HandleGroupListResponseReceived;
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

        public void RequestEventLogs()
        {
            Send(new EventLogsRequest().GetContainer());
        }

        public void RequestChatHistory(string targetId, ChatTypes chatType)
        {
            Send(new ChatHistoryRequest(targetId, Id, chatType).GetContainer());
        }

        public void SendMessage(string targetId, string body, ChatTypes chatType)
        {
            Send(new MessageRequest(body, Id, targetId, chatType).GetContainer());
        }

        public void RequestGroupCreation(string groupTitle, ObservableCollection<Client> selectedClients)
        {
            List<string> clientIds = selectedClients.Select(client => client.Id.ToString()).ToList();
            clientIds.Add(Id);
            Send(new GroupCreationRequest(groupTitle, clientIds, Id).GetContainer());
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

        private void HandleGroupListResponseReceived(object sender, GroupListResponseReceivedEventArgs args)
        {
            Groups = args.Groups;
            RequestChatList();
        }

        private void HandleConnectionResponseReceived(object sender, ConnectionResponseReceivedEventArgs args)
        {
            if (args.Result == ResultCodes.Failure)
            {
                Disconnect();
                return;
            }

            Id = args.ClientId;
            RequestGroupList();
            _keepAlive = new KeepAlive(this, args.KeepAliveInterval);
            _keepAlive.Start();
        }

        private void RequestChatList()
        {
            Send(new ChatListRequest(Id, Groups).GetContainer());
        }

        private void RequestGroupList()
        {
            Send(new GroupListRequest(Id).GetContainer());
        }

        private static void OnError(object sender, ErrorEventArgs args)
        {
            throw args.Exception;
        }

        private void OnMessage(object sender, MessageEventArgs args)
        {
            if (args.IsPing)
            {
                _keepAlive.ResetPingResponseCounter();
                return;
            }

            MessageContainerReceived?.Invoke(this, new MessageContainerReceivedEventArgs(args.Data));
        }

        private void OnOpen(object sender, EventArgs args)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(true));
        }

        private void OnClose(object sender, CloseEventArgs args)
        {
            _keepAlive?.Stop();
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(false));
        }

        #endregion
    }
}
