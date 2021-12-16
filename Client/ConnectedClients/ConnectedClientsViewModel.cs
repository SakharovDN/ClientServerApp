namespace Client.ConnectedClients
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Common;

    public class ConnectedClientsViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<Client> _connectedClientsCollection;
        private Client _connectedClientsCollectionSelectedItem;

        #endregion

        #region Properties

        public ObservableCollection<Client> ConnectedClientsCollection
        {
            get => _connectedClientsCollection;
            set
            {
                _connectedClientsCollection = value;
                OnPropertyChanged();
            }
        }

        public Client ConnectedClientsCollectionSelectedItem
        {
            get => _connectedClientsCollectionSelectedItem;
            set
            {
                _connectedClientsCollectionSelectedItem = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler ClientSelected;

        #endregion

        #region Constructors

        public ConnectedClientsViewModel(ObservableCollection<Client> connectedClientsCollection)
        {
            PropertyChanged += HandlePropertyChanged;
            ConnectedClientsCollection = connectedClientsCollection;
        }

        #endregion

        #region Methods

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ConnectedClientsCollectionSelectedItem) && ConnectedClientsCollectionSelectedItem != null)
            {
                ClientSelected?.Invoke(null, EventArgs.Empty);
            }
        }

        #endregion
    }
}
