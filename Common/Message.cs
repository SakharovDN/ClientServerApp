namespace Common
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Message
    {
        #region Properties

        [Required]
        public string Body { get; set; }

        public string ChatId { get; set; }

        public string SourceName { get; set; }

        public DateTime Timestamp { get; set; }

        #endregion
    }
}
