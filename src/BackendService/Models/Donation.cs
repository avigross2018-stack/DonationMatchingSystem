using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DonorId { get; set; }

        [Required]
        public int PopulationTypeId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal RemainingAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MaxAmountPerRecipient { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpirationDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        [ForeignKey(nameof(DonorId))]
        public Donor Donor { get; set; } = null!;

        [ForeignKey(nameof(PopulationTypeId))]
        public PopulationType PopulationType { get; set; } = null!;

        public ICollection<Redemption> Redemptions { get; set; }
            = new List<Redemption>();
    }
}