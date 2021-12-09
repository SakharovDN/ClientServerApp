namespace Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;
    using Common.Messages;

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

        public event EventHandler<ChatHistoryRequestHandledEventArgs> ChatHistoryRequestHandled;

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
                LastMessageTimestamp = DateTime.Now,
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
            _storage.AddQueueItem(new UpdateChatRecordItem(args.ChatId, args.Timestamp));
        }

        public void HandleChatListRequest(object sender, ChatListRequestReceivedEventArgs args)
        {
            var chats = new List<Chat>
            {
                CommonChat
            };
            chats.AddRange(GetPrivateChats(args.ClientId));
            chats.AddRange(GetGroupChats(args.ClientGroups));
            ChatListRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new ChatListResponse(chats).GetContainer()));
        }

        public void HandleChatHistoryRequest(object sender, ChatHistoryRequestReceivedEventArgs args)
        {
            string chatId = null;

            foreach (Chat chat in _storage.Chats)
            {
                if (args.TargetId == chat.TargetId)
                {
                    if (chat.Type == ChatTypes.Common || chat.Type == ChatTypes.Group)
                    {
                        chatId = chat.Id.ToString();
                        break;
                    }

                    if (args.SourceId == chat.SourceId)
                    {
                        chatId = chat.Id.ToString();
                        break;
                    }
                }
                else if (args.TargetId == chat.SourceId)
                {
                    if (args.SourceId == chat.TargetId)
                    {
                        chatId = chat.Id.ToString();
                        break;
                    }
                }
            }

            if (chatId == null)
            {
                return;
            }

            List<Message> chatHistory = _storage.Messages.Where(message => message.ChatId == chatId).ToList();
            MessageContainer chatHistoryResponse = new ChatHistoryResponse(chatHistory).GetContainer();
            ChatHistoryRequestHandled?.Invoke(sender, new ChatHistoryRequestHandledEventArgs(chatHistoryResponse));
        }

        public Chat GetChatById(string chatId)
        {
            return _storage.Chats.Find(Guid.Parse(chatId));
        }

        private IEnumerable<Chat> GetPrivateChats(string clientId)
        {
            return _storage.Chats.Where(chat => chat.SourceId == clientId || chat.TargetId == clientId).ToList();
        }

        private IEnumerable<Chat> GetGroupChats(IEnumerable<Group> clientGroups)
        {
            return (from @group in clientGroups
                    from chat in _storage.Chats
                    where chat.Type == ChatTypes.Group
                    where chat.TargetId == @group.Id.ToString()
                    select chat).ToList();
        }

        #endregion
    }
}
