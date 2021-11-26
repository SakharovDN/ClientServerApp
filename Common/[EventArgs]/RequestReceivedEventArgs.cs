namespace Common
{
    using System;

    public class RequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ConnectionId { get; }

        public string Request { get; }

        #endregion

        #region Constructors

        public RequestReceivedEventArgs(string connectionId, string request)
        {
            ConnectionId = connectionId;
            Request = request;
        }

        #endregion
    }
}
