namespace Server
{
    using System;
    using System.Net;

    using Common;

    public class NetworkManager
    {
        #region Fields

        private readonly WsServer _wsServer;

        private readonly ConfigSettings _configSetting;

        #endregion

        #region Constructors

        public NetworkManager()
        {
            _configSetting = ConfigSettings.Receive();
            _wsServer = new WsServer(new IPEndPoint(IPAddress.Any, _configSetting.Port));
        }

        #endregion

        #region Methods

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {IPAddress.Any}:{_configSetting.Port}");
            _wsServer.Start();
        }

        public void Stop()
        {
            _wsServer.Stop();
        }

        #endregion
    }
}
