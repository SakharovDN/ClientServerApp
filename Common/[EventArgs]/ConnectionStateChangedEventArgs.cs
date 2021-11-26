namespace Common
{
    using System;

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        #region Properties

        public string ClientName { get; }

        public bool IsConnected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEventArgs(string clientName, bool isConnected)
        {
            ClientName = clientName;
            IsConnected = isConnected;
        }

        #endregion
    }
}
