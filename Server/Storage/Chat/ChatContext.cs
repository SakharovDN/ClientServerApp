namespace Server.Storage.Chat
{
    using System;
    using System.Data.Entity;

    public class ChatContext : DbContext
    {
        #region Properties

        public DbSet<Chat> Chats { get; set; }

        #endregion

        #region Constructors

        public ChatContext()
            : base("DbConnection")
        {
        }

        #endregion

        #region Methods

        public void AddToChatDt(string name)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Name = name
            };
            Chats.Add(chat);
            SaveChanges();
        }

        #endregion
    }
}
