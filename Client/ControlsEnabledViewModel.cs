namespace Client
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class ControlsEnabledViewModel : INotifyPropertyChanged
    {
        #region Fields

        private bool _addressIsEnabled;
        private bool _portIsEnabled;
        private bool _clientNameIsEnabled;
        private bool _buttonStartIsEnabled;
        private bool _buttonStopIsEnabled;
        private bool _buttonSignInIsEnabled;
        private bool _buttonGetEventLogsIsEnabled;

        #endregion

        #region Properties

        public bool ButtonGetEventLogsIsEnabled
        {
            get => _buttonGetEventLogsIsEnabled;
            set
            {
                _buttonGetEventLogsIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool AddressIsEnabled
        {
            get => _addressIsEnabled;
            set
            {
                _addressIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool PortIsEnabled
        {
            get => _portIsEnabled;
            set
            {
                _portIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ClientNameIsEnabled
        {
            get => _clientNameIsEnabled;
            set
            {
                _clientNameIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonStartIsEnabled
        {
            get => _buttonStartIsEnabled;
            set
            {
                _buttonStartIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonStopIsEnabled
        {
            get => _buttonStopIsEnabled;
            set
            {
                _buttonStopIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonSignInIsEnabled
        {
            get => _buttonSignInIsEnabled;
            set
            {
                _buttonSignInIsEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ControlsEnabledViewModel()
        {
            SetDefaultControlsState();
        }

        #endregion

        #region Methods

        public void SetDefaultControlsState()
        {
            AddressIsEnabled = true;
            PortIsEnabled = true;
            ClientNameIsEnabled = false;
            ButtonStartIsEnabled = true;
            ButtonStopIsEnabled = false;
            ButtonSignInIsEnabled = false;
            ButtonGetEventLogsIsEnabled = false;
        }

        public void SetAfterStartControlsState()
        {
            AddressIsEnabled = false;
            PortIsEnabled = false;
            ClientNameIsEnabled = true;
            ButtonStartIsEnabled = false;
            ButtonStopIsEnabled = true;
            ButtonSignInIsEnabled = true;
            ButtonGetEventLogsIsEnabled = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
