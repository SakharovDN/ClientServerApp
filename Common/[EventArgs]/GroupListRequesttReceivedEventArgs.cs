namespace Common
{
    using System;

    public class GroupListRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientId { get; }

        #endregion

        #region Constructors

        public GroupListRequestReceivedEventArgs(string clientId)
        {
            ClientId = clientId;
        }

        #endregion
    }
}
