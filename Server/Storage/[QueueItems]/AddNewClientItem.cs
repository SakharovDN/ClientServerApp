namespace Server.Storage
{
    using Common;

    public class AddNewClientItem : QueueItem
    {
        #region Fields

        private readonly Client _client;

        #endregion

        #region Constructors

        public AddNewClientItem(Client client)
        {
            _client = client;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            storage.ClientContext.AddNewClientToDt(_client);
        }

        #endregion
    }
}
