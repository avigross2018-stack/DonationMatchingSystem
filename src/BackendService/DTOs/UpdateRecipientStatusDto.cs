using System.ComponentModel.DataAnnotations;

namespace BackendService.DTOs
{
    public sealed class UpdateRecipientStatusDto
    {
        [Required]
        public bool? IsActive { get; set; }
    }
}