namespace Common
{
    using System;

    using Messages;

    public class ConnectionRequestHandledEventArgs : EventArgs
    {
        #region Properties

        public Client Client { get; }

        public ConnectionResponse ConnectionResponse { get; }

        #endregion

        #region Constructors

        public ConnectionRequestHandledEventArgs(Client client, ConnectionResponse connectionResponse)
        {
            Client = client;
            ConnectionResponse = connectionResponse;
        }

        #endregion
    }
}
