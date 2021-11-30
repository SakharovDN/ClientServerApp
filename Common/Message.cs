namespace Common
{
    using System;

    public class Message
    {
        #region Properties

        public int Id { get; set; }

        public string Body { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}
