namespace Common
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Chat
    {
        #region Properties

        public Guid Id { get; set; }

        public ChatTypes Type { get; set; }

        public string SourceId { get; set; }

        public string TargetId { get; set; }

        [NotMapped]
        public string TargetName { get; set; }

        public DateTime LastMessageTimestamp { get; set; }

        public int MessageAmount { get; set; }

        #endregion
    }
}
