namespace Client.Messenger
{
    using System.Windows;

    using Common;

    public class ClientMessage : Message
    {
        #region Properties

        public HorizontalAlignment Position { get; set; }

        #endregion

        #region Constructors

        public ClientMessage(Message message, string clientName)
        {
            Body = message.Body;
            SourceName = message.SourceName;
            Timestamp = message.Timestamp;
            Position = clientName == SourceName ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        #endregion
    }
}
