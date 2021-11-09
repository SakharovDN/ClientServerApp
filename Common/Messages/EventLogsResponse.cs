namespace Common.Messages
{
    using System.Data;

    public class EventLogsResponse
    {
        #region Properties

        public DataTable EventLogs { get; set; }

        #endregion

        #region Constructors

        public EventLogsResponse(DataTable eventLogs)
        {
            EventLogs = eventLogs;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.EventLogsResponse,
                Payload = this
            };
        }

        #endregion
    }
}
