namespace Server
{
    public class Request
    {
        #region Properties

        public object Payload { get; set; }

        public MessageHandler MessageHandler { get; set; }

        #endregion

        #region Constructors

        public Request(object payload, MessageHandler messageHandler)
        {
            Payload = payload;
            MessageHandler = messageHandler;
        }

        #endregion
    }
}
