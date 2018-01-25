namespace Marketplace.Framework
{
    using System.Collections.Generic;

    public class Metadata : Dictionary<string, string>
    {
        public Metadata() { }

        public Metadata(IDictionary<string, string> dictionary) : base(dictionary ?? new Dictionary<string, string>()) { }

        public Metadata(params (string key, string value)[] entries) => AddRange(entries);

        public Metadata AddRange(params (string key, string value)[] entries)
        {
            foreach (var entry in entries) Add(entry.key, entry.value);
            return this;
        } 
        
        public Metadata Add<T>(string key, T value)
        {
            base.Add(key, value.ToString());
            return this;
        }
    }
}
