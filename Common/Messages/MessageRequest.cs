namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Message { get; set; }

        public string SenderName { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(string senderName, string message)
        {
            Message = message;
            SenderName = senderName;
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
