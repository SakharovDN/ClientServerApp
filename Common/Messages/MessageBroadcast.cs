namespace Common.Messages
{
    public class MessageBroadcast
    {
        #region Properties

        public string Message { get; set; }

        #endregion

        #region Constructors

        public MessageBroadcast(string message)
        {
            Message = message;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
                            {
                                Identifier = nameof(MessageBroadcast),
                                Payload = this
                            };

            return container;
        }

        #endregion
    }
}
