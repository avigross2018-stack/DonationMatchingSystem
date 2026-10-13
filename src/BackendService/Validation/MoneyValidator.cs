using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Validation
{
    public class MoneyValidator
    {
        public static bool IsValidPositiveAmount(decimal amount)
        {
            return amount > 0m &&
                   amount <= 99999999.99m &&
                   decimal.Round(amount, 2) == amount;
        }
    }
}