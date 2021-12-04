namespace Common
{
    using System;

    using Messages;

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Body { get; }

        public string SourceId { get; }

        public Chat Chat { get; }

        public Action<object, MessageContainer> Send { get; }

        public Action<object, MessageContainer, string> SendTo { get; }

        public Action<MessageContainer> SendBroadcast { get; }

        #endregion

        #region Constructors

        public MessageRequestReceivedEventArgs(
            string body,
            string sourceId,
            Chat chat,
            Action<object, MessageContainer> send,
            Action<object, MessageContainer, string> sendTo,
            Action<MessageContainer> sendBroadcast)
        {
            Body = body;
            SourceId = sourceId;
            Chat = chat;
            Send = send;
            SendTo = sendTo;
            SendBroadcast = sendBroadcast;
        }

        #endregion
    }
}
