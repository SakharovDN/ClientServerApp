namespace Client.EventLog
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Common;

    public class EventLogViewModel : ViewModelBase
    {
        #region Fields

        private DateTime _selectedDateFrom;
        private DateTime _selectedDateTo;
        private List<EventLog> _eventLogs;
        private readonly List<EventLog> _eventLogsGlobal;

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

        public List<EventLog> EventLogs
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

        public EventLogViewModel(IEnumerable<EventLog> eventLogs)
        {
            _eventLogsGlobal = new List<EventLog>(eventLogs.OrderByDescending(eventLog => eventLog.Timestamp));
            EventLogs = new List<EventLog>(_eventLogsGlobal);
            SelectedDateTo = DateTime.Now;
            SelectedDateFrom = DateTime.Now - TimeSpan.FromDays(7);
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
            List<EventLog> eventLogs = _eventLogsGlobal
                                      .Where(eventLog => eventLog.Timestamp > SelectedDateFrom
                                                         && eventLog.Timestamp < SelectedDateTo + TimeSpan.FromDays(1)).ToList();
            EventLogs = new List<EventLog>(eventLogs);
        }

        #endregion
    }
}
