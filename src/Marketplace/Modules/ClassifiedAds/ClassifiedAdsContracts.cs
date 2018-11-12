using System;

// ReSharper disable CheckNamespace

namespace Marketplace.Contracts
{
    public static class ClassifiedAds
    {
        public static class V1
        {
            public class RegisterAd
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }

                public override string ToString() => $"Registering new classified ad {Id}...";
            }

            public class ChangeTitle
            {
                public Guid ClassifiedAdId { get; set; }
                public string Title { get; set; }
                
                public override string ToString() 
                    => $"Renaming Classified Ad {ClassifiedAdId} to '{(Title?.Length > 25 ? $"{Title.Substring(0, 22)}..." : Title )}'";
            }

            public class ChangeText
            {
                public Guid ClassifiedAdId { get; set; }
                public string Text { get; set; }
                public Guid TextChangedBy { get; set; }
            }

            public class ChangePrice
            {
                public Guid ClassifiedAdId { get; set; }
                public double Price { get; set; }
            }

            public class Publish
            {
                public Guid ClassifiedAdId { get; set; }
            }

            public class MarkAsSold
            {
                public Guid ClassifiedAdId { get; set; }
            }

            public class Remove
            {
                public Guid ClassifiedAdId { get; set; }
            }

            public class GetAvailableAds
            {
                public int Page { get; set; }
                public int PageSize { get; set; }

                public class Result : Shared.V1.ItemsPage<Result.Item>
                {
                    public class Item
                    {
                        public Guid ClassifiedAdId { get; set; }
                        public string Title { get; set; }
                        public string Text { get; set; }
                        public double Price { get; set; }
                        public DateTimeOffset PublishedAt { get; set; }
                    }
                }
            }
        }
    }
    
    public static class Shared
    {
        public static class V1
        {
            public class ItemsPage<T>
            {
                public ItemsPage() { }

                public ItemsPage(int page, int pageSize, int totalPages, int totalItems, params T[] items) {
                    Page       = page;
                    PageSize   = pageSize;
                    TotalPages = totalPages;
                    TotalItems = totalItems;
                    Items      = items;
                }

                public int Page { get; set; }
                public int PageSize { get; set; }
                public int TotalPages { get; set; }
                public int TotalItems { get; set; }
                public T[] Items { get; set; }
            }
            
            public class Picture
            {
                public string Url { get; set; }
                public string Description { get; set; }
            }
        }
    }
}
