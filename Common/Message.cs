namespace Common
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        #region Properties

        public long Id { get; set; }

        public string Body { get; set; }

        public string ChatId { get; set; }

        public string SourceId { get; set; }

        [NotMapped]
        public string SourceName { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}
