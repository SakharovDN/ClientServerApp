namespace Common.Messages
{
    using _Enums_;

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
                                Identifier = nameof(ConnectionResponse),
                                Payload = this
                            };

            return container;
        }

        #endregion
    }
}
