namespace Client.Messenger
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Common;

    using NewGroupWindow;

    public class Messenger : ViewModelBase
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
        private ObservableCollection<Client> _connectedClientsCollection;
        private Client _connectedClientsCollectionSelectedItem;
        private ObservableCollection<Chat> _chatsCollection;
        private Chat _chatsCollectionSelectedItem;
        private CommandHandler _sendCommand;
        private CommandHandler _createNewGroupCommand;
        private Visibility _messageVisibility;

        #endregion

        #region Properties

        public ICommand SendCommand => _sendCommand ?? (_sendCommand = new CommandHandler(PerformSendButton));

        public ICommand CreateNewGroupCommand => _createNewGroupCommand ?? (_createNewGroupCommand = new CommandHandler(CreateNewGroup));

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

        public ObservableCollection<Client> ConnectedClientsCollection
        {
            get => _connectedClientsCollection;
            set
            {
                _connectedClientsCollection = value;
                OnPropertyChanged();
            }
        }

        public Client ConnectedClientsCollectionSelectedItem
        {
            get => _connectedClientsCollectionSelectedItem;
            set
            {
                _connectedClientsCollectionSelectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Chat> ChatsCollection
        {
            get => _chatsCollection;
            set
            {
                _chatsCollection = value;
                OnPropertyChanged();
            }
        }

        public Chat ChatsCollectionSelectedItem
        {
            get => _chatsCollectionSelectedItem;
            set
            {
                _chatsCollectionSelectedItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public Messenger(WsClient client)
        {
            _client = client;
            _client.MessageHandler.MessageReceived += HandleMessageReceived;
            _client.MessageHandler.ConnectionResponseReceived += HandleConnectionResponseReceived;
            _client.MessageHandler.ChatListResponseReceived += HandledChatListResponseReceived;
            _client.MessageHandler.ConnectionStateChangedBroadcastReceived += HandleConnectionStateChangedBroadcastReceived;
            _client.MessageHandler.ChatHistoryReceived += HandleChatHistoryReceived;
            _client.MessageHandler.ChatCreatedBroadcastReceived += HandleChatCreatedBroadcastReceived;
            ConnectedClientsCollection = new ObservableCollection<Client>();
            MessagesCollection = new ObservableCollection<ClientMessage>();
            ChatsCollection = new ObservableCollection<Chat>();
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

        private void PerformSendButton(object commandParameter)
        {
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            Message = _messageRegex.Replace(Message, REPLACEMENT).Trim();

            if (ChatsCollectionSelectedItem != null)
            {
                _client.SendMessage(
                    ChatsCollectionSelectedItem.TargetId == _client.Id ? ChatsCollectionSelectedItem.SourceId : ChatsCollectionSelectedItem.TargetId,
                    Message);
            }
            else if (ConnectedClientsCollectionSelectedItem != null)
            {
                _client.SendMessage(ConnectedClientsCollectionSelectedItem.Id.ToString(), Message);
            }

            Message = string.Empty;
        }

        private void CreateNewGroup(object obj)
        {
            var newGroupWindow = new CreateNewGroupWindow(ConnectedClientsCollection);

            if (newGroupWindow.ShowDialog() == true)
            {
                _client.RequestGroupCreation(newGroupWindow.GroupTitle, newGroupWindow.SelectedClients);
            }
        }

        private void HandleConnectionResponseReceived(object sender, ConnectionResponseReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Result == ResultCodes.Failure)
                    {
                        return;
                    }

                    foreach (Client client in args.ConnectedClients.Where(client => client.Name != _client.Name))
                    {
                        ConnectedClientsCollection.Add(client);
                    }
                });
        }

        private void HandledChatListResponseReceived(object sender, ChatListResponseReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    foreach (Chat chat in args.Chats)
                    {
                        if (chat.TargetId == _client.Id)
                        {
                            chat.TargetName = chat.SourceName;
                        }

                        ChatsCollection.Add(chat);
                    }

                    RefreshChatsCollection();
                });
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    foreach (Chat chat in ChatsCollection)
                    {
                        if (chat.Id.ToString() != args.Message.ChatId) //?????id?
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

        private void HandleConnectionStateChangedBroadcastReceived(object sender, ConnectionStateChangedBroadcastReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Client.Name == _client.Name)
                    {
                        return;
                    }

                    if (args.IsConnected)
                    {
                        ConnectedClientsCollection.Add(args.Client);
                    }
                    else
                    {
                        ConnectedClientsCollection.Remove(ConnectedClientsCollection.FirstOrDefault(client => client.Id == args.Client.Id));
                    }
                });
        }

        private void HandleChatHistoryReceived(object sender, ChatHistoryReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    MessagesCollection.Clear();

                    if (args.ChatHistory == null || args.ChatHistory.Count == 0)
                    {
                        return;
                    }

                    foreach (Message message in args.ChatHistory)
                    {
                        MessagesCollection.Add(new ClientMessage(message, _client.Name));
                    }

                    MessagesCollection = new ObservableCollection<ClientMessage>(MessagesCollection.OrderBy(message => message.Timestamp));
                });
        }

        private void HandleChatCreatedBroadcastReceived(object sender, ChatCreatedBroadcastReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Chat.TargetId == _client.Id)
                    {
                        args.Chat.TargetName = args.Chat.SourceName;
                    }

                    ChatsCollection.Add(args.Chat);
                    RefreshChatsCollection();

                    if (ConnectedClientsCollectionSelectedItem == null)
                    {
                        return;
                    }

                    if (args.Chat.TargetName != ConnectedClientsCollectionSelectedItem.Name)
                    {
                        return;
                    }

                    ChatsCollectionSelectedItem = args.Chat;
                    ConnectedClientsCollectionSelectedItem = null;
                });
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(ChatsCollectionSelectedItem) when ChatsCollectionSelectedItem == null:
                    break;

                case nameof(ChatsCollectionSelectedItem) when ChatsCollectionSelectedItem != null:
                    ConnectedClientsCollectionSelectedItem = null;
                    MessageVisibility = Visibility.Visible;
                    _client.RequestChatHistory(
                        _client.Id == ChatsCollectionSelectedItem.TargetId
                            ? ChatsCollectionSelectedItem.SourceId
                            : ChatsCollectionSelectedItem.TargetId);
                    break;

                case nameof(ConnectedClientsCollectionSelectedItem) when ConnectedClientsCollectionSelectedItem == null:
                    break;

                case nameof(ConnectedClientsCollectionSelectedItem) when ConnectedClientsCollectionSelectedItem != null:
                {
                    MessagesCollection.Clear();

                    foreach (Chat chat in ChatsCollection)
                    {
                        if (ConnectedClientsCollectionSelectedItem.Name != chat.TargetName)
                        {
                            continue;
                        }

                        ChatsCollectionSelectedItem = chat;
                        ConnectedClientsCollectionSelectedItem = null;
                        goto End;
                    }

                    ChatsCollectionSelectedItem = null;
                    MessageVisibility = Visibility.Visible;
                    End: 
                    break;
                }
            }
        }

        private void RefreshChatsCollection()
        {
            ChatsCollection = new ObservableCollection<Chat>(ChatsCollection.OrderByDescending(chat => chat.LastMessage.Timestamp));
            CollectionViewSource.GetDefaultView(ChatsCollection).Refresh();
        }

        #endregion
    }
}
