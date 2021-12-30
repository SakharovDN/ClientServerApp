namespace Common.Messages
{
    public class EventLogsRequest
    {
        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.EventLogsRequest,
                Payload = this,
            };
        }

        #endregion
    }
}
