namespace Common.Messages
{
    using System.Collections.Generic;

    public class GroupCreationRequest
    {
        #region Properties

        public string GroupTitle { get; set; }

        public List<string> ClientIds { get; set; }

        public string CreatorId { get; set; }

        #endregion

        #region Constructors

        public GroupCreationRequest(string groupTitle, List<string> clientIds, string creatorId)
        {
            GroupTitle = groupTitle;
            ClientIds = clientIds;
            CreatorId = creatorId;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.GroupCreationRequest,
                Payload = this
            };
        }

        #endregion
    }
}
