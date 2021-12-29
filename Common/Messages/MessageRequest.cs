namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Body { get; set; }

        public string SourceId { get; set; }

        public string ChatId { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(string body, string sourceId, string chatId)
        {
            Body = body;
            SourceId = sourceId;
            ChatId = chatId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.MessageRequest,
                Payload = this,
            };
        }

        #endregion
    }
}
