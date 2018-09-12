using System;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;

namespace Marketplace.Contracts
{
    public static class ClassifiedAds
    {
        public static class V1
        {
            public class Create
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }
                public Guid CreatedBy { get; set; }

                public override string ToString() => $"Creating Classified Ad {Id}";
            }

            public class RenameAd
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public Guid RenamedBy { get; set; }
                
                public override string ToString() 
                    => $"Renaming Classified Ad {Id} to '{(Title?.Length > 25 ? $"{Title.Substring(0, 22)}..." : Title )}'";
            }
        }
    }
}
