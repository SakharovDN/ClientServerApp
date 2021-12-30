namespace Server.Services
{
    using System;
    using System.Collections.Generic;

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

        public event EventHandler<RequestHandledEventArgs> GroupLeavingRequestHandled;

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

        public void HandleGroupLeavingRequest(object sender, GroupLeavingRequestReceivedEventArgs args)
        {
            if (!Guid.TryParse(args.ChatId, out Guid chatId))
            {
                return;
            }

            Chat chat = _storage.Chats.Find(chatId);

            if (chat == null)
            {
                return;
            }

            if (!Guid.TryParse(chat.TargetId, out Guid groupId))
            {
                return;
            }

            Group group = _storage.Groups.Find(groupId);

            if (group == null)
            {
                return;
            }

            var clientIds = JsonConvert.DeserializeObject<List<string>>(group.ClientIds);

            if (clientIds == null)
            {
                return;
            }

            clientIds.Remove(args.ClientId);
            _storage.AddQueueItem(new UpdateGroupClientsItem(chat.TargetId, clientIds));
            GroupLeavingRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new GroupLeavingResponse(args.ChatId).GetContainer()));
        }

        #endregion
    }
}
