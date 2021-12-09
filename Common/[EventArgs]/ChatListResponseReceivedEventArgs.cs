namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ChatListResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public List<Chat> Chats { get; }

        #endregion

        #region Constructors

        public ChatListResponseReceivedEventArgs(List<Chat> chats)
        {
            Chats = chats;
        }

        #endregion
    }
}
