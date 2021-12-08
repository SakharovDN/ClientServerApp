namespace Common
{
    using System;

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Body { get; }

        public string SourceId { get; }

        public string TargetId { get; }

        public ChatTypes ChatType { get; }

        #endregion

        #region Constructors

        public MessageRequestReceivedEventArgs(string body, string sourceId, string targetId, ChatTypes chatType)
        {
            Body = body;
            SourceId = sourceId;
            TargetId = targetId;
            ChatType = chatType;
        }

        #endregion
    }
}
