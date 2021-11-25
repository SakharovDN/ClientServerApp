namespace Common
{
    using System;

    using Messages;

    public class MessageContainerReceivedEventArgs : EventArgs
    {
        #region Properties

        public string MessageContainer { get; }

        #endregion

        #region Constructors

        public MessageContainerReceivedEventArgs(string messageContainer)
        {
            MessageContainer = messageContainer;
        }

        #endregion
    }
}
