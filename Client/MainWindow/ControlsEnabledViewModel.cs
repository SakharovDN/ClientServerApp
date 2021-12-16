namespace Client
{
    public class ControlsEnabledViewModel : ViewModelBase
    {
        #region Fields

        private bool _messengerControlIsEnabled;
        private bool _connectionControlIsEnabled;

        #endregion

        #region Properties

        public bool MessengerControlIsEnabled
        {
            get => _messengerControlIsEnabled;
            set
            {
                _messengerControlIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool ConnectionControlIsEnabled
        {
            get => _connectionControlIsEnabled;
            set
            {
                _connectionControlIsEnabled = value;
                OnPropertyChanged();
            }
        }

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
            MessengerControlIsEnabled = false;
            ConnectionControlIsEnabled = true;
        }

        public void SetAfterStartControlsState()
        {
            MessengerControlIsEnabled = true;
            ConnectionControlIsEnabled = false;
        }

        #endregion
    }
}
