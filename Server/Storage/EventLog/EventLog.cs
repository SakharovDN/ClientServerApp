namespace Server.Storage.EventLog
{
    using System;

    public class EventLog
    {
        #region Properties

        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

        #endregion
    }
}
