using System;

namespace Marketplace.Contracts
{
    public static class ClassifiedAds
    {
        public static class V1
        {
            /// <summary>
            ///     Create a new ad command
            /// </summary>
            public class CreateClassifiedAd
            {
                /// <summary>
                ///     New ad id
                /// </summary>
                public Guid Id { get; set; }
                
                /// <summary>
                ///     Ad owner id
                /// </summary>
                public Guid OwnerId { get; set; }
                
                /// <summary>
                ///     Creation date
                /// </summary>
                public DateTime CreatedAt { get; set; }
                
                /// <summary>
                ///     Id of the user who created the ad
                /// </summary>
                public Guid CreatedBy { get; set; }

                public override string ToString() => $"Creating Classified Ad {Id}";
            }

            public class RenameClassifiedAd
            {
                /// <summary>
                ///     New ad id
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                ///     The new title
                /// </summary>
                public string Title { get; set; }
                
                /// <summary>
                ///     Rename date
                /// </summary>
                public DateTime RenamedAt { get; set; }
                
                /// <summary>
                ///     Id of the user who renamed the ad
                /// </summary>
                public Guid RenamedBy { get; set; }
                
                public override string ToString() 
                    => $"Renaming Classified Ad {Id} to '{(Title?.Length > 25 ? $"{Title.Substring(0, 22)}..." : Title )}'";
            }
        }
    }
}
