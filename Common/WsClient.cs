namespace Common
{
    using System;

    using _Enums_;

    using _EventArgs_;

    using Messages;

    using WebSocketSharp;

    public class WsClient
    {
        #region Fields

        private WebSocket _socket;

        #endregion

        #region Properties

        public string ClientName { get; set; }

        public bool IsConnected => _socket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public static event EventHandler<ClientConnectionStateChangedEventArgs> ClientConnectionStateChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Methods

        public void Connect(string address, string port)
        {
            _socket = new WebSocket($"ws://{address}:{port}/");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.Connect();
        }

        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }

            if (IsConnected)
            {
                _socket.Close();
            }

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;

            _socket = null;
            ClientName = string.Empty;
        }

        public void LogIn(string clientName)
        {
            ClientName = clientName;
            Send(ClientName, MessageTypes.ConnectionRequest);
        }

        public void Send(string message, MessageTypes type)
        {
            if (!IsConnected)
            {
                return;
            }

            message = MessageHandler.SendMessageToServer(message, type);
            _socket.Send(message);
        }

        private void OnClose(object sender, CloseEventArgs args)
        {
            ClientConnectionStateChanged?.Invoke(this, new ClientConnectionStateChangedEventArgs(ClientName, this, false));
        }

        private void OnOpen(object sender, EventArgs args)
        {
            ClientConnectionStateChanged?.Invoke(this, new ClientConnectionStateChangedEventArgs(ClientName, this, true));
        }

        private void OnMessage(object sender, MessageEventArgs args)
        {
            if (!args.IsText)
            {
                return;
            }

            object receivedMessage = MessageHandler.HandleMessageFromServer(args.Data);

            if (receivedMessage.GetType() == typeof(ConnectionResponse))
            {
                var connectionResponse = (ConnectionResponse) receivedMessage;
                HandleConnectionResponse(connectionResponse);
            }
        }

        private void HandleConnectionResponse(ConnectionResponse connectionResponse)
        {
            if (connectionResponse != null && connectionResponse.Result == ResultCodes.Ok)
            {
                ClientConnectionStateChanged?.Invoke(this, new ClientConnectionStateChangedEventArgs(ClientName, this, true));
            }
            else
            {
                if (connectionResponse != null)
                {
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(ClientName, connectionResponse.Reason));
                }
            }
        }

        #endregion
    }
}
