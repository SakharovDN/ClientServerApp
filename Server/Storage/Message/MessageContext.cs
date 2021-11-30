namespace Server.Storage.Message
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

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

        public List<Message> GetCommonChatHistory()
        {
            return Messages.Where(message => message.Target == "Common").ToList();
        }

        public List<Message> GetChatHistory(List<string> participants)
        {
            return Messages.Where(message => participants.Contains(message.Target) && participants.Contains(message.Source)).ToList();
        }

        #endregion
    }
}
