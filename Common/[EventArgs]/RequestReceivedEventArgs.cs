﻿namespace Common
{
    using System;

    public class RequestReceivedEventArgs : EventArgs
    {
        #region Properties

        public string ConnectionId { get; }

        public string SerializedRequest { get; }

        #endregion

        #region Constructors

        public RequestReceivedEventArgs(string connectionId, string serializedRequest)
        {
            ConnectionId = connectionId;
            SerializedRequest = serializedRequest;
        }

        #endregion
    }
}
