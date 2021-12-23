namespace Server.Services
{
    using System;

    using Common;

    public interface IClientService
    {
        #region Events

        event EventHandler<RequestHandledEventArgs> ConnectionRequestHandled;

        event EventHandler<ConnectionStateChangedEventArgs> ClientConnected;

        #endregion

        #region Methods

        Client GetClientById(string clientId);

        void HandleConnectionRequest(object sender, ConnectionRequestReceivedEventArgs args);
        void SetClientDisconnected(object sender, ConnectionStateChangedEventArgs args);

        #endregion
    }
}
