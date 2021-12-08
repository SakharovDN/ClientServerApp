namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ChatListRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientId { get; }

        public List<Group> ClientGroups { get; }

        #endregion

        #region Constructors

        public ChatListRequestReceivedEventArgs(string clientId, List<Group> clientGroups)
        {
            ClientId = clientId;
            ClientGroups = clientGroups;
        }

        #endregion
    }
}
