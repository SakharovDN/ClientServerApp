namespace Server.Storage
{
    using System;

    using Common;

    public class StorageMessage : Message
    {
        #region Properties

        public Guid Id { get; set; }

        public string SourceId { get; set; }

        #endregion
    }
}
