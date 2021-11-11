namespace Client.Messenger
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
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
            _client.ClientMessageReceived += HandleClientMessageReceived;
            _client.ConnectionStateChanged += HandleConnectionStateChanged;
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
            _client?.Send(Message);
            Message = string.Empty;
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (!args.Connected)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(args.Client.Name))
                    {
                        MessagesList.Add("Клиент подключен к серверу.");
                        MessagesList.Add("Авторизируйтесь, чтобы отправлять сообщения.");
                    }
                    else
                    {
                        MessagesList.Add("Авторизация прошла успешно");
                        //ControlsEnabledViewModel.SetAfterSignInControlsState();
                    }
                });
        }

        private void HandleClientMessageReceived(object sender, MessageReceivedEventArgs args)
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
