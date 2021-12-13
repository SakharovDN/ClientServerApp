namespace Server.Storage
{
    public class AddNewMessageItem : QueueItem
    {
        #region Fields

        private readonly StorageMessage _message;

        #endregion

        #region Constructors

        public AddNewMessageItem(StorageMessage message)
        {
            _message = message;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            storage.Messages.Add(_message);
        }

        #endregion
    }
}
