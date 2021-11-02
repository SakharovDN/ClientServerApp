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

        public MessageContainer GetContainer()
        {
            return new MessageContainer()
            {
                Type = MessageTypes.ConnectionRequest,
                Payload = this
            };
        }
        #endregion
    }
}
