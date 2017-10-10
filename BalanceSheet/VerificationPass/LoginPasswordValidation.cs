using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BalanceSheet.VerificationPass
{
    class LoginPasswordValidation: ValidationRule
    {
        public override ValidationResult Validate(object value,
    System.Globalization.CultureInfo cultureInfo)
        {

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(true, null);
            }
            return ValidationResult.ValidResult;
        }

    }
}
