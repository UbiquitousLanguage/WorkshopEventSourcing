using System.Threading.Tasks;

// ReSharper disable CheckNamespace

namespace Marketplace.Domain.Shared.Services
{
    public delegate Task<bool> CheckTextForProfanity(string text);   
}
