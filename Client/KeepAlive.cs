namespace Client
{
    using System.Timers;

    public class KeepAlive
    {
        #region Fields

        private int _pingResponseCounter;
        private readonly Callback _ping;
        private readonly Callback _disconnect;
        private readonly Timer _timer;

        #endregion

        #region Delegates

        public delegate void Callback();

        #endregion

        #region Constructors

        public KeepAlive(WsClient client, int keepAliveInterval)
        {
            _timer = new Timer(keepAliveInterval);
            _pingResponseCounter = 0;
            _timer.Elapsed += Perform;
            _ping = client.Ping;
            _disconnect = client.Disconnect;
        }

        #endregion

        #region Methods

        public void Start()
        {
            _pingResponseCounter = 0;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void ResetPingResponseCounter()
        {
            _pingResponseCounter = 0;
        }

        private void Perform(object sender, ElapsedEventArgs args)
        {
            if (_pingResponseCounter > 3)
            {
                _disconnect.Invoke();
                return;
            }

            _ping.Invoke();
            _pingResponseCounter++;
        }

        #endregion
    }
}
