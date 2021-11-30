namespace Common.Messages
{
    using System.Collections.Generic;

    public class ChatHistoryRequest
    {
        #region Properties

        public List<string> Participants { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryRequest(List<string> participants)
        {
            Participants = participants;
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
