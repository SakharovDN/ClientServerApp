namespace Common.Messages
{
    public class ConnectionResponse
    {
        #region Properties

        public ResultCodes Result { get; set; }

        public string Reason { get; set; }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Type = MessageTypes.ConnectionResponse,
                Payload = this
            };
            return container;
        }

        #endregion
    }
}
