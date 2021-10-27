namespace Common
{
    using System;

    using _EventArgs_;

    using Messages;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsConnection : WebSocketBehavior
    {
        #region Properties

        public Guid Id { get; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        #endregion

        #region Events

        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        #endregion

        #region Constructors

        public WsConnection()
        {
            Id = Guid.NewGuid();
        }

        #endregion

        #region Methods

        public void Close()
        {
            Context.WebSocket.Close();
        }

        protected override void OnOpen()
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Id, this, true));
        }

        protected override void OnClose(CloseEventArgs args)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(Id, this, false));
        }

        protected override void OnMessage(MessageEventArgs args)
        {
            if (!args.IsText)
            {
                return;
            }
            string message = MessageHandler.HandleMessageFromClient(args.Data);
            Send(message);
        }

        #endregion
    }
}
