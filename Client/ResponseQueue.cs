namespace Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Timers;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Timer = System.Timers.Timer;

    public class ResponseQueue
    {
        #region Fields

        private readonly ConcurrentQueue<MessageContainer> _responses;
        private readonly Timer _timer;
        private int _handling;

        #endregion

        #region Events

        public event EventHandler<EventLogsReceivedEventArgs> EventLogsReceived;

        public event EventHandler<ConnectionResponseReceivedEventArgs> ConnectionResponseReceived;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler<ConnectionStateChangedBroadcastReceivedEventArgs> ConnectionStateChangedBroadcastReceived;

        public event EventHandler<ChatHistoryReceivedEventArgs> ChatHistoryReceived;

        public event EventHandler<ChatCreatedBroadcastReceivedEventArgs> ChatCreatedBroadcastReceived;

        public event EventHandler<ChatListResponseReceivedEventArgs> ChatListResponseReceived;

        #endregion

        #region Constructors

        public ResponseQueue()
        {
            _responses = new ConcurrentQueue<MessageContainer>();
            _handling = 0;
            _timer = new Timer(100);
            _timer.Elapsed += DequeueResponse;
        }

        #endregion

        #region Methods

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void EnqueueResponse(object sender, MessageContainerReceivedEventArgs args)
        {
            var container = JsonConvert.DeserializeObject<MessageContainer>(args.MessageContainer);

            if (container == null)
            {
                return;
            }

            _responses.Enqueue(container);
        }

        private void DequeueResponse(object sender, ElapsedEventArgs args)
        {
            if (_responses.TryDequeue(out MessageContainer container) && Interlocked.CompareExchange(ref _handling, 1, 0) == 0)
            {
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
                    case MessageTypes.ConnectionStateChangedBroadcast:
                        HandleConnectionStateChangedBroadcast(container);
                        break;
                    case MessageTypes.ChatHistoryResponse:
                        HandleChatHistoryResponse(container);
                        break;
                    case MessageTypes.ChatCreatedBroadcast:
                        HandleChatCreatedBroadcast(container);
                        break;
                    case MessageTypes.ChatListResponse:
                        HandleChatListResponse(container);
                        break;
                }

                _handling = 0;
            }
        }

        private void HandleChatListResponse(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ChatListResponse)) is ChatListResponse chatListResponse)
            {
                ChatListResponseReceived?.Invoke(null, new ChatListResponseReceivedEventArgs(chatListResponse.Chats));
            }
        }

        private void HandleConnectionStateChangedBroadcast(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ConnectionStateChangedBroadcast)) is ConnectionStateChangedBroadcast
                connectionStateChangedBroadcast)
            {
                ConnectionStateChangedBroadcastReceived?.Invoke(
                    null,
                    new ConnectionStateChangedBroadcastReceivedEventArgs(
                        connectionStateChangedBroadcast.Client,
                        connectionStateChangedBroadcast.IsConnected));
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

        private void HandleChatCreatedBroadcast(MessageContainer container)
        {
            if (((JObject)container.Payload).ToObject(typeof(ChatCreatedBroadcast)) is ChatCreatedBroadcast chatCreatedBroadcast)
            {
                ChatCreatedBroadcastReceived?.Invoke(null, new ChatCreatedBroadcastReceivedEventArgs(chatCreatedBroadcast.Chat));
            }
        }

        #endregion
    }
}
