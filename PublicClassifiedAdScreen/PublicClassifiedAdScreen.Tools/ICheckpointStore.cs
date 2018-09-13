using System.Threading.Tasks;

namespace PublicClassifiedAdScreen.Tools
{
    public interface ICheckpointStore
    {
        Task<T> GetLastCheckpoint<T>(string projection);
        Task SetCheckpoint<T>(T checkpoint, string projection);
    }
}
