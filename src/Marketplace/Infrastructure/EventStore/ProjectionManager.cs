using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework;
using Serilog.Events;

namespace Marketplace.Infrastructure.EventStore
{
    public class ProjectionManager
    {
        public static readonly ProjectionManagerBuilder With = new ProjectionManagerBuilder();

        static readonly Serilog.ILogger Log = Serilog.Log.ForContext<ProjectionManager>();

        readonly ICheckpointStore _checkpointStore;
        readonly IEventStoreConnection _connection;
        readonly int _maxLiveQueueSize;
        readonly Projection[] _projections;
        readonly int _readBatchSize;
        readonly ISerializer _serializer;
        readonly TypeMapper _typeMapper;

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

        public async Task Activate()
        {
            foreach (var projection in _projections)
            {
                await StartProjection(projection);
            }
            //return Task.WhenAll(_projections.Select(StartProjection));
        }

        async Task StartProjection(Projection projection)
        {
            var lastCheckpoint = await _checkpointStore.GetLastCheckpoint<Position>(projection);

            var settings = new CatchUpSubscriptionSettings(
                _maxLiveQueueSize,
                _readBatchSize,
                Log.IsEnabled(LogEventLevel.Verbose),
                false,
                projection);

            _connection.SubscribeToAllFrom(
                lastCheckpoint,
                settings,
                EventAppeared(projection),
                LiveProcessingStarted(projection),
                SubscriptionDropped(projection));
        }

        Action<EventStoreCatchUpSubscription, ResolvedEvent> EventAppeared(Projection projection)
            => async (_, e) =>
            {
                // always double check if it is a system event ;)
                if (e.OriginalEvent.EventType.StartsWith("$")) return;

                // get the configured clr type name for deserializing the event
                if (!_typeMapper.TryGetType(e.Event.EventType, out var eventType))
                {
                    Log.Debug("Failed to find clr type for {eventType}. Skipping...", e.Event.EventType);
                }
                else
                {
                    // deserialize event
                    var domainEvent = _serializer.Deserialize(e.Event.Data, eventType);

                    // try to execute the projection
                    if (projection.CanHandle(domainEvent))
                    {
                        await projection.Handle(domainEvent);
                        Log.Debug("{projection} handled {event}", projection, domainEvent);
                    }
                }

                // store the current checkpoint
                await _checkpointStore.SetCheckpoint(e.OriginalPosition, projection);
            };

        Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception>SubscriptionDropped(Projection projection)
            => (subscription, reason, ex) =>
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
                        Log.Error(
                            "{projection} projection stopped because of a transient error ({reason}). " +
                            "Attempting to restart...",
                            ex, projection, reason);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        Task.Run(() => StartProjection(projection));
                        break;
                    default:
                        Log.Fatal(
                            "{projection} projection stopped because of an internal error ({reason}). " +
                            "Please check your logs for details.",
                            ex, projection, reason);
                        break;
                }
            };

        static Action<EventStoreCatchUpSubscription> LiveProcessingStarted(Projection projection)
            => _ => Log.Debug("{projection} projection has caught up, now processing live!", projection);
    }
}
