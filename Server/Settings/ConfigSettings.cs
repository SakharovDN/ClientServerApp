namespace Server.Settings
{
    using Newtonsoft.Json;

    public class ConfigSettings
    {
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

        #region Methods

        public ConfigSettings GetDefaultConfigSettings()
        {
            return new ConfigSettings
            {
                NetworkInterface = "WebSocket",
                Port = 65000,
                InactivityTimeoutInterval = 60000,
                DbServerName = @"localhost\SQLExpress"
            };
        }

        #endregion
    }
}
