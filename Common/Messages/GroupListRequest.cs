namespace Common.Messages
{
    public class GroupListRequest
    {
        #region Properties

        public string ClientId { get; set; }

        #endregion

        #region Constructors

        public GroupListRequest(string clientId)
        {
            ClientId = clientId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.GroupListRequest,
                Payload = this
            };
        }

        #endregion
    }
}
