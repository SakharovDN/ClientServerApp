namespace Server.WebSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Timers;

    using Common;
    using Common.EventLog;
    using Common.Messages;

    using Newtonsoft.Json;

    using Services;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsConnection : WebSocketBehavior
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private int _inactivityTimeoutInterval;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private readonly MessageService _messageService;
        private readonly EventLogContext _eventLogContext;
        private readonly Stopwatch _pingStopwatch;
        private readonly Timer _pingTimer;
        private readonly Timer _checkConnectionTimer;

        #endregion

        #region Properties

        public Guid Id { get; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Constructors

        public WsConnection()
        {
            EmitOnPing = true;
            _pingStopwatch = new Stopwatch();
            _pingTimer = new Timer();
            _checkConnectionTimer = new Timer();
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            Id = Guid.NewGuid();
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            _messageService = new MessageService();
            _messageService.ConnectionStateChanged += HandleConnectionStateChanged;
            _eventLogContext = new EventLogContext();
        }

        #endregion

        #region Methods

        public void SetInactivityTimeoutInterval(int inactivityTimeoutInterval)
        {
            _inactivityTimeoutInterval = inactivityTimeoutInterval;
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

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsPing)
            {
                _pingStopwatch.Restart();
                return;
            }

            _messageService.HandleMessage(e.Data, this);
        }

        protected override void OnOpen()
        {
            _pingTimer.Interval = _inactivityTimeoutInterval / 2;
            _pingTimer.Elapsed += Ping;
            _checkConnectionTimer.Interval = _inactivityTimeoutInterval;
            _checkConnectionTimer.Elapsed += CheckConnection;
            _pingTimer.Start();
            _checkConnectionTimer.Start();
            _pingStopwatch.Start();
        }

        protected override void OnClose(CloseEventArgs closeEventArgs)
        {
            _pingTimer.Stop();
            _checkConnectionTimer.Stop();
            _pingStopwatch.Reset();
        }

        private void Ping(object sender, ElapsedEventArgs e)
        {
            Sessions.PingTo(ID);
        }

        private void CheckConnection(object sender, ElapsedEventArgs e)
        {
            if (_pingStopwatch.Elapsed.Milliseconds > _inactivityTimeoutInterval)
            {
                Context.WebSocket.Close();
            }
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

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Context.WebSocket.CloseAsync();
                return;
            }

            SendImpl();
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.Connected ? "подключён" : "отключён";
            string message = $"Клиент {e.Client.Name} {clientState}";

            if (e.Connected)
            {
                ClientService.Add(e.Client);
            }
            else
            {
                ClientService.Remove(e.Client);
            }

            SendBroadcastMessage(message);
            SendBroadcastClientsList();
            _eventLogContext.ConnectionEventLog(message);
        }

        private void SendBroadcastMessage(string message)
        {
            string serializedMessages = JsonConvert.SerializeObject(new MessageBroadcast(message).GetContainer(), _settings);
            Sessions.Broadcast(serializedMessages);
        }

        private void SendBroadcastClientsList()
        {
            string serializedMessages = JsonConvert.SerializeObject(new ClientsListResponse(ClientService.Clients).GetContainer(), _settings);
            Sessions.Broadcast(serializedMessages);
        }

        #endregion
    }
}
