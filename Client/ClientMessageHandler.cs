namespace Client
{
    using System;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class ClientMessageHandler
    {
        #region Events

        public static event EventHandler<EventLogsReceivedEventArgs> EventLogsReceived;

        public static event EventHandler<ConnectionResponseReceivedEventArgs> ConnectionResponseReceived;

        public static event EventHandler<DisconnectionResponseReceivedEventArgs> DisconnectionResponseReceived;

        public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public static event EventHandler<ConnectionStateChangedEchoReceivedEventArgs> ConnectionStateChangedEchoReceived;

        #endregion

        #region Methods

        public static void HandleMessage(string message)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(message);

            if (container == null)
            {
                return;
            }

            switch (container.Type)
            {
                case MessageTypes.ConnectionResponse:
                    HandleConnectionResponse(container);
                    break;

                case MessageTypes.DisconnectionResponse:
                    HandleDisconnectionResponse(container);
                    break;

                case MessageTypes.MessageBroadcast:
                    HandleMessageBroadcast(container);
                    break;

                case MessageTypes.EventLogsResponse:
                    HandleEventLogsResponse(container);
                    break;

                case MessageTypes.ConnectionStateChangedEcho:
                    HandleConnectionStateChangedEcho(container);
                    break;
            }
        }

        private static void HandleDisconnectionResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(DisconnectionResponse)) is DisconnectionResponse disconnectionResponse)
            {
                DisconnectionResponseReceived?.Invoke(null, new DisconnectionResponseReceivedEventArgs());
            }
        }

        private static void HandleConnectionStateChangedEcho(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionStateChangedEcho)) is ConnectionStateChangedEcho connectionStateChangedEcho)
            {
                ConnectionStateChangedEchoReceived?.Invoke(
                    null,
                    new ConnectionStateChangedEchoReceivedEventArgs(connectionStateChangedEcho.ClientName, connectionStateChangedEcho.IsConnected));
            }
        }

        private static void HandleEventLogsResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(EventLogsResponse)) is EventLogsResponse eventLogsResponse)
            {
                EventLogsReceived?.Invoke(null, new EventLogsReceivedEventArgs(eventLogsResponse.EventLogs));
            }
        }

        private static void HandleMessageBroadcast(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) is MessageBroadcast messageBroadcast)
            {
                MessageReceived?.Invoke(null, new MessageReceivedEventArgs(messageBroadcast.SenderName, messageBroadcast.Message));
            }
        }

        private static void HandleConnectionResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) is ConnectionResponse connectionResponse)
            {
                ConnectionResponseReceived?.Invoke(
                    null,
                    new ConnectionResponseReceivedEventArgs(
                        connectionResponse.Result,
                        connectionResponse.Reason,
                        connectionResponse.ConnectedClients));
            }
        }

        #endregion
    }
}
