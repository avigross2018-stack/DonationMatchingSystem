using System.ComponentModel.DataAnnotations;

namespace BackendService.DTOs
{
    public sealed class UpdateAdminRecipientDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal IncomeAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int AmountOfChildren { get; set; }

        [MaxLength(250)]
        public string? RequestReason { get; set; }
    }
}