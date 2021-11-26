namespace Common
{
    using System;

    using Messages;

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string SenderName { get; }

        public string Message { get; }

        public Action<object, MessageContainer> SendBroadcast { get; }

        #endregion

        #region Constructors

        public MessageRequestReceivedEventArgs(string senderName, string message, Action<object, MessageContainer> sendBroadcast)
        {
            SenderName = senderName;
            Message = message;
            SendBroadcast = sendBroadcast;
        }

        #endregion
    }
}
