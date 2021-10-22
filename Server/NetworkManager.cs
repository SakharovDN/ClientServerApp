namespace Common
{
    using System;
    using System.IO;
    using System.Net;

    using Newtonsoft.Json;

    public class NetworkManager
    {
        #region Constants

        private const string CONFIG_FILE_PATH = @"config.json";

        #endregion

        #region Fields

        private readonly WsServer _wsServer;

        private readonly ConfigSettings _configSetting;

        #endregion

        #region Constructors

        public NetworkManager()
        {
            _configSetting = ConfigSettings.Read(CONFIG_FILE_PATH);
            _wsServer = new WsServer(new IPEndPoint(IPAddress.Any, 65000));
        }

        #endregion

        #region Methods

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {IPAddress.Any}:{65000}");
            _wsServer.Start();
        }

        public void Stop()
        {
            _wsServer.Stop();
        }

        #endregion
    }
}
