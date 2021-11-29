namespace Server
{
    using System.IO;

    using Newtonsoft.Json;

    public class ConfigSettings
    {
        #region Constants

        private const string CONFIG_FILE_PATH = "config.json";

        #endregion

        #region Properties

        [JsonProperty("NetworkInterface")]
        public string NetworkInterface { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("InactivityTimeout")]
        public int InactivityTimeoutInterval { get; set; }

        [JsonProperty("DbServerName")]
        public string DbServerName { get; set; }

        #endregion

        #region Constructors

        private ConfigSettings()
        {
            NetworkInterface = "WebSocket";
            Port = 65000;
            InactivityTimeoutInterval = 10000;
            DbServerName = @"localhost\SQLExpress";
        }

        #endregion

        #region Methods

        public static ConfigSettings ReadConfigFile()
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                return JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(CONFIG_FILE_PATH));
            }

            var settings = new ConfigSettings();
            File.WriteAllText(CONFIG_FILE_PATH, JsonConvert.SerializeObject(settings, Formatting.Indented));
            return settings;
        }

        #endregion
    }
}
