namespace Common
{
    using System;

    public class ConnectionStateChangedBroadcastReceivedEventArgs : EventArgs
    {
        #region Properties

        public Client Client { get; }

        public bool IsConnected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedBroadcastReceivedEventArgs(Client client, bool isConnected)
        {
            Client = client;
            IsConnected = isConnected;
        }

        #endregion
    }
}
