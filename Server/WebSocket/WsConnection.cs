namespace Server.WebSocket
{
    using System;
    using System.Collections.Concurrent;

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
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private readonly MessageService _messageService;
        private readonly EventLogContext _eventLogContext;

        #endregion

        #region Properties

        public Guid Id { get; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Constructors

        public WsConnection()
        {
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

        public void Send(MessageContainer container)
        {
            if (!IsConnected)
            {
                return;
            }

            _sendQueue.Enqueue(container);
            SendImpl();
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            _messageService.HandleMessage(e.Data, this);
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
