namespace Common
{
    using System;
    using System.Collections.Generic;

    public class ChatHistoryReceivedEventArgs : EventArgs
    {
        #region Properties

        public List<Message> ChatHistory { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryReceivedEventArgs(List<Message> chatHistory)
        {
            ChatHistory = chatHistory;
        }

        #endregion
    }
}
