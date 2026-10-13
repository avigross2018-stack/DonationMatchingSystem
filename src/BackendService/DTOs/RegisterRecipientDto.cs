using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class RegisterRecipientDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int PopulationTypeId { get; set; }

        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal IncomeAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int AmountOfChildren { get; set; }

        [MaxLength(250)]
        public string? RequestReason { get; set; }        
    }
}