namespace Common
{
    using System;

    using Messages;

    public class ConnectionResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public ConnectionResponse ConnectionResponse { get; set; }

        #endregion

        #region Constructors

        public ConnectionResponseReceivedEventArgs(ConnectionResponse connectionResponse)
        {
            ConnectionResponse = connectionResponse;
        }

        #endregion
    }
}
