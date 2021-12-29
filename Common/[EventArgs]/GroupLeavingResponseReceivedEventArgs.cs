namespace Common
{
    public class GroupLeavingResponseReceivedEventArgs
    {
        #region Properties

        public string ChatId { get; }

        #endregion

        #region Constructors

        public GroupLeavingResponseReceivedEventArgs(string chatId)
        {
            ChatId = chatId;
        }

        #endregion
    }
}
