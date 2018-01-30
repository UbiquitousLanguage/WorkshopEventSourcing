using System;
using System.Threading.Tasks;
using FluentAssertions;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;

namespace Marketplace.Tests
{
    using AutoFixture;

    public class Create_classified_ad : Specification<ClassifiedAd, ClassifiedAds.V1.Create>
    {
        public readonly Fixture AutoFixture = new Fixture();

        public override Func<ClassifiedAds.V1.Create, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(store).Handle(cmd);

        public override object[] Given() => new object[0];

        public override ClassifiedAds.V1.Create When()
            => AutoFixture.Create<ClassifiedAds.V1.Create>();

        [Then]
        public void Classified_ad_is_created()
        {
            RaisedEvents.ShouldBeEquivalentTo(new [] { new Events.V1.ClassifiedAdCreated {
                Id        = Command.Id,
                Owner     = Command.OwnerId,
                CreatedBy = Command.CreatedBy,
                CreatedAt = Command.CreatedAt
            }});
        }
    }
}
