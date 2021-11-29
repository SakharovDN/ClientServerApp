namespace Server.Storage.Client
{
    public class AddNewClientItem : QueueItem
    {
        #region Fields

        private readonly string _clientName;

        #endregion

        #region Constructors

        public AddNewClientItem(string clientName)
        {
            _clientName = clientName;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            if (!storage.ClientContext.ClientExists(_clientName))
            {
                storage.ClientContext.AddNewClientToDt(_clientName);
            }
        }

        #endregion
    }
}
