using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Models
{
    public class Recipient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PopulationTypeId { get; set; }


        [MaxLength(250)]
        public string? RequestReason { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue)]
        public decimal IncomeAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int AmountOfChildren { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal EligibilityAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal RemainingEligibility { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(PopulationTypeId))]
        public PopulationType PopulationType { get; set; } = null!;

        public ICollection<Redemption> Redemptions { get; set; }
            = new List<Redemption>();
    }
}