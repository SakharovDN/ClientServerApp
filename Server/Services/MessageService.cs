namespace Server.Services
{
    using System.Collections.Generic;

    using Common;

    using Storage;

    public class MessageService
    {
        #region Fields

        private readonly InternalStorage _storage;
        private readonly IClientService _clientService;

        #endregion

        #region Constructors

        public MessageService(InternalStorage storage)
        {
            _storage = storage;
            _clientService = new ClientService(_storage);
        }

        #endregion

        #region Methods

        public void AddNewMessage(Message message)
        {
            _storage.AddQueueItem(new AddNewMessageItem(message));
        }

        public List<Message> GetChatHistory(string chatId)
        {
            var chatHistory = new List<Message>();

            foreach (Message message in _storage.MessageContext.Messages)
            {
                if (message.ChatId != chatId)
                {
                    continue;
                }

                message.SourceName = _clientService.GetClientById(message.SourceId).Name;
                chatHistory.Add(message);
            }

            return chatHistory;
        }

        #endregion
    }
}
