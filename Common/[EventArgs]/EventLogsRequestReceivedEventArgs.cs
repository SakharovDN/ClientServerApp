namespace Common
{
    using System;

    using Messages;

    public class EventLogsRequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public Action<object, MessageContainer> Send { get; }

        #endregion

        #region Constructors

        public EventLogsRequestReceivedEventArgs(Action<object, MessageContainer> send)
        {
            Send = send;
        }

        #endregion
    }
}
