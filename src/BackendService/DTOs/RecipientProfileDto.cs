using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class RecipientProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public int PopulationTypeId { get; set; }
        public string PopulationTypeName { get; set; } = string.Empty;

        public decimal IncomeAmount { get; set; }
        public int AmountOfChildren { get; set; }
        public string? RequestReason { get; set; }

        public decimal EligibilityAmount { get; set; }
        public decimal RemainingEligibility { get; set; }        
    }
}