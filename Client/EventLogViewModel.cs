namespace Client
{
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class EventLogViewModel : INotifyPropertyChanged
    {
        #region Fields

        private DataTable _eventLogs;

        #endregion

        #region Properties

        public DataTable EventLogs
        {
            get => _eventLogs;
            set
            {
                _eventLogs = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public EventLogViewModel(DataTable eventLogs)
        {
            EventLogs = eventLogs;
        }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
