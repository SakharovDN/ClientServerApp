namespace Common
{
    public class GroupLeavingRequestReceivedEventArgs
    {
        #region Properties

        public string ChatId { get; }

        public string ClientId { get; }

        #endregion

        #region Constructors

        public GroupLeavingRequestReceivedEventArgs(string chatId, string clientId)
        {
            ChatId = chatId;
            ClientId = clientId;
        }

        #endregion
    }
}
