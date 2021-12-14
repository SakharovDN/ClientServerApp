namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ChatListRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientId { get; }

        #endregion

        #region Constructors

        public ChatListRequestReceivedEventArgs(string clientId)
        {
            ClientId = clientId;
        }

        #endregion
    }
}
