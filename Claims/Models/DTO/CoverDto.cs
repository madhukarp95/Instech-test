

using Claims.Models.Cover;
using System.ComponentModel.DataAnnotations;

namespace Claims.Models.DTO
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0#ivalidatableobject
    /// </summary>
    public class CoverDto : IValidatableObject
    {

        [Required]
        public required DateOnly? StartDate { get; set; }

        [Required]
        public required DateOnly? EndDate { get; set; }

        [Required]
        public CoverType Type { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate == null || EndDate == null)
                yield break;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (StartDate < today)
            {
                yield return new ValidationResult("StartDate cannot be in the past.", new[] { nameof(StartDate) });
            }

            if (EndDate <= StartDate)
            {
                yield return new ValidationResult("EndDate must be after StartDate.", new[] { nameof(EndDate) });
            }

            if ((EndDate.Value.DayNumber - StartDate.Value.DayNumber) > 365)
            {
                yield return new ValidationResult("Total insurance period cannot exceed 1 year.", new[] { nameof(EndDate), nameof(StartDate) });
            }
        }
    }
}
