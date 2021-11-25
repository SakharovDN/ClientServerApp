namespace Common
{
    using System;

    using Messages;

    public class ResponseConstructedEventArgs : EventArgs
    {
        #region Properties

        public MessageContainer MessageContainer { get; }

        #endregion

        #region Constructors

        public ResponseConstructedEventArgs(MessageContainer messageContainer)
        {
            MessageContainer = messageContainer;
        }

        #endregion
    }
}
