namespace Marketplace.Framework
{
    using System.Threading.Tasks;

    public interface ICheckpointStore
    {
        Task<T> GetLastCheckpoint<T>(string projection);
        Task    SetCheckpoint<T>(T checkpoint, string projection);
    }
}
