namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Body { get; set; }

        public string SourceId { get; set; }

        public Chat Chat { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(string body, string sourceId, Chat chat)
        {
            Body = body;
            SourceId = sourceId;
            Chat = chat;
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
