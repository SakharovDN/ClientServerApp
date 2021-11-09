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

        public static event EventHandler<EventLogsReceivedEventArgs> EventLogsReceived;

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

                case MessageTypes.EventLogsResponse:
                    HandleEventLogsResponse(container);
                    break;
            }
        }

        private static void HandleEventLogsResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(EventLogsResponse)) is EventLogsResponse eventLogsResponse)
            {
                EventLogsReceived?.Invoke(null, new EventLogsReceivedEventArgs(eventLogsResponse.EventLogs));
            }
        }

        private static void HandleClientsListResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ClientsListResponse)) is ClientsListResponse clientsListResponse)
            {
                ClientsListReceived?.Invoke(null, new ClientsListReceivedEventArgs(clientsListResponse.Clients));
            }
        }

        private static void HandleMessageBroadcast(WsClient client, MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) is MessageBroadcast messageBroadcast)
            {
                client.Receive(messageBroadcast.Message);
            }
        }

        private static void HandleConnectionResponse(WsClient client, MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) is ConnectionResponse connectionResponse)
                || connectionResponse.Result != ResultCodes.Failure)
            {
                return;
            }

            client.Name = string.Empty;
            client.Receive(connectionResponse.Reason);
        }

        #endregion
    }
}
