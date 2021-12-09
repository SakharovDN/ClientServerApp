namespace Common.Messages
{
    using System.Collections.Generic;

    public class ChatListResponse
    {
        #region Properties

        public List<Chat> Chats { get; set; }

        #endregion

        #region Constructors

        public ChatListResponse(List<Chat> chats)
        {
            Chats = chats;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatListResponse,
                Payload = this
            };
        }

        #endregion
    }
}
