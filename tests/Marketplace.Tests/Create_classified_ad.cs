using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Modules.ClassifiedAds;
using Xunit.Abstractions;

namespace Marketplace.Tests
{
    public class Create_classified_ad : Specification<ClassifiedAd, ClassifiedAds.V1.CreateAd>
    {
        public Create_classified_ad(ITestOutputHelper output) : base(output) {}
              
        public readonly Fixture AutoFixture = new Fixture();

        public override Func<ClassifiedAds.V1.CreateAd, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(store: store, 
                getUtcNow: () => DateTimeOffset.MinValue, 
                checkTextForProfanity: _ => Task.FromResult(false)).Handle(cmd);

        public override object[] Given() => new object[0];

        public override ClassifiedAds.V1.CreateAd When()
            => AutoFixture.Create<ClassifiedAds.V1.CreateAd>();

        [Then]
        public void Classified_ad_is_created()
        {
            RaisedEvents.Should().BeEquivalentTo(new Events.V1.ClassifiedAdCreated {
                Id        = Command.Id,
                Owner     = Command.OwnerId,
                CreatedBy = Command.OwnerId,
                CreatedAt = DateTimeOffset.MinValue
            });
        }
    }
}
