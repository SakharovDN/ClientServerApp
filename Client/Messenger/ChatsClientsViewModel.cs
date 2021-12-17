namespace Client.Messenger
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Common;

    using ConnectedClients;

    using NewGroupWindow;

    public partial class MessengerViewModel
    {
        #region Fields

        private ObservableCollection<Client> _connectedClientsCollection;
        private ObservableCollection<Chat> _chatsCollection;
        private Chat _chatsCollectionSelectedItem;
        private CommandHandler _createNewGroupCommand;
        private CommandHandler _showConnectedClientsCommand;
        private Client _selectedClient;

        #endregion

        #region Properties

        public ObservableCollection<Client> ConnectedClientsCollection
        {
            get => _connectedClientsCollection;
            set
            {
                _connectedClientsCollection = value;
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

        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateNewGroupCommand => _createNewGroupCommand ?? (_createNewGroupCommand = new CommandHandler(CreateNewGroup));

        public ICommand ShowConnectedClientsCommand =>
            _showConnectedClientsCommand ?? (_showConnectedClientsCommand = new CommandHandler(ShowConnectedClients));

        #endregion

        #region Methods

        private void ShowConnectedClients(object obj)
        {
            var connectedClientWindow = new ConnectedClientsWindow(ConnectedClientsCollection);

            if (connectedClientWindow.ShowDialog() != true)
            {
                return;
            }

            ChatsCollectionSelectedItem = null;

            foreach (Chat chat in ChatsCollection)
            {
                if (chat.TargetName != connectedClientWindow.SelectedClient.Name)
                {
                    continue;
                }

                ChatsCollectionSelectedItem = chat;
                break;
            }

            if (ChatsCollectionSelectedItem == null)
            {
                SelectedClient = connectedClientWindow.SelectedClient;
            }
        }

        private void CreateNewGroup(object obj)
        {
            var newGroupWindow = new CreateNewGroupWindow(ConnectedClientsCollection);

            if (newGroupWindow.ShowDialog() == true)
            {
                _client.RequestGroupCreation(newGroupWindow.GroupTitle, newGroupWindow.SelectedClients);
            }
        }

        private void HandleConnectionResponse(object sender, ConnectionResponseReceivedEventArgs args)
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

        private void HandledChatListResponse(object sender, ChatListResponseReceivedEventArgs args)
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

        private void HandleConnectionStateChangedBroadcast(object sender, ConnectionStateChangedBroadcastReceivedEventArgs args)
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

        private void ShowChatHistory(object sender, ChatHistoryReceivedEventArgs args)
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

        private void HandleChatCreatedBroadcast(object sender, ChatCreatedBroadcastReceivedEventArgs args)
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

                    if (args.Chat.TargetName == SelectedClient?.Name)
                    {
                        ChatsCollectionSelectedItem = args.Chat;
                    }
                });
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(ChatsCollectionSelectedItem) when ChatsCollectionSelectedItem != null:
                    SelectedClient = null;
                    MessageVisibility = Visibility.Visible;
                    _client.RequestChatHistory(ChatsCollectionSelectedItem.Id.ToString());
                    break;
                case nameof(SelectedClient) when SelectedClient != null:
                    ChatsCollectionSelectedItem = null;
                    MessagesCollection.Clear();
                    MessageVisibility = Visibility.Visible;
                    break;
                case nameof(ChatsCollectionSelectedItem):
                case nameof(SelectedClient) when SelectedClient == null && ChatsCollectionSelectedItem == null:
                    MessagesCollection.Clear();
                    MessageVisibility = Visibility.Hidden;
                    break;
            }
        }

        private void RefreshChatsCollection()
        {
            ChatsCollection = new ObservableCollection<Chat>(ChatsCollection.OrderByDescending(chat => chat.LastMessage?.Timestamp));
            CollectionViewSource.GetDefaultView(ChatsCollection).Refresh();
        }

        #endregion
    }
}
