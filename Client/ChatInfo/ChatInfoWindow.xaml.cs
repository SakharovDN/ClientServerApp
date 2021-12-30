namespace Client.ChatInfo
{
    using System;
    using System.Windows;

    using Common;

    /// <summary>
    /// Interaction logic for ChatInfoWindow.xaml
    /// </summary>
    public partial class ChatInfoWindow : Window
    {
        #region Constructors

        public ChatInfoWindow(Chat chat, Group group)
        {
            InitializeComponent();
            var chatInfoViewModel = new ChatInfoViewModel(chat, group);
            chatInfoViewModel.LeaveGroupButtonClicked += HandleLeaveGroupButtonClicked;
            DataContext = chatInfoViewModel;
            Owner = Application.Current.MainWindow;
        }

        #endregion

        #region Methods

        private void HandleLeaveGroupButtonClicked(object sender, EventArgs args)
        {
            DialogResult = true;
            Close();
        }

        #endregion
    }
}
