namespace Client.Groups
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Common;

    public class GroupsViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<Client> _connectedClients;
        private CommandHandler _createNewGroupCommand;
        private ObservableCollection<Client> _selectedClients;
        private string _groupTitle;

        #endregion

        #region Properties

        public ICommand CreateNewGroupCommand => _createNewGroupCommand ?? (_createNewGroupCommand = new CommandHandler(CreateNewGroup));

        public ObservableCollection<Client> ConnectedClients
        {
            get => _connectedClients;
            set
            {
                _connectedClients = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Client> SelectedClients
        {
            get => _selectedClients;
            set
            {
                _selectedClients = value;
                OnPropertyChanged();
            }
        }

        public string GroupTitle
        {
            get => _groupTitle;
            set
            {
                _groupTitle = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler GroupCreated;

        #endregion

        #region Constructors

        public GroupsViewModel(ObservableCollection<Client> connectedClients)
        {
            ConnectedClients = connectedClients;
            SelectedClients = new ObservableCollection<Client>();
        }

        #endregion

        #region Methods

        private void CreateNewGroup(object obj)
        {
            if (!string.IsNullOrEmpty(GroupTitle) && SelectedClients.Count != 0)
            {
                GroupCreated?.Invoke(null, EventArgs.Empty);
            }
        }

        #endregion
    }
}
