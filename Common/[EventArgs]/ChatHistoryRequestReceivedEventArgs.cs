namespace Common
{
    using System;

    public class ChatHistoryRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string TargetId { get; }

        public string SourceId { get; }

        public ChatTypes ChatType { get; }

        #endregion

        #region Constructors

        public ChatHistoryRequestReceivedEventArgs(string targetId, string sourceId, ChatTypes chatType)
        {
            TargetId = targetId;
            SourceId = sourceId;
            ChatType = chatType;
        }

        #endregion
    }
}
