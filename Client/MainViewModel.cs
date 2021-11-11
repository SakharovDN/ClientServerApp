namespace Client
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    using Common;

    using EventLog;

    using Messenger;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly WsClient _client;
        private CommandHandler _startCommand;
        private CommandHandler _stopCommand;
        private CommandHandler _signInCommand;

        private CommandHandler _getEventLogsButton;
        private string _address;
        private string _port;
        private string _clientName;

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

        public ICommand StartCommand => _startCommand ?? (_startCommand = new CommandHandler(PerformStartButton));

        public ICommand StopCommand => _stopCommand ?? (_stopCommand = new CommandHandler(PerformStopButton));

        public ICommand SignInCommand => _signInCommand ?? (_signInCommand = new CommandHandler(PerformSignInButton));

        public ICommand GetEventLogsButton => _getEventLogsButton ?? (_getEventLogsButton = new CommandHandler(PerformGetEventLogsButton));

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MainViewModel()
        {
            _client = new WsClient();
            ControlsEnabledViewModel = new ControlsEnabledViewModel();
            ClientMessageHandler.EventLogsReceived += HandleEventLogsReceived;
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

        private static void HandleEventLogsReceived(object sender, EventLogsReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    var eventLogWindow = new EventLogWindow(args.EventLogs);
                    eventLogWindow.Show();
                });
        }

        private void PerformStartButton(object commandParameter)
        {
            try
            {
                _client.Connect(Address, Port);
                ControlsEnabledViewModel.SetAfterStartControlsState();
            }
            catch (Exception ex)
            {
                //MessagesList.Add(ex.Message);
                ControlsEnabledViewModel.SetDefaultControlsState();
            }
        }

        private void OpenMessenger()
        {
            var messenger = new MessengerWindow(_client);
            messenger.Show();
        }

        private void PerformStopButton(object commandParameter)
        {
            _client?.Disconnect();
            ControlsEnabledViewModel.SetDefaultControlsState();
        }

        private void PerformSignInButton(object commandParameter)
        {
            _client?.SignIn(ClientName);
            OpenMessenger();
        }

        private void PerformGetEventLogsButton(object commandParameter)
        {
            _client.RequestEventLogs();
        }

        #endregion
    }
}
