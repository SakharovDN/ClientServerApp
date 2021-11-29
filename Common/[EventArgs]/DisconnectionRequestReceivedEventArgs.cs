namespace Common
{
    using System;

    using Messages;

    public class DisconnectionRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientName { get; }

        public Action<object, MessageContainer> SendBroadcast { get; }

        #endregion

        #region Constructors

        public DisconnectionRequestReceivedEventArgs(string clientName, Action<object, MessageContainer> sendBroadcast)
        {
            ClientName = clientName;
            SendBroadcast = sendBroadcast;
        }

        #endregion
    }
}
