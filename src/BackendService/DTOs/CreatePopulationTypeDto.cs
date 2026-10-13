using System.ComponentModel.DataAnnotations;

namespace BackendService.DTOs
{
    public sealed class CreatePopulationTypeDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal BaseEligibilityAmount { get; set; }
    }
}