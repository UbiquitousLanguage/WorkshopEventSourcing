using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Marketplace.Framework
{
    public class GesAggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;

        private readonly GetStreamName _getStreamName;
        private readonly TypeMapper _typeMapper;
        private readonly UserCredentials _userCredentials;

        public GesAggregateStore(
            GetStreamName getStreamName,
            IEventStoreConnection connection,
            TypeMapper typeMapper,
            UserCredentials userCredentials = null)
        {
            _getStreamName = getStreamName ?? throw new ArgumentNullException(nameof(connection));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _typeMapper = typeMapper ?? throw new ArgumentNullException(nameof(typeMapper));
            _userCredentials = userCredentials;
        }

        /// <summary>
        ///     Loads and returns an aggregate by id, from the store.
        /// </summary>
        public async Task<T> Load<T>(string aggregateId) where T : Aggregate, new()
        {
            var stream = _getStreamName(typeof(T), aggregateId);
            var aggregate = new T();

            var page = await _connection.ReadStreamEventsForwardAsync(
                stream, 0, 1000, false, _userCredentials);

            aggregate.Load(page.Events.Select(resolvedEvent =>
            {
                var dataType = _typeMapper.GetType(resolvedEvent.Event.EventType);
                var data = JsonConvert.DeserializeObject(
                    Encoding.UTF8.GetString(resolvedEvent.Event.Data), dataType, DefaultSettings);
                return data;
            }).ToArray());

            return aggregate;
        }

        /// <summary>
        ///     Saves changes to the store.
        /// </summary>
        public Task Save<T>(T aggregate) where T : Aggregate
        {
            var changes = aggregate.GetChanges()
                .Select(e => new EventData(
                    Guid.NewGuid(),
                    _typeMapper.GetTypeName(e.GetType()),
                    true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e, DefaultSettings)),
                    null))
                .ToArray();

            var stream = _getStreamName(typeof(T), aggregate.Id.ToString());

            return _connection.AppendToStreamAsync(stream, aggregate.Version, changes, _userCredentials);
        }

        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
