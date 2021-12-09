namespace Server.Storage
{
    using Common;

    public class CreateNewChatItem : QueueItem
    {
        #region Fields

        private readonly Chat _chat;

        #endregion

        #region Constructors

        public CreateNewChatItem(Chat chat)
        {
            _chat = chat;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            storage.Chats.Add(_chat);
            storage.SaveChanges();
        }

        #endregion
    }
}
