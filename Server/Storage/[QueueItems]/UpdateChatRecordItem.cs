namespace Server.Storage
{
    using System;

    using Common;

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
            Chat chat = storage.Chats.Find(Guid.Parse(_chatId));

            if (chat != null)
            {
                chat.LastMessageTimestamp = _timestamp;
                chat.MessageAmount++;
            }

            storage.SaveChanges();
        }

        #endregion
    }
}
