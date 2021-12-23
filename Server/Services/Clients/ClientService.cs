namespace Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;
    using Common.Messages;

    using Storage;

    using WebSocket;

    public class ClientService : IClientService
    {
        #region Constants

        private const string COMMON_CHAT_NAME = "Common";

        #endregion

        #region Fields

        private readonly InternalStorage _storage;
        private readonly HashSet<Client> _connectedClients;

        #endregion

        #region Events

        public event EventHandler<RequestHandledEventArgs> ConnectionRequestHandled;

        public event EventHandler<ConnectionStateChangedEventArgs> ClientConnected;

        #endregion

        #region Constructors

        public ClientService(InternalStorage storage)
        {
            _storage = storage;
            _connectedClients = new HashSet<Client>();
        }

        #endregion

        #region Methods

        public void SetClientDisconnected(object sender, ConnectionStateChangedEventArgs args)
        {
            if (args.IsConnected)
            {
                return;
            }

            Client client = GetClientById(args.ClientId);
            _connectedClients.Remove(client);
            _storage.AddQueueItem(new AddEventLogItem(client.Name, args.IsConnected));
        }

        public Client GetClientById(string clientId)
        {
            return _storage.Clients.Find(Guid.Parse(clientId));
        }

        public void HandleConnectionRequest(object sender, ConnectionRequestReceivedEventArgs args)
        {
            var connectionResponse = new ConnectionResponse();

            if (string.Equals(args.ClientName, COMMON_CHAT_NAME, StringComparison.CurrentCultureIgnoreCase))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = "The name \"Common\" is not available. Please choose another name.";
                ConnectionRequestHandled?.Invoke(sender, new RequestHandledEventArgs(connectionResponse.GetContainer()));
                return;
            }

            Client client = GetOrCreateClient(args.ClientName);

            if (ClientIsConnected(client.Id.ToString()))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{client.Name}' is already connected.";
            }
            else
            {
                if (sender is WsConnection connection)
                {
                    connection.ClientId = client.Id.ToString();
                }

                connectionResponse.Result = ResultCodes.Ok;
                connectionResponse.ClientId = client.Id.ToString();
                connectionResponse.ConnectedClients = _connectedClients;
                SetClientConnected(client);
                ClientConnected?.Invoke(sender, new ConnectionStateChangedEventArgs(client.Id.ToString(), true));
            }

            ConnectionRequestHandled?.Invoke(sender, new RequestHandledEventArgs(connectionResponse.GetContainer()));
        }

        private bool ClientIsConnected(string clientId)
        {
            return _connectedClients.Any(client => clientId == client.Id.ToString());
        }

        private Client GetClientByName(string clientName)
        {
            return _storage.Clients.FirstOrDefault(client => client.Name == clientName);
        }

        private void SetClientConnected(Client client)
        {
            _connectedClients.Add(client);
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
