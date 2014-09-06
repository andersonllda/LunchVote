using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO : ViewModelBase
    {
        #region ----- Construtor -----
        public vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO(wrpSumarioAvaliacaoMedicaCO pSumarioAvaliacaoMedicaCO)
        {
            if (pSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano == null)
                pSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano = new wrpSumarioAvaliacaoMedicaCOPlano(pSumarioAvaliacaoMedicaCO);
            this._sumarioAvaliacaoMedicaCOPlano = pSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano;
        }

        #endregion

        #region ----- Propriedades Privadas -----

        private wrpSumarioAvaliacaoMedicaCOPlano _sumarioAvaliacaoMedicaCOPlano { get; set; }                    
        #endregion

        #region ----- Propriedades Públicas -----

        public wrpSumarioAvaliacaoMedicaCOPlano SumarioAvaliacaoMedicaCOPlano
        {
            get
            {
                return this._sumarioAvaliacaoMedicaCOPlano;
            }
            set
            {
                this._sumarioAvaliacaoMedicaCOPlano = value;
            }
        }

        public bool AcompanhaParto
        {
            get
            {
                return _sumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.AcompanhamentoTrabalhoParto;
            }
            set
            {
                SumarioAvaliacaoMedicaCOPlano.Justificativa = null;
                _sumarioAvaliacaoMedicaCOPlano.TipoParto = null;
                if (value)                                         
                    _sumarioAvaliacaoMedicaCOPlano.TipoParto = TipoPartoCO.AcompanhamentoTrabalhoParto;                
                
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x=> x.AcompanhaParto);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaUrgencia);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaEletiva);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaJustifica);            
            }
        }

        public bool CesariaUrgencia
        {
            get
            {
                return _sumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaUrgencia;
            }
            set
            {                
                _sumarioAvaliacaoMedicaCOPlano.TipoParto = null;
                if (value)                                          
                    _sumarioAvaliacaoMedicaCOPlano.TipoParto = TipoPartoCO.CesareaUrgencia;               
               
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.AcompanhaParto);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaUrgencia);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaEletiva);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaJustifica);
            }
        }

        public bool CesariaEletiva
        {
            get
            {
                return _sumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaEletiva;
            }

            set
            {              
                _sumarioAvaliacaoMedicaCOPlano.TipoParto = null;
                if (value)
                    _sumarioAvaliacaoMedicaCOPlano.TipoParto = TipoPartoCO.CesareaEletiva;
                        
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.AcompanhaParto);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaUrgencia);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaEletiva);
                base.OnPropertyChanged<vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO>(x => x.CesariaJustifica);                

            }
        }

        public bool CesariaJustifica
        {
            get
            {
                return (CesariaEletiva || CesariaUrgencia);
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
