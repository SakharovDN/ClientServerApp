namespace Common.Messages
{
    using System.Collections.Generic;

    public class ChatHistoryResponse
    {
        #region Properties

        public List<Message> ChatHistory { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryResponse(List<Message> chatHistory)
        {
            ChatHistory = chatHistory;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatHistoryResponse,
                Payload = this
            };
        }

        #endregion
    }
}
