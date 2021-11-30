namespace Server.Storage
{
    public abstract class QueueItem
    {
        #region Methods

        public abstract void Accept(InternalStorage storage);

        #endregion
    }
}
