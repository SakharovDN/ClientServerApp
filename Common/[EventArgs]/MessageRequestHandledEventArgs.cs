namespace Common
{
    using Messages;

    public class MessageRequestHandledEventArgs
    {
        #region Properties

        public MessageContainer MessageBroadcast { get; set; }

        #endregion

        #region Constructors

        public MessageRequestHandledEventArgs(MessageContainer messageBroadcast)
        {
            MessageBroadcast = messageBroadcast;
        }

        #endregion
    }
}
