using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Modules.ClassifiedAds;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming

namespace Marketplace.Tests.Modules.ClassifiedAds
{
    public class Create_classified_ad : Specification<ClassifiedAd, Contracts.ClassifiedAds.V1.Register>
    {
        private Fixture AutoFixture { get; } = new Fixture();

        public Create_classified_ad(ITestOutputHelper output) : base(output) { }

        public override Func<Contracts.ClassifiedAds.V1.Register, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(
                store,
                () => DateTimeOffset.MinValue,
                _ => Task.FromResult(false)
            ).Handle(cmd);

        public override object[] Given() => new object[0];

        public override Contracts.ClassifiedAds.V1.Register When()
            => AutoFixture.Create<Contracts.ClassifiedAds.V1.Register>();

        [Then]
        public void Classified_ad_is_created()
            => RaisedEvents.Should().BeEquivalentTo(new Events.V1.ClassifiedAdRegistered
            {
                ClassifiedAdId = Command.ClassifiedAdId,
                Owner = Command.OwnerId,
                RegisteredAt = DateTimeOffset.MinValue
            });

    }
}
