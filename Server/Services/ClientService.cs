namespace Server.Services
{
    using System.Collections.Concurrent;
    using System.Timers;

    using Common;
    using Common.Messages;

    using Storage.Client;
    using Storage.EventLog;

    public class ClientService
    {
        #region Fields

        private readonly ClientContext _clientContext;
        private readonly EventLogContext _eventLogContext;
        private readonly ConcurrentQueue<Request> _connectionRequests;
        private readonly ConcurrentQueue<Request> _disconnectionRequests;
        private readonly Timer _timer;

        #endregion

        #region Constructors

        public ClientService(string dbConnection)
        {
            _clientContext = new ClientContext(dbConnection);
            _eventLogContext = new EventLogContext(dbConnection);
            _connectionRequests = new ConcurrentQueue<Request>();
            _disconnectionRequests = new ConcurrentQueue<Request>();
            _timer = new Timer(100);
            _timer.Elapsed += ConstructConnectionResponse;
            _timer.Elapsed += ConstructDisconnectionResponse;
            _timer.Start();
        }

        #endregion

        #region Methods

        public void EnqueueConnectionRequest(Request request)
        {
            _connectionRequests.Enqueue(request);
        }

        public void EnqueueDisconnectionRequest(Request request)
        {
            _disconnectionRequests.Enqueue(request);
        }

        public void Stop()
        {
            _clientContext?.Dispose();
            _eventLogContext?.Dispose();
            _timer?.Dispose();
        }

        private void ConstructDisconnectionResponse(object sender, ElapsedEventArgs e)
        {
            if (!_disconnectionRequests.TryDequeue(out Request request))
            {
                return;
            }

            var disconnectionRequest = (DisconnectionRequest)request.Payload;

            if (!_clientContext.ClientIsConnected(disconnectionRequest.ClientName))
            {
                return;
            }

            _clientContext.ChangeConnectionStatus(disconnectionRequest.ClientName);
            _eventLogContext.AddEventLogToDt($"Client {disconnectionRequest.ClientName} is disconnected");
            MessageContainer connectionStateChangedEcho = new ConnectionStateChangedEcho(disconnectionRequest.ClientName, false).GetContainer();
            request.MessageHandler.SendBroadcast(connectionStateChangedEcho);
            MessageContainer disconnectionResponse = new DisconnectionResponse().GetContainer();
            request.MessageHandler.Send(disconnectionResponse);
        }

        private void ConstructConnectionResponse(object sender, ElapsedEventArgs e)
        {
            if (!_connectionRequests.TryDequeue(out Request request))
            {
                return;
            }

            var connectionRequest = (ConnectionRequest)request.Payload;
            var connectionResponse = new ConnectionResponse
            {
                Result = ResultCodes.Ok
            };

            if (_clientContext.ClientIsConnected(connectionRequest.ClientName))
            {
                connectionResponse.Result = ResultCodes.Failure;
                connectionResponse.Reason = $"Client named '{connectionRequest.ClientName}' is already connected.";
                connectionResponse.ConnectedClients = null;
            }

            if (connectionResponse.Result == ResultCodes.Ok)
            {
                connectionResponse.ConnectedClients = _clientContext.GetConnectedClients();

                if (!_clientContext.ClientExists(connectionRequest.ClientName))
                {
                    _clientContext.AddNewClientToDt(connectionRequest.ClientName);
                }
                else
                {
                    _clientContext.ChangeConnectionStatus(connectionRequest.ClientName);
                }

                _eventLogContext.AddEventLogToDt($"Client {connectionRequest.ClientName} is connected");
                MessageContainer connectionStateChangedEcho = new ConnectionStateChangedEcho(connectionRequest.ClientName, true).GetContainer();
                request.MessageHandler.SendBroadcast(connectionStateChangedEcho);
            }

            request.MessageHandler.Send(connectionResponse.GetContainer());
        }

        #endregion
    }
}
