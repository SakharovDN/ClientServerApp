namespace Common.Messages
{
    public class ChatInfoRequest
    {
        #region Properties

        public string ChatId { get; set; }

        #endregion

        #region Constructors

        public ChatInfoRequest(string chatId)
        {
            ChatId = chatId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatInfoRequest,
                Payload = this,
            };
        }

        #endregion
    }
}
