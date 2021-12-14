namespace Common
{
    using System;

    public class MessageAddedToDbEventArgs : EventArgs
    {
        #region Properties

        public string ChatId { get; }

        public string LastMessageId { get; }

        #endregion

        #region Constructors

        public MessageAddedToDbEventArgs(string chatId, string lastMessageId)
        {
            ChatId = chatId;
            LastMessageId = lastMessageId;
        }

        #endregion
    }
}
