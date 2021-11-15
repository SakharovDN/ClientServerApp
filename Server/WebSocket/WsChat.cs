namespace Server.WebSocket
{
    using Common;

    using Newtonsoft.Json;

    using Services;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsChat : WebSocketBehavior
    {
        #region Fields

        private readonly JsonSerializerSettings _settings;
        private readonly MessageService _messageService;

        #endregion

        #region Constructors

        public WsChat()
        {
            _settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            _messageService = new MessageService();
            _messageService.MessageRequestHandled += MessageRequestHandled;
        }

        #endregion

        #region Methods

        protected override void OnMessage(MessageEventArgs e)
        {
            _messageService.HandleMessage(e.Data, this);
        }

        private void MessageRequestHandled(object sender, MessageRequestHandledEventArgs e)
        {
            string serializedMessages = JsonConvert.SerializeObject(e.MessageBroadcast, _settings);
            Sessions.Broadcast(serializedMessages);
        }

        #endregion
    }
}
