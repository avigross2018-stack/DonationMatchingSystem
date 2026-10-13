using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class UpdateRecipientDto
    {
        [Range(typeof(decimal), "0", "99999999.99")]
        public decimal IncomeAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int AmountOfChildren { get; set; }

        [MaxLength(250)]
        public string? RequestReason { get; set; }
    }        
    
}