namespace Common.Messages
{
    public class DisconnectionResponse
    {
        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.DisconnectionResponse,
                Payload = this
            };
        }

        #endregion
    }
}
