namespace Server.WebSocket
{
    using System;
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
        private DateTime _lastActivity;
        private Timer _checkActivityTimer;
        private int _inactivityTimeoutInterval;

        #endregion

        #region Properties

        public string ClientId { get; set; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<RequestReceivedEventArgs> RequestReceived;

        #endregion

        #region Constructors

        public WsConnection()
        {
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

        public void SendTo(MessageContainer container, string targetId)
        {
            if (!IsConnected)
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(container, _settings);
            Sessions.SendToAsync(serializedMessages, targetId, SendCompleted);
        }

        public void SetInactivityTimeout(int inactivityTimeoutInterval)
        {
            _inactivityTimeoutInterval = inactivityTimeoutInterval;
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        protected override void OnMessage(MessageEventArgs args)
        {
            _lastActivity = DateTime.Now;
            RequestReceived?.Invoke(this, new RequestReceivedEventArgs(ID, args.Data));
        }

        protected override void OnOpen()
        {
            _server.AddConnection(this);
            _checkActivityTimer = new Timer(_inactivityTimeoutInterval);
            _checkActivityTimer.Elapsed += CheckConnection;
            _checkActivityTimer.Start();
        }

        protected override void OnClose(CloseEventArgs args)
        {
            _checkActivityTimer.Stop();
            _server.ReleaseConnection(ID);
        }

        private void SendCompleted(bool completed)
        {
            if (completed)
            {
                return;
            }

            Close();
        }

        private void CheckConnection(object sender, ElapsedEventArgs args)
        {
            if (DateTime.Now > _lastActivity + TimeSpan.FromMilliseconds(5 * _inactivityTimeoutInterval))
            {
                Close();
            }
        }

        #endregion
    }
}
