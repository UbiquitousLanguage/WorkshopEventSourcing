using System;
using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;

namespace Marketplace
{
    public class ClassifiedAdsApplicationService
    {
        private readonly IAggregateStore _store;

        public ClassifiedAdsApplicationService(IAggregateStore store)
        {
            _store = store;
        }

        public Task Handle(ClassifiedAds.V1.Create command)
        {
            var ad = ClassifiedAd.Create(
                command.Id,
                command.OwnerId,
                command.CreatedBy,
                command.CreatedAt);
            return _store.Save(ad);
        }

        public async Task Handle(ClassifiedAds.V1.RenameAd command)
        {
            var ad = await _store.Load<ClassifiedAd>(command.Id.ToString());
            ad.Rename(command.Title, command.RenamedAt, command.RenamedBy);
            await _store.Save(ad);
        }
    }
}
