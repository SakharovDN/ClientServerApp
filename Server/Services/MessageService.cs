namespace Server.Services
{
    using System;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using WebSocket;

    public static class MessageService
    {
        #region Events

        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Methods

        public static void HandleMessage(string message, WsServer server, WsConnection connection)
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

                case MessageTypes.MessageRequest:
                    HandleMessageRequest(container);
                    break;

                case MessageTypes.ClientsListRequest:
                    HandleClientsListRequest(server, connection);
                    break;
            }
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

        private static void HandleMessageRequest(MessageContainer container)
        {
            var messageRequest = ((JObject)container.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
            MessageReceived?.Invoke(null, new MessageReceivedEventArgs(messageRequest?.Client.Name, messageRequest?.Message));
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
