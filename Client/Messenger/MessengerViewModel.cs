namespace Client.Messenger
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;

    using Annotations;

    using Common;

    public class MessengerViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly WsClient _client;
        private string _message;
        private ObservableCollection<string> _messagesList;
        private ObservableCollection<string> _clientsList;
        private CommandHandler _sendButton;

        #endregion

        #region Properties

        public string MessagesListSelectedItem { get; set; }

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

        public MessengerViewModel(WsClient client)
        {
            _client = client;
            ClientMessageHandler.MessageReceived += HandleMessageReceived;
            ClientMessageHandler.ClientsListReceived += HandleClientsListReceived;
            _client.RequestClientsList();
            ClientsList = new ObservableCollection<string>();
            MessagesList = new ObservableCollection<string>();
        }

        #endregion

        #region Methods

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _client?.SignOut();
            Application.Current.MainWindow?.Show();
        }

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

                    foreach (KeyValuePair<Guid, WsClient> client in e.Clients)
                    {
                        ClientsList.Add(client.Value.Name);
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
                    MessagesList.Add(args.Message);
                });
        }

        #endregion
    }
}
