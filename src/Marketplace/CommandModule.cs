using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Framework;

namespace Marketplace
{
    public class CommandModule
    {
        private readonly IAggregateStore _store;

        public CommandModule(IAggregateStore store)
        {
            _store = store;
        }

        public async Task Handle(object command)
        {
            switch (command)
            {
                case ClassifiedAds.V1.CreateClassifiedAd cmd:
                    var ad = Domain.ClassifiedAds.ClassifiedAd.Create(
                        cmd.Id,
                        cmd.OwnerId,
                        cmd.CreatedBy,
                        cmd.CreatedAt);
                    await _store.Save(ad);
                    break;
            }
        }
    }
}
