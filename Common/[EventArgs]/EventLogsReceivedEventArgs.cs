namespace Common
{
    using System;
    using System.Data;

    public class EventLogsReceivedEventArgs : EventArgs
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
