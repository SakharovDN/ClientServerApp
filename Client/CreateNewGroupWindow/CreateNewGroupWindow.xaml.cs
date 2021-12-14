namespace Client.NewGroupWindow
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Common;

    /// <summary>
    /// Interaction logic for CreateNewGroupWindow.xaml
    /// </summary>
    public partial class CreateNewGroupWindow : Window
    {
        #region Fields

        private readonly CreateNewGroupViewModel _createNewGroupViewModel;

        #endregion

        #region Properties

        public ObservableCollection<Client> SelectedClients { get; set; }

        public string GroupTitle { get; set; }

        #endregion

        #region Constructors

        public CreateNewGroupWindow(ObservableCollection<Client> connectedClients)
        {
            InitializeComponent();
            _createNewGroupViewModel = new CreateNewGroupViewModel(connectedClients);
            DataContext = _createNewGroupViewModel;
            _createNewGroupViewModel.GroupCreated += HandleGroupCreated;
        }

        #endregion

        #region Methods

        private void HandleGroupCreated(object sender, EventArgs e)
        {
            DialogResult = true;
            SelectedClients = _createNewGroupViewModel.SelectedClients;
            GroupTitle = _createNewGroupViewModel.GroupTitle;
            Close();
        }

        #endregion
    }
}
