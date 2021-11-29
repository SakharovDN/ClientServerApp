namespace Server.WebSocket
{
    using System;
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

        private WsServer _server;
        private readonly JsonSerializerSettings _settings;
        private Stopwatch _pingStopwatch;
        private Timer _pingTimer;
        private Timer _checkConnectionTimer;
        private int _inactivityTimeoutInterval;

        #endregion

        #region Properties

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<RequestReceivedEventArgs> RequestReceived;

        #endregion

        #region Constructors

        public WsConnection()
        {
            EmitOnPing = true;
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
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

            string serializedMessages = JsonConvert.SerializeObject(container, _settings);
            SendAsync(serializedMessages, SendCompleted);
        }

        public void Broadcast(MessageContainer container)
        {
            if (!IsConnected)
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(container, _settings);
            Sessions.BroadcastAsync(serializedMessages, null);
        }

        public void SetInactivityTimeout(int inactivityTimeoutInterval)
        {
            _inactivityTimeoutInterval = inactivityTimeoutInterval;
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            _pingStopwatch.Restart();

            if (e.IsPing)
            {
                return;
            }

            RequestReceived?.Invoke(this, new RequestReceivedEventArgs(ID, e.Data));
        }

        protected override void OnOpen()
        {
            _server.AddConnection(this);
            _pingTimer = new Timer(_inactivityTimeoutInterval / 2);
            _pingTimer.Elapsed += Ping;
            _checkConnectionTimer = new Timer(_inactivityTimeoutInterval);
            _checkConnectionTimer.Elapsed += CheckConnection;
            _pingStopwatch = new Stopwatch();
            _pingTimer.Start();
            _checkConnectionTimer.Start();
            _pingStopwatch.Start();
        }

        protected override void OnClose(CloseEventArgs closeEventArgs)
        {
            _server.FreeConnection(ID);
            _pingTimer.Stop();
            _checkConnectionTimer.Stop();
            _pingStopwatch.Reset();
        }

        private void SendCompleted(bool completed)
        {
            if (completed)
            {
                return;
            }

            _server.FreeConnection(ID);
            Close();
        }

        private void Ping(object sender, ElapsedEventArgs e)
        {
            try
            {
                Sessions.PingTo(ID);
            }
            catch
            {
                Close();
            }
        }

        private void CheckConnection(object sender, ElapsedEventArgs e)
        {
            if (_pingStopwatch.Elapsed.Milliseconds > 3 * _inactivityTimeoutInterval)
            {
                Close();
            }
        }

        #endregion
    }
}
