namespace Common
{
    using System.Data;

    public class EventLogsReceivedEventArgs
    {
        #region Properties

        public DataTable EventLogs { get; }

        #endregion

        #region Constructors

        public EventLogsReceivedEventArgs(DataTable eventLogs)
        {
            EventLogs = eventLogs;
        }

        #endregion
    }
}
