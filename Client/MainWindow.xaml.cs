namespace Client
{
    using System.Windows;

    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            var viewModelBase = new MainViewModel();
            DataContext = viewModelBase;
            Closing += viewModelBase.OnWindowClosing;
        }

        #endregion
    }
}
