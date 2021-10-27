namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Message { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(string message)
        {
            Message = message;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
                            {
                                Identifier = nameof(MessageRequest),
                                Payload = this
                            };

            return container;
        }

        #endregion
    }
}
