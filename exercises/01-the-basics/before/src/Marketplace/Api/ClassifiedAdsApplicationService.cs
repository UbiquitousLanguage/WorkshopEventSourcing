using System.Threading.Tasks;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;

namespace Marketplace
{
    public class ClassifiedAdsApplicationService
    {
        public ClassifiedAdsApplicationService(IAggregateStore store){}
        
        public async Task Handle(ClassifiedAds.V1.Create command) =>
            ClassifiedAd.Create(
                command.Id,
                command.OwnerId,
                command.CreatedBy,
                command.CreatedAt);
    }
}
