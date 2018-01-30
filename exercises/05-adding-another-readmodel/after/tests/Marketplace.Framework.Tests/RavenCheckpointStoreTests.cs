using System;
using System.Threading.Tasks;
using AutoFixture;
using EventStore.ClientAPI;
using FluentAssertions;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Xunit;

namespace Marketplace.Framework.Tests
{
    public class RavenCheckpointStoreTests : IDisposable
    {
        public RavenCheckpointStoreTests()
        {
            AutoFixture = new Fixture();
            GetDocumentSession = () => LazyStore.Value.OpenAsyncSession();
        }

        public void Dispose()
        {
            LazyStore.Value?.Dispose();
        }

        private Func<IAsyncDocumentSession> GetDocumentSession { get; }
        private Fixture AutoFixture { get; }

        private static readonly Lazy<IDocumentStore> LazyStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Urls = new[] {"http://localhost:8080"},
                Database = "Default"
            };

            return store.Initialize();
        });

        [Fact]
        public async Task can_set_and_get_checkpoint()
        {
            var sut = new RavenCheckpointStore(GetDocumentSession);
            var projection = AutoFixture.Create<string>();
            var expectedCheckpoint = AutoFixture.Create<Position>();

            Func<Task> setCheckpoint = () => sut.SetCheckpoint(expectedCheckpoint, projection);

            setCheckpoint.ShouldNotThrow();

            var checkpoint = await sut.GetLastCheckpoint<Position>(projection);

            checkpoint.ShouldBeEquivalentTo(expectedCheckpoint);
        }
    }
}
