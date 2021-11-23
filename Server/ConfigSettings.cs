namespace Server
{
    using System.IO;

    using Newtonsoft.Json;

    public class ConfigSettings
    {
        #region Constants

        private const string CONFIG_FILE_PATH = @"config.json";

        #endregion

        #region Properties

        public string NetworkInterface { get; set; }

        public int Port { get; set; }

        public int InactivityTimeoutInterval { get; set; }

        public string DbConnection { get; set; }

        #endregion

        #region Methods

        public static ConfigSettings Receive()
        {
            string jsonString = File.ReadAllText(CONFIG_FILE_PATH);
            return JsonConvert.DeserializeObject<ConfigSettings>(jsonString);
        }

        #endregion
    }
}
