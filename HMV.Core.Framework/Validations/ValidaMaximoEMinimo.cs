using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using HMV.Core.Framework.Expression;

namespace HMV.Core.Framework.Validations
{
    /// <summary>
    /// Valor máximo e mínimo
    /// </summary>


    public class ValidaMaximoEMinimo : ValidationAttribute
    {
        string _propriedadepodevalidar;
        int vMin;
        int vMax;

        public ValidaMaximoEMinimo(int min, int max)
        {
            vMin = min;
            vMax = max;                 
        }

        public ValidaMaximoEMinimo(int min, int max, string pPropriedadePodeValidar)
        {
            _propriedadepodevalidar = pPropriedadePodeValidar;
            vMin = min;
            vMax = max;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(_propriedadepodevalidar))
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(this._propriedadepodevalidar);
                if (propertyInfo != null)
                {
                    var _devevalidar = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                    if (typeof(bool) == _devevalidar.GetType())
                        if ((bool)_devevalidar)
                        {
                            _propriedadepodevalidar = string.Empty;
                            return IsValid(value, validationContext);
                        }
                }
            }
            else
            {
                if (value == null)
                    return ValidationResult.Success;

                double valor = Convert.ToDouble(value);

                if (valor < vMin)
                    return new ValidationResult("Valor mínimo é : " + vMin);
                else if ((valor > vMax))
                    return new ValidationResult("Valor máximo é : " + vMax);
            }

            return ValidationResult.Success;
        }
      

    }
}
