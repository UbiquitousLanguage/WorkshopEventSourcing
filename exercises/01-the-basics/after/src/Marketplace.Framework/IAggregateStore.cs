using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IAggregateStore
    {
        /// <summary>
        ///     Loads and returns an aggregate by id, from the store.
        /// </summary>
        Task<T> Load<T>(string aggregateId) where T : Aggregate, new();

        /// <summary>
        ///     Saves changes to the store.
        /// </summary>
        Task Save<T>(T aggregate) where T : Aggregate;
    }
}
