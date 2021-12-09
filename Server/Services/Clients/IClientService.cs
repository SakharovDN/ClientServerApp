namespace Server.Services
{
    using System;

    using Common;

    public interface IClientService
    {
        #region Events

        event EventHandler<ConnectionRequestHandledEventArgs> ConnectionRequestHandled;

        #endregion

        #region Methods

        Client GetClientById(string clientId);

        void HandleConnectionRequest(object sender, ConnectionRequestReceivedEventArgs args);

        bool ClientIsConnected(string clientId);

        void SetClientDisconnected(Client client);

        #endregion
    }
}
