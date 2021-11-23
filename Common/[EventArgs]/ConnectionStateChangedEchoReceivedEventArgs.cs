namespace Common
{
    using System;

    public class ConnectionStateChangedEchoReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientName { get; }

        public bool IsConnected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEchoReceivedEventArgs(string clientName, bool isConnected)
        {
            ClientName = clientName;
            IsConnected = isConnected;
        }

        #endregion
    }
}
