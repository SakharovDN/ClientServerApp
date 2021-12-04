namespace Common
{
    using System;

    using Messages;

    public class ConnectionClosedEventArgs : EventArgs
    {
        #region Properties

        public Action<MessageContainer> SendBroadcast { get; }

        #endregion

        #region Constructors

        public ConnectionClosedEventArgs(Action<MessageContainer> sendBroadcast)
        {
            SendBroadcast = sendBroadcast;
        }

        #endregion
    }
}
