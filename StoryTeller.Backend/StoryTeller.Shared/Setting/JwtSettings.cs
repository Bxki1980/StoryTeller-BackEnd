using System.ComponentModel.DataAnnotations;

namespace StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting
{
    public class JwtSettings
    {
        [Required(ErrorMessage = "The Secret field is required.")]
        public string Secret { get; set; }

        [Required]
        public string Issuer { get; set; }

        [Required]
        public string Audience { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ExpiresInMinutes must be greater than 0")]
        public int ExpiresInMinutes { get; set; }

        [Range(1, 365, ErrorMessage = "RefreshTokenExpiryDays must be between 1 and 365")]
        public int RefreshTokenExpiryDays { get; set; }
    }
}
