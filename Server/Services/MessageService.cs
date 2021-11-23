namespace Server.Services
{
    using System.Data;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using WebSocket;

    public class MessageService
    {
        #region Methods

        public static void HandleMessage(string message, WsChat chat)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            if (container == null)
            {
                return;
            }

            HandleMessageRequest(chat, container);
        }

        public static void HandleMessage(string message, WsConnection connection)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            if (container == null)
            {
                return;
            }

            switch (container.Type)
            {
                case MessageTypes.ConnectionRequest:
                    HandleConnectionRequest(connection, container);
                    break;

                case MessageTypes.DisconnectionRequest:
                    HandleDisconnectionRequest(connection, container);
                    break;

                case MessageTypes.ClientsListRequest:
                    HandleClientsListRequest(connection);
                    break;

                case MessageTypes.EventLogsRequest:
                    HandleEventLogsRequest(connection);
                    break;
            }
        }

        public static void HandleMessageRequest(WsChat chat, MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(MessageRequest)) is MessageRequest messageRequest))
            {
                return;
            }

            string messageBroadcast =
                JsonConvert.SerializeObject(new MessageBroadcast(messageRequest.Message, messageRequest.SenderName).GetContainer());
            chat.BroadcastMessage(messageBroadcast);
        }

        private static void HandleEventLogsRequest(WsConnection connection)
        {
            DataTable eventLogs = EventLogService.GetEventLogs();
            connection.Send(new EventLogsResponse(eventLogs).GetContainer());
        }

        private static void HandleDisconnectionRequest(WsConnection connection, MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(DisconnectionRequest)) is DisconnectionRequest disconnectionRequest))
            {
                return;
            }

            if (!ClientService.ClientExists(disconnectionRequest.ClientName))
            {
                return;
            }

            ClientService.Remove(disconnectionRequest.ClientName);
            EventLogService.AddEventLog($"Client {disconnectionRequest.ClientName} is disconnected");
            connection.Broadcast(new ConnectionStateChangedEcho(disconnectionRequest.ClientName, false).GetContainer());
            connection.Send(new DisconnectionResponse().GetContainer());
        }

        private static void HandleClientsListRequest(WsConnection connection)
        {
            var clientsListResponse = new ClientsListResponse(ClientService.Clients);
            connection.Send(clientsListResponse.GetContainer());
        }

        private static void HandleConnectionRequest(WsConnection connection, MessageContainer container)
        {
            if (!(((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) is ConnectionRequest connectionRequest))
            {
                return;
            }

            var connectionResponse = new ConnectionResponse
            {
                Result = ResultCodes.Ok
            };

            if (ClientService.ClientExists(connectionRequest.ClientName))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{connectionRequest.ClientName}' is already connected.";
            }

            if (connectionResponse.Result == ResultCodes.Ok)
            {
                ClientService.Add(connectionRequest.ClientName);
                EventLogService.AddEventLog($"Client {connectionRequest.ClientName} is connected");
                connection.Broadcast(new ConnectionStateChangedEcho(connectionRequest.ClientName, true).GetContainer());
            }

            connection.Send(connectionResponse.GetContainer());
        }

        #endregion
    }
}
