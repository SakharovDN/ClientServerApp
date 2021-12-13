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
            Chat targetChat = null;

            foreach (Chat chat in _storage.Chats)
            {
                if (args.TargetId == chat.TargetId)
                {
                    if (chat.Type == ChatTypes.Common || chat.Type == ChatTypes.Group)
                    {
                        targetChat = chat;
                        break;
                    }

                    if (args.SourceId == chat.SourceId)
                    {
                        targetChat = chat;
                        break;
                    }
                }
                else if (args.TargetId == chat.SourceId)
                {
                    if (args.SourceId == chat.TargetId)
                    {
                        targetChat = chat;
                        break;
                    }
                }
            }

            if (targetChat == null)
            {
                targetChat = new Chat
                {
                    Id = Guid.NewGuid(),
                    Type = ChatTypes.Private,
                    SourceId = args.SourceId,
                    SourceName = _storage.Clients.Find(Guid.Parse(args.SourceId))?.Name,
                    TargetId = args.TargetId,
                    TargetName = _storage.Clients.Find(Guid.Parse(args.TargetId))?.Name,
                    LastMessageTimestamp = timestamp,
                    MessageAmount = 0
                };
                ChatNotExists?.Invoke(sender, new ChatNotExistsEventArgs(targetChat));
            }

            var message = new StorageMessage
            {
                Body = args.Body,
                ChatId = targetChat.Id.ToString(),
                SourceId = args.SourceId,
                SourceName = _storage.Clients.Find(Guid.Parse(args.SourceId))?.Name,
                Timestamp = timestamp
            };
            _storage.AddQueueItem(new AddNewMessageItem(message));
            MessageAddedToDb?.Invoke(sender, new MessageAddedToDbEventArgs(targetChat.Id.ToString(), timestamp));
            MessageRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new MessageBroadcast(message).GetContainer(), targetChat));
        }

        #endregion
    }
}
