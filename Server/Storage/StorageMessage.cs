namespace Server.Storage
{
    using Common;

    public class StorageMessage : Message
    {
        #region Properties

        public long Id { get; set; }

        public string SourceId { get; set; }

        #endregion
    }
}
