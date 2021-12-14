namespace Client.NewGroupWindow
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Common;

    public class CreateNewGroupViewModel : ViewModelBase
    {
        #region Fields

        private bool? _dialogResult;
        private ObservableCollection<Client> _connectedClients;
        private CommandHandler _createNewGroupCommand;
        private ObservableCollection<Client> _selectedClients;
        private string _groupTitle;

        #endregion

        #region Properties

        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                if (value == _dialogResult)
                {
                    return;
                }

                _dialogResult = value;
                OnPropertyChanged();
            }
        }

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

        public CreateNewGroupViewModel(ObservableCollection<Client> connectedClients)
        {
            ConnectedClients = connectedClients;
            SelectedClients = new ObservableCollection<Client>();
        }

        #endregion

        #region Methods

        private void CreateNewGroup(object obj)
        {
            DialogResult = true;
            GroupCreated?.Invoke(null, EventArgs.Empty);
        }

        #endregion
    }
}
