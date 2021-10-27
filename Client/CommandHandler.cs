namespace Client
{
    using System;
    using System.Windows.Input;

    public class CommandHandler : ICommand
    {
        #region Fields

        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Constructors

        public CommandHandler(Action<object> action, Func<object, bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        #endregion
    }
}
