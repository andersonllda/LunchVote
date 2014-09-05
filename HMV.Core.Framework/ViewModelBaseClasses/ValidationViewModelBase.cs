using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using HMV.Core.Framework.Interfaces;

namespace HMV.Core.Framework.ViewModelBaseClasses
{
    /// <summary>    
    /// Classe Base para a validação das viewmodels    
    /// usa a interface IDataErrorInfo.    
    /// As propriedades podem ter definições de atributos usando os Validations atributes definidos em
    /// System.ComponentModel.DataAnnotations.
    /// </summary>
    
    public class ValidationViewModelBase : NotifyPropertyChanged, IDataErrorInfo, IValidationExceptionHandler
    {
        //private readonly Dictionary<string, Func<ValidationViewModelBase, object>> propertyGetters;
        //private readonly Dictionary<string, ValidationAttribute[]> validators;

        public ValidationViewModelBase()
        {
            //this.validators = this.GetType()
            //    .GetProperties()
            //    .Where(p => this.GetValidations(p).Length != 0)
            //    .ToDictionary(p => p.Name, p => this.GetValidations(p));

            //this.propertyGetters = this.GetType()
            //    .GetProperties()
            //    .Where(p => this.GetValidations(p).Length != 0)
            //    .ToDictionary(p => p.Name, p => this.GetValueGetter(p));
        }

        /// <summary>        
        /// Retorna a menssagem de erro para a propriedade do parâmetro.
        /// </summary>
        /// <param name="propertyName">Nome da Propriedade</param>
        public virtual string this[string propertyName]
        {
            get
            {
                //if (this.propertyGetters.ContainsKey(propertyName))
                //{
                PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);                                
                if (propertyInfo != null)
                {
                    var results = new List<ValidationResult>();

                    var result = Validator.TryValidateProperty(
                                              propertyInfo.GetValue(this, null),
                                              new ValidationContext(this, null, null)
                                              {
                                                  MemberName = propertyName
                                              },
                                              results);

                    if (!result)
                    {                        
                        return string.Join(Environment.NewLine, results.Select(x=> x.ErrorMessage));
                    }
                }
                return string.Empty;
                #region Código Antigo - Modificado por Anderson Amaral
                //if (this.propertyGetters.ContainsKey(propertyName))
                //{
                //    var propertyValue = this.propertyGetters[propertyName](this);                    

                //    var errorMessages = this.validators[propertyName]
                //        .Where(v => !v.IsValid(propertyValue))
                //        .Select(v => v.ErrorMessage).ToArray();

                //    return string.Join(Environment.NewLine, errorMessages);
                //}
                //return string.Empty;
                #endregion
            }
        }        


        /// <summary>        
        /// Retorna as messagens de erro de todas as propriedades.
        /// </summary>
        public virtual string Error
        {
            get
            {                
                var results = new List<ValidationResult>();
                var result = Validator.TryValidateObject(this, new ValidationContext(this, null, null), results, true);
                if (!result)
                {                    
                    return string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage));
                }

                return string.Empty;
                #region Código Antigo - Modificado por Anderson Amaral
                //var errors = from validator in this.validators
                //             from attribute in validator.Value
                //             where !attribute.IsValid(this.propertyGetters[validator.Key](this))
                //             select attribute.ErrorMessage;
                //return string.Join(Environment.NewLine, errors.ToArray());
                #endregion
            }
        }

        /// <summary>        
        /// Retorna se o objeto avaliado é válido
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                var results = new List<ValidationResult>();
                return Validator.TryValidateObject(this, new ValidationContext(this, null, null), results, true);
                #region Código Antigo - Modificado por Anderson Amaral
                //var errors = from validator in this.validators
                //             from attribute in validator.Value
                //             where !attribute.IsValid(this.propertyGetters[validator.Key](this))
                //             select attribute.ErrorMessage;
                //return errors.Count().Equals(0);
                #endregion
            }
        }
        /// <summary>        
        /// Retona o número de propriedades válidas.
        /// </summary>
        public int ValidPropertiesCount
        {
            get
            {
                //var query = from validator in this.validators
                //            where validator.Value.All(attribute =>
                //            attribute.IsValid(this.propertyGetters[validator.Key](this)))
                //            select validator;

                //var count = query.Count() - this.validationExceptionCount;
                //return count;
                return 0;
            }
        }

        /// <summary>        
        /// Retona o número de propriedades inválidas.
        /// </summary>
        public int InvalidPropertiesCount
        {
            get
            {
                return this.validationExceptionCount;
            }
        }

        /// <summary>        
        /// Retorna o número de propriedades que possuem o atributo de validação
        /// </summary>
        public int TotalPropertiesWithValidationCount
        {
            get
            {
                //return this.validators.Count();
                return 0;
            }
        }

        //private ValidationAttribute[] GetValidations(PropertyInfo property)
        //{
        //    return (ValidationAttribute[])property.GetCustomAttributes(typeof(ValidationAttribute), true);
        //}

        //private Func<ValidationViewModelBase, object> GetValueGetter(PropertyInfo property)
        //{
        //    return new Func<ValidationViewModelBase, object>(viewmodel => property.GetValue(viewmodel, null));
        //}

        private int validationExceptionCount;

        public void ValidationExceptionsChanged(int count)
        {
            this.validationExceptionCount = count;
            this.OnPropertyChanged("ValidPropertiesCount");
        }    
    }
}
