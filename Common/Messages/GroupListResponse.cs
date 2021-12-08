namespace Common.Messages
{
    using System.Collections.Generic;

    public class GroupListResponse
    {
        #region Properties

        public List<Group> Groups { get; set; }

        #endregion

        #region Constructors

        public GroupListResponse(List<Group> groups)
        {
            Groups = groups;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            return new MessageContainer
            {
                Type = MessageTypes.GroupListResponse,
                Payload = this
            };
        }

        #endregion
    }
}
