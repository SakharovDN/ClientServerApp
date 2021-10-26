﻿namespace ServerService
{
    using System.ServiceProcess;

    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] {new Service1()};
            ServiceBase.Run(ServicesToRun);
        }

        #endregion
    }
}
