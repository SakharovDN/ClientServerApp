namespace Common.Messages
{
    public class GroupLeavingRequest
    {
        #region Properties

        public string ChatId { get; set; }

        public string ClientId { get; set; }

        #endregion

        #region Constructors

        public GroupLeavingRequest(string chatId, string clientId)
        {
            ChatId = chatId;
            ClientId = clientId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.GroupLeavingRequest,
                Payload = this,
            };
        }

        #endregion
    }
}
