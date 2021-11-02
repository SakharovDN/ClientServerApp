namespace Common
{
    public class ConnectionStateChangedEventArgs
    {
        #region Properties

        public string ClientName { get; }

        public bool Connected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEventArgs(string clientName, bool connected)
        {
            ClientName = clientName;
            Connected = connected;
        }

        #endregion
    }
}
