namespace Server.Services
{
    using System;

    using Common;

    public interface IGroupService
    {
        #region Events

        event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        #endregion

        #region Methods

        void HandleGroupCreationRequest(object sender, GroupCreationRequestReceivedEventArgs args);

        #endregion
    }
}
