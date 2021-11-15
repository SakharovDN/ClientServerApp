namespace Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using WebSocketSharp;

    public static class ClientService
    {
        #region Fields

        public static Dictionary<Guid, WsClient> Clients;

        #endregion

        #region Methods

        public static bool ClientExists(string clientName)
        {
            return Clients.Any(client => client.Value.Name == clientName);
        }

        public static void Add(WsClient client)
        {
            if (ClientExists(client.Name) || client.Name.IsNullOrEmpty())
            {
                return;
            }

            Clients.Add(client.Id, client);
        }

        public static void Remove(WsClient client)
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
