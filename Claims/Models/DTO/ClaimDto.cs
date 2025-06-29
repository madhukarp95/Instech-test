
using Claims.Models.Claim;
using System.ComponentModel.DataAnnotations;

namespace Claims.Models.DTO
{
    public class ClaimDto
    {
        [Required]
        public required string CoverId { get; set; }

        [Required]
        public DateOnly? Created { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Name { get; set; }

        [Required]
        public ClaimType Type { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Damage cost must be between 1 and 100,000")]
        public decimal DamageCost { get; set; }
    }
}
