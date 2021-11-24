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

        #endregion

        #region Constructors

        public ConnectionResponseReceivedEventArgs(ResultCodes result, string reason, List<string> connectedClients)
        {
            Result = result;
            Reason = reason;
            ConnectedClients = connectedClients;
        }

        #endregion
    }
}
