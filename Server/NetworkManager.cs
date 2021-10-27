namespace Server
{
    using System;
    using System.Net;

    using Common;
    using Common._EventArgs_;

    using NLog;

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
            _wsServer.ConnectionStateChanged += HandleConnectionStateChanged;
            _wsServer.MessageReceived += HandleMessageReceived;
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

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = $"Клиент '{e.ClientName}' отправил сообщение '{e.Message}'.";

            Console.WriteLine(message);

            _wsServer.Send(message);
        }

        private void HandleConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            string clientState = e.Connected ? "подключен" : "отключен";
            string message = $"Клиент '{e.ClientName}' {clientState}.";

            Console.WriteLine(message);

            _wsServer.Send(message);
        }

        #endregion
    }
}
