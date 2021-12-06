namespace Server.Services
{
    using Common;

    public interface IClientService
    {
        #region Methods

        Client GetClientById(string clientId);
        Client GetClientByName(string clientName);

        #endregion
    }
}
