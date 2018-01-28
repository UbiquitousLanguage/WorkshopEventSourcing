using System;
using System.Threading.Tasks;
using FluentAssertions;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;

namespace Marketplace.Tests
{
    using AutoFixture;

    public class Create_classified_ad : Specification<ClassifiedAd, ClassifiedAds.V1.CreateClassifiedAd>
    {
        public readonly Fixture AutoFixture = new Fixture();

        public override Func<ClassifiedAds.V1.CreateClassifiedAd, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => ClassifiedAdsApplicationService.Handle(cmd, store);

        public override object[] Given() => new object[0];

        public override ClassifiedAds.V1.CreateClassifiedAd When()
            => AutoFixture.Create<ClassifiedAds.V1.CreateClassifiedAd>();

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
