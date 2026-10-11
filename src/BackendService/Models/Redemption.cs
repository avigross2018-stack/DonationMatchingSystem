using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Models
{
    public class Redemption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DonationId { get; set; }

        [Required]
        public int RecipientId { get; set; }

        public int? HandledByAdminId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RedeemedAt { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [ForeignKey(nameof(DonationId))]
        public Donation Donation { get; set; } = null!;

        [ForeignKey(nameof(RecipientId))]
        public Recipient Recipient { get; set; } = null!;

        [ForeignKey(nameof(HandledByAdminId))]
        public Admin? HandledByAdmin { get; set; }
    }
}