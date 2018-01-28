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
        
        public static async Task Handle(ClassifiedAds.V1.RenameClassifiedAd command, IAggregateStore store)
        {
            var ad = await store.Load<ClassifiedAd>(command.Id.ToString());

            ad.Rename(command.Title, command.RenamedAt, command.RenamedBy);

            await store.Save(ad);
        }
    }
}
