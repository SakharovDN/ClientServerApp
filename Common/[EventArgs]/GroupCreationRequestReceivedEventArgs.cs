namespace Common
{
    using System.Collections.Generic;

    public class GroupCreationRequestReceivedEventArgs
    {
        #region Properties

        public string GroupTitle { get; }

        public List<string> ClientIds { get; }

        public string CreatorId { get; }

        #endregion

        #region Constructors

        public GroupCreationRequestReceivedEventArgs(string groupTitle, List<string> clientIds, string creatorId)
        {
            GroupTitle = groupTitle;
            ClientIds = clientIds;
            CreatorId = creatorId;
        }

        #endregion
    }
}
