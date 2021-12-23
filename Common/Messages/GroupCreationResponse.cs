namespace Common.Messages
{
    public class GroupCreationResponse
    {
        #region Properties

        public ResultCodes Result { get; set; }

        public string Reason { get; set; }

        #endregion

        #region Constructors

        public GroupCreationResponse(ResultCodes result, string reason)
        {
            Result = result;
            Reason = reason;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.GroupCreationResponse,
                Payload = this
            };
        }

        #endregion
    }
}
