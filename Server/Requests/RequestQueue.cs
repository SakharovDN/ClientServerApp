namespace Server.Requests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Timers;

    using Common;

    using Timer = System.Timers.Timer;

    public class RequestQueue
    {
        #region Fields

        private readonly ConcurrentQueue<Request> _requests;
        private readonly Timer _timer;
        private int _handling;

        #endregion

        #region Events

        public event EventHandler<RequestDequeuedEventArgs> RequestDequeued;

        #endregion

        #region Constructors

        public RequestQueue()
        {
            _requests = new ConcurrentQueue<Request>();
            _handling = 0;
            _timer = new Timer(100);
            _timer.Elapsed += DequeueRequest;
            _timer.Start();
        }

        #endregion

        #region Methods

        public void EnqueueRequest(object sender, RequestReceivedEventArgs args)
        {
            _requests.Enqueue(new Request(args.ConnectionId, args.SerializedRequest));
        }

        private void DequeueRequest(object sender, ElapsedEventArgs args)
        {
            if (_requests.TryDequeue(out Request request) && Interlocked.CompareExchange(ref _handling, 1, 0) == 0)
            {
                RequestDequeued?.Invoke(null, new RequestDequeuedEventArgs(request.ConnectionId, request.SerializedRequest));
                _handling = 0;
            }
        }

        #endregion
    }
}
