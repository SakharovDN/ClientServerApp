namespace Common
{
    public class ChatInfoRequestReceivedEventArgs
    {
        #region Properties

        public string ChatId { get; }

        #endregion

        #region Constructors

        public ChatInfoRequestReceivedEventArgs(string chatId)
        {
            ChatId = chatId;
        }

        #endregion
    }
}
