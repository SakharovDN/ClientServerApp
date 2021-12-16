namespace Server.Settings
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    using NLog;

    public class SettingsManager
    {
        #region Constants

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
            if (File.Exists(CONFIG_FILE_PATH))
            {
                try
                {
                    var configSettings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(CONFIG_FILE_PATH));
                    CheckSettingsValidity(configSettings);
                }
                catch (Exception ex)
                {
                    _logger.Error(() => $"{ex}");
                    Environment.Exit(1);
                }
            }

            var settings = new ConfigSettings();
            File.WriteAllText(CONFIG_FILE_PATH, JsonConvert.SerializeObject(settings, Formatting.Indented));
            return settings;
        }

        private void CheckSettingsValidity(ConfigSettings configSettings)
        {
            if (configSettings.NetworkInterface != "WebSocket" && configSettings.NetworkInterface != "TcpSocket")
            {
                throw new Exception("Configuration settings are invalid. The network interface can be either \"WebSocket\" or \"TcpSocket\"");
            }

            if (configSettings.InactivityTimeoutInterval <= 59999)
            {
                throw new Exception("Configuration settings are invalid. Inactivity timeout interval must be at least 60000 milliseconds.");
            }
        }

        #endregion
    }
}
