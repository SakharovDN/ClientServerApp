using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ServerService
{
    public partial class Service1 : ServiceBase
    {
        NetworkManager networkManager = new NetworkManager();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            networkManager.Start();
        }

        protected override void OnStop()
        {
            networkManager.Stop();
        }
    }
}
