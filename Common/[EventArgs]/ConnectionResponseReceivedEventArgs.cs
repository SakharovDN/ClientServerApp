namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ConnectionResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public ResultCodes Result { get; }

        public string Reason { get; }

        public List<Chat> AvailableChats { get; }

        public List<Client> ConnectedClients { get; }

        public string ClientId { get; }

        public int KeepAliveInterval { get; }

        #endregion

        #region Constructors

        public ConnectionResponseReceivedEventArgs(
            ResultCodes result,
            string reason,
            List<Chat> availableChats,
            List<Client> connectedClients,
            string clientId,
            int keepAliveInterval)
        {
            Result = result;
            Reason = reason;
            AvailableChats = availableChats;
            ConnectedClients = connectedClients;
            ClientId = clientId;
            KeepAliveInterval = keepAliveInterval;
        }

        #endregion
    }
}
