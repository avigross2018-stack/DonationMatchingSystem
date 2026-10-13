using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class RedemptionResponseDto
    {
        public int Id { get; set; }
        public int DonationId { get; set; }

        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;

        public string? DonationDescription { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }        
    }
}