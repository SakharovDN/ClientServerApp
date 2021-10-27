namespace Common.Messages
{
    using _Enums_;

    using Newtonsoft.Json;

    public static class MessageHandler
    {
        #region Methods

        public static object HandleMessageFromServer(string message)
        {
            object deserializedMessage = JsonConvert.DeserializeObject(message);

            return deserializedMessage;
        }

        internal static string SendMessageToServer(string message, MessageTypes type)
        {
            switch (type)
            {
                case MessageTypes.ConnectionRequest:
                    var connectionRequest = new ConnectionRequest
                    {
                        ClientName = message
                    };
                    string result = JsonConvert.SerializeObject(connectionRequest);

                    return result;
                default:
                    return string.Empty;
            }
        }
        
        internal static string HandleMessageFromClient(string message)
        {
            object deserializedMessage = JsonConvert.DeserializeObject(message);

            if (deserializedMessage != null && deserializedMessage.GetType() == typeof(ConnectionRequest))
            {
                var connectionRequest = (ConnectionRequest) deserializedMessage;
                var connectionResponse = new ConnectionResponse
                {
                    Result = ResultCodes.Ok
                };

                if (!WsServer.ClientExists(connectionRequest.ClientName))
                {
                    connectionResponse.Result = ResultCodes.Failure;
                    connectionResponse.Reason = $"Клиент с именем '{connectionRequest.ClientName}' уже подключен.";
                }

                string result = JsonConvert.SerializeObject(connectionResponse);

                return result;
            }

            return string.Empty;
        }

        #endregion
    }
}
