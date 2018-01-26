using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework.Logging;

namespace Marketplace.Framework
{
    public class ProjectionManager
    {
        public static readonly ProjectionManagerBuilder With = new ProjectionManagerBuilder();

        private static readonly ILog Log = LogProvider.For<ProjectionManager>();
        private readonly ICheckpointStore _checkpointStore;

        private readonly IEventStoreConnection _connection;
        private readonly int _maxLiveQueueSize;
        private readonly Projection[] _projections;
        private readonly int _readBatchSize;
        private readonly ISerializer _serializer;
        private readonly TypeMapper _typeMapper;

        internal ProjectionManager(
            IEventStoreConnection connection, ICheckpointStore checkpointStore, ISerializer serializer,
            TypeMapper typeMapper, Projection[] projections, int? maxLiveQueueSize, int? readBatchSize)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _checkpointStore = checkpointStore ?? throw new ArgumentNullException(nameof(checkpointStore));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _typeMapper = typeMapper ?? throw new ArgumentNullException(nameof(typeMapper));
            _projections = projections ?? throw new ArgumentNullException(nameof(projections));
            _maxLiveQueueSize = maxLiveQueueSize ?? 10000;
            _readBatchSize = readBatchSize ?? 500;
        }

        public Task Activate() =>
            Task.WhenAll(_projections.Select(StartProjection));

        private async Task StartProjection(Projection projection)
        {
            var lastCheckpoint = await _checkpointStore.GetLastCheckpoint<Position>(projection);

            var settings = new CatchUpSubscriptionSettings(
                _maxLiveQueueSize,
                _readBatchSize,
                Log.IsTraceEnabled(),
                false,
                projection);

            _connection.SubscribeToAllFrom(
                lastCheckpoint,
                settings,
                EventAppeared(projection),
                LiveProcessingStarted(projection),
                SubscriptionDropped(projection));
        }

        private Action<EventStoreCatchUpSubscription, ResolvedEvent> EventAppeared(Projection projection) =>
            async (_, e) =>
            {
                // always double check if it is a system event ;)
                if (e.OriginalEvent.EventType.StartsWith("$")) return;

                // get the configured clr type name for deserializing the event
                var eventType = _typeMapper.GetType(e.Event.EventType);

                // try to execute the projection
                await projection.Handle(_serializer.Deserialize(e.Event.Data, eventType));

                Log.Trace("{projection} projected {eventType}({eventId})", projection, e.Event.EventType,
                    e.Event.EventId);

                // store the current checkpoint
                await _checkpointStore.SetCheckpoint(e.OriginalPosition, projection);
            };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception>
            SubscriptionDropped(Projection projection) =>
            (subscription, reason, ex) =>
            {
                // TODO: Reevaluate stopping subscriptions when issues with reconnect get fixed.
                // https://github.com/EventStore/EventStore/issues/1127
                // https://groups.google.com/d/msg/event-store/AdKzv8TxabM/VR7UDIRxCgAJ

                subscription.Stop();

                switch (reason)
                {
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

        private static Action<EventStoreCatchUpSubscription> LiveProcessingStarted(Projection projection) => 
            _ => Log.Debug("{projection} projection has caught up, now processing live!", projection);
    }

}
