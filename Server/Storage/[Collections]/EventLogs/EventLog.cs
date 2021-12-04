namespace Server.Storage
{
    using System;

    public class EventLog
    {
        #region Properties

        public long Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        #endregion
    }
}
