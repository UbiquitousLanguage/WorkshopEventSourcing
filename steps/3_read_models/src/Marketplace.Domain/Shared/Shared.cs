namespace Marketplace.Domain.Events
{
    public static class Shared
    {
        public static class V1
        {
            public class Picture
            {
                public string Url { get; set; }
                public string Description { get; set; }
            }
        }
    }
}
