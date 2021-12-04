namespace Common
{
    using System;

    public class ChatCreatedBroadcastReceivedEventArgs : EventArgs
    {
        #region Properties

        public Chat Chat { get; }

        #endregion

        #region Constructors

        public ChatCreatedBroadcastReceivedEventArgs(Chat chat)
        {
            Chat = chat;
        }

        #endregion
    }
}
