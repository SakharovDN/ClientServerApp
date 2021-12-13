namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Body { get; set; }

        public string SourceId { get; set; }

        public string TargetId { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(string body, string sourceId, string targetId)
        {
            Body = body;
            SourceId = sourceId;
            TargetId = targetId;
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
