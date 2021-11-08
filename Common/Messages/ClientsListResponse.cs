namespace Common.Messages
{
    using System;
    using System.Collections.Generic;

    public class ClientsListResponse
    {
        #region Properties

        public Dictionary<Guid, WsClient> Clients { get; set; }

        #endregion

        #region Constructors

        public ClientsListResponse(Dictionary<Guid, WsClient> clients)
        {
            Clients = clients;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ClientsListResponse,
                Payload = this
            };
        }

        #endregion
    }
}
