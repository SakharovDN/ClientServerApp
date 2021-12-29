namespace Common.Messages
{
    public class ConnectionStateChangedBroadcast
    {
        #region Properties

        public Client Client { get; set; }

        public bool IsConnected { get; set; }

        #endregion

        #region Constructors

        public ConnectionStateChangedBroadcast(Client client, bool isConnected)
        {
            Client = client;
            IsConnected = isConnected;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ConnectionStateChangedBroadcast,
                Payload = this,
            };
        }

        #endregion
    }
}
