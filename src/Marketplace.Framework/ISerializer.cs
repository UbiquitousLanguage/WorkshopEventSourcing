namespace Marketplace.Framework
{
    using System;

    public interface ISerializer
    {
        bool IsJsonSerializer { get; }

        byte[] Serialize(object obj);
        object Deserialize(byte[] data, Type type);
    }
}
