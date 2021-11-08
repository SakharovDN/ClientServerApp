namespace Common
{
    public class ClientsListItemsAddedEventArgs
    {
        #region Properties

        public string ClientName { get; set; }

        #endregion

        #region Constructors

        public ClientsListItemsAddedEventArgs(string clientName)
        {
            ClientName = clientName;
        }

        #endregion
    }
}
