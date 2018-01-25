namespace Marketplace.Domain.Shared.Services.ContentModeration
{
    using System.Threading.Tasks;

    public interface IContentValidator
    {
        Task<ValidationResult> ValidateText(string text);
        Task<ValidationResult> ValidatePicture(string uri);
    }

    public class ValidationResult        
    {
        public bool Success { get; set; }
    }
    
}
