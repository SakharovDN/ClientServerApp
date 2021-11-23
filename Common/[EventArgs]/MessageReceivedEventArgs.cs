﻿namespace Common
{
    using System;

    public class MessageReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ClientName { get; }

        public string Message { get; }

        #endregion

        #region Constructors

        public MessageReceivedEventArgs(string clientName, string message)
        {
            ClientName = clientName;
            Message = message;
        }

        #endregion
    }
}
