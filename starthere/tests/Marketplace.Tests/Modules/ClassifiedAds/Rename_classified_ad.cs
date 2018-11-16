using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Modules.ClassifiedAds;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming

namespace Marketplace.Tests.Modules.ClassifiedAds
{
    public class Change_classified_ad_title : Specification<ClassifiedAd, Contracts.ClassifiedAds.V1.ChangeTitle>
    {
        public Change_classified_ad_title(ITestOutputHelper output) : base(output) {}

        private Fixture Auto { get; } = new Fixture();
        private Guid ClassifiedAdId { get; } = Guid.NewGuid();
        private Guid Owner { get; } = Guid.NewGuid();

        public override Func<Contracts.ClassifiedAds.V1.ChangeTitle, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(
                store,
                () => DateTimeOffset.MinValue,
                _ => Task.FromResult(false)
            ).Handle(cmd);


        public override object[] Given()
            => Auto.Build<Events.V1.ClassifiedAdRegistered>()
                .With(e => e.ClassifiedAdId, ClassifiedAdId)
                .With(e => e.Owner, Owner)
                .CreateMany(1)
                .ToArray();

        public override Contracts.ClassifiedAds.V1.ChangeTitle When()
            => Auto.Build<Contracts.ClassifiedAds.V1.ChangeTitle>()
                .With(cmd => cmd.ClassifiedAdId, ClassifiedAdId)
                .Create();

        [Then]
        public void The_title_should_change()
            => RaisedEvents.Should().BeEquivalentTo(new Events.V1.ClassifiedAdTitleChanged
            {
                ClassifiedAdId = Command.ClassifiedAdId,
                Title = Command.Title,
                Owner = Owner,
                ChangedAt = DateTimeOffset.MinValue
            });
    }
}
