using System.Threading.Tasks;
using Marketplace.Domain.Shared.Services;
using Marketplace.Framework;
using static System.String;

namespace Marketplace.Domain.ClassifiedAds
{
    public class AdText : Value<AdText>
    {
        public static readonly AdText Default = new AdText(Empty);
        
        internal AdText(string value) => Value = value;
        
        public readonly string Value;
        
        public static async Task<AdText> Parse(string text, CheckTextForProfanity checkTextForProfanity)
        {
            var containsProfanity = await checkTextForProfanity(text);
            if (containsProfanity)
                throw new ProfanityFound();   
            
            return new AdText(text);
        }
        
        public static implicit operator string(AdText self) => self.Value;
    }
}
