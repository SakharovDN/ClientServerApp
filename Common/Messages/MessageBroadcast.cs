﻿namespace Common.Messages
{
    public class MessageBroadcast
    {
        #region Properties

        public Message Message { get; set; }

        #endregion

        #region Constructors

        public MessageBroadcast(Message message)
        {
            Message = message;
        }

        #endregion

        #region Methods

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Type = MessageTypes.MessageBroadcast,
                Payload = this
            };
            return container;
        }

        #endregion
    }
}
