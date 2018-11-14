using System;
using FluentValidation;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Marketplace.Contracts
{
    public static partial class ClassifiedAds
    {
        public static partial class V1
        {
            public class RegisterValidator : AbstractValidator<Register>
            {
                public RegisterValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty();
                    RuleFor(x => x.OwnerId).NotEmpty();
                }
            }

            public class ChangeTitleValidator : AbstractValidator<ChangeTitle>
            {
                public ChangeTitleValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty().NotEqual(Guid.Empty);
                    RuleFor(x => x.Title).NotEmpty();
                }
            }

            public class ChangeTextValidator : AbstractValidator<ChangeText>
            {
                public ChangeTextValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty();
                    RuleFor(x => x.Text).NotEmpty();
                }
            }

            public class ChangePriceValidator : AbstractValidator<ChangePrice>
            {
                public ChangePriceValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty();
                    RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
                }
            }

            public class PublishValidator : AbstractValidator<Publish>
            {
                public PublishValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty();
                }
            }

            public class RemoveValidator : AbstractValidator<Remove>
            {
                public RemoveValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty();
                }
            }

            public class MarkAsSoldValidator : AbstractValidator<MarkAsSold>
            {
                public MarkAsSoldValidator()
                {
                    RuleFor(x => x.ClassifiedAdId).NotEmpty();
                }
            }

            public class GetAvailableAdsValidator : AbstractValidator<GetAvailableAds>
            {
                public GetAvailableAdsValidator()
                {
                    RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
                    RuleFor(x => x.PageSize).GreaterThanOrEqualTo(0);
                }
            }
        }
    }
}
