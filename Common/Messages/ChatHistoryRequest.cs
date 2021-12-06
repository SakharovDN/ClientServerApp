namespace Common.Messages
{
    public class ChatHistoryRequest
    {
        #region Properties

        public string ChatId { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryRequest(string chatId)
        {
            ChatId = chatId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatHistoryRequest,
                Payload = this
            };
        }

        #endregion
    }
}
