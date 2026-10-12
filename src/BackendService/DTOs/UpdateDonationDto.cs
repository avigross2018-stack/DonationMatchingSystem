using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class UpdateDonationDto
    {
        [Range(typeof(decimal), "0.01", "99999999.99")]
        public decimal TotalAmount { get; set; }

        [Range(typeof(decimal), "0.01", "99999999.99")]
        public decimal MaxAmountPerRecipient { get; set; }

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public DateTime ExpirationDate { get; set; }        
    }
}