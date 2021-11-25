namespace Server.Services
{
    using System;
    using System.Data;

    using Common;
    using Common.Messages;

    using Storage.EventLog;

    public class EventLogService
    {
        #region Fields

        private readonly EventLogContext _eventLogContext;

        #endregion

        #region Events

        public event EventHandler<ResponseConstructedEventArgs> EventLogsResponseConstructed;

        #endregion

        #region Constructors

        public EventLogService(string dbConnection)
        {
            _eventLogContext = new EventLogContext(dbConnection);
        }

        #endregion

        #region Methods

        public void ConstructEventLogsResponse()
        {
            DataTable eventLogs = _eventLogContext.GetEventLogs();
            MessageContainer eventLogsResponse = new EventLogsResponse(eventLogs).GetContainer();
            EventLogsResponseConstructed?.Invoke(this, new ResponseConstructedEventArgs(eventLogsResponse));
        }

        public void Stop()
        {
            _eventLogContext?.Dispose();
        }

        #endregion
    }
}
