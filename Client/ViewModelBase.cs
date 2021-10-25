namespace Client
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using Annotations;

    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Fields

        public bool IsEditMode;
        private CommandHandler _clickCommand;

        #endregion

        #region Properties

        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ??
                       (_clickCommand = new CommandHandler(
                            obj =>
                            {
                                MyAction();
                            }));
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        public void MyAction()
        {
            //AddressTextBox.IsEnabled = false;
            //PortTextBox.IsEnabled = false;
            //MessageTextBox.IsEnabled = false;
            //StartButton.IsEnabled = false;
            //StopButton.IsEnabled = true;
            //LoginButton.IsEnabled = true;
            //LoginTextBox.IsEnabled = true;
            OnPropertyChanged("IsEditMode");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
