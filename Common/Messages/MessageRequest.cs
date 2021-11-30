namespace Common.Messages
{
    public class MessageRequest
    {
        #region Properties

        public string Body { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        #endregion

        #region Constructors

        public MessageRequest(string body, string source, string target)
        {
            Body = body;
            Source = source;
            Target = target;
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
