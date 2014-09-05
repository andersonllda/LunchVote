using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using HMV.Core.Framework.Expression;

namespace HMV.Core.Framework.Validations
{
    /// <summary>
    /// Número máximo de caracteres valor default 60
    /// </summary>
    
    public class ValidaMaximoCaracteres : StringLengthAttribute
    {
        public ValidaMaximoCaracteres()
            : base(60)
        { this.ErrorMessage = "Máximo de 60 Caracteres Excedido "; }

        public ValidaMaximoCaracteres(int pMax)
            : base(pMax)
        {
            this.ErrorMessage = "Máximo de ".CombineAndSeperate(" Caracteres Excedido", pMax.ToString());
        }

    }
}
