namespace Common
{
    using System;

    public class MessageReceivedEventArgs : EventArgs
    {
        #region Properties

        public Message Message { get; }

        #endregion

        #region Constructors

        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }

        #endregion
    }
}
