namespace Client.EventLog
{
    using System.Data;
    using System.Windows;

    public partial class EventLogWindow : Window
    {
        #region Constructors

        public EventLogWindow(DataTable eventLogs)
        {
            InitializeComponent();
            DataContext = new EventLogViewModel(eventLogs);
        }

        #endregion
    }
}
