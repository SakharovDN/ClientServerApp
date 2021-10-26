namespace ServerService
{
    using System.ServiceProcess;

    using Server;

    public partial class Service1 : ServiceBase
    {
        #region Fields

        private NetworkManager _networkManager;

        #endregion

        #region Constructors

        public Service1()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void OnStart(string[] args)
        {
            _networkManager = new NetworkManager();
            _networkManager.Start();
        }

        protected override void OnStop()
        {
            _networkManager?.Stop();
            _networkManager = null;
        }

        #endregion
    }
}
