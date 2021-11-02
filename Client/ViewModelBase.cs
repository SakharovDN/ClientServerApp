namespace Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    using Common;
    using Common.Messages;

    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        private WsClient _client;
        private CommandHandler _startCommand;
        private CommandHandler _stopCommand;
        private CommandHandler _signInCommand;
        private CommandHandler _sendButton;
        private CommandHandler _clearButton;

        #endregion

        #region Properties

        public ControlsEnabledViewModel ControlsEnabledViewModel { get; set; }

        public string Address { get; set; }

        public string Port { get; set; }

        public string ClientName { get; set; }

        public string Message { get; set; }

        public ObservableCollection<string> MessagesList { get; set; }

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
            MessagesList = new ObservableCollection<string>();
            ControlsEnabledViewModel = new ControlsEnabledViewModel();
        }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PerformStartButton(object commandParameter)
        {
            try
            {
                _client = new WsClient();
                _client.ConnectionStateChanged += HandleConnectionStateChanged;
                _client.MessageReceived += HandleMessageReceived;
                MessageHandler.ConnectionStateChanged += HandleConnectionStateChanged;
                MessageHandler.MessageReceived += HandleMessageReceived;
                _client.Connect(Address, Port);
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
            if (_client != null)
            {
                _client.ConnectionStateChanged -= HandleConnectionStateChanged;
                _client.MessageReceived -= HandleMessageReceived;
                MessageHandler.ConnectionStateChanged -= HandleConnectionStateChanged;
                MessageHandler.MessageReceived -= HandleMessageReceived;
                _client.Disconnect();
            }

            ControlsEnabledViewModel.SetDefaultControlsState();
        }

        private void PerformSignInButton(object commandParameter)
        {
            _client?.SignIn(ClientName);
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
            Application.Current.Dispatcher.Invoke(
                delegate
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
                });
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
