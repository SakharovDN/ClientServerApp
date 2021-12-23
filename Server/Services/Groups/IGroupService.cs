namespace Server.Services
{
    using System;

    using Common;

    public interface IGroupService
    {
        #region Events

        event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        event EventHandler<RequestHandledEventArgs> GroupCreationRequestHandled;

        #endregion

        #region Methods

        void HandleGroupCreationRequest(object sender, GroupCreationRequestReceivedEventArgs args);

        #endregion
    }
}
