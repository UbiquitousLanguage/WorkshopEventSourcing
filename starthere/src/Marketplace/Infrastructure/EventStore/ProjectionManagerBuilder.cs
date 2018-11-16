using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework;

namespace Marketplace.Infrastructure.EventStore
{
    public class ProjectionManagerBuilder
    {
        ICheckpointStore _checkpointStore;
        IEventStoreConnection _connection;
        int? _maxLiveQueueSize;
        Projection[] _projections;
        int? _readBatchSize;
        ISerializer _serializer;
        TypeMapper _typeMapper;

        public ProjectionManagerBuilder Connection(IEventStoreConnection connection)
        {
            _connection = connection;
            return this;
        }

        public ProjectionManagerBuilder CheckpointStore(ICheckpointStore checkpointStore)
        {
            _checkpointStore = checkpointStore;
            return this;
        }

        public ProjectionManagerBuilder Serializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public ProjectionManagerBuilder TypeMapper(TypeMapper typeMapper)
        {
            _typeMapper = typeMapper;
            return this;
        }

        public ProjectionManagerBuilder MaxLiveQueueSize(int maxLiveQueueSize)
        {
            _maxLiveQueueSize = maxLiveQueueSize;
            return this;
        }

        public ProjectionManagerBuilder ReadBatchSize(int readBatchSize)
        {
            _readBatchSize = readBatchSize;
            return this;
        }

        public ProjectionManagerBuilder Projections(params Projection[] projections)
        {
            _projections = projections;
            return this;
        }

        public ProjectionManager Build() =>
            new ProjectionManager(
                _connection,
                _checkpointStore,
                _serializer,
                _typeMapper,
                _projections,
                _maxLiveQueueSize,
                _readBatchSize
            );

        public async Task<ProjectionManager> Activate()
        {
            var manager = Build();
            await manager.Activate();
            return manager;
        }
    }
}
