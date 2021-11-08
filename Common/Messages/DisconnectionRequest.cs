namespace Common.Messages
{
    public class DisconnectionRequest
    {
        #region Properties

        public WsClient Client { get; set; }

        #endregion

        #region Constructors

        public DisconnectionRequest(WsClient client)
        {
            Client = client;
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
