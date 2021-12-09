namespace Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;
    using Common.Messages;

    using Storage;

    public class ClientService : IClientService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Properties

        public List<Client> ConnectedClients { get; set; }

        #endregion

        #region Events

        public event EventHandler<ConnectionRequestHandledEventArgs> ConnectionRequestHandled;

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
            return ConnectedClients.Any(client => clientId == client.Id.ToString());
        }

        public Client GetClientById(string clientId)
        {
            return _storage.Clients.Find(Guid.Parse(clientId));
        }

        public void SetClientDisconnected(Client client)
        {
            ConnectedClients.Remove(client);
            _storage.AddQueueItem(new AddEventLogItem(client.Name, false));
        }

        public void HandleConnectionRequest(object sender, ConnectionRequestReceivedEventArgs args)
        {
            Client client = GetOrCreateClient(args.ClientName);
            var connectionResponse = new ConnectionResponse();

            if (ClientIsConnected(client.Id.ToString()))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{client.Name}' is already connected.";
            }
            else
            {
                connectionResponse.Result = ResultCodes.Ok;
                connectionResponse.ClientId = client.Id.ToString();
                connectionResponse.ConnectedClients = ConnectedClients;
                SetClientConnected(client);
            }

            ConnectionRequestHandled?.Invoke(sender, new ConnectionRequestHandledEventArgs(client, connectionResponse));
        }

        private Client GetClientByName(string clientName)
        {
            return _storage.Clients.FirstOrDefault(client => client.Name == clientName);
        }

        private void SetClientConnected(Client client)
        {
            ConnectedClients.Add(client);
            _storage.AddQueueItem(new AddEventLogItem(client.Name, true));
        }

        private void CreateNewClient(Client client)
        {
            _storage.AddQueueItem(new AddNewClientItem(client));
        }

        private Client GetOrCreateClient(string clientName)
        {
            var client = new Client
            {
                Name = clientName
            };

            if (!ClientExists(client.Name))
            {
                client.Id = Guid.NewGuid();
                CreateNewClient(client);
            }
            else
            {
                client = GetClientByName(client.Name);
            }

            return client;
        }

        private bool ClientExists(string clientName)
        {
            return _storage.Clients.Any(client => client.Name == clientName);
        }

        #endregion
    }
}
