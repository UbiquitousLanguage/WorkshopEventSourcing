using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;

namespace Marketplace
{
    public class ClassifiedAdsApplicationService
    {
        public void Handle(ClassifiedAds.V1.Create command) =>
            ClassifiedAd.Create(
                command.Id,
                command.OwnerId,
                command.CreatedBy,
                command.CreatedAt);
    }
}
