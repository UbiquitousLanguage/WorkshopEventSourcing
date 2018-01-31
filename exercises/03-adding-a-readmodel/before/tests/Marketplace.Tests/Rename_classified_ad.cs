using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;
using Xunit.Abstractions;

namespace Marketplace.Tests
{

    public class Change_classified_ad_title : Specification<ClassifiedAd, ClassifiedAds.V1.RenameAd>
    {
        public Change_classified_ad_title(ITestOutputHelper output) : base(output) {}
        
        public readonly Fixture AutoFixture = new Fixture();

        public override Func<ClassifiedAds.V1.RenameAd, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(store).Handle(cmd);

        private Guid ClassifiedAdId { get; set; } = Guid.NewGuid();
    
        public override object[] Given() 
            => AutoFixture.Build<Events.V1.ClassifiedAdCreated>()
                          .With(e => e.Id, ClassifiedAdId)
                          .CreateMany(1)
                          .ToArray();

        public override ClassifiedAds.V1.RenameAd When()
            => AutoFixture.Build<ClassifiedAds.V1.RenameAd>()
                          .With(cmd => cmd.Id, ClassifiedAdId)
                          .Create();

        [Then]
        public void The_title_changed()
        {
            RaisedEvents.ShouldBeEquivalentTo(new [] { new Events.V1.ClassifiedAdRenamed {
                Id        = Command.Id,
                Title     = Command.Title,
                RenamedBy = Command.RenamedBy,
                RenamedAt = Command.RenamedAt
            }});
        }
    }
}