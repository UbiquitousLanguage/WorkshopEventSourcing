namespace Marketplace.Framework
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::EventStore.ClientAPI;
    using Marketplace.Framework.Logging;

    public class ProjectionManager
    {
        public static readonly ProjectionManagerBuilder With = new ProjectionManagerBuilder();

        private static readonly ILog Log = LogProvider.For<ProjectionManager>();

        private readonly IEventStoreConnection _connection;
        private readonly ICheckpointStore      _checkpointStore;
        private readonly ISerializer           _serializer;
        private readonly TypeMapper            _typeMapper;
        private readonly int                   _maxLiveQueueSize;
        private readonly int                   _readBatchSize;
        private readonly Projection[]          _projections;
            
        internal ProjectionManager(
            IEventStoreConnection connection, ICheckpointStore checkpointStore, ISerializer serializer,
            TypeMapper typeMapper, Projection[] projections, int? maxLiveQueueSize, int? readBatchSize) {
            _connection       = connection       ?? throw new ArgumentNullException(nameof(connection));
            _checkpointStore  = checkpointStore  ?? throw new ArgumentNullException(nameof(checkpointStore));
            _serializer       = serializer       ?? throw new ArgumentNullException(nameof(serializer));
            _typeMapper       = typeMapper       ?? throw new ArgumentNullException(nameof(typeMapper));
            _projections      = projections      ?? throw new ArgumentNullException(nameof(projections));
            _maxLiveQueueSize = maxLiveQueueSize ?? 10000;
            _readBatchSize    = readBatchSize    ?? 500;
        }

        public Task Activate() => Task.WhenAll(_projections.Select(StartProjection));

        private async Task StartProjection(Projection projection) {
            var lastCheckpoint = await _checkpointStore.GetLastCheckpoint<Position>(projection);

            var settings = new CatchUpSubscriptionSettings(
                maxLiveQueueSize: _maxLiveQueueSize,
                readBatchSize   : _readBatchSize,
                verboseLogging  : Log.IsTraceEnabled(), 
                resolveLinkTos  : false, 
                subscriptionName: projection);
            
            _connection.SubscribeToAllFrom(
                lastCheckpoint,
                settings,
                EventAppeared(projection),
                LiveProcessingStarted(projection),
                SubscriptionDropped(projection));
        }

        private Action<EventStoreCatchUpSubscription, ResolvedEvent> EventAppeared(Projection projection)
            => async (_, e) => {
                // always double check if it is a system event ;)
                if (e.OriginalEvent.EventType.StartsWith("$")) return; 
                
                // get the configured clr type name for deserializing the event
                var eventType = _typeMapper.GetType(e.Event.EventType);
    
                // try to execute the projection
                await projection.Handle(_serializer.Deserialize(e.Event.Data, eventType));
    
                Log.Trace("{projection} projected {eventType}({eventId})", projection, e.Event.EventType, e.Event.EventId);

                // store the current checkpoint
                await _checkpointStore.SetCheckpoint(e.OriginalPosition, projection);
            };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> SubscriptionDropped(Projection projection) 
            => (subscription, reason, ex) => {
                // TODO: Reevaluate stopping subscriptions when issues with reconnect get fixed.
                // https://github.com/EventStore/EventStore/issues/1127
                // https://groups.google.com/d/msg/event-store/AdKzv8TxabM/VR7UDIRxCgAJ
    
                subscription.Stop();
    
                switch (reason) {
                    case SubscriptionDropReason.UserInitiated:
                        Log.Debug("{projection} projection stopped gracefully.", projection);
                        break;
                    case SubscriptionDropReason.SubscribingError:
                    case SubscriptionDropReason.ServerError:
                    case SubscriptionDropReason.ConnectionClosed:
                    case SubscriptionDropReason.CatchUpError:
                    case SubscriptionDropReason.ProcessingQueueOverflow:
                    case SubscriptionDropReason.EventHandlerException:
                        Log.ErrorException(
                            "{projection} projection stopped because of a transient error ({reason}). " +
                            "Attempting to restart...",
                            ex, projection, reason);
                        Task.Run(() => StartProjection(projection));
                        break;
                    default:
                        Log.FatalException(
                            "{projection} projection stopped because of an internal error ({reason}). " +
                            "Please check your logs for details.",
                            ex, projection, reason);
                        break;
                }
            };

        private static Action<EventStoreCatchUpSubscription> LiveProcessingStarted(Projection projection)
            => _ => Log.Debug("{projection} projection has caught up, now processing live!", projection);
    }
    
    public class FunctionalProjectionManager
    {
        private static readonly ILog Log = LogProvider.For<ProjectionManager>();

        private readonly IEventStoreConnection                      _connection;
        private readonly ICheckpointStore                           _checkpointStore;
        private readonly ISerializer                                _serializer;
        private readonly TypeMapper                                 _typeMapper;
        private readonly int                                        _maxLiveQueueSize;
        private readonly int                                        _readBatchSize;
        private readonly (string Name, Func<object, Task> Handle)[] _projections;
            
        internal FunctionalProjectionManager(
            IEventStoreConnection connection, ICheckpointStore checkpointStore, ISerializer serializer,
            TypeMapper typeMapper, (string Name, Func<object, Task>)[] projections, 
            int? maxLiveQueueSize, int? readBatchSize) {
            _connection       = connection       ?? throw new ArgumentNullException(nameof(connection));
            _checkpointStore  = checkpointStore  ?? throw new ArgumentNullException(nameof(checkpointStore));
            _serializer       = serializer       ?? throw new ArgumentNullException(nameof(serializer));
            _typeMapper       = typeMapper       ?? throw new ArgumentNullException(nameof(typeMapper));
            _projections      = projections      ?? throw new ArgumentNullException(nameof(projections));
            _maxLiveQueueSize = maxLiveQueueSize ?? 10000;
            _readBatchSize    = readBatchSize    ?? 500;
        }

        public Task Activate() => Task.WhenAll(_projections.Select(StartProjection));

        private async Task StartProjection((string Name, Func<object, Task>) projection) {
            var lastCheckpoint = await _checkpointStore.GetLastCheckpoint<Position>(projection.Name);

            var settings = new CatchUpSubscriptionSettings(
                maxLiveQueueSize: _maxLiveQueueSize,
                readBatchSize   : _readBatchSize,
                verboseLogging  : Log.IsTraceEnabled(), 
                resolveLinkTos  : false, 
                subscriptionName: projection.Name);
            
            _connection.SubscribeToAllFrom(
                lastCheckpoint,
                settings,
                EventAppeared(projection),
                LiveProcessingStarted(projection),
                SubscriptionDropped(projection));
        }

        private Action<EventStoreCatchUpSubscription, ResolvedEvent> EventAppeared((string Name, Func<object, Task> Handle) projection)
            => async (_, e) => {
                // always double check if it is a system event ;)
                if (e.OriginalEvent.EventType.StartsWith("$")) return; 
                
                // get the configured clr type name for deserializing the event
                var eventType = _typeMapper.GetType(e.Event.EventType);
    
                // try to execute the projection
                await projection.Handle(_serializer.Deserialize(e.Event.Data, eventType));
    
                Log.Trace("{projection} projected {eventType}({eventId})", projection.Name, e.Event.EventType, e.Event.EventId);

                // store the current checkpoint
                await _checkpointStore.SetCheckpoint(e.OriginalPosition, projection.Name);
            };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> SubscriptionDropped((string Name, Func<object, Task> Handle) projection) 
            => (subscription, reason, ex) => {
                // TODO: Reevaluate stopping subscriptions when issues with reconnect get fixed.
                // https://github.com/EventStore/EventStore/issues/1127
                // https://groups.google.com/d/msg/event-store/AdKzv8TxabM/VR7UDIRxCgAJ
    
                subscription.Stop();
    
                switch (reason) {
                    case SubscriptionDropReason.UserInitiated:
                        Log.Debug("{projection} projection stopped gracefully.", projection.Name);
                        break;
                    case SubscriptionDropReason.SubscribingError:
                    case SubscriptionDropReason.ServerError:
                    case SubscriptionDropReason.ConnectionClosed:
                    case SubscriptionDropReason.CatchUpError:
                    case SubscriptionDropReason.ProcessingQueueOverflow:
                    case SubscriptionDropReason.EventHandlerException:
                        Log.ErrorException(
                            "{projection} projection stopped because of a transient error ({reason}). " +
                            "Attempting to restart...",
                            ex, projection.Name, reason);
                        Task.Run(() => StartProjection(projection));
                        break;
                    default:
                        Log.FatalException(
                            "{projection} projection stopped because of an internal error ({reason}). " +
                            "Please check your logs for details.",
                            ex, projection.Name, reason);
                        break;
                }
            };

        private static Action<EventStoreCatchUpSubscription> LiveProcessingStarted((string Name, Func<object, Task> Handle) projection)
            => _ => Log.Debug("{projection} projection has caught up, now processing live!", projection.Name);
    }
}