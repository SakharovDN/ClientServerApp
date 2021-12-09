namespace Server.Services
{
    using System;

    using Common;

    public interface IMessageService
    {
        #region Events

        event EventHandler<RequestHandledEventArgs> MessageRequestHandled;

        event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        event EventHandler<MessageAddedToDbEventArgs> MessageAddedToDb;

        #endregion

        #region Methods

        void HandleMessageRequest(object sender, MessageRequestReceivedEventArgs args);

        #endregion
    }
}
