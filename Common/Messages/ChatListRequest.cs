namespace Common.Messages
{
    public class ChatListRequest
    {
        #region Properties

        public string ClientId { get; set; }

        #endregion

        #region Constructors

        public ChatListRequest(string clientId)
        {
            ClientId = clientId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatListRequest,
                Payload = this,
            };
        }

        #endregion
    }
}
