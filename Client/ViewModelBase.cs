namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using Common;
    using Common._EventArgs_;

    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private WsClient _client;
        private string _address;
        private string _port;
        private string _clientName;
        private string _message;
        private List<string> _messagesList;
        private CommandHandler _startCommand;
        private CommandHandler _stopCommand;
        private CommandHandler _loginCommand;
        private CommandHandler _sendButton;
        private CommandHandler _clearButton;

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

        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value;
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

        public List<string> MessagesList
        {
            get => _messagesList;
            set
            {
                _messagesList = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => _startCommand ?? (_startCommand = new CommandHandler(Start));

        public ICommand StopCommand => _stopCommand ?? (_stopCommand = new CommandHandler(Stop));

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new CommandHandler(Login));

        public ICommand SendButton => _sendButton ?? (_sendButton = new CommandHandler(PerformSendButton));

        public ICommand ClearButton => _clearButton ?? (_clearButton = new CommandHandler(PerformClearButton));

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ViewModelBase()
        {
            ControlsEnabledViewModel = new ControlsEnabledViewModel();
        }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Start(object commandParameter)
        {
            try
            {
                _client = new WsClient();
                _client.ConnectionStateChanged += HandleConnectionStateChanged;
                _client.MessageReceived += HandleMessageReceived;
                _client.Connect(Address, Port);
                ControlsEnabledViewModel.SetAfterStartControlsState();
            }
            catch (Exception ex)
            {
                MessagesList.Add(ex.Message);
                ControlsEnabledViewModel.SetDefaultControlsState();
            }
        }

        private void Stop(object commandParameter)
        {
            if (_client != null)
            {
                _client.ConnectionStateChanged -= HandleConnectionStateChanged;
                _client.MessageReceived -= HandleMessageReceived;
                _client.Disconnect();
            }

            ControlsEnabledViewModel.SetDefaultControlsState();
        }

        private void Login(object commandParameter)
        {
            //_client?.LogIn(ClientName);
            ControlsEnabledViewModel.SetAfterLoginControlsState();
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
            if (args.Connected)
            {
                if (string.IsNullOrEmpty(args.ClientName))
                {
                    MessagesList.Add("Клиент подключен к серверу.");
                    MessagesList.Add("Авторизируйтесь, чтобы отправлять сообщения.");
                }
                else
                {
                    MessagesList.Add("Авторизация прошла успешно");
                }
            }
            else
            {
                MessagesList.Add("Клиент отключен от сервера");
            }
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            MessagesList.Add(args.Message);
        }

        #endregion
    }
}
