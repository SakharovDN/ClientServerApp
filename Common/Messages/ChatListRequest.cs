namespace Common.Messages
{
    using System.Collections.Generic;

    public class ChatListRequest
    {
        #region Properties

        public string ClientId { get; set; }

        public List<Group> ClientGroups { get; set; }

        #endregion

        #region Constructors

        public ChatListRequest(string clientId, List<Group> clientGroups)
        {
            ClientId = clientId;
            ClientGroups = clientGroups;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.ChatListRequest,
                Payload = this
            };
        }

        #endregion
    }
}
