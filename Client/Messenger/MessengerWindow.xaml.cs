namespace Client.Messenger
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Common;

    /// <summary>
    /// Interaction logic for MessengerWindow.xaml
    /// </summary>
    public partial class MessengerWindow : Window
    {
        #region Constructors

        public MessengerWindow(WsClient client)
        {
            InitializeComponent();
            var messengerViewModel = new MessengerViewModel(client);
            DataContext = messengerViewModel;
            Closing += messengerViewModel.OnWindowClosing;
            ((INotifyCollectionChanged)MessagesListBox.Items).CollectionChanged += MessagesListBox_CollectionChanged;
        }

        #endregion

        #region Methods

        private void MessagesListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(MessagesListBox) <= 0)
            {
                return;
            }

            var border = (Border)VisualTreeHelper.GetChild(MessagesListBox, 0);
            var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            scrollViewer.ScrollToBottom();
        }

        #endregion
    }
}
