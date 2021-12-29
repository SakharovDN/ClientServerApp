namespace Client.ChatInfo
{
    using System;
    using System.Windows.Input;

    using Common;

    public class ChatInfoViewModel : ViewModelBase
    {
        #region Fields

        private Chat _chat;
        private Group _group;

        #endregion

        #region Properties

        public ICommand LeaveGroupCommand { get; set; }

        public Chat Chat
        {
            get => _chat;
            set
            {
                _chat = value;
                OnPropertyChanged();
            }
        }

        public Group Group
        {
            get => _group;
            set
            {
                _group = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler LeaveGroupButtonClicked;

        #endregion

        #region Constructors

        public ChatInfoViewModel(Chat chat, Group group)
        {
            Chat = chat;
            Group = group;

            if (Group != null)
            {
                LeaveGroupCommand = new CommandHandler(LeaveGroup);
            }
        }

        #endregion

        #region Methods

        private void LeaveGroup(object obj)
        {
            LeaveGroupButtonClicked?.Invoke(null, EventArgs.Empty);
        }

        #endregion
    }
}
