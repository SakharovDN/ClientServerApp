namespace Server.Services
{
    using System;

    using Common;

    using Newtonsoft.Json;

    using Storage;

    public class GroupService : IGroupService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Events

        public event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

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
