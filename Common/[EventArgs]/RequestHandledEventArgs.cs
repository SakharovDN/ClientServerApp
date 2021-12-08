namespace Common
{
    using System;

    using Messages;

    public class RequestHandledEventArgs : EventArgs
    {
        #region Properties

        public Chat Chat { get; }

        public MessageContainer Response { get; }

        #endregion

        #region Constructors

        public RequestHandledEventArgs(MessageContainer response, Chat chat = null)
        {
            Chat = chat;
            Response = response;
        }

        #endregion
    }
}
