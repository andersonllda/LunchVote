using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia(wrpSumarioAvaliacaoMedicaEndoscopia pSumarioAvaliacaoMedicaEndoscopia)
        {
            this._sumarioavaliacaomedicaendoscopia = pSumarioAvaliacaoMedicaEndoscopia;
        }
        #endregion

        #region ----- Propriedades Privadas -----

        #endregion

        #region ----- Propriedades Públicas -----
        public SimNao? Cancer
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.Cancer;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.Cancer = value;
                if (value == SimNao.Nao)
                    this._sumarioavaliacaomedicaendoscopia.CancerObservacao = string.Empty;
                else
                    this._sumarioavaliacaomedicaendoscopia.Ausencia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerObservacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Cancer);
            }
        }
        public string CancerObservacao
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.CancerObservacao;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.CancerObservacao = value;
                if (value.IsNotEmptyOrWhiteSpace())
                {
                    this._sumarioavaliacaomedicaendoscopia.Cancer = SimNao.Sim;
                    this._sumarioavaliacaomedicaendoscopia.Ausencia = SimNao.Nao;
                    this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
                }
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Cancer);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerObservacao);
            }
        }
        public SimNao? CancerIntestino
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.CancerIntestino;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.CancerIntestino = value;
                if (value == SimNao.Nao)
                    this._sumarioavaliacaomedicaendoscopia.CancerIntestinoObservacao = string.Empty;
                else
                    this._sumarioavaliacaomedicaendoscopia.Ausencia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerIntestinoObservacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerIntestino);
            }
        }
        public string CancerIntestinoObservacao
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.CancerIntestinoObservacao;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.CancerIntestinoObservacao = value;
                if (value.IsNotEmptyOrWhiteSpace())
                {
                    this._sumarioavaliacaomedicaendoscopia.CancerIntestino = SimNao.Sim;
                    this._sumarioavaliacaomedicaendoscopia.Ausencia = SimNao.Nao;
                    this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
                }
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerIntestino);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerIntestinoObservacao);
            }
        }
        public SimNao? Outro
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.Outro;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.Outro = value;
                if (value == SimNao.Nao)
                    this._sumarioavaliacaomedicaendoscopia.OutroObservacao = string.Empty;
                else
                    this._sumarioavaliacaomedicaendoscopia.Ausencia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.OutroObservacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Outro);
            }
        }
        public string OutroObservacao
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.OutroObservacao;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.OutroObservacao = value;
                if (value.IsNotEmptyOrWhiteSpace())
                {
                    this._sumarioavaliacaomedicaendoscopia.Outro = SimNao.Sim;
                    this._sumarioavaliacaomedicaendoscopia.Ausencia = SimNao.Nao;
                    this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
                }

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Outro);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.OutroObservacao);
            }
        }
        public SimNao? Ausencia
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.Ausencia;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.Ausencia = value;
                if (value == SimNao.Sim)
                {
                    this._sumarioavaliacaomedicaendoscopia.CancerIntestino = SimNao.Nao;
                    this._sumarioavaliacaomedicaendoscopia.Cancer = SimNao.Nao;
                    this._sumarioavaliacaomedicaendoscopia.Outro = SimNao.Nao;
                    this._sumarioavaliacaomedicaendoscopia.OutroObservacao = string.Empty;
                    this._sumarioavaliacaomedicaendoscopia.CancerIntestinoObservacao = string.Empty;
                    this._sumarioavaliacaomedicaendoscopia.CancerObservacao = string.Empty;
                }

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Cancer);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerIntestino);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Outro);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.OutroObservacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerIntestinoObservacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.CancerObservacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia>(x => x.Ausencia);
            }
        }
        #endregion

        #region ----- Métodos Privados -----
        private wrpSumarioAvaliacaoMedicaEndoscopia _sumarioavaliacaomedicaendoscopia;
        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----

        #endregion
    }
}
