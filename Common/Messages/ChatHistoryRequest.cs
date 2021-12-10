namespace Common.Messages
{
    public class ChatHistoryRequest
    {
        #region Properties

        public string TargetId { get; set; }

        public string SourceId { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryRequest(string targetId, string sourceId)
        {
            TargetId = targetId;
            SourceId = sourceId;
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
