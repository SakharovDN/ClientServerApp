namespace Server.Storage.Message
{
    using System;

    public class Message
    {
        #region Properties

        public Guid Id { get; set; }

        public string Body { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid ClientId { get; set; }

        public Guid ChatId { get; set; }

        #endregion
    }
}
