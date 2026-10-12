using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class DonationResponseDto
    {
        public int Id { get; set; }
        public int PopulationTypeId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal MaxAmountPerRecipient { get; set; }
        public string? Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; } = string.Empty;        
    }
}