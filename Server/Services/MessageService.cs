namespace Server.Services
{
    using Common;

    using Storage;

    public class MessageService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Constructors

        public MessageService(InternalStorage storage)
        {
            _storage = storage;
        }

        #endregion

        #region Methods

        public void AddNewMessage(Message message)
        {
            _storage.AddQueueItem(new AddNewMessageItem(message));
        }

        #endregion
    }
}
