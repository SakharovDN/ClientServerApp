namespace Server.Services
{
    using System;
    using System.Collections.Generic;

    using Common;

    public interface IGroupService
    {
        #region Events

        event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        #endregion

        #region Methods

        List<Guid> GetClientIds(string groupId);

        void HandleGroupCreationRequest(object sender, GroupCreationRequestReceivedEventArgs args);

        #endregion
    }
}
