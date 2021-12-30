namespace Server.Services

{
    using System;

    using Common;

    public interface IChatService
    {
        #region Events

        event EventHandler<RequestHandledEventArgs> NewChatCreated;

        event EventHandler<RequestHandledEventArgs> ChatListRequestHandled;

        event EventHandler<RequestHandledEventArgs> ChatHistoryRequestHandled;

        event EventHandler<RequestHandledEventArgs> ChatInfoRequestHandled;

        #endregion

        #region Methods

        void CreateNewChat(object sender, ChatNotExistsEventArgs args);

        void UpdateChatRecord(object sender, MessageAddedToDbEventArgs args);

        void HandleChatListRequest(object sender, ChatListRequestReceivedEventArgs args);

        void HandleChatHistoryRequest(object sender, ChatHistoryRequestReceivedEventArgs args);

        void HandleChatInfoRequest(object sender, ChatInfoRequestReceivedEventArgs args);

        #endregion
    }
}
