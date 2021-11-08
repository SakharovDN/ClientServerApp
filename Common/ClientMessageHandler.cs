namespace Common
{
    using System;

    using Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class ClientMessageHandler
    {
        #region Events

        public static event EventHandler<ClientsListReceivedEventArgs> ClientsListReceived;

        #endregion

        #region Methods

        public static void HandleMessage(string message, WsClient client)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            if (container == null)
            {
                return;
            }

            switch (container.Type)
            {
                case MessageTypes.ConnectionResponse:
                    HandleConnectionResponse(client, container);
                    break;

                case MessageTypes.MessageBroadcast:
                    HandleMessageBroadcast(client, container);
                    break;

                case MessageTypes.ClientsListResponse:
                    HandleClientsListResponse(container);
                    break;
            }
        }

        private static void HandleClientsListResponse(MessageContainer container)
        {
            var clientsListResponse = ((JObject)container.Payload).ToObject(typeof(ClientsListResponse)) as ClientsListResponse;
            ClientsListReceived?.Invoke(null, new ClientsListReceivedEventArgs(clientsListResponse.Clients));
        }

        private static void HandleMessageBroadcast(WsClient client, MessageContainer container)
        {
            var messageBroadcast = ((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) as MessageBroadcast;
            client.Receive(messageBroadcast?.Message);
        }

        private static void HandleConnectionResponse(WsClient client, MessageContainer container)
        {
            var connectionResponse = ((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;

            if (connectionResponse == null || connectionResponse.Result != ResultCodes.Failure)
            {
                return;
            }

            client.Name = string.Empty;
            client.Receive(connectionResponse.Reason);
        }

        #endregion
    }
}
