namespace Common
{
    using System;
    using System.Collections.Generic;

    public class GroupListResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public List<Group> Groups { get; }

        #endregion

        #region Constructors

        public GroupListResponseReceivedEventArgs(List<Group> groups)
        {
            Groups = groups;
        }

        #endregion
    }
}
