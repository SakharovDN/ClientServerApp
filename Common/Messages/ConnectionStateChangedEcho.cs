namespace Common.Messages
{
    public class ConnectionStateChangedEcho
    {
        #region Properties

        public string ClientName { get; set; }

        public bool IsConnected { get; set; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEcho(string clientName, bool isConnected)
        {
            ClientName = clientName;
            IsConnected = isConnected;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ConnectionStateChangedEcho,
                Payload = this
            };
        }

        #endregion
    }
}
