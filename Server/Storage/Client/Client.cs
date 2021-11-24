namespace Server.Storage.Client
{
    using System;

    public class Client
    {
        #region Properties

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsConnected { get; set; }

        #endregion
    }
}
