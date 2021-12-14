namespace Common
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Chat
    {
        #region Properties

        public Guid Id { get; set; }

        [Required]
        public ChatTypes Type { get; set; }

        public string SourceId { get; set; }

        public string SourceName { get; set; }

        public string TargetId { get; set; }

        public string TargetName { get; set; }

        public string LastMessageId { get; set; }

        [NotMapped]
        public Message LastMessage { get; set; }

        public int MessageAmount { get; set; }

        #endregion
    }
}
