namespace Server.Services
{
    using System;

    using Common;
    using Common.Messages;

    using Storage;

    public class MessageService : IMessageService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Events

        public event EventHandler<RequestHandledEventArgs> MessageRequestHandled;

        public event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        public event EventHandler<MessageAddedToDbEventArgs> MessageAddedToDb;

        #endregion

        #region Constructors

        public MessageService(InternalStorage storage)
        {
            _storage = storage;
        }

        #endregion

        #region Methods

        public void HandleMessageRequest(object sender, MessageRequestReceivedEventArgs args)
        {
            DateTime timestamp = DateTime.Now;
            Chat chat = _storage.Chats.Find(Guid.Parse(args.ChatId));

            if (chat == null)
            {
                chat = new Chat
                {
                    Id = Guid.NewGuid(),
                    Type = ChatTypes.Private,
                    SourceId = args.SourceId,
                    SourceName = _storage.Clients.Find(Guid.Parse(args.SourceId))?.Name,
                    TargetId = args.ChatId,
                    TargetName = _storage.Clients.Find(Guid.Parse(args.ChatId))?.Name,
                    MessageAmount = 0

                };
                ChatNotExists?.Invoke(sender, new ChatNotExistsEventArgs(chat));
            }

            var message = new StorageMessage
            {
                Id = Guid.NewGuid(),
                Body = args.Body,
                ChatId = chat.Id.ToString(),
                SourceId = args.SourceId,
                SourceName = _storage.Clients.Find(Guid.Parse(args.SourceId))?.Name,
                Timestamp = timestamp
            };
            chat.LastMessage = message;
            _storage.AddQueueItem(new AddNewMessageItem(message));
            MessageAddedToDb?.Invoke(sender, new MessageAddedToDbEventArgs(chat.Id.ToString(), message.Id.ToString()));
            MessageRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new MessageBroadcast(message).GetContainer(), chat));
        }

        #endregion
    }
}
