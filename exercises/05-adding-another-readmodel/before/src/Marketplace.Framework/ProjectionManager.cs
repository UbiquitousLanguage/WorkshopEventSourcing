using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework.Logging;

namespace Marketplace.Framework
{
    public class ProjectionManager
    {
        private readonly ICheckpointStore _checkpointStore;

        private readonly IEventStoreConnection _connection;
        private readonly Projection[] _projections;
        private readonly ISerializer _serializer;
        private readonly TypeMapper _typeMapper;

        public ProjectionManager(
            IEventStoreConnection connection, ICheckpointStore checkpointStore, ISerializer serializer,
            TypeMapper typeMapper, Projection[] projections)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _checkpointStore = checkpointStore ?? throw new ArgumentNullException(nameof(checkpointStore));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _typeMapper = typeMapper ?? throw new ArgumentNullException(nameof(typeMapper));
            _projections = projections ?? throw new ArgumentNullException(nameof(projections));
        }

        public Task Activate() =>
            Task.WhenAll(_projections.Select(StartProjection));

        private async Task StartProjection(Projection projection)
        {
            var lastCheckpoint = await _checkpointStore.GetLastCheckpoint<Position>(projection);

            var settings = new CatchUpSubscriptionSettings(
                10000,
                500,
                true,
                false,
                projection);

            _connection.SubscribeToAllFrom(
                lastCheckpoint,
                settings,
                EventAppeared(projection));
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

                // store the current checkpoint
                await _checkpointStore.SetCheckpoint(e.OriginalPosition, projection);
            };

    }

}
