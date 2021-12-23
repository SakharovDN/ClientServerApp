namespace Server.Services
{
    using System;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using Storage;

    public class GroupService : IGroupService
    {
        #region Constants

        private const string COMMON_CHAT_NAME = "Common";

        #endregion

        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Events

        public event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        public event EventHandler<RequestHandledEventArgs> GroupCreationRequestHandled;

        #endregion

        #region Constructors

        public GroupService(InternalStorage storage)
        {
            _storage = storage;
        }

        #endregion

        #region Methods

        public void CreateNewGroup(Group group)
        {
            _storage.AddQueueItem(new CreateNewGroupItem(group));
        }

        public void HandleGroupCreationRequest(object sender, GroupCreationRequestReceivedEventArgs args)
        {
            if (string.Equals(args.GroupTitle, COMMON_CHAT_NAME, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageContainer groupCreationResponse = new GroupCreationResponse(
                    ResultCodes.Failure,
                    "The title \"Common\" is not available. Please choose another title.").GetContainer();
                GroupCreationRequestHandled?.Invoke(sender, new RequestHandledEventArgs(groupCreationResponse));
                return;
            }

            string clientIds = JsonConvert.SerializeObject(args.ClientIds);
            var group = new Group
            {
                Id = Guid.NewGuid(),
                ClientIds = clientIds,
                CreatorId = args.CreatorId
            };
            CreateNewGroup(group);
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Type = ChatTypes.Group,
                SourceId = null,
                SourceName = null,
                TargetId = group.Id.ToString(),
                TargetName = args.GroupTitle,
                MessageAmount = 0
            };
            ChatNotExists?.Invoke(sender, new ChatNotExistsEventArgs(chat, args.ClientIds));
        }

        #endregion
    }
}
