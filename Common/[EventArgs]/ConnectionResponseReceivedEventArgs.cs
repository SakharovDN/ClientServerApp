namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ConnectionResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public ResultCodes Result { get; }

        public string Reason { get; }

        public List<string> ConnectedClients { get; }

        public int ClientId { get; }

        public int KeepAliveInterval { get; }

        #endregion

        #region Constructors

        public ConnectionResponseReceivedEventArgs(
            ResultCodes result,
            string reason,
            List<string> connectedClients,
            int clientId,
            int keepAliveInterval)
        {
            Result = result;
            Reason = reason;
            ConnectedClients = connectedClients;
            ClientId = clientId;
            KeepAliveInterval = keepAliveInterval;
        }

        #endregion
    }
}
