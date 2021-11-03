namespace Common.Messages
{
    public class ClientsListRequest
    {
        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ClientsListRequest,
                Payload = this
            };
        }

        #endregion
    }
}
