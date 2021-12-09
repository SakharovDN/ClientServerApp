namespace Common
{
    using System;

    public class ChatNotExistsEventArgs : EventArgs
    {
        #region Properties

        public Chat Chat { get; }

        #endregion

        #region Constructors

        public ChatNotExistsEventArgs(Chat chat)
        {
            Chat = chat;
        }

        #endregion
    }
}
