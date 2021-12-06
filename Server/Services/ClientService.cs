namespace Server.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using Storage;

    public class ClientService : IClientService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Properties

        public List<Client> ConnectedClients { get; set; }

        #endregion

        #region Constructors

        public ClientService(InternalStorage storage)
        {
            _storage = storage;
            ConnectedClients = new List<Client>();
        }

        #endregion

        #region Methods

        public bool ClientIsConnected(string clientId)
        {
            return ConnectedClients.Any(connectedClient => clientId == connectedClient.Id.ToString());
        }

        public void SetClientConnected(Client client)
        {
            ConnectedClients.Add(client);
            _storage.AddQueueItem(new AddEventLogItem(client.Name, true));
        }

        public void SetClientDisconnected(Client client)
        {
            ConnectedClients.Remove(client);
            _storage.AddQueueItem(new AddEventLogItem(client.Name, false));
        }

        public void CreateNewClient(Client client)
        {
            _storage.AddQueueItem(new AddNewClientItem(client));
        }

        public Client GetClientById(string clientId)
        {
            return _storage.ClientContext.GetClientById(clientId);
        }

        public Client GetClientByName(string clientName)
        {
            return _storage.ClientContext.GetClientByName(clientName);
        }

        #endregion
    }
}
