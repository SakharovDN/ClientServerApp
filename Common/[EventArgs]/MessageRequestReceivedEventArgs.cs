namespace Common
{
    using System;

    public class MessageRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string Body { get; }

        public string SourceId { get; }

        public string ChatId { get; }

        #endregion

        #region Constructors

        public MessageRequestReceivedEventArgs(string body, string sourceId, string chatId)
        {
            Body = body;
            SourceId = sourceId;
            ChatId = chatId;
        }

        #endregion
    }
}
