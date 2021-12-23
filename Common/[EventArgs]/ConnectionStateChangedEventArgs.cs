namespace Common
{
    using System;

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        #region Properties

        public string ClientId { get; }

        public bool IsConnected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }

        public ConnectionStateChangedEventArgs(string clientId, bool isConnected)
        {
            ClientId = clientId;
            IsConnected = isConnected;
        }

        #endregion
    }
}
