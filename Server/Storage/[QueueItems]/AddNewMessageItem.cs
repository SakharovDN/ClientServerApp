namespace Server.Storage
{
    using Common;

    public class AddNewMessageItem : QueueItem
    {
        #region Fields

        private readonly Message _message;

        #endregion

        #region Constructors

        public AddNewMessageItem(Message message)
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
