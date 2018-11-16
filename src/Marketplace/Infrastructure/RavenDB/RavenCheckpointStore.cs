using System;
using System.Threading.Tasks;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure.RavenDB
{
    public class RavenCheckpointStore : ICheckpointStore
    {
        static readonly Serilog.ILogger Log = Serilog.Log.ForContext<RavenCheckpointStore>();

        readonly Func<IAsyncDocumentSession> _getSession;

        public RavenCheckpointStore(Func<IAsyncDocumentSession> getSession) => _getSession = getSession;

        public async Task<T> GetLastCheckpoint<T>(string projection)
        {
            Log.Verbose("Getting last checkpoint for {projection}...", projection);

            using (var session = _getSession())
            {
                var doc = await session.LoadAsync<CheckpointDocument>(GetCheckpointDocumentId(projection));

                if (doc != null)
                {
                    var checkpoint = (T) doc.Checkpoint;
                    Log.Debug("{projection} last checkpoint is {checkpoint}", projection, checkpoint);
                    return checkpoint;
                }

                Log.Verbose("Checkpoint for {projection} not found", projection);
                return default;
            }
        }

        public async Task SetCheckpoint<T>(T checkpoint, string projection)
        {
            Log.Verbose("Setting checkpoint {checkpoint} for {projection}...", checkpoint, projection);

            using (var session = _getSession())
            {
                var id = GetCheckpointDocumentId(projection);
                var doc = await session.LoadAsync<CheckpointDocument>(id);

                if (doc != null)
                    doc.Checkpoint = checkpoint;
                else
                    await session.StoreAsync(new CheckpointDocument { Checkpoint = checkpoint }, id);

                await session.SaveChangesAsync();

                Log.Debug("{projection} checkpoint set to {checkpoint}", projection, checkpoint);
            }
        }

        static string GetCheckpointDocumentId(string projection) =>
            $"Checkpoints/{projection}";

        class CheckpointDocument
        {
            public object Checkpoint { get; set; }
        }
    }
}
