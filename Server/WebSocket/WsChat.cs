namespace Server.WebSocket
{
    using Services;

    using WebSocketSharp;
    using WebSocketSharp.Server;

    public class WsChat : WebSocketBehavior
    {
        #region Methods

        public void BroadcastMessage(string messageBroadcast)
        {
            Sessions.Broadcast(messageBroadcast);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            MessageService.HandleMessage(e.Data, this);
        }

        #endregion
    }
}
