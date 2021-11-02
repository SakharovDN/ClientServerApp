namespace Common.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using WebSocketSharp;

    public static class ClientService
    {
        #region Properties

        public static List<WsClient> Clients = new List<WsClient>();

        #endregion

        #region Methods

        public static List<WsClient> GetClients()
        {
            return Clients;
        }

        public static bool ClientExists(string login)
        {
            return Clients.Any(client => client.Login == login);
        }

        public static void Add(WsClient client)
        {
            if (ClientExists(client.Login) || client.Login.IsNullOrEmpty())
            {
                return;
            }

            Clients.Add(client);
        }

        public static void Remove(WsClient client)
        {
            if (ClientExists(client.Login))
            {
                Clients.Remove(client);
            }
        }
        #endregion
    }
}
