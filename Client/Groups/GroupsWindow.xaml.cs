namespace Client.Groups
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Common;

    /// <summary>
    /// Interaction logic for CreateNewGroupWindow.xaml
    /// </summary>
    public partial class GroupsWindow : Window
    {
        #region Fields

        private readonly GroupsViewModel _groupsViewModel;

        #endregion

        #region Properties

        public ObservableCollection<Client> SelectedClients { get; set; }

        public string GroupTitle { get; set; }

        #endregion

        #region Constructors

        public GroupsWindow(ObservableCollection<Client> connectedClients)
        {
            InitializeComponent();
            _groupsViewModel = new GroupsViewModel(connectedClients);
            DataContext = _groupsViewModel;
            _groupsViewModel.GroupCreated += HandleGroupCreated;
            Owner = Application.Current.MainWindow;
        }

        #endregion

        #region Methods

        private void HandleGroupCreated(object sender, EventArgs args)
        {
            DialogResult = true;
            SelectedClients = _groupsViewModel.SelectedClients;
            GroupTitle = _groupsViewModel.GroupTitle;
            Close();
        }

        #endregion
    }
}
