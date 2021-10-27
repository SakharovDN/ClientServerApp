namespace Common.Messages
{
    public class ConnectionRequest
    {
        #region Properties

        public string Login { get; set; }

        #endregion

        #region Constructors

        public ConnectionRequest(string login)
        {
            Login = login;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
                            {
                                Identifier = nameof(ConnectionRequest),
                                Payload = this
                            };

            return container;
        }

        #endregion
    }
}
