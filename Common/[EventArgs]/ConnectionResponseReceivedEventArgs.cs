namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ConnectionResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public ResultCodes Result { get; }

        public string Reason { get; }

        public HashSet<Client> ConnectedClients { get; }

        public string ClientId { get; }

        #endregion

        #region Constructors

        public ConnectionResponseReceivedEventArgs(
            ResultCodes result,
            string reason,
            HashSet<Client> connectedClients,
            string clientId)
        {
            Result = result;
            Reason = reason;
            ConnectedClients = connectedClients;
            ClientId = clientId;
        }

        #endregion
    }
}
