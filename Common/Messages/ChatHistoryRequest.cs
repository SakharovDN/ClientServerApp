namespace Common.Messages
{
    public class ChatHistoryRequest
    {
        #region Properties

        public string TargetId { get; set; }

        public string SourceId { get; set; }

        public ChatTypes ChatType { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryRequest(string targetId, string sourceId, ChatTypes chatType)
        {
            TargetId = targetId;
            SourceId = sourceId;
            ChatType = chatType;
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
