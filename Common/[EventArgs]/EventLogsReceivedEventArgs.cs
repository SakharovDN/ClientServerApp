namespace Common
{
    using System;
    using System.Collections.Generic;

    public class EventLogsReceivedEventArgs : EventArgs
    {
        #region Properties

        public List<EventLog> EventLogs { get; }

        #endregion

        #region Constructors

        public EventLogsReceivedEventArgs(List<EventLog> eventLogs)
        {
            EventLogs = eventLogs;
        }

        #endregion
    }
}
