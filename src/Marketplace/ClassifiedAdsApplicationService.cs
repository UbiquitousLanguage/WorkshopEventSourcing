using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;

namespace Marketplace
{
    public static class ClassifiedAdsApplicationService
    {
        public static Task Handle(ClassifiedAds.V1.CreateClassifiedAd command, IAggregateStore store) =>
            store.Save(ClassifiedAd.Create(
                command.Id,
                command.OwnerId,
                command.CreatedBy,
                command.CreatedAt));
    }
}
