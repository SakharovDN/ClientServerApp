namespace Server.Storage
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    using Client;

    using EventLog;

    using Message;

    using NLog;

    public class InternalStorage
    {
        #region Fields

        private readonly Logger _logger;
        private readonly ConcurrentQueue<QueueItem> _workQueue;
        private Task _queueItemTask;

        #endregion

        #region Properties

        public ClientContext ClientContext { get; set; }

        public EventLogContext EventLogContext { get; set; }

        public MessageContext MessageContext { get; set; }

        #endregion

        #region Constructors

        public InternalStorage(string dbServerName)
        {
            _workQueue = new ConcurrentQueue<QueueItem>();
            _logger = LogManager.GetCurrentClassLogger();
            string dbConnection = GetDbConnectionString(dbServerName);
            //todo: check connection
            ClientContext = new ClientContext(dbConnection);
            EventLogContext = new EventLogContext(dbConnection);
            MessageContext = new MessageContext(dbConnection);
        }

        #endregion

        #region Methods

        internal void AddQueueItem(QueueItem queueItem)
        {
            _workQueue.Enqueue(queueItem);

            lock (this)
            {
                if (_queueItemTask == null || _queueItemTask.IsCompleted)
                {
                    _queueItemTask = Task.Run(HandleQueueItems);
                }
            }
        }

        private string GetDbConnectionString(string dbServerName)
        {
            return $"Data Source={dbServerName};Initial Catalog=ClientServerApp;Integrated Security=True";
        }

        private void HandleQueueItems()
        {
            while (_workQueue.TryDequeue(out QueueItem queueItem))
            {
                try
                {
                    queueItem.Accept(this);
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"{ex}");
                }
            }
        }

        #endregion
    }
}
