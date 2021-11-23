namespace Common.Messages
{
    public class DisconnectionRequest
    {
        #region Properties

        public string ClientName { get; set; }

        #endregion

        #region Constructors

        public DisconnectionRequest(string clientName)
        {
            ClientName = clientName;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.DisconnectionRequest,
                Payload = this
            };
        }

        #endregion
    }
}
