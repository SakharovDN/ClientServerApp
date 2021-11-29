namespace Server.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Storage;
    using Storage.Client;
    using Storage.EventLog;

    public class ClientService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Properties

        public List<string> ConnectedClients { get; set; }

        #endregion

        #region Constructors

        public ClientService(InternalStorage storage)
        {
            _storage = storage;
            ConnectedClients = new List<string>();
        }

        #endregion

        #region Methods

        public bool ClientIsConnected(string clientName)
        {
            return ConnectedClients.Any(client => client == clientName);
        }

        public void AddClient(string clientName)
        {
            ConnectedClients.Add(clientName);
            _storage.AddQueueItem(new AddNewClientItem(clientName));
            _storage.AddQueueItem(new AddEventLogItem(clientName, true));
        }

        public void RemoveClient(string clientName)
        {
            ConnectedClients.Remove(clientName);
            _storage.AddQueueItem(new AddEventLogItem(clientName, false));
        }

        #endregion
    }
}
