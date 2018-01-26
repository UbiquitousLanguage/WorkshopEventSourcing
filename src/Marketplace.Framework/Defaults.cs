using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Marketplace.Framework
{
    public static class Defaults
    {
        public static async Task<IEventStoreConnection> GetConnection()
        {
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            await connection.ConnectAsync();
            return connection;
        }
    }
}
