using System;
using System.Threading.Tasks;
using FluentAssertions;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;
using AutoFixture;
using Xunit.Abstractions;

namespace Marketplace.Tests
{
    public class Create_classified_ad : Specification<ClassifiedAd, ClassifiedAds.V1.Create>
    {
        public Create_classified_ad(ITestOutputHelper output) : base(output) {}
              
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
