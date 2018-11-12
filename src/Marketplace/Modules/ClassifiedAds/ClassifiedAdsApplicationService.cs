using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Domain.Shared.Services;
using Marketplace.Framework;
using static Marketplace.Contracts.ClassifiedAds;

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

        public Task Handle<T>(T cmd) where T : class
        {
            switch (cmd)
            {
                 case V1.RegisterAd x:
                     return _store.Save(ClassifiedAd.Register(x.Id, x.OwnerId, _getUtcNow));

                 case V1.ChangeTitle x: 
                     return Execute(x.ClassifiedAdId, ad => ad.ChangeTitle(x.Title, _getUtcNow));
                     
                 case V1.ChangeText x: 
                     return Execute(x.ClassifiedAdId, ad => ad.ChangeText(x.Text, _getUtcNow, _checkTextForProfanity));

                 case V1.ChangePrice x: 
                     return Execute(x.ClassifiedAdId, ad => ad.ChangePrice(x.Price, _getUtcNow));
                     
                 case V1.Publish x: 
                     return Execute(x.ClassifiedAdId, ad => ad.Publish(_getUtcNow));
                 
                 case V1.MarkAsSold x: 
                     return Execute(x.ClassifiedAdId, ad => ad.MarkAsSold(_getUtcNow));
                 
                 case V1.Remove x: 
                     return Execute(x.ClassifiedAdId, ad => ad.Remove(_getUtcNow));
                 
                 default:
                     return Task.CompletedTask;
            }
        }
        
        private async Task Execute(Guid id, Func<ClassifiedAd, Task> update)
        {
            var ad = await _store.Load<ClassifiedAd>(id.ToString());
            await update(ad);
            await _store.Save(ad);
        }
        
        private Task Execute(Guid id, Action<ClassifiedAd> update) 
            => Execute(id, ad => { update(ad); return Task.CompletedTask; });
    }
}
