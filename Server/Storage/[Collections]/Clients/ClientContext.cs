namespace Server.Storage
{
    using System.Data.Entity;
    using System.Linq;

    using Common;

    public class ClientContext : DbContext
    {
        #region Properties

        public DbSet<Client> Clients { get; set; }

        #endregion

        #region Constructors

        public ClientContext(string dbConnection)
            : base(dbConnection)
        {
        }

        #endregion

        #region Methods

        public void AddNewClientToDt(Client client)
        {
            Clients.Add(client);
            SaveChanges();
        }

        public bool ClientExists(string clientName)
        {
            return Enumerable.Any(Clients, client => client.Name == clientName);
        }

        public Client GetClientById(string clientId)
        {
            return Enumerable.FirstOrDefault(Clients, client => client.Id.ToString() == clientId);
        }

        public Client GetClientByName(string clientName)
        {
            return Enumerable.FirstOrDefault(Clients, client => client.Name == clientName);
        }

        #endregion
    }
}
