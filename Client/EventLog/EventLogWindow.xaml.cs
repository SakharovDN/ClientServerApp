namespace Client.EventLog
{
    using System.Collections.Generic;
    using System.Windows;

    using Common;

    public partial class EventLogWindow : Window
    {
        #region Constructors

        public EventLogWindow(IEnumerable<EventLog> eventLogs)
        {
            InitializeComponent();
            DataContext = new EventLogViewModel(eventLogs);
            Owner = Application.Current.MainWindow;
        }

        #endregion
    }
}
