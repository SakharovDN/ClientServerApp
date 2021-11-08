namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    using Common;

    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private readonly WsClient _client;
        private CommandHandler _startCommand;
        private CommandHandler _stopCommand;
        private CommandHandler _signInCommand;
        private CommandHandler _sendButton;
        private CommandHandler _clearButton;
        private string _address;
        private string _port;
        private string _clientName;
        private string _message;
        private ObservableCollection<string> _messagesList;
        private ObservableCollection<string> _clientsList;

        #endregion

        #region Properties

        public ControlsEnabledViewModel ControlsEnabledViewModel { get; set; }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value;
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

        public ICommand StartCommand => _startCommand ?? (_startCommand = new CommandHandler(PerformStartButton));

        public ICommand StopCommand => _stopCommand ?? (_stopCommand = new CommandHandler(PerformStopButton));

        public ICommand SignInCommand => _signInCommand ?? (_signInCommand = new CommandHandler(PerformSignInButton));

        public ICommand SendButton => _sendButton ?? (_sendButton = new CommandHandler(PerformSendButton));

        public ICommand ClearButton => _clearButton ?? (_clearButton = new CommandHandler(PerformClearButton));

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ViewModelBase()
        {
            _client = new WsClient();
            _client.ClientMessageReceived += HandleClientMessageReceived;
            _client.ConnectionStateChanged += HandleConnectionStateChanged;
            ClientsList = new ObservableCollection<string>();
            MessagesList = new ObservableCollection<string>();
            ControlsEnabledViewModel = new ControlsEnabledViewModel();
            ClientMessageHandler.ClientsListReceived += HandleClientsListReceived;
            Address = "127.0.0.1";
            Port = "65000";
        }

        #endregion

        #region Methods

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _client?.Disconnect();
        }

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

        private void PerformStartButton(object commandParameter)
        {
            try
            {
                _client.Connect(Address, Port);
                _client.RequestClientsList();
                ControlsEnabledViewModel.SetAfterStartControlsState();
            }
            catch (Exception ex)
            {
                MessagesList.Add(ex.Message);
                ControlsEnabledViewModel.SetDefaultControlsState();
            }
        }

        private void PerformStopButton(object commandParameter)
        {
            _client?.Disconnect();
            ControlsEnabledViewModel.SetDefaultControlsState();
            ClientName = string.Empty;
            Message = string.Empty;
            MessagesList.Clear();
            ClientsList.Clear();
        }

        private void PerformSignInButton(object commandParameter)
        {
            _client?.SignIn(ClientName);
        }

        private void PerformSendButton(object commandParameter)
        {
            _client?.Send(Message);
        }

        private void PerformClearButton(object commandParameter)
        {
            MessagesList.Clear();
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
                        ControlsEnabledViewModel.SetAfterSignInControlsState();
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
