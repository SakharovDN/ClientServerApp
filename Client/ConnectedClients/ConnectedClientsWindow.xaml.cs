namespace Client.ConnectedClients
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Common;

    /// <summary>
    /// Interaction logic for ConnectedClientsWindow.xaml
    /// </summary>
    public partial class ConnectedClientsWindow : Window
    {
        #region Fields

        private readonly ConnectedClientsViewModel _connectedClientsViewModel;

        #endregion

        #region Properties

        public Client SelectedClient { get; set; }

        #endregion

        #region Constructors

        public ConnectedClientsWindow(ObservableCollection<Client> connectedClientsCollection)
        {
            InitializeComponent();
            _connectedClientsViewModel = new ConnectedClientsViewModel(connectedClientsCollection);
            DataContext = _connectedClientsViewModel;
            _connectedClientsViewModel.ClientSelected += ShowChatWithSelectedClient;
            Owner = Application.Current.MainWindow;
        }

        #endregion

        #region Methods

        private void ShowChatWithSelectedClient(object sender, EventArgs args)
        {
            DialogResult = true;
            SelectedClient = _connectedClientsViewModel.ConnectedClientsCollectionSelectedItem;
            Close();
        }

        #endregion
    }
}
