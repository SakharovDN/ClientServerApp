namespace Common
{
    using System;

    using Messages;

    public class ChatHistoryRequestHandledEventArgs : EventArgs
    {
        #region Properties

        public MessageContainer ChatHistoryResponse { get; }

        #endregion

        #region Constructors

        public ChatHistoryRequestHandledEventArgs(MessageContainer chatHistoryResponse)
        {
            ChatHistoryResponse = chatHistoryResponse;
        }

        #endregion
    }
}
