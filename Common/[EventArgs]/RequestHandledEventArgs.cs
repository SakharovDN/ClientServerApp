namespace Common
{
    using System;
    using System.Collections.Generic;

    using Messages;

    public class RequestHandledEventArgs : EventArgs
    {
        #region Properties

        public Chat Chat { get; }

        public MessageContainer Response { get; }

        public List<string> ClientIds { get; }

        #endregion

        #region Constructors

        public RequestHandledEventArgs(MessageContainer response, Chat chat = null, List<string> clientIds = null)
        {
            Response = response;
            Chat = chat;
            ClientIds = clientIds;
        }

        #endregion
    }
}
