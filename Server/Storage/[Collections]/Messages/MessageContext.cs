namespace Server.Storage
{
    using System.Data.Entity;

    using Common;

    public class MessageContext : DbContext
    {
        #region Properties

        public DbSet<Message> Messages { get; set; }

        #endregion

        #region Constructors

        public MessageContext(string dbConnection)
            : base(dbConnection)
        {
        }

        #endregion

        #region Methods

        public void AddMessageToDt(Message message)
        {
            Messages.Add(message);
            SaveChanges();
        }

        #endregion
    }
}
