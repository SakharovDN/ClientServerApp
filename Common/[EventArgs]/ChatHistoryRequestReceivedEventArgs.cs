namespace Common
{
    using System;

    public class ChatHistoryRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string TargetId { get; }

        public string SourceId { get; }

        #endregion

        #region Constructors

        public ChatHistoryRequestReceivedEventArgs(string targetId, string sourceId)
        {
            TargetId = targetId;
            SourceId = sourceId;
        }

        #endregion
    }
}
