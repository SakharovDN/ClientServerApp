namespace Common
{
    using System;

    using Messages;

    public class ChatHistoryRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ChatId { get; set; }

        public Action<object, MessageContainer> Send { get; set; }

        #endregion

        #region Constructors

        public ChatHistoryRequestReceivedEventArgs(string chatId, Action<object, MessageContainer> send)
        {
            ChatId = chatId;
            Send = send;
        }

        #endregion
    }
}
