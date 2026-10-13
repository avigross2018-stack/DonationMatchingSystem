using System.ComponentModel.DataAnnotations;

namespace BackendService.DTOs
{
    public sealed class UpdateDonorStatusDto
    {
        [Required]
        public bool? IsActive { get; set; }
    }
}