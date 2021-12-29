namespace Common.Messages
{
    public class ChatInfoResponse
    {
        #region Properties

        public Group Group { get; set; }

        #endregion

        #region Constructors

        public ChatInfoResponse(Group group)
        {
            Group = group;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatInfoResponse,
                Payload = this,
            };
        }

        #endregion
    }
}
