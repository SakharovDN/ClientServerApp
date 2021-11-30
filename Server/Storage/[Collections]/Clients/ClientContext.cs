namespace Server.Storage
{
    using System.Data.Entity;
    using System.Linq;

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

        public void AddNewClientToDt(string name)
        {
            var client = new Client
            {
                Name = name
            };
            Clients.Add(client);
            SaveChanges();
        }

        public bool ClientExists(string name)
        {
            return Enumerable.Any(Clients, client => client.Name == name);
        }

        public int GetClientId(string clientName)
        {
            return Enumerable.FirstOrDefault(from client in Clients where client.Name == clientName select client.Id);
        }

        #endregion
    }
}
