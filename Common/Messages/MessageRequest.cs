namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Message { get; set; }

        public WsClient Client { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(WsClient client, string message)
        {
            Client = client;
            Message = message;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Type = MessageTypes.MessageRequest,
                Payload = this
            };
            return container;
        }

        #endregion
    }
}
