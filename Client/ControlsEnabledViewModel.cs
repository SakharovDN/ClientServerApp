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
        private bool _messageIsEnabled;
        private bool _buttonStartIsEnabled;
        private bool _buttonStopIsEnabled;
        private bool _buttonSendIsEnabled;
        private bool _buttonLoginIsEnabled;
        private bool _buttonClearIsEnabled;

        #endregion

        #region Properties

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

        public bool MessageIsEnabled
        {
            get => _messageIsEnabled;
            set
            {
                _messageIsEnabled = value;
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

        public bool ButtonSendIsEnabled
        {
            get => _buttonSendIsEnabled;
            set
            {
                _buttonSendIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonLoginIsEnabled
        {
            get => _buttonLoginIsEnabled;
            set
            {
                _buttonLoginIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonClearIsEnabled
        {
            get => _buttonClearIsEnabled;
            set
            {
                _buttonClearIsEnabled = value;
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
            MessageIsEnabled = false;
            ClientNameIsEnabled = false;
            ButtonStartIsEnabled = true;
            ButtonStopIsEnabled = false;
            ButtonLoginIsEnabled = false;
            ButtonSendIsEnabled = false;
        }

        public void SetAfterStartControlsState()
        {
            AddressIsEnabled = false;
            PortIsEnabled = false;
            MessageIsEnabled = false;
            ClientNameIsEnabled = true;
            ButtonStartIsEnabled = false;
            ButtonStopIsEnabled = true;
            ButtonLoginIsEnabled = true;
            ButtonSendIsEnabled = false;
        }

        public void SetAfterLoginControlsState()
        {
            AddressIsEnabled = false;
            PortIsEnabled = false;
            MessageIsEnabled = true;
            ClientNameIsEnabled = false;
            ButtonStartIsEnabled = false;
            ButtonStopIsEnabled = true;
            ButtonLoginIsEnabled = false;
            ButtonSendIsEnabled = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
