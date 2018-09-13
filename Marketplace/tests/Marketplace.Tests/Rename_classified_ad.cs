using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Modules.ClassifiedAds;
using Xunit.Abstractions;
using static Marketplace.Contracts.ClassifiedAds.V1;

namespace Marketplace.Tests
{
    public class Change_classified_ad_title : Specification<ClassifiedAd, RenameAd>
    {
        public Change_classified_ad_title(ITestOutputHelper output) : base(output) {}
        
        public readonly Fixture AutoFixture = new Fixture();

        public override Func<RenameAd, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(
                store, () => DateTimeOffset.UtcNow, t => Task.FromResult(true)).Handle(cmd);

        private Guid ClassifiedAdId { get; } = Guid.NewGuid();
        private Guid Owner          { get; } = Guid.NewGuid();
    
        public override object[] Given() 
            => AutoFixture.Build<Events.V1.ClassifiedAdCreated>()
                .With(e => e.Id, ClassifiedAdId)
                .With(e => e.Owner, Owner)
                .CreateMany(1)
                .ToArray();

        public override RenameAd When()
            => AutoFixture.Build<RenameAd>()
                .With(cmd => cmd.Id, ClassifiedAdId)
                .Create();

        [Then]
        public void The_title_should_change() 
            => RaisedEvents.Should().BeEquivalentTo(
                new Events.V1.ClassifiedAdRenamed {
                    Id        = Command.Id,
                    Title     = Command.Title,
                    Owner     = Owner,
                    RenamedBy = Command.RenamedBy,
                });
    }
}