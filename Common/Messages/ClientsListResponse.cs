namespace Common.Messages
{
    using System.Collections.Generic;

    public class ClientsListResponse
    {
        #region Properties

        public List<string> Clients { get; set; }

        #endregion

        #region Constructors

        public ClientsListResponse(List<string> clients)
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
