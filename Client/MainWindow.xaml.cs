namespace Client
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            var viewModelBase = new MainViewModel();
            DataContext = viewModelBase;
            Closing += viewModelBase.OnWindowClosing;
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
