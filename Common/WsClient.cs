namespace Common
{
    using System;
    using System.Collections.Concurrent;

    using _Enums_;

    using _EventArgs_;

    using Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        private WebSocket _socket;

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private string _login;

        #endregion

        #region Properties

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Constructors

        public WsClient()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
        }

        #endregion

        #region Methods

        public void Connect(string address, string port)
        {
            if (IsConnected)
            {
                Disconnect();
            }

            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.ConnectAsync();
        }

        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }

            if (IsConnected)
            {
                _socket.CloseAsync();
            }

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;

            _socket = null;
            _login = string.Empty;
        }

        public void Login(string login)
        {
            _login = login;
            // ставит в конец коллекции (конец очереди) контейнер с сообщением (запрос на подключение)
            _sendQueue.Enqueue(new ConnectionRequest(_login).GetContainer());

            SendImpl();
        }

        public void Send(string message)
        {
            // ставит в конец коллекции (конец очереди) контейнер с сообщением (запрос на отправку сообщения)
            _sendQueue.Enqueue(new MessageRequest(message).GetContainer());

            SendImpl();
        }

        private void SendImpl()
        {
            if (!IsConnected)
            {
                return;
            }

            // если нельзя удалить и вернуть первый в очереди контейнер с сообщением (очередь пустая) => return
            // в message возвращается первый в очереди контейнер
            if (!_sendQueue.TryDequeue(out MessageContainer message))
            {
                return;
            }

            var settings = new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           };
            string serializedMessages = JsonConvert.SerializeObject(message, settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                // Обработчик события при изменении состояния подключения (будет вызываться в wpfклиенте)
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, false));

                return;
            }

            SendImpl();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(null, false));
        }

        private void OnOpen(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(null, true));
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText)
            {
                return;
            }

            var container = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            switch (container.Identifier)
            {
                case nameof(ConnectionResponse):
                    var connectionResponse = ((JObject) container.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;

                    if (connectionResponse.Result == ResultCodes.Failure)
                    {
                        _login = string.Empty;
                        // вызывается в клиенте
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(_login, connectionResponse.Reason));
                    }

                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(_login, true));

                    break;
                case nameof(MessageBroadcast):
                    var messageBroadcast = ((JObject) container.Payload).ToObject(typeof(MessageBroadcast)) as MessageBroadcast;
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(_login, messageBroadcast.Message));

                    break;
            }
        }

        #endregion
    }
}
