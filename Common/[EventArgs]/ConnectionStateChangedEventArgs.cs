namespace Common
{
    public class ConnectionStateChangedEventArgs
    {
        #region Properties

        public WsClient Client { get; }

        public bool Connected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEventArgs(WsClient client, bool connected)
        {
            Client = client;
            Connected = connected;
        }

        #endregion
    }
}
