namespace Common.Messages
{
    using System.Collections.Generic;

    public class ConnectionResponse
    {
        #region Properties

        public ResultCodes Result { get; set; }

        public string Reason { get; set; }

        public HashSet<Client> ConnectedClients { get; set; }

        public string ClientId { get; set; }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Type = MessageTypes.ConnectionResponse,
                Payload = this
            };
            return container;
        }

        #endregion
    }
}
