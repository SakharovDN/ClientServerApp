namespace Common
{
    public class MessageReceivedEventArgs
    {
        #region Properties

        public string ClientName { get; }

        public string Message { get; }

        #endregion

        #region Constructors

        public MessageReceivedEventArgs(string clientName, string message)
        {
            ClientName = clientName;
            Message = message;
        }

        #endregion
    }
}
