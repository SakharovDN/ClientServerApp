namespace Common.Messages
{
    public class MessageBroadcast
    {
        #region Properties

        public string Message { get; set; }

        public string SenderName { get; set; }

        #endregion

        #region Constructors

        public MessageBroadcast(string message, string senderName)
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
                Type = MessageTypes.MessageBroadcast,
                Payload = this
            };
            return container;
        }

        #endregion
    }
}
