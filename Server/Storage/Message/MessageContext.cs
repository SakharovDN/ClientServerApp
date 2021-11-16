namespace Server.Storage.Message
{
    using System;
    using System.Data.Entity;

    public class MessageContext : DbContext
    {
        #region Properties

        public DbSet<Message> Messages { get; set; }

        #endregion

        #region Constructors

        public MessageContext()
            : base("DbConnection")
        {
        }

        #endregion

        #region Methods

        public void AddToMessageDt(string body, Guid clientId, Guid chatId)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Body = body,
                Timestamp = DateTime.Now,
                ClientId = clientId,
                ChatId = chatId
            };
            Messages.Add(message);
            SaveChanges();
        }

        #endregion
    }
}
