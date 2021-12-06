namespace Server.Storage
{
    using System;

    internal class UpdateChatRecordItem : QueueItem
    {
        #region Fields

        private readonly string _chatId;
        private readonly DateTime _timestamp;

        #endregion

        #region Constructors

        public UpdateChatRecordItem(string chatId, DateTime timestamp)
        {
            _chatId = chatId;
            _timestamp = timestamp;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            storage.ChatContext.UpdateChatRecord(_chatId, _timestamp);
        }

        #endregion
    }
}
