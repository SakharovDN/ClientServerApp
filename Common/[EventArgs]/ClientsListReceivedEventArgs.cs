namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ClientsListReceivedEventArgs : EventArgs
    {
        #region Properties

        public List<string> Clients { get; }

        #endregion

        #region Constructors

        public ClientsListReceivedEventArgs(List<string> clients)
        {
            Clients = clients;
        }

        #endregion
    }
}
