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

            switch (args.ChatType)
            {
                case ChatTypes.Common:
                    foreach (Chat chat in _storage.Chats)
                    {
                        if (chat.Type == ChatTypes.Common)
                        {
                            targetChat = chat;
                            break;
                        }
                    }

                    break;

                case ChatTypes.Private:
                {
                    foreach (Chat chat in _storage.Chats)
                    {
                        if (chat.SourceId == args.SourceId && chat.TargetId == args.TargetId)
                        {
                            targetChat = chat;
                        }
                        else if (chat.SourceId == args.TargetId && chat.TargetId == args.SourceId)
                        {
                            targetChat = chat;
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

                    break;
                }

                case ChatTypes.Group:
                {
                    foreach (Chat chat in _storage.Chats)
                    {
                        if (chat.TargetId == args.TargetId)
                        {
                            targetChat = chat;
                            break;
                        }
                    }

                    break;
                }
            }

            if (targetChat == null)
            {
                return;
            }

            var message = new Message
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

        private void AddNewMessage(Message message)
        {
            _storage.AddQueueItem(new AddNewMessageItem(message));
        }

        #endregion
    }
}
