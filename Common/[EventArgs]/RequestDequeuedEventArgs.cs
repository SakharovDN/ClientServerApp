namespace Common
{
    using System;

    public class RequestDequeuedEventArgs : EventArgs
    {
        #region Properties

        public string ConnectionId { get; }

        public string SerializedRequest { get; }

        #endregion

        #region Constructors

        public RequestDequeuedEventArgs(string connectionId, string serializedRequest)
        {
            ConnectionId = connectionId;
            SerializedRequest = serializedRequest;
        }

        #endregion
    }
}
