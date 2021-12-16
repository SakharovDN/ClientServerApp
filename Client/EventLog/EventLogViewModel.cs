namespace Client.EventLog
{
    using System;
    using System.ComponentModel;
    using System.Data;

    public class EventLogViewModel : ViewModelBase
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
                if (value <= SelectedDateTo)
                {
                    _selectedDateFrom = value;
                }

                OnPropertyChanged();
            }
        }

        public DateTime SelectedDateTo
        {
            get => _selectedDateTo;
            set
            {
                if (value >= SelectedDateFrom)
                {
                    _selectedDateTo = value;
                }

                OnPropertyChanged();
            }
        }

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

        #region Constructors

        public EventLogViewModel(DataTable eventLogs)
        {
            _eventLogsGlobal = eventLogs.Copy();
            EventLogs = eventLogs.Copy();
            SelectedDateTo = _eventLogsGlobal.Rows[0].Field<DateTime>("Timestamp").Date;
            SelectedDateFrom = _eventLogsGlobal.Rows[_eventLogsGlobal.Rows.Count - 1].Field<DateTime>("Timestamp").Date;
            PropertyChanged += HandlePropertyChanged;
        }

        #endregion

        #region Methods

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SelectedDateFrom) || args.PropertyName == nameof(SelectedDateTo))
            {
                FilterEventLogs();
            }
        }

        private void FilterEventLogs()
        {
            string filter = $"Timestamp >= '{SelectedDateFrom}' AND Timestamp < '{SelectedDateTo + TimeSpan.FromDays(1)}'";
            string sort = "Timestamp desc";

            try
            {
                EventLogs = _eventLogsGlobal.Select(filter, sort).CopyToDataTable();
            }
            catch
            {
                EventLogs = null;
            }
        }

        #endregion
    }
}
