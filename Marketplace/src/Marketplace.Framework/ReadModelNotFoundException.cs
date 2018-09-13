using System;

namespace Marketplace.Projections
{
    public class ReadModelNotFoundException : Exception
    {
        public ReadModelNotFoundException(string name, string id)
            : base($"Read model {name} with id {id} cannot be found")
        {
        }
    }
}
