using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class ActiveClassifiedAds : Projection
    {
        private readonly Func<IAsyncDocumentSession> _openSession;

        public ActiveClassifiedAds(Func<IAsyncDocumentSession> getSession) => _openSession = getSession;

    }
}
