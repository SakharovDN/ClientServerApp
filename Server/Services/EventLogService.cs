namespace Server.Services
{
    using System.Data;

    using Storage.EventLog;

    public class EventLogService
    {
        #region Fields

        private static EventLogContext _eventLogsDt;

        #endregion

        #region Constructors

        public EventLogService(string dbConnection)
        {
            _eventLogsDt = new EventLogContext(dbConnection);
        }

        #endregion

        #region Methods

        public static void AddEventLog(string message)
        {
            _eventLogsDt.AddEventLogToDt(message);
        }

        public static DataTable GetEventLogs()
        {
            return _eventLogsDt.EventLogs.ToDataTable();
        }

        #endregion
    }
}
