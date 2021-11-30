namespace Server.Storage
{
    public class AddEventLogItem : QueueItem
    {
        #region Fields

        private readonly string _clientName;
        private readonly bool _isConnected;

        #endregion

        #region Constructors

        public AddEventLogItem(string clientName, bool isConnected)
        {
            _clientName = clientName;
            _isConnected = isConnected;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            string clientState = _isConnected ? "is connected" : "is disconnected";
            string eventLog = $"Client '{_clientName}' {clientState}.";
            storage.EventLogContext.AddEventLogToDt(eventLog);
        }

        #endregion
    }
}
