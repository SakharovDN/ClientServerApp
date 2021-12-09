﻿namespace Server.Services
{
    using System;
    using System.Collections.Generic;

    using Common;
    using Common.Messages;

    using Newtonsoft.Json;

    using Storage;

    public class GroupService : IGroupService
    {
        #region Fields

        private readonly InternalStorage _storage;

        #endregion

        #region Events

        public event EventHandler<ChatNotExistsEventArgs> ChatNotExists;

        public event EventHandler<RequestHandledEventArgs> GroupListRequestHandled;

        #endregion

        #region Constructors

        public GroupService(InternalStorage storage)
        {
            _storage = storage;
        }

        #endregion

        #region Methods

        public Group GetGroupById(string groupId)
        {
            return _storage.Groups.Find(Guid.Parse(groupId));
        }

        public List<string> GetClientIds(string groupId)
        {
            return JsonConvert.DeserializeObject<List<string>>(GetGroupById(groupId).ClientIds);
        }

        public void CreateNewGroup(Group group)
        {
            _storage.AddQueueItem(new CreateNewGroupItem(group));
        }

        public void HandleGroupCreationRequest(object sender, GroupCreationRequestReceivedEventArgs args)
        {
            DateTime timestamp = DateTime.Now;
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
                LastMessageTimestamp = timestamp,
                MessageAmount = 0
            };
            ChatNotExists?.Invoke(sender, new ChatNotExistsEventArgs(chat));
        }

        public void HandleGroupListRequest(object sender, GroupListRequestReceivedEventArgs args)
        {
            var groups = new List<Group>();

            foreach (Group group in _storage.Groups)
            {
                List<string> clientIds = GetClientIds(group.Id.ToString());

                if (clientIds.Contains(args.ClientId))
                {
                    groups.Add(group);
                }
            }

            GroupListRequestHandled?.Invoke(sender, new RequestHandledEventArgs(new GroupListResponse(groups).GetContainer()));
        }

        #endregion
    }
}