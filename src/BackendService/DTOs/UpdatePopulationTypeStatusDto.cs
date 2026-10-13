using System.ComponentModel.DataAnnotations;

namespace BackendService.DTOs
{
    public sealed class UpdatePopulationTypeStatusDto
    {
        [Required]
        public bool? IsActive { get; set; }
    }
}