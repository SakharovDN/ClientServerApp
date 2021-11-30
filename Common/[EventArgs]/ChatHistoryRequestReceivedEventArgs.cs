namespace Common
{
    using System;
    using System.Collections.Generic;

    using Messages;

    public class ChatHistoryRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public List<string> Participants { get; set; }

        public Action<object, MessageContainer> Send { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryRequestReceivedEventArgs(List<string> participants, Action<object, MessageContainer> send)
        {
            Participants = participants;
            Send = send;
        }

        #endregion
    }
}
