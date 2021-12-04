namespace Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Annotations;

    using Common;

    public class Messenger : INotifyPropertyChanged
    {
        #region Constants

        private const string PATTERN = @"[ ]{2,}";
        private const string REPLACEMENT = @" ";

        #endregion

        #region Fields

        private readonly Regex _messageRegex;
        private readonly WsClient _client;
        private string _message;
        private ObservableCollection<Message> _messagesCollection;
        private ObservableCollection<Client> _connectedClientsCollection;
        private Client _connectedClientsCollectionSelectedItem;
        private ObservableCollection<Chat> _chatsCollection;
        private Chat _chatsCollectionSelectedItem;
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

        public ObservableCollection<Message> MessagesCollection
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

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public Messenger(WsClient client)
        {
            _client = client;
            _client.MessageHandler.MessageReceived += HandleMessageReceived;
            _client.MessageHandler.ConnectionResponseReceived += HandleConnectionResponseReceived;
            _client.MessageHandler.ConnectionStateChangedEchoReceived += HandleConnectionStateChangedEchoReceived;
            _client.MessageHandler.ChatHistoryReceived += HandleChatHistoryReceived;
            _client.MessageHandler.ChatCreatedEchoReceived += HandleChatCreatedEchoReceived;
            ConnectedClientsCollection = new ObservableCollection<Client>();
            MessagesCollection = new ObservableCollection<Message>();
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PerformSendButton(object commandParameter)
        {
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            Message = _messageRegex.Replace(Message, REPLACEMENT).Trim();
            Chat chat = ChatsCollectionSelectedItem ?? new Chat
            {
                Id = Guid.Empty,
                Type = ChatTypes.Private,
                SourceId = _client.Id,
                TargetId = ConnectedClientsCollectionSelectedItem.Id.ToString()
            };
            _client.SendMessage(Message, chat);
            Message = string.Empty;
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

                    foreach (Chat availableChat in args.AvailableChats)
                    {
                        ChatsCollection.Add(availableChat);
                    }

                    RefreshChatsCollection();

                    foreach (Client client in args.ConnectedClients)
                    {
                        ConnectedClientsCollection.Add(client);
                    }
                });
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
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

                        chat.LastMessageTimestamp = args.Message.Timestamp;
                        break;
                    }

                    RefreshChatsCollection();

                    if (ChatsCollectionSelectedItem == null)
                    {
                        return;
                    }

                    if (args.Message.ChatId == ChatsCollectionSelectedItem.Id.ToString())
                    {
                        MessagesCollection.Add(args.Message);
                    }
                });
        }

        private void HandleConnectionStateChangedEchoReceived(object sender, ConnectionStateChangedBroadcastReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Client.Id.ToString() == _client.Id)
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
                        MessagesCollection.Add(message);
                    }
                });
        }

        private void HandleChatCreatedEchoReceived(object sender, ChatCreatedBroadcastReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
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
                    _client.RequestChatHistory(ChatsCollectionSelectedItem.Id.ToString());
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
            ChatsCollection = new ObservableCollection<Chat>(ChatsCollection.OrderByDescending(chat => chat.LastMessageTimestamp));
            CollectionViewSource.GetDefaultView(ChatsCollection).Refresh();
        }

        #endregion
    }
}
