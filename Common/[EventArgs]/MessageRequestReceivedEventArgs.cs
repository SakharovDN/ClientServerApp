namespace Common
{
    using System;

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Body { get; }

        public string SourceId { get; }

        public string TargetId { get; }

        #endregion

        #region Constructors

        public MessageRequestReceivedEventArgs(string body, string sourceId, string targetId)
        {
            Body = body;
            SourceId = sourceId;
            TargetId = targetId;
        }

        #endregion
    }
}
