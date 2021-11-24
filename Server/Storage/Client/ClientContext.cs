namespace Server.Storage.Client
{
    using System;
    using System.Collections.Generic;
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
                Id = Guid.NewGuid(),
                Name = name,
                IsConnected = true
            };
            Clients.Add(client);
            SaveChanges();
        }

        public bool ClientExists(string name)
        {
            return Enumerable.Any(Clients, client => client.Name == name);
        }

        public bool ClientIsConnected(string name)
        {
            return Enumerable.Any(Clients.Where(client => client.Name == name), client => client.IsConnected);
        }

        public List<string> GetConnectedClients()
        {
            return (from client in Clients where client.IsConnected select client.Name).ToList();
        }

        public void ChangeConnectionStatus(string name)
        {
            foreach (Client client in Clients)
            {
                if (client.Name != name)
                {
                    continue;
                }

                client.IsConnected = !client.IsConnected;
                break;
            }

            SaveChanges();
        }

        #endregion
    }
}
