namespace Client
{
    using System;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MessageHandler
    {
        #region Events

        public event EventHandler<EventLogsReceivedEventArgs> EventLogsReceived;

        public event EventHandler<ConnectionResponseReceivedEventArgs> ConnectionResponseReceived;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<ConnectionStateChangedBroadcastReceivedEventArgs> ConnectionStateChangedEchoReceived;

        public event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;

        public event EventHandler<ChatCreatedBroadcastReceivedEventArgs> ChatCreatedEchoReceived;

        #endregion

        #region Methods

        public void HandleMessageContainer(object sender, MessageContainerReceivedEventArgs e)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(e.MessageContainer);

            if (container == null)
            {
                return;
            }

            switch (container.Type)
            {
                case MessageTypes.ConnectionResponse:
                    HandleConnectionResponse(container);
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

                case MessageTypes.ChatHistoryResponse:
                    HandleChatHistoryResponse(container);
                    break;

                case MessageTypes.ChatCreatedEcho:
                    HandleChatCreatedEcho(container);
                    break;
            }
        }

        private void HandleConnectionStateChangedEcho(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionStateChangedBroadcast)) is ConnectionStateChangedBroadcast
                connectionStateChangedEcho)
            {
                ConnectionStateChangedEchoReceived?.Invoke(
                    null,
                    new ConnectionStateChangedBroadcastReceivedEventArgs(connectionStateChangedEcho.Client, connectionStateChangedEcho.IsConnected));
            }
        }

        private void HandleEventLogsResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(EventLogsResponse)) is EventLogsResponse eventLogsResponse)
            {
                EventLogsReceived?.Invoke(null, new EventLogsReceivedEventArgs(eventLogsResponse.EventLogs));
            }
        }

        private void HandleMessageBroadcast(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(MessageBroadcast)) is MessageBroadcast messageBroadcast)
            {
                MessageReceived?.Invoke(null, new MessageReceivedEventArgs(messageBroadcast.Message));
            }
        }

        private void HandleConnectionResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionResponse)) is ConnectionResponse connectionResponse)
            {
                ConnectionResponseReceived?.Invoke(
                    null,
                    new ConnectionResponseReceivedEventArgs(
                        connectionResponse.Result,
                        connectionResponse.Reason,
                        connectionResponse.AvailableChats,
                        connectionResponse.ConnectedClients,
                        connectionResponse.ClientId,
                        connectionResponse.KeepAliveInterval));
            }
        }

        private void HandleChatHistoryResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ChatHistoryResponse)) is ChatHistoryResponse chatHistoryResponse)
            {
                ChatHistoryReceived?.Invoke(null, new ChatHistoryReceivedEventArgs(chatHistoryResponse.ChatHistory));
            }
        }

        private void HandleChatCreatedEcho(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ChatCreatedBroadcast)) is ChatCreatedBroadcast chatCreatedEcho)
            {
                ChatCreatedEchoReceived?.Invoke(null, new ChatCreatedBroadcastReceivedEventArgs(chatCreatedEcho.Chat));
            }
        }

        #endregion
    }
}
