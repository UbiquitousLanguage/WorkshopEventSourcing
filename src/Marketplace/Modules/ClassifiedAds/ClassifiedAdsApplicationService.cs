using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Domain.Shared.Services.ContentModeration;
using Marketplace.Framework;

namespace Marketplace.Modules.ClassifiedAds
{
    public class ClassifiedAdsApplicationService
    {
        private readonly IAggregateStore _store;
        private readonly Func<DateTimeOffset> _getUtcNow;
        private readonly CheckTextForProfanity _checkTextForProfanity;

        public ClassifiedAdsApplicationService(IAggregateStore store, Func<DateTimeOffset> getUtcNow, CheckTextForProfanity checkTextForProfanity)
        {
            _store = store;
            _getUtcNow = getUtcNow;
            _checkTextForProfanity = checkTextForProfanity;
        }

        public Task Handle(Contracts.ClassifiedAds.V1.CreateAd command) =>
            _store.Save(ClassifiedAd.Create(command.Id, command.OwnerId, _getUtcNow));

        public Task Handle(Contracts.ClassifiedAds.V1.RenameAd command) =>
            HandleUpdate(command.Id, ad =>
                ad.Rename(command.Title, command.RenamedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.UpdateText command) =>
            HandleUpdate(command.Id, ad =>
                ad.UpdateText(command.Text, command.TextChangedBy, _getUtcNow, _checkTextForProfanity)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.ChangePrice command) =>
            HandleUpdate(command.Id, ad =>
                ad.ChangePrice(command.Price, command.PriceChangedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.Publish command) =>
            HandleUpdate(command.Id, ad =>
                ad.Publish(command.PublishedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.Activate command) =>
            HandleUpdate(command.Id, ad =>
                ad.Activate(command.ActivatedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.Report command) =>
            HandleUpdate(command.Id, ad =>
                ad.Report(command.Reason, command.ReportedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.Reject command) =>
            HandleUpdate(command.Id, ad =>
                ad.Reject(command.Reason, command.RejectedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.Deactivate command) =>
            HandleUpdate(command.Id, ad =>
                ad.Deactivate(command.DeactivatedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.MarkAsSold command) =>
            HandleUpdate(command.Id, ad =>
                ad.MarkAsSold(command.MarkedBy, _getUtcNow)
            );

        public Task Handle(Contracts.ClassifiedAds.V1.Remove command) =>
            HandleUpdate(command.Id, ad =>
                ad.Remove(command.RemovedBy, _getUtcNow)
            );

        private async Task HandleUpdate(Guid id, Action<ClassifiedAd> update)
        {
            var ad = await _store.Load<ClassifiedAd>(id.ToString());
            update(ad);
            await _store.Save(ad);
        }
    }
}
