namespace Server.Storage
{
    using Common;

    public class CreateNewGroupItem : QueueItem
    {
        #region Fields

        private readonly Group _group;

        #endregion

        #region Constructors

        public CreateNewGroupItem(Group group)
        {
            _group = group;
        }

        #endregion

        #region Methods

        public override void Accept(InternalStorage storage)
        {
            storage.Groups.Add(_group);
            storage.SaveChanges();
        }

        #endregion
    }
}
