namespace Server.Storage
{
    using System;

    using Common;

    internal class UpdateChatRecordItem : QueueItem
    {
        #region Fields

        private readonly string _chatId;
        private readonly string _lastMessageId;

        #endregion

        #region Constructors

        public UpdateChatRecordItem(string chatId, string lastMessageId)
        {
            _chatId = chatId;
            _lastMessageId = lastMessageId;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            Chat chat = storage.Chats.Find(Guid.Parse(_chatId));

            if (chat != null)
            {
                chat.LastMessageId = _lastMessageId;
                chat.MessageAmount++;
            }

            storage.SaveChanges();
        }

        #endregion
    }
}
