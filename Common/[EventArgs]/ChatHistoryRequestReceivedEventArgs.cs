namespace Common
{
    using System;

    public class ChatHistoryRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ChatId { get; }

        #endregion

        #region Constructors

        public ChatHistoryRequestReceivedEventArgs(string chatId)
        {
            ChatId = chatId;
        }

        #endregion
    }
}
