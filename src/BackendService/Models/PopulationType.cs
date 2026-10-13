using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Models
{
    public class PopulationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseEligibilityAmount { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Donation> Donations { get; set; }
            = new List<Donation>();

        public ICollection<Recipient> Recipients { get; set; }
            = new List<Recipient>();
    }
}