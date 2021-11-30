namespace Client
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;

    using Annotations;

    using Common;

    public class Messenger : INotifyPropertyChanged
    {
        #region Fields

        private readonly WsClient _client;
        private string _message;
        private ObservableCollection<Message> _messagesCollection;
        private ObservableCollection<string> _clientsCollection;
        private string _clientsCollectionSelectedItem;
        private CommandHandler _sendButton;
        private Visibility _messageVisibility;

        #endregion

        #region Properties

        public ICommand SendButton => _sendButton ?? (_sendButton = new CommandHandler(PerformSendButton));

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

        public ObservableCollection<string> ClientsCollection
        {
            get => _clientsCollection;
            set
            {
                _clientsCollection = value;
                OnPropertyChanged();
            }
        }

        public string ClientsCollectionSelectedItem
        {
            get => _clientsCollectionSelectedItem;
            set
            {
                _clientsCollectionSelectedItem = value;
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
            ClientsCollection = new ObservableCollection<string>();
            MessagesCollection = new ObservableCollection<Message>();
            MessageVisibility = Visibility.Hidden;
            PropertyChanged += HandlePropertyChanged;
        }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleConnectionResponseReceived(object sender, ConnectionResponseReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Result == ResultCodes.Failure || args.ConnectedClients == null)
                    {
                        return;
                    }

                    ClientsCollection.Clear();
                    ClientsCollection.Add("Common");

                    foreach (string client in args.ConnectedClients)
                    {
                        ClientsCollection.Add(client);
                    }
                });
        }

        private void PerformSendButton(object commandParameter)
        {
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
            Message = regex.Replace(Message, @" ").Trim();
            _client.SendMessage(Message, ClientsCollectionSelectedItem);
            Message = string.Empty;
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Message.Target == "Common")
                    {
                        if (ClientsCollectionSelectedItem == "Common")
                        {
                            MessagesCollection.Add(args.Message);
                        }
                    }
                    else
                    {
                        if (args.Message.Target == ClientsCollectionSelectedItem || args.Message.Source == ClientsCollectionSelectedItem)
                        {
                            MessagesCollection.Add(args.Message);
                        }
                    }
                });
        }

        private void HandleConnectionStateChangedEchoReceived(object sender, ConnectionStateChangedEchoReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.ClientName == _client.Name)
                    {
                        return;
                    }

                    if (args.IsConnected)
                    {
                        ClientsCollection.Add(args.ClientName);
                    }
                    else
                    {
                        ClientsCollection.Remove(args.ClientName);
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

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ClientsCollectionSelectedItem))
            {
                if (ClientsCollectionSelectedItem != null)
                {
                    MessageVisibility = Visibility.Visible;
                }

                _client.RequestChatHistory(ClientsCollectionSelectedItem);
            }
        }

        #endregion
    }
}
