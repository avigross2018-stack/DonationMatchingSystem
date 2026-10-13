using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Services
{
    public class EligibilityService
    {
        private const decimal AmountPerChild = 500m;
        private const decimal IncomeDeductionRate = 0.20m;

        public decimal CalculateEligibility(
            decimal baseAmount,
            decimal incomeAmount,
            int amountOfChildren)
        {
            if (baseAmount < 0 ||
                incomeAmount < 0 ||
                amountOfChildren < 0)
            {
                throw new ArgumentException(
                    "Eligibility inputs cannot be negative.");
            }

            decimal childrenBonus =
                amountOfChildren * AmountPerChild;

            decimal incomeDeduction =
                incomeAmount * IncomeDeductionRate;

            decimal eligibility =
                baseAmount + childrenBonus - incomeDeduction;

            return Math.Round(
                Math.Max(0m, eligibility),
                2,
                MidpointRounding.AwayFromZero);
        }        
    }
}