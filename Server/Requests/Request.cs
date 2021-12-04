namespace Server.Requests
{
    public class Request
    {
        #region Properties

        public string ConnectionId { get; set; }

        public string SerializedRequest { get; set; }

        #endregion

        #region Constructors

        public Request(string connectionId, string serializedRequest)
        {
            ConnectionId = connectionId;
            SerializedRequest = serializedRequest;
        }

        #endregion
    }
}
