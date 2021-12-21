namespace Server.Settings
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    using NLog;

    public class SettingsManager
    {
        #region Constants

        private const int MIN_PORT = 8000;
        private const int MAX_PORT = 65535;
        private const int MIN_INACTIVITY_TIMEOUT_INTERVAL = 60000;
        private const string CONFIG_FILE_PATH = "config.json";

        #endregion

        #region Fields

        private readonly Logger _logger;
        private readonly ConfigSettings _configSettings;

        #endregion

        #region Constructors

        public SettingsManager()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _configSettings = ReadConfigFile();
        }

        #endregion

        #region Methods

        public ConfigSettings GetConfigSettings()
        {
            return _configSettings;
        }

        public string GetDbConnectionString()
        {
            return
                $"Data Source={_configSettings.DbServerName};Initial Catalog=ClientServerApp;Integrated Security=True;MultipleActiveResultSets=True";
        }

        private ConfigSettings ReadConfigFile()
        {
            var configSettings = new ConfigSettings();

            if (File.Exists(CONFIG_FILE_PATH))
            {
                try
                {
                    configSettings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(CONFIG_FILE_PATH));
                    CheckSettingsValidity(configSettings);
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"{ex}");
                    Environment.Exit(1);
                }
            }
            else
            {
                configSettings = configSettings.GetDefaultConfigSettings();
                File.WriteAllText(CONFIG_FILE_PATH, JsonConvert.SerializeObject(configSettings, Formatting.Indented));
            }

            return configSettings;
        }

        private void CheckSettingsValidity(ConfigSettings configSettings)
        {
            if (configSettings.NetworkInterface != "WebSocket" && configSettings.NetworkInterface != "TcpSocket")
            {
                throw new Exception("Configuration settings are invalid. The network interface can be either \"WebSocket\" or \"TcpSocket\"");
            }

            if (configSettings.InactivityTimeoutInterval < MIN_INACTIVITY_TIMEOUT_INTERVAL)
            {
                throw new Exception("Configuration settings are invalid. Inactivity timeout interval must be at least 60000 milliseconds.");
            }

            if (configSettings.Port < MIN_PORT || configSettings.Port > MAX_PORT)
            {
                throw new Exception("Configuration settings are invalid. The port value must be in the range 8000 to 65535.");
            }

            if (string.IsNullOrEmpty(configSettings.DbServerName))
            {
                throw new Exception("Configuration settings are invalid. DbServerName is null or empty.");
            }
        }

        #endregion
    }
}
