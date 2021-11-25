namespace Server.Services
{
    using System.Collections.Concurrent;
    using System.Timers;

    using Common.Messages;

    public class MessageService
    {
        #region Fields

        private readonly ConcurrentQueue<Request> _messageRequests;
        private readonly Timer _timer;

        #endregion

        #region Constructors

        public MessageService()
        {
            _messageRequests = new ConcurrentQueue<Request>();
            _timer = new Timer(100);
            _timer.Elapsed += ConstructMessageBroadcast;
            _timer.Start();
        }

        #endregion

        #region Methods

        public void EnqueueMessageRequest(Request request)
        {
            _messageRequests.Enqueue(request);
        }

        public void Stop()
        {
            _timer?.Dispose();
        }

        private void ConstructMessageBroadcast(object sender, ElapsedEventArgs e)
        {
            if (!_messageRequests.TryDequeue(out Request request))
            {
                return;
            }

            var messageRequest = (MessageRequest)request.Payload;
            MessageContainer messageBroadcast = new MessageBroadcast(messageRequest.Message, messageRequest.SenderName).GetContainer();
            request.MessageHandler.SendBroadcast(messageBroadcast);
        }

        #endregion
    }
}
