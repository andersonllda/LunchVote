using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Collections.ObjectModel;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoM
{
    public class vmNotasAdicionais : ViewModelBase
    {
        #region ----- Construtor -----
        public vmNotasAdicionais(SumarioAvaliacaoMedica pSumarioAvaliacaoMedica)
        {
            this._sumarioavaliacaomedica = new wrpSumarioAvaliacaoMedica(pSumarioAvaliacaoMedica);
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedica _sumarioavaliacaomedica { get; set; }
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoMedica SumarioAvaliacaoMedica
        {
            get { return this._sumarioavaliacaomedica; }
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
