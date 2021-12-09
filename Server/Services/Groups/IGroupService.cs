namespace Server.Services
{
    using System;
    using System.Collections.Generic;

    using Common;

    public interface IGroupService
    {
        #region Events

        event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        event EventHandler<RequestHandledEventArgs> GroupListRequestHandled;

        #endregion

        #region Methods

        List<string> GetClientIds(string groupId);

        void HandleGroupCreationRequest(object sender, GroupCreationRequestReceivedEventArgs args);

        void HandleGroupListRequest(object sender, GroupListRequestReceivedEventArgs args);

        #endregion
    }
}
