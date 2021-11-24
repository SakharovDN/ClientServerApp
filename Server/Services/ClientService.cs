namespace Server.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using WebSocketSharp;

    public class ClientService
    {
        #region Fields

        public static List<string> Clients;

        #endregion

        #region Constructors

        public ClientService()
        {
            Clients = new List<string>();
        }

        #endregion

        #region Methods

        public static bool ClientExists(string clientName)
        {
            return Clients.Any(client => client == clientName);
        }

        public static List<string> GetConnectedClients()
        {
            return Clients.ToList();
        }

        public static void Add(string client)
        {
            if (ClientExists(client) || client.IsNullOrEmpty())
            {
                return;
            }

            Clients.Add(client);
        }

        public static void Remove(string client)
        {
            if (!ClientExists(client))
            {
                return;
            }

            Clients.Remove(client);
        }

        #endregion
    }
}
