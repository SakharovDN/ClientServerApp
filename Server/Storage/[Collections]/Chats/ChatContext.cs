namespace Server.Storage
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Common;

    public class ChatContext : DbContext
    {
        #region Properties

        public DbSet<Chat> Chats { get; set; }

        #endregion

        #region Constructors

        public ChatContext(string dbConnection)
            : base(dbConnection)
        {
        }

        #endregion

        #region Methods

        public void AddNewChatToDt(Chat chat)
        {
            Chats.Add(chat);
            SaveChanges();
        }

        public void UpdateChatRecord(string chatId, DateTime timestamp)
        {
            Chat chat = Chats.Find(Guid.Parse(chatId));

            if (chat != null)
            {
                chat.LastMessageTimestamp = timestamp;
                chat.MessageAmount++;
            }

            SaveChanges();
        }

        public bool CommonChatExists()
        {
            return Enumerable.Any(Chats, chat => chat.Type == ChatTypes.Common);
        }

        #endregion
    }
}
