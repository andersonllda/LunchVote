using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.Core.Framework.Validations
{
    public class ValidaCampoMenorMesAno : ValidationAttribute
    {
        private string _propriedadepodevalidar { get; set; }
        private string _outrapropriedade { get; set; }

        public ValidaCampoMenorMesAno(string pOutraPropriedade, string pPropriedadePodeValidar)
            : base("{0} deve ser menor que {1}")
        {
            this._outrapropriedade = pOutraPropriedade;
            this._propriedadepodevalidar = pPropriedadePodeValidar;
        }

        public ValidaCampoMenorMesAno(string pOutraPropriedade)
            : base("{0} deve ser menor que {1}")
        {
            _outrapropriedade = pOutraPropriedade;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _outrapropriedade);
        }


        private bool ValidaValor(int valorinicial, int valorFinal)
        {


            return valorFinal >= valorinicial|| valorFinal==0;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext != null)
            {
                if (this._propriedadepodevalidar != null)
                {
                    var propertyInfo = validationContext.ObjectType.GetProperty(this._propriedadepodevalidar);
                  
                    var _devevalidar = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                    if (typeof(bool) == _devevalidar.GetType())
                        if ((bool)_devevalidar)
                        {
                            var secondpropertyInfo = validationContext.ObjectType.GetProperty(this._outrapropriedade);
                            if (secondpropertyInfo != null)
                            {
                                var _secondvalue = secondpropertyInfo.GetValue(validationContext.ObjectInstance, null);
                                if (_secondvalue != null)
                                {

                                    if ((typeof(int) == value.GetType()) && (typeof(int) == _secondvalue.GetType()))
                                    {
                                        if (!ValidaValor((int)value,(int)_secondvalue))
                                        {
                                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                                        }
                                    }

                                    else
                                        new System.Exception("Não e possìvel efetuar validação tipo não compativo");
                                }
                            }
                        }
                }
                else
                {
                    var secondpropertyInfo = validationContext.ObjectType.GetProperty(this._outrapropriedade);
                    if (value != null)
                    {
                        if (secondpropertyInfo != null)
                        {
                            var _secondvalue = secondpropertyInfo.GetValue(validationContext.ObjectInstance, null);
                            if (_secondvalue != null)
                            {
                                if ((typeof(int) == value.GetType()) && (typeof(int) == _secondvalue.GetType()))
                                {
                                    if (!ValidaValor((int)value, (int)_secondvalue))
                                    {
                                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                                    }

                                }
                                else
                                    new System.Exception("Não e possìvel efetuar validação tipo não compativo");
                            }
                        }
                    }
                }
            }        
            return ValidationResult.Success;
        }
    }
}
