namespace Common.Messages
{
    public class MessageContainer
    {
        #region Properties

        public MessageTypes Type { get; set; }

        public object Payload { get; set; }

        #endregion
    }
}
