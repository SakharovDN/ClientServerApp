namespace Server.Storage.Client
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ClientContext : DbContext
    {
        #region Properties

        public DbSet<Client> Clients { get; set; }

        #endregion

        #region Constructors

        public ClientContext()
            : base("DbConnection")
        {
        }

        #endregion

        #region Methods

        public void AddToClientDt(string name)
        {
            Client client = Clients.FirstOrDefault(c => c.Name == name);

            if (client != null)
            {
                return;
            }

            client = new Client
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsOnline = true
            };
            Clients.Add(client);
            SaveChanges();
        }

        public void ChangeActivityStatus(Guid id)
        {
            Client client = Clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
            {
                return;
            }

            client.IsOnline = !client.IsOnline;
            SaveChanges();
        }
        #endregion
    }
}
