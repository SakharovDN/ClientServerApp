namespace Server.Storage
{
    using Common;

    public class AddNewMessageItem : QueueItem
    {
        #region Properties

        public Message Message { get; set; }

        #endregion

        #region Constructors

        public AddNewMessageItem(Message message)
        {
            Message = message;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            storage.MessageContext.AddMessageToDt(Message);
        }

        #endregion
    }
}
