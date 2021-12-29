namespace Server.Storage
{
    using System;
    using System.Collections.Generic;

    using Common;

    using Newtonsoft.Json;

    public class UpdateGroupClientsItem : QueueItem
    {
        #region Fields

        private readonly string _groupId;
        private readonly List<string> _clientIds;

        #endregion

        #region Constructors

        public UpdateGroupClientsItem(string groupId, List<string> clientIds)
        {
            _groupId = groupId;
            _clientIds = clientIds;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            if (Guid.TryParse(_groupId, out Guid id))
            {
                Group group = storage.Groups.Find(id);

                if (group == null)
                {
                    return;
                }

                group.ClientIds = JsonConvert.SerializeObject(_clientIds);
                storage.SaveChanges();
            }
            else
            {
                throw new Exception("Failed to get group id");
            }
        }

        #endregion
    }
}
