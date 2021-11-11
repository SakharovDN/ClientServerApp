namespace Common
{
    using Messages;

    public class ConnectionResponseReceivedEventArgs
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
