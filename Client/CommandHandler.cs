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

        /// <summary>
        /// Wires CanExecuteChanged event
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name = "action">Action to be executed by the command</param>
        /// <param name = "canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action<object> action, Func<object, bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name = "parameter"></param>
        /// <returns></returns>
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
