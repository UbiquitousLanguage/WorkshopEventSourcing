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

        public Task Handle(ClassifiedAds.V1.Create command) =>
            _store.Save(ClassifiedAd.Create(
                command.Id,
                command.OwnerId,
                command.CreatedBy,
                command.CreatedAt));

        public Task Handle(ClassifiedAds.V1.RenameAd command) =>
            HandleUpdate(command.Id, ad =>
                ad.Rename(command.Title, command.RenamedAt, command.RenamedBy)
            );

        private async Task HandleUpdate(Guid id, Action<ClassifiedAd> update)
        {
            var ad = await _store.Load<ClassifiedAd>(id.ToString());
            update(ad);
            await _store.Save(ad);
        }
    }
}
