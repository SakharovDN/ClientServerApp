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
        private ObservableCollection<string> _messagesList;
        private ObservableCollection<string> _clientsList;
        private CommandHandler _sendButton;

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

        public ObservableCollection<string> MessagesList
        {
            get => _messagesList;
            set
            {
                _messagesList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ClientsList
        {
            get => _clientsList;
            set
            {
                _clientsList = value;
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
            ClientMessageHandler.MessageReceived += HandleMessageReceived;
            ClientMessageHandler.ClientsListReceived += HandleClientsListReceived;
            ClientMessageHandler.ConnectionStateChangedEchoReceived += HandleConnectionStateChangedEchoReceived;
            ClientsList = new ObservableCollection<string>();
            MessagesList = new ObservableCollection<string>();
        }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandleClientsListReceived(object sender, ClientsListReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    ClientsList.Clear();

                    foreach (string client in e.Clients)
                    {
                        ClientsList.Add(client);
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
            _client?.Send(Message);
            Message = string.Empty;
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    MessagesList.Add($"{args.ClientName}: {args.Message}");
                });
        }

        private void HandleConnectionStateChangedEchoReceived(object sender, ConnectionStateChangedEchoReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (e.IsConnected)
                    {
                        ClientsList.Add(e.ClientName);
                    }
                    else
                    {
                        ClientsList.Remove(e.ClientName);
                    }
                });
        }

        #endregion
    }
}
