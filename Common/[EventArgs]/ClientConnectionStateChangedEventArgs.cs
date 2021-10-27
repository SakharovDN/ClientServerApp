namespace Common._EventArgs_
{
    public class ClientConnectionStateChangedEventArgs
    {
        #region Properties

        public string ClientName { get; }

        public WsClient Client { get; }

        public bool Connected { get; }

        #endregion

        #region Constructors

        public ClientConnectionStateChangedEventArgs(string clientName, WsClient client, bool connected)
        {
            ClientName = clientName;
            Client = client;
            Connected = connected;
        }

        #endregion
    }
}
