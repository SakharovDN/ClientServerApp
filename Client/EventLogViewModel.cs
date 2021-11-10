namespace Client
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.CompilerServices;

    using Annotations;

    public class EventLogViewModel : INotifyPropertyChanged
    {
        #region Fields

        private DateTime _selectedDateFrom;
        private DateTime _selectedDateTo;
        private DataTable _eventLogs;
        private readonly DataTable _eventLogsGlobal;

        #endregion

        #region Properties

        public DateTime SelectedDateFrom
        {
            get => _selectedDateFrom;
            set
            {
                _selectedDateFrom = value;
                OnPropertyChanged(nameof(SelectedDateFrom));
            }
        }

        public DateTime SelectedDateTo
        {
            get => _selectedDateTo;
            set
            {
                _selectedDateTo = value;
                OnPropertyChanged(nameof(SelectedDateTo));
            }
        }

        public DataTable EventLogs
        {
            get => _eventLogs;
            set
            {
                _eventLogs = value;
                OnPropertyChanged(nameof(EventLogs));
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public EventLogViewModel(DataTable eventLogs)
        {
            _eventLogsGlobal = eventLogs.Copy();
            EventLogs = eventLogs.Copy();
            SelectedDateFrom = _eventLogsGlobal.Rows[_eventLogsGlobal.Rows.Count - 1].Field<DateTime>("Timestamp").Date;
            SelectedDateTo = _eventLogsGlobal.Rows[0].Field<DateTime>("Timestamp").Date;
            PropertyChanged += HandlePropertyChanged;
        }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDateFrom" || e.PropertyName == "SelectedDateTo")
            {
                FilterEventLogs();
            }
        }

        private void FilterEventLogs()
        {
            string filter = $"Timestamp >= '{SelectedDateFrom}' AND Timestamp < '{SelectedDateTo + TimeSpan.FromDays(1)}'";
            string sort = "Timestamp desc";
            EventLogs = _eventLogsGlobal.Select(filter, sort).CopyToDataTable();
        }

        #endregion
    }
}
