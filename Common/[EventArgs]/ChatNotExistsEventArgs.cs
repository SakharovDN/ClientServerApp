namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ChatNotExistsEventArgs : EventArgs
    {
        #region Properties

        public Chat Chat { get; }

        public List<string> ClientIds { get; }

        #endregion

        #region Constructors

        public ChatNotExistsEventArgs(Chat chat, List<string> clientIds = null)
        {
            Chat = chat;
            ClientIds = clientIds;
        }

        #endregion
    }
}
