namespace Common.Messages
{
    using System.Collections.Generic;

    public class EventLogsResponse
    {
        #region Properties

        public List<EventLog> EventLogs { get; set; }

        #endregion

        #region Constructors

        public EventLogsResponse(List<EventLog> eventLogs)
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
