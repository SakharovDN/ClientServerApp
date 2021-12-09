namespace Client
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class ControlsEnabledViewModel : INotifyPropertyChanged
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
            MessengerControlIsEnabled = false;
            ConnectionControlIsEnabled = true;
        }

        public void SetAfterStartControlsState()
        {
            MessengerControlIsEnabled = true;
            ConnectionControlIsEnabled = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
