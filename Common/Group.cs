namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Group
    {
        #region Properties

        public Guid Id { get; set; }

        public string CreatorId { get; set; }

        [NotMapped]
        public Client Creator { get; set; }

        public string ClientIds { get; set; }

        [NotMapped]
        public List<Client> Clients { get; set; }

        #endregion
    }
}
