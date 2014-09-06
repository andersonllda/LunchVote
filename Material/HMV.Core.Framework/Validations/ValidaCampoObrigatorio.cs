using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HMV.Core.Framework.Validations
{
    
    public class ValidaCampoObrigatorio : RequiredAttribute 
    {
        private string _propriedadepodevalidar { get; set; }

        public ValidaCampoObrigatorio()
        {
            this._mesagemdeerrocustomizada();
        }

        public ValidaCampoObrigatorio(string pPropriedadePodeValidar)
        {
            this._propriedadepodevalidar = pPropriedadePodeValidar;
            this._mesagemdeerrocustomizada();
        }

        private void _mesagemdeerrocustomizada()
        {
            this.ErrorMessage = "Campo Obrigatório";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext != null)
            {
                if (this._propriedadepodevalidar != null)
                {
                    var propertyInfo = validationContext.ObjectType.GetProperty(this._propriedadepodevalidar);
                    if (propertyInfo != null)
                    {
                        var _devevalidar = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                        if (typeof(bool) == _devevalidar.GetType())
                            if ((bool)_devevalidar)
                            {
                                return base.IsValid(value, validationContext);
                            }
                    }
                }
                else
                {
                    return base.IsValid(value, validationContext);
                }
            }
            return ValidationResult.Success;
        }
    }   
}
