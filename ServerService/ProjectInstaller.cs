namespace ServerService
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        #region Constructors

        public ProjectInstaller()
        {
            InitializeComponent();
            serviceInstaller1.StartType = ServiceStartMode.Manual;
            serviceInstaller1.ServiceName = "ClientServerAppService";
            serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
        }

        #endregion
    }
}
