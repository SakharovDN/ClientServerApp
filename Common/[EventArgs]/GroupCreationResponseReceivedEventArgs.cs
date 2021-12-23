namespace Common
{
    using System;

    public class GroupCreationResponseReceivedEventArgs : EventArgs
    {
        #region Properties

        public ResultCodes Result { get; }

        public string Reason { get; }

        #endregion

        #region Constructors

        public GroupCreationResponseReceivedEventArgs(ResultCodes result, string reason)
        {
            Result = result;
            Reason = reason;
        }

        #endregion
    }
}
