namespace Common._EventArgs_
{
    using System;

    public class ConnectionStateChangedEventArgs
    {
        #region Properties

        public Guid Id { get; }

        public WsConnection Connection { get; }

        public bool Connected { get; }

        #endregion

        #region Constructors

        public ConnectionStateChangedEventArgs(Guid id, WsConnection connection, bool connected)
        {
            Id = id;
            Connection = connection;
            Connected = connected;
        }

        #endregion
    }
}
