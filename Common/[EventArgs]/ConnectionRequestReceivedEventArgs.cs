namespace Common
{
    using System;

    public class ConnectionRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientName { get; }

        #endregion

        #region Constructors

        public ConnectionRequestReceivedEventArgs(string clientName)
        {
            ClientName = clientName;
        }

        #endregion
    }
}
