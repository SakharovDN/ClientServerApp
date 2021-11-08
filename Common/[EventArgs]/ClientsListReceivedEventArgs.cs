namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ClientsListReceivedEventArgs
    {
        #region Properties

        public Dictionary<Guid, WsClient> Clients { get; }

        #endregion

        #region Constructors

        public ClientsListReceivedEventArgs(Dictionary<Guid, WsClient> clients)
        {
            Clients = clients;
        }

        #endregion
    }
}
