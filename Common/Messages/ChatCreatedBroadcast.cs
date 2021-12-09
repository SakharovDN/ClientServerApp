namespace Common.Messages
{
    public class ChatCreatedBroadcast
    {
        #region Properties

        public Chat Chat { get; set; }

        #endregion

        #region Constructors

        public ChatCreatedBroadcast(Chat chat)
        {
            Chat = chat;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatCreatedBroadcast,
                Payload = this
            };
        }

        #endregion
    }
}
