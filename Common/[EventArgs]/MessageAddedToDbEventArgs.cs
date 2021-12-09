namespace Common
{
    using System;

    public class MessageAddedToDbEventArgs : EventArgs
    {
        #region Properties

        public string ChatId { get; }

        public DateTime Timestamp { get; }

        #endregion

        #region Constructors

        public MessageAddedToDbEventArgs(string chatId, DateTime timestamp)
        {
            ChatId = chatId;
            Timestamp = timestamp;
        }

        #endregion
    }
}
