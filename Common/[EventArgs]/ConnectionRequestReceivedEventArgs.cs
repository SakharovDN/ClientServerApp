namespace Common
{
    using System;

    using Messages;

    public class ConnectionRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientName { get; }

        public Action<object, MessageContainer> Send { get; }

        public Action<MessageContainer> SendBroadcast { get; }

        #endregion

        #region Constructors

        public ConnectionRequestReceivedEventArgs(string clientName, Action<object, MessageContainer> send, Action<MessageContainer> sendBroadcast)
        {
            ClientName = clientName;
            Send = send;
            SendBroadcast = sendBroadcast;
        }

        #endregion
    }
}
