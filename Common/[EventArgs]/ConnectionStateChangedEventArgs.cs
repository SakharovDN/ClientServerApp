namespace Common
{
    using System;

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        #region Properties

        public bool IsConnected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }

        #endregion
    }
}
