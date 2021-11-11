namespace Client.Messenger
{
    using System.Windows;

    using Common;

    /// <summary>
    /// Interaction logic for MessengerWindow.xaml
    /// </summary>
    public partial class MessengerWindow : Window
    {
        public MessengerWindow(WsClient client)
        {
            InitializeComponent();
            var messengerViewModel = new MessengerViewModel(client);
            DataContext = messengerViewModel;
            Closing += messengerViewModel.OnWindowClosing;
        }
    }
}
