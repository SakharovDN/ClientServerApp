namespace Common.Messages
{
    public class ConnectionRequest
    {
        #region Properties

        public string ClientName { get; set; }

        #endregion

        #region Constructors

        public ConnectionRequest(string clientName)
        {
            ClientName = clientName;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ConnectionRequest,
                Payload = this,
            };
        }

        #endregion
    }
}
