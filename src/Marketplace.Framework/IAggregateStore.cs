using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IAggregateStore
    {
        /// <summary>
        ///     Loads and returns an aggregate by id, from the store.
        /// </summary>
        Task<T> Load<T>(string aggregateId, CancellationToken cancellationToken = default) where T : Aggregate, new();

        /// <summary>
        ///     Saves changes to the store.
        /// </summary>
        Task<(long NextExpectedVersion, long LogPosition, long CommitPosition )> Save<T>(
            T aggregate, CancellationToken cancellationToken = default) where T : Aggregate;

        /// <summary>
        ///     Returns the last version of the aggregate if found.
        /// </summary>
        Task<long> GetLastVersionOf<T>(string aggregateId, CancellationToken cancellationToken = default)
            where T : Aggregate;
    }
}
