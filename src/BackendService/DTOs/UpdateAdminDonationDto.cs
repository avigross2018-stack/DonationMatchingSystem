using System.ComponentModel.DataAnnotations;

namespace BackendService.DTOs
{
    public class UpdateAdminDonationDto
    {
        [Range(
            typeof(decimal),
            "0.01",
            "99999999.99",
            ErrorMessage = "Total amount must be positive.")]
        public decimal TotalAmount { get; set; }

        [Range(
            typeof(decimal),
            "0.01",
            "99999999.99",
            ErrorMessage = "Maximum amount per recipient must be positive.")]
        public decimal MaxAmountPerRecipient { get; set; }

        [StringLength(
            250,
            ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Expiration date is required.")]
        public DateTime? ExpirationDate { get; set; }
    }
}