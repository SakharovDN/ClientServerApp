namespace Server.Services
{
    using System;
    using System.Data;

    using Common;
    using Common.EventLog;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using WebSocket;

    public class MessageService
    {
        #region Events

        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public event EventHandler<MessageRequestHandledEventArgs> MessageRequestHandled;

        #endregion

        #region Methods

        public void HandleMessage(string message, WsChat chat)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            if (container == null)
            {
                return;
            }

            HandleMessageRequest(container);
        }

        public void HandleMessage(string message, WsServer server, WsConnection connection)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            if (container == null)
            {
                return;
            }

            switch (container.Type)
            {
                case MessageTypes.ConnectionRequest:
                    HandleConnectionRequest(server, connection, container);
                    break;

                case MessageTypes.DisconnectionRequest:
                    HandleDisconnectionRequest(server, container);
                    break;

                case MessageTypes.ClientsListRequest:
                    HandleClientsListRequest(server, connection);
                    break;

                case MessageTypes.EventLogsRequest:
                    HandleEventLogsRequest(connection);
                    break;
            }
        }

        public void HandleMessageRequest(MessageContainer container)
        {
            var messageRequest = ((JObject)container.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
            string message = $"{messageRequest?.Client.Name}: {messageRequest?.Message}";
            MessageRequestHandled?.Invoke(null, new MessageRequestHandledEventArgs(new MessageBroadcast(message).GetContainer()));
        }

        private static void HandleEventLogsRequest(WsConnection connection)
        {
            DataTable eventLogs;

            using (var db = new EventLogContext())
            {
                eventLogs = db.EventLogs.ToDataTable();
            }

            connection.Send(new EventLogsResponse(eventLogs).GetContainer());
        }

        private static void HandleDisconnectionRequest(WsServer server, MessageContainer container)
        {
            var disconnectionRequest = ((JObject)container.Payload).ToObject(typeof(DisconnectionRequest)) as DisconnectionRequest;

            if (server.ClientService.ClientExists(disconnectionRequest?.Client.Name))
            {
                ConnectionStateChanged?.Invoke(null, new ConnectionStateChangedEventArgs(disconnectionRequest?.Client, false));
            }
        }

        private static void HandleClientsListRequest(WsServer server, WsConnection connection)
        {
            var clientsListResponse = new ClientsListResponse(server.ClientService.Clients);
            connection.Send(clientsListResponse.GetContainer());
        }

        private static void HandleConnectionRequest(WsServer server, WsConnection connection, MessageContainer container)
        {
            var connectionRequest = ((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
            var connectionResponse = new ConnectionResponse
            {
                Result = ResultCodes.Ok
            };

            if (server.ClientService.ClientExists(connectionRequest?.Client.Name))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Клиент с именем '{connectionRequest?.Client.Name}' уже подключен.";
            }
            else
            {
                ConnectionStateChanged?.Invoke(null, new ConnectionStateChangedEventArgs(connectionRequest?.Client, true));
            }

            connection.Send(connectionResponse.GetContainer());
        }

        #endregion
    }
}
