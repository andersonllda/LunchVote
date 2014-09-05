using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.Types;
using System.ComponentModel;

namespace HMV.Core.Framework.Commands
{
    public enum BaseCadastrosPOS
    {
        [CustomDisplay("Cadastro de Instituição")]
        [Description("HMV.Core.Wrappers.ObjectWrappers.wrpInstituicao,HMV.Core.Wrappers")]
        Instituicao,
        [CustomDisplay("Cadastro de Cargo")]
        [Description("HMV.Core.Wrappers.ObjectWrappers.wrpCargo,HMV.Core.Wrappers")]
        Cargo
    }

    public static class BaseCadastrosPOSExtension
    {
        public static string CustomDisplay(this BaseCadastrosPOS self)
        { return Enum<BaseCadastrosPOS>.GetCustomDisplayOf(self); }

        public static string Description(this BaseCadastrosPOS self)
        { return Enum<BaseCadastrosPOS>.GetDescriptionOf(self); }
    }

    public class BaseCadastrosPOSEventArgs : EventArgs
    {
        public BaseCadastrosPOSEventArgs(BaseCadastrosPOS pBaseCadastroPOS)
        {
            this._basecadastropos = pBaseCadastroPOS;
        }
        private BaseCadastrosPOS _basecadastropos { get; set; }
        public BaseCadastrosPOS BaseCadastrosPOS
        {
            get
            {
                return this._basecadastropos;
            }        
        }
    }
}
