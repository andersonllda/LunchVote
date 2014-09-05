using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PesqClinica;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.PEP.PesqClinica
{
    public class vmPesquisaClinica : ViewModelBase
    {

        #region ----- Construtor -----
        public vmPesquisaClinica(wrpPesquisaClinica pPesquisa)
        {
            this.PesquisaClinica = pPesquisa;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpPesquisaClinica _pesquisa;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpPesquisaClinica PesquisaClinica
        {
            get
            {
                return this._pesquisa;
            }
            set
            {
                this._pesquisa = value;
                this.OnPropertyChanged<vmPesquisaClinica>(x => x.PesquisaClinica);
            }
        }

        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----

        #endregion
    }
}
