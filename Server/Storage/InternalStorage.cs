namespace Server.Storage
{
    using System;
    using System.Collections.Concurrent;
    using System.Data.Entity;
    using System.Threading.Tasks;

    using Common;

    using NLog;

    public class InternalStorage : DbContext
    {
        #region Fields

        private readonly Logger _logger;
        private readonly ConcurrentQueue<QueueItem> _workQueue;
        private Task _queueItemTask;

        #endregion

        #region Properties

        public DbSet<Client> Clients { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<StorageMessage> Messages { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        #endregion

        #region Constructors

        public InternalStorage(string dbConnection)
            : base(dbConnection)
        {
            _workQueue = new ConcurrentQueue<QueueItem>();
            _logger = LogManager.GetCurrentClassLogger();
            //todo: check connection
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
