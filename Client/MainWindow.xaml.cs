using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModelBase();
            SetDefaultButtonState();
        }

        private void SetDefaultButtonState()
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            LoginButton.IsEnabled = false;
            SendButton.IsEnabled = false;

            AddressTextBox.IsEnabled = true;
            PortTextBox.IsEnabled = true;
            MessageTextBox.IsEnabled = false;
            LoginTextBox.IsEnabled = false;
        }

        private void HandleButtonStartClick(object sender, EventArgs e)
        {
            try
            {
                //_currentTransport = TransportFactory.Create((TransportType)_transport.SelectedItem);
                //_currentTransport.ConnectionStateChanged += HandleConnectionStateChanged;
                //_currentTransport.MessageReceived += HandleMessageReceived;
                //_currentTransport.Connect(_serverAddress.Text, _serverPort.Text);

                AddressTextBox.IsEnabled = false;
                PortTextBox.IsEnabled = false;
                MessageTextBox.IsEnabled = false;
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                LoginButton.IsEnabled = true;
                LoginTextBox.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessagesListBox.Items.Add(ex.Message);
                SetDefaultButtonState();
            }
        }
    }
}
