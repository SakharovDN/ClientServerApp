namespace Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using WebSocketSharp;

    public class ClientService
    {
        #region Fields

        public Dictionary<Guid, WsClient> Clients;

        #endregion

        #region Constructors

        public ClientService()
        {
            Clients = new Dictionary<Guid, WsClient>();
        }

        #endregion

        #region Methods

        public bool ClientExists(string clientName)
        {
            return Clients.Any(client => client.Value.Name == clientName);
        }

        public void Add(WsClient client)
        {
            if (ClientExists(client.Name) || client.Name.IsNullOrEmpty())
            {
                return;
            }

            Clients.Add(client.Id, client);
        }

        public void Remove(WsClient client)
        {
            if (!ClientExists(client.Name))
            {
                return;
            }

            Clients.Remove(client.Id);
        }

        #endregion
    }
}
