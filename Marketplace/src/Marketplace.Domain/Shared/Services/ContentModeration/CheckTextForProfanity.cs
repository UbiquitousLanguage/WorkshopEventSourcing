using System.Threading.Tasks;

namespace Marketplace.Domain.Shared.Services.ContentModeration
{
    public delegate Task<bool> CheckTextForProfanity(string text);   
}
