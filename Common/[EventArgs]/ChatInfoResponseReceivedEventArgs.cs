namespace Common
{
    public class ChatInfoResponseReceivedEventArgs
    {
        #region Properties

        public Group Group { get; }

        #endregion

        #region Constructors

        public ChatInfoResponseReceivedEventArgs(Group group)
        {
            Group = group;
        }

        #endregion
    }
}
