﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.Core.Framework.Validations
{
    
    public class ValidaCampoIgual : ValidationAttribute
    {
        private string _propriedadepodevalidar { get; set; }
        private string _outrapropriedade { get; set; }

        public ValidaCampoIgual(string pOutraPropriedade, string pPropriedadePodeValidar)
            : base("{0} deve ser igual a {1}")
        {
            this._outrapropriedade = pOutraPropriedade;
            this._propriedadepodevalidar = pPropriedadePodeValidar;
        }

        public ValidaCampoIgual(string pOutraPropriedade)
            : base("{0} deve ser igual a {1}")
        {
            _outrapropriedade = pOutraPropriedade;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, _outrapropriedade);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext != null)
            {
                if (this._propriedadepodevalidar != null)
                {
                    var propertyInfo = validationContext.ObjectType.GetProperty(this._propriedadepodevalidar);

                    #region --TODO--
                    //var propertyInfo = validationContext.ObjectType.GetProperty(ExpressionEx.PropertyName<ViewModelBase>(x => x.ValidaCampos));
                    //if (propertyInfo != null)
                    //{
                    //    var _devevalidar = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                    //    if (typeof(List<string>) == _devevalidar.GetType())
                    //    {
                    //        foreach (var item in (List<string>)_devevalidar)
                    //        {
                    //            if (item == validationContext.DisplayName)
                    //            {
                    //                var secondpropertyInfo = validationContext.ObjectType.GetProperty(this._outrapropriedade);
                    //                if (secondpropertyInfo != null)
                    //                {
                    //                    var _secondvalue = secondpropertyInfo.GetValue(validationContext.ObjectInstance, null);
                    //                    if (value != null)
                    //                    {
                    //                        if (!value.Equals(_secondvalue))
                    //                        {
                    //                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    //                        }
                    //                    }
                    //                    else
                    //                        if (_secondvalue !=null)
                    //                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    //                }
                    //            }
                    //        }
                    //    }
                    #endregion
                    var _devevalidar = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                    if (typeof(bool) == _devevalidar.GetType())
                        if ((bool)_devevalidar)
                        {
                            var secondpropertyInfo = validationContext.ObjectType.GetProperty(this._outrapropriedade);
                            if (secondpropertyInfo != null)
                            {
                                var _secondvalue = secondpropertyInfo.GetValue(validationContext.ObjectInstance, null);
                                if (!value.Equals(_secondvalue))
                                {
                                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                                }
                            }
                        }
                }
                else
                {
                    var secondpropertyInfo = validationContext.ObjectType.GetProperty(this._outrapropriedade);
                    if (secondpropertyInfo != null )
                    {
                        var _secondvalue = secondpropertyInfo.GetValue(validationContext.ObjectInstance, null);
                        if (value != null && !value.Equals(_secondvalue))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
            }            
            //}
            return ValidationResult.Success;
        }
    }
}