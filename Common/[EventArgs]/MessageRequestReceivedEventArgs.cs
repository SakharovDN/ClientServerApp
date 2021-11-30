namespace Common
{
    using System;

    using Messages;

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Body { get; }

        public string Source { get; }

        public string Target { get; }

        public Action<object, MessageContainer> Send { get; }

        public Action<object, MessageContainer, string> SendTo { get; }

        public Action<object, MessageContainer> SendBroadcast { get; }

        #endregion

        #region Constructors

        public MessageRequestReceivedEventArgs(
            string body,
            string source,
            string target,
            Action<object, MessageContainer> send,
            Action<object, MessageContainer, string> sendTo,
            Action<object, MessageContainer> sendBroadcast)
        {
            Body = body;
            Source = source;
            Target = target;
            Send = send;
            SendTo = sendTo;
            SendBroadcast = sendBroadcast;
        }

        #endregion
    }
}
