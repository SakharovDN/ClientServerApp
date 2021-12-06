namespace Server.Services
{
    using System;
    using System.Collections.Generic;

    using Common;

    using Storage;

    public class ChatService
    {
        #region Constants

        private const string COMMON_CHAT_NAME = "Common";

        #endregion

        #region Fields

        private readonly InternalStorage _storage;
        private readonly IClientService _clientService;

        #endregion

        #region Constructors

        public ChatService(InternalStorage storage)
        {
            _storage = storage;
            _clientService = new ClientService(_storage);

            if (_storage.ChatContext.CommonChatExists())
            {
                return;
            }

            var commonChat = new Chat
            {
                Id = Guid.NewGuid(),
                Type = ChatTypes.Common,
                SourceId = null,
                TargetId = null,
                LastMessageTimestamp = DateTime.Now,
                MessageAmount = 0
            };
            _storage.ChatContext.AddNewChatToDt(commonChat);
        }

        #endregion

        #region Methods

        public List<Chat> GetAvailableChats(string clientId)
        {
            var availableChats = new List<Chat>();

            foreach (Chat chat in _storage.ChatContext.Chats)
            {
                switch (chat.Type)
                {
                    case ChatTypes.Common:
                        chat.TargetName = COMMON_CHAT_NAME;
                        availableChats.Add(chat);
                        break;

                    case ChatTypes.Private when clientId == chat.TargetId:
                        chat.TargetName = _clientService.GetClientById(chat.SourceId).Name;
                        availableChats.Add(chat);
                        break;

                    case ChatTypes.Private when clientId == chat.SourceId:
                        chat.TargetName = _clientService.GetClientById(chat.TargetId).Name;
                        availableChats.Add(chat);
                        break;
                }
            }

            return availableChats;
        }

        public void CreateNewChat(Chat chat)
        {
            _storage.AddQueueItem(new CreateNewChatItem(chat));
        }

        public void UpdateRecord(string chatId, DateTime timestamp)
        {
            _storage.AddQueueItem(new UpdateChatRecordItem(chatId, timestamp));
        }

        #endregion
    }
}
