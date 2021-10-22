namespace Common
{
    using System.IO;

    using Newtonsoft.Json;

    public class ConfigSettings
    {
        #region Properties

        public string NetworkInterface { get; set; }

        public int Port { get; set; }

        public int InactivityTimeoutInterval { get; set; }

        public string DbConnection { get; set; }

        #endregion

        #region Methods

        public static ConfigSettings Read(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                var serializer = new JsonSerializer();

                return (ConfigSettings) serializer.Deserialize(file, typeof(ConfigSettings));
            }
        }

        #endregion
    }
}
