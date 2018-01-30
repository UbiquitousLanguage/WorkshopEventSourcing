using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface ICheckpointStore
    {
        Task<T> GetLastCheckpoint<T>(string projection);
        Task SetCheckpoint<T>(T checkpoint, string projection);
    }
}
