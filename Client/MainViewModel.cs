namespace Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    using Common;

    using EventLog;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly WsClient _client;
        private CommandHandler _startCommand;
        private CommandHandler _stopCommand;
        private CommandHandler _getEventLogsButton;
        private string _address;
        private string _port;
        private string _clientName;
        private ObservableCollection<string> _messagesList;

        #endregion

        #region Properties

        public Messenger Messenger { get; set; }

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

        public ObservableCollection<string> MessagesList
        {
            get => _messagesList;
            set
            {
                _messagesList = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => _startCommand ?? (_startCommand = new CommandHandler(PerformStartButton));

        public ICommand StopCommand => _stopCommand ?? (_stopCommand = new CommandHandler(PerformStopButton));

        public ICommand GetEventLogsButton => _getEventLogsButton ?? (_getEventLogsButton = new CommandHandler(PerformGetEventLogsButton));

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public MainViewModel()
        {
            _client = new WsClient();
            _client.ConnectionStateChanged += HandleConnectionStateChanged;
            _client.MessageHandler.EventLogsReceived += HandleEventLogsReceived;
            _client.MessageHandler.ConnectionResponseReceived += HandleConnectionResponseReceived;
            _client.MessageHandler.ConnectionStateChangedEchoReceived += HandleConnectionStateChangedEchoReceived;
            ControlsEnabledViewModel = new ControlsEnabledViewModel();
            Messenger = new Messenger(_client);
            MessagesList = new ObservableCollection<string>();
            Address = "127.0.0.1";
            Port = "65000";
        }

        #endregion

        #region Methods

        public void OnWindowClosing(object sender, CancelEventArgs args)
        {
            if (_client.IsConnected)
            {
                _client.LogOut();
            }
        }

        public void OnWindowClosed(object sender, EventArgs args)
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PerformStartButton(object commandParameter)
        {
            try
            {
                _client.Connect(Address, Port);
                _client?.LogIn(ClientName);
                ControlsEnabledViewModel.SetAfterStartControlsState();
            }
            catch (Exception ex)
            {
                _client.Disconnect();
                MessagesList.Add(ex.Message);
                ControlsEnabledViewModel.SetDefaultControlsState();
            }
        }

        private void PerformStopButton(object commandParameter)
        {
            _client.LogOut();
            ControlsEnabledViewModel.SetDefaultControlsState();
        }

        private void PerformGetEventLogsButton(object commandParameter)
        {
            _client.RequestEventLogs();
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    MessagesList.Add(args.IsConnected ? "Connection established" : "Connection lost");
                });
        }

        private void HandleConnectionStateChangedEchoReceived(object sender, ConnectionStateChangedEchoReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    string clientState = args.IsConnected ? "is connected" : "is disconnected";
                    string message = $"Client {args.ClientName} {clientState}";
                    MessagesList.Add(message);
                });
        }

        private void HandleConnectionResponseReceived(object sender, ConnectionResponseReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    if (args.Result == ResultCodes.Ok)
                    {
                        return;
                    }

                    _client.Disconnect();
                    MessagesList.Add(args.Reason);
                    ControlsEnabledViewModel.SetDefaultControlsState();
                });
        }

        private static void HandleEventLogsReceived(object sender, EventLogsReceivedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                delegate
                {
                    var eventLogWindow = new EventLogWindow(args.EventLogs);
                    eventLogWindow.ShowDialog();
                });
        }

        #endregion
    }
}
