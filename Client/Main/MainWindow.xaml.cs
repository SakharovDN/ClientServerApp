﻿namespace Client
{
    using System.Collections.Specialized;
    using System.Windows;
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
            ((INotifyCollectionChanged)MessagesPanel.Items).CollectionChanged += MessagesCollection_CollectionChanged;
        }

        #endregion

        #region Methods

        private void MessagesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (VisualTreeHelper.GetChildrenCount(MessagesPanel) <= 0)
            {
                return;
            }

            MessagePanelScrollViewer.ScrollToBottom();
        }

        #endregion
    }
}
