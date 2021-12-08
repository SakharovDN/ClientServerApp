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

        private readonly NewGroupViewModel _newGroupViewModel;

        #endregion

        #region Properties

        public ObservableCollection<Client> SelectedClients { get; set; }

        public string GroupTitle { get; set; }

        #endregion

        #region Constructors

        public CreateNewGroupWindow(ObservableCollection<Client> connectedClients)
        {
            InitializeComponent();
            _newGroupViewModel = new NewGroupViewModel(connectedClients);
            DataContext = _newGroupViewModel;
            _newGroupViewModel.GroupCreated += HandleGroupCreated;
        }

        #endregion

        #region Methods

        private void HandleGroupCreated(object sender, EventArgs e)
        {
            DialogResult = _newGroupViewModel.DialogResult;
            SelectedClients = _newGroupViewModel.SelectedClients;
            GroupTitle = _newGroupViewModel.GroupTitle;
            Close();
        }

        #endregion
    }
}
