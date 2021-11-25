namespace Server.WebSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Timers;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsConnection : WebSocketBehavior
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private int _inactivityTimeoutInterval;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private readonly Stopwatch _pingStopwatch;
        private readonly Timer _pingTimer;
        private readonly Timer _checkConnectionTimer;
        private MessageHandler _messageHandler;
        private string _dbConnection;

        #endregion

        #region Properties

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<MessageContainerReceivedEventArgs> MessageContainerReceived;

        #endregion

        #region Constructors

        public WsConnection()
        {
            EmitOnPing = true;
            _pingStopwatch = new Stopwatch();
            _pingTimer = new Timer();
            _checkConnectionTimer = new Timer();
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        #endregion

        #region Methods

        public void Send(MessageContainer container)
        {
            if (!IsConnected)
            {
                return;
            }

            _sendQueue.Enqueue(container);
            SendImpl();
        }

        public void Broadcast(MessageContainer container)
        {
            if (!IsConnected)
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(container, _settings);
            Sessions.Broadcast(serializedMessages);
        }

        public void SetConfigSettings(ConfigSettings configSettings)
        {
            _inactivityTimeoutInterval = configSettings.InactivityTimeoutInterval;
            _dbConnection = configSettings.DbConnection;
            _messageHandler = new MessageHandler(this, _dbConnection);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsPing)
            {
                _pingStopwatch.Restart();
                return;
            }

            MessageContainerReceived?.Invoke(this, new MessageContainerReceivedEventArgs(e.Data));
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
            Send(serializedMessages);
        }

        #endregion
    }
}
