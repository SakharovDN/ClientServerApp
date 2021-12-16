namespace Client.Messenger
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;

    using Common;

    public partial class MessengerViewModel : ViewModelBase
    {
        #region Constants

        private const string PATTERN = @"[ ]{2,}";
        private const string REPLACEMENT = @" ";

        #endregion

        #region Fields

        private readonly Regex _messageRegex;
        private readonly WsClient _client;
        private string _message;
        private ObservableCollection<ClientMessage> _messagesCollection;
        private CommandHandler _sendCommand;
        private Visibility _messageVisibility;

        #endregion

        #region Properties

        public ICommand SendCommand => _sendCommand ?? (_sendCommand = new CommandHandler(PerformSendButton));

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public Visibility MessageVisibility
        {
            get => _messageVisibility;
            set
            {
                _messageVisibility = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ClientMessage> MessagesCollection
        {
            get => _messagesCollection;
            set
            {
                _messagesCollection = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public MessengerViewModel(WsClient client)
        {
            _client = client;
            _client.MessageHandler.MessageReceived += HandleReceivedMessage;
            _client.MessageHandler.ConnectionResponseReceived += HandleConnectionResponse;
            _client.MessageHandler.ChatListResponseReceived += HandledChatListResponse;
            _client.MessageHandler.ConnectionStateChangedBroadcastReceived += HandleConnectionStateChangedBroadcast;
            _client.MessageHandler.ChatHistoryReceived += ShowChatHistory;
            _client.MessageHandler.ChatCreatedBroadcastReceived += HandleChatCreatedBroadcast;
            ConnectedClientsCollection = new ObservableCollection<Client>();
            MessagesCollection = new ObservableCollection<ClientMessage>();
            ChatsCollection = new ObservableCollection<Chat>();
            ChatsCollectionSelectedItem = new Chat();
            MessageVisibility = Visibility.Hidden;
            _messageRegex = new Regex(PATTERN, RegexOptions.None);
            PropertyChanged += HandlePropertyChanged;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            ChatsCollection.Clear();
            ConnectedClientsCollection.Clear();
            MessagesCollection.Clear();
            MessageVisibility = Visibility.Hidden;
        }

        private void PerformSendButton(object obj)
        {
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            Message = _messageRegex.Replace(Message, REPLACEMENT).Trim();
            _client.SendMessage(
                ChatsCollectionSelectedItem != null
                    ? ChatsCollectionSelectedItem.Id.ToString()
                    : _selectedClient.Id.ToString(),
                Message);
            Message = string.Empty;
        }

        private void HandleReceivedMessage(object sender, MessageReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    foreach (Chat chat in ChatsCollection)
                    {
                        if (chat.Id.ToString() != args.Message.ChatId)
                        {
                            continue;
                        }

                        chat.LastMessage = args.Message;
                        break;
                    }

                    RefreshChatsCollection();

                    if (ChatsCollectionSelectedItem == null)
                    {
                        return;
                    }

                    if (args.Message.ChatId != ChatsCollectionSelectedItem.Id.ToString())
                    {
                        return;
                    }

                    MessagesCollection.Add(new ClientMessage(args.Message, _client.Name));
                    MessagesCollection = new ObservableCollection<ClientMessage>(MessagesCollection.OrderBy(message => message.Timestamp));
                });
        }

        #endregion
    }
}
