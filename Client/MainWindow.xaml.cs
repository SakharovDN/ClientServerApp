namespace Client
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            var viewModelBase = new ViewModelBase();
            DataContext = viewModelBase;
            Closing += viewModelBase.OnWindowClosing;
        }

        #endregion
    }
}
