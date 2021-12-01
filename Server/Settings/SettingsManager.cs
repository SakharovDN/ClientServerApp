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

        #endregion

        #region Constructors

        public SettingsManager()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Methods

        public ConfigSettings ReadConfigFile()
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                var configSettings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(CONFIG_FILE_PATH));

                if (SettingIsValid(configSettings))
                {
                    return configSettings;
                }

                Environment.Exit(1);
            }

            var settings = new ConfigSettings();
            File.WriteAllText(CONFIG_FILE_PATH, JsonConvert.SerializeObject(settings, Formatting.Indented));
            return settings;
        }

        private bool SettingIsValid(ConfigSettings configSettings)
        {
            bool isValid = true;

            if (configSettings.NetworkInterface != "WebSocket" && configSettings.NetworkInterface != "TcpSocket")
            {
                _logger.Error("Configuration settings are invalid. The network interface can be either \"WebSocket\" or \"TcpSocket\"");
                isValid = false;
            }
            else if (configSettings.InactivityTimeoutInterval <= 0)
            {
                _logger.Error("Configuration settings are invalid. Inactivity timeout interval must be greater than 0.");
                isValid = false;
            }

            return isValid;
        }

        #endregion
    }
}
