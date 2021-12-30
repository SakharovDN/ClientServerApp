namespace Common.Messages
{
    public class GroupLeavingResponse
    {
        #region Properties

        public string ChatId { get; set; }

        #endregion

        #region Constructors

        public GroupLeavingResponse(string chatId)
        {
            ChatId = chatId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.GroupLeavingResponse,
                Payload = this,
            };
        }

        #endregion
    }
}
