namespace Common
{
    public class ClientsListItemsRemovedEventArgs
    {
        #region Properties

        public string ClientName { get; set; }

        #endregion

        #region Constructors

        public ClientsListItemsRemovedEventArgs(string clientName)
        {
            ClientName = clientName;
        }

        #endregion
    }
}
