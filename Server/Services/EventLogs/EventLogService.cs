namespace Server.Services
{
    using System;
    using System.Linq;

    using Common;
    using Common.Messages;

    using Storage;

    public class EventLogService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Events

        public event EventHandler<RequestHandledEventArgs> EventLogRequestHandled;

        #endregion

        #region Constructors

        public EventLogService(InternalStorage storage)
        {
            _storage = storage;
        }

        #endregion

        #region Methods

        public void HandleEventLogsRequest(object sender, EventArgs args)
        {
            MessageContainer eventLogsResponse = new EventLogsResponse(_storage.EventLogs.ToList()).GetContainer();
            EventLogRequestHandled?.Invoke(sender, new RequestHandledEventArgs(eventLogsResponse));
        }

        #endregion
    }
}
