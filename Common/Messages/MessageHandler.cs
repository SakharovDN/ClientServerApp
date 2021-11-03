namespace Common.Messages
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class MessageHandler
    {
        #region Events

        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

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
                    var connectionResponse = ((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;
                    HandleConnectionResponse(client, connectionResponse);
                    break;

                case MessageTypes.MessageBroadcast:
                    var messageBroadcast = ((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) as MessageBroadcast;
                    HandleMessageBroadcast(client, messageBroadcast);
                    break;

                case MessageTypes.ClientsListResponse:
                    var clientsListResponse = ((JObject)container.Payload).ToObject(typeof(ClientsListResponse)) as ClientsListResponse;
                    HandleClientsListResponse(clientsListResponse);
                    break;
            }
        }

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
                    var connectionRequest = ((JObject)container.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
                    HandleConnectionRequest(server, connection, connectionRequest);
                    break;

                case MessageTypes.DisconnectionRequest:
                    var disconnectionRequest = ((JObject)container.Payload).ToObject(typeof(DisconnectionRequest)) as DisconnectionRequest;
                    HandleDisconnectionRequest(server, disconnectionRequest);
                    break;

                case MessageTypes.MessageRequest:
                    var messageRequest = ((JObject)container.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
                    HandleMessageRequest(messageRequest);
                    break;

                case MessageTypes.ClientsListRequest:
                    HandleClientsListRequest(server, connection);
                    break;
            }
        }
        
        private static void HandleDisconnectionRequest(WsServer server, DisconnectionRequest disconnectionRequest)
        {
            if (server.ClientService.ClientExists(disconnectionRequest.Client.Name))
            {
                ConnectionStateChanged?.Invoke(null, new ConnectionStateChangedEventArgs(disconnectionRequest.Client, false));
            }
        }

        private static void HandleClientsListRequest(WsServer server, WsConnection connection)
        {
            var clientsListResponse = new ClientsListResponse(server.ClientService.Clients);
            connection.Send(clientsListResponse.GetContainer());
        }

        private static void HandleMessageRequest(MessageRequest messageRequest)
        {
            MessageReceived?.Invoke(null, new MessageReceivedEventArgs(messageRequest.Client.Name, messageRequest.Message));
        }

        private static void HandleConnectionRequest(WsServer server, WsConnection connection, ConnectionRequest connectionRequest)
        {
            var connectionResponse = new ConnectionResponse
            {
                Result = ResultCodes.Ok
            };

            if (server.ClientService.ClientExists(connectionRequest.Client.Name))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Клиент с именем '{connectionRequest.Client.Name}' уже подключен.";
                connection.Send(connectionResponse.GetContainer());
            }
            else
            {
                connection.Send(connectionResponse.GetContainer());
                ConnectionStateChanged?.Invoke(null, new ConnectionStateChangedEventArgs(connectionRequest.Client, true));
            }
        }

        private static void HandleClientsListResponse(ClientsListResponse clientsListResponse)
        {
            ClientsListReceived?.Invoke(null, new ClientsListReceivedEventArgs(clientsListResponse.Clients));
        }

        private static void HandleMessageBroadcast(WsClient client, MessageBroadcast messageBroadcast)
        {
            client.Receive(messageBroadcast.Message);
        }

        private static void HandleConnectionResponse(WsClient client, ConnectionResponse connectionResponse)
        {
            if (connectionResponse != null && connectionResponse.Result == ResultCodes.Failure)
            {
                client.Name = string.Empty;
                client.Receive(connectionResponse.Reason);
            }
        }

        #endregion
    }
}
