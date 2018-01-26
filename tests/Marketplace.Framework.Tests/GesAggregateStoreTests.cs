using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EventStore.ClientAPI;
using FluentAssertions;
using Marketplace.Domain.ClassifiedAds;
using Xunit;

namespace Marketplace.Framework.Tests
{
    public class GesAggregateStoreTests
    {
        public GesAggregateStoreTests()
        {
            Connection = GetConnection().Result;
            Serializer = new JsonNetSerializer();
            TypeMapper = new TypeMapper();
            AutoFixture = new Fixture();

            TypeMapper.Map<Events.V1.ClassifiedAdCreated>("ClassifiedAdCreated");
            TypeMapper.Map<Events.V1.ClassifiedAdPublished>("ClassifiedAdPublished");
            TypeMapper.Map<Events.V1.ClassifiedAdMarkedAsSold>("ClassifiedAdMarkedAsSold");
        }

        private IEventStoreConnection Connection { get; }
        private ISerializer Serializer { get; }
        private TypeMapper TypeMapper { get; }
        private Fixture AutoFixture { get; }

        private static async Task<IEventStoreConnection> GetConnection()
        {
            var connection = EventStoreConnection.Create(
                new IPEndPoint(IPAddress.Loopback, 1113)
            );

            await connection
                .ConnectAsync()
                .ConfigureAwait(false);

            return connection;
        }

        [Fact]
        public async Task can_save_aggregate()
        {
            var aggregate = new ClassifiedAd();

            aggregate.Apply(AutoFixture.Create<Events.V1.ClassifiedAdCreated>());
            aggregate.Apply(AutoFixture.Create<Events.V1.ClassifiedAdPublished>());
            aggregate.Apply(AutoFixture.Create<Events.V1.ClassifiedAdMarkedAsSold>());

            var sut = new GesAggregateStore((type, id) => id, Connection, Serializer, TypeMapper);

            var result = await sut.Save(aggregate);

            // act & assert           

            result.NextExpectedVersion.ShouldBeEquivalentTo(2);
        }
    }
}
