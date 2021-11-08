namespace Server
{
    using System;
    using System.Net;

    using NLog;

    using WebSocket;

    public class NetworkManager
    {
        #region Fields

        private readonly WsServer _wsServer;

        private readonly ConfigSettings _configSetting;
        private readonly Logger _logger;

        #endregion

        #region Constructors

        public NetworkManager()
        {
            _configSetting = ConfigSettings.Receive();
            _wsServer = new WsServer(new IPEndPoint(IPAddress.Any, _configSetting.Port));
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Methods

        public void Start()
        {
            try
            {
                _wsServer.Start();
                _logger.Info("Server started successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.Source);
            }
        }

        public void Stop()
        {
            try
            {
                _wsServer.Stop();
                _logger.Info("Server stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex.Source);
            }
        }

        #endregion
    }
}
