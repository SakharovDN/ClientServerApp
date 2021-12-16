namespace Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using Storage;

    public class ChatService : IChatService
    {
        #region Constants

        private const string COMMON_CHAT_NAME = "Common";

        #endregion

        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Properties

        public Chat CommonChat { get; }

        #endregion

        #region Events

        public event EventHandler<RequestHandledEventArgs> NewChatCreated;

        public event EventHandler<RequestHandledEventArgs> ChatListRequestHandled;

        public event EventHandler<RequestHandledEventArgs> ChatHistoryRequestHandled;

        #endregion

        #region Constructors

        public ChatService(InternalStorage storage)
        {
            _storage = storage;
            CommonChat = _storage.Chats.FirstOrDefault(chat => chat.Type == ChatTypes.Common);

            if (CommonChat != null)
            {
                return;
            }

            CommonChat = new Chat
            {
                Id = Guid.NewGuid(),
                Type = ChatTypes.Common,
                SourceId = null,
                SourceName = null,
                TargetId = null,
                TargetName = COMMON_CHAT_NAME,
                MessageAmount = 0
            };
            CreateNewChat(null, new ChatNotExistsEventArgs(CommonChat));
        }

        #endregion

        #region Methods

        public void CreateNewChat(object sender, ChatNotExistsEventArgs args)
        {
            _storage.AddQueueItem(new CreateNewChatItem(args.Chat));
            MessageContainer chatCreatedBroadcast = new ChatCreatedBroadcast(args.Chat).GetContainer();
            NewChatCreated?.Invoke(sender, new RequestHandledEventArgs(chatCreatedBroadcast, args.Chat));
        }

        public void UpdateChatRecord(object sender, MessageAddedToDbEventArgs args)
        {
            _storage.AddQueueItem(new UpdateChatRecordItem(args.ChatId, args.LastMessageId));
        }

        public void HandleChatListRequest(object sender, ChatListRequestReceivedEventArgs args)
        {
            List<Chat> chats = GetClientsChats(args.ClientId);
            ChatListRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new ChatListResponse(chats).GetContainer()));
        }

        public void HandleChatHistoryRequest(object sender, ChatHistoryRequestReceivedEventArgs args)
        {
            Chat chat = _storage.Chats.Find(Guid.Parse(args.ChatId));
            List<Message> chatHistory = _storage.Messages.Where(message => message.ChatId == chat.Id.ToString()).ToList().Cast<Message>().ToList();
            ChatHistoryRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new ChatHistoryResponse(chatHistory).GetContainer()));
        }

        private List<Chat> GetClientsChats(string clientId)
        {
            var chats = new List<Chat>();

            foreach (Chat chat in _storage.Chats)
            {
                if (chat.Type == ChatTypes.Common)
                {
                    if (chat.LastMessageId != null)
                    {
                        chat.LastMessage = _storage.Messages.Find(Guid.Parse(chat.LastMessageId));
                    }

                    chats.Add(chat);
                }

                if (chat.Type != ChatTypes.Private || chat.SourceId != clientId && chat.TargetId != clientId)
                {
                    continue;
                }

                foreach (StorageMessage message in _storage.Messages)
                {
                    if (chat.LastMessageId != message.Id.ToString())
                    {
                        continue;
                    }

                    chat.LastMessage = message;
                    break;
                }

                chats.Add(chat);
            }

            foreach (Group group in _storage.Groups)
            {
                var groupClientIds = JsonConvert.DeserializeObject<List<string>>(group.ClientIds);

                if (groupClientIds != null && !groupClientIds.Contains(clientId))
                {
                    continue;
                }

                foreach (Chat chat in _storage.Chats)
                {
                    if (chat.TargetId != group.Id.ToString())
                    {
                        continue;
                    }

                    if (chat.LastMessageId != null)
                    {
                        chat.LastMessage = _storage.Messages.Find(Guid.Parse(chat.LastMessageId));
                    }

                    chats.Add(chat);
                }
            }

            return chats;
        }

        #endregion
    }
}
