namespace Common.Messages
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Services;

    public static class MessageHandler
    {
        #region Events

        public static event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

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
                    var connectionResponse = ((JObject) container.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;

                    if (connectionResponse != null && connectionResponse.Result == ResultCodes.Failure)
                    {
                        client.Login = string.Empty;
                        MessageReceived?.Invoke(client, new MessageReceivedEventArgs(client.Login, connectionResponse.Reason));
                    }

                    ConnectionStateChanged?.Invoke(client, new ConnectionStateChangedEventArgs(client.Login, true));
                    ClientService.Add(client);
                    break;
            }
        }

        public static void HandleMessage(string message, WsConnection connection)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            switch (container.Type)
            {
                case MessageTypes.ConnectionRequest:
                    var connectionRequest = ((JObject) container.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
                    var connectionResponse = new ConnectionResponse
                    {
                        Result = ResultCodes.Ok
                    };

                    if (ClientService.ClientExists(connection.Login))
                    {
                        connectionResponse.Result = ResultCodes.Failure;
                        connectionResponse.Reason = $"Клиент с именем '{connectionRequest.Login}' уже подключен.";
                        connection.Send(connectionResponse.GetContainer());
                    }
                    else
                    {
                        connection.Login = connectionRequest.Login;
                        connection.Send(connectionResponse.GetContainer());
                    }

                    break;
            }
        }

        #endregion
    }
}
