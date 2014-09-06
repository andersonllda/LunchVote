using System;
using System.Collections.Generic;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.CentroObstetrico;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;


namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmExameFisicoSumarioAvaliacaoMedicaCO : ViewModelBase
    {
        #region ----- Construtor -----
        public vmExameFisicoSumarioAvaliacaoMedicaCO(wrpSumarioAvaliacaoMedicaCO pSumarioAvaliacaoMedicaCO)
        {
            if (pSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico == null)
                pSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico = new wrpSumarioAvaliacaoMedicaCOExameFisico(pSumarioAvaliacaoMedicaCO);

            this._sumarioavaliacaomedicacoexamefisico = pSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico;
        }

        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaCOExameFisico _sumarioavaliacaomedicacoexamefisico;
        private DateTime? _diamembrana;
        private DateTime? _horahoramembrana;

        #endregion

        #region ----- Propriedades Públicas -----

        public wrpSumarioAvaliacaoMedicaCOExameFisico SumarioAvaliacaoMedicaCOExameFisico
        {
            get
            {
                return this._sumarioavaliacaomedicacoexamefisico;
            }
            set
            {
                this._sumarioavaliacaomedicacoexamefisico = value;
            }

        }      

        public bool Claro
        {
            get
            {
                return _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico == LiquidoAmniotico.Claro;
            }

            set
            {
                LiquidoAmnioticoObservacao = string.Empty;
                _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = null;
                if (value)
                    _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = LiquidoAmniotico.Claro;
                base.OnPropertyChanged("Claro");
                base.OnPropertyChanged("TintoMeconio");
                base.OnPropertyChanged("MeconioEspesso");
                base.OnPropertyChanged("Outros");
            }
        }

        public bool TintoMeconio
        {
            get
            {
                return _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico == LiquidoAmniotico.TintoMeconio;
            }

            set
            {
                LiquidoAmnioticoObservacao = string.Empty;
                _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = null;
                if (value)
                    _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = LiquidoAmniotico.TintoMeconio;
                base.OnPropertyChanged("TintoMeconio");
                base.OnPropertyChanged("Claro");
                base.OnPropertyChanged("MeconioEspesso");
                base.OnPropertyChanged("Outros");
            }
        }

        public bool MeconioEspesso
        {
            get
            {
                return _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico == LiquidoAmniotico.MeconioEspesso;
            }

            set
            {
                LiquidoAmnioticoObservacao = string.Empty;
                _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = null;
                if (value)
                    _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = LiquidoAmniotico.MeconioEspesso;
                base.OnPropertyChanged("Claro");
                base.OnPropertyChanged("TintoMeconio");
                base.OnPropertyChanged("Outros");
                base.OnPropertyChanged("MeconioEspesso");
            }
        }

        public bool Outros
        {
            get
            {
                return (_sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico == LiquidoAmniotico.Outros);
            }

            set
            {
                _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = null;
                if (value)
                {
                    _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = LiquidoAmniotico.Outros;
                    base.OnPropertyChanged("Claro");
                    base.OnPropertyChanged("TintoMeconio");
                    base.OnPropertyChanged("MeconioEspesso");
                }
                else
                {
                    LiquidoAmnioticoObservacao = string.Empty;
                }
                base.OnPropertyChanged("Outros");
            }
        }

        public bool HabilitaObs
        {
            get
            {
                return (_sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico != LiquidoAmniotico.Claro && _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico != LiquidoAmniotico.MeconioEspesso && _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico != LiquidoAmniotico.TintoMeconio);
            }

            set
            {
                _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = null;
                if (value)
                    _sumarioavaliacaomedicacoexamefisico.LiquidoAmniotico = LiquidoAmniotico.MeconioEspesso;

                base.OnPropertyChanged("HabilitaObs");
                base.OnPropertyChanged("Outros");
                base.OnPropertyChanged("Claro");
                base.OnPropertyChanged("TintoMeconio");
                base.OnPropertyChanged("MeconioEspesso");
            }
        }

        public bool integras
        {
            get
            {
                return (_sumarioavaliacaomedicacoexamefisico.Membrana == Membranas.Integras);
            }

            set
            {
                _sumarioavaliacaomedicacoexamefisico.Membrana = null;
                if (value)
                {
                    _sumarioavaliacaomedicacoexamefisico.Membrana = Membranas.Integras;
                    this._sumarioavaliacaomedicacoexamefisico.DataMembrana = null;
                    this._diamembrana = null;
                    this._horahoramembrana = null;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.integras));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.rotas));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.DiaMembrana));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.HoraMembrana));
            }
        }
        public bool rotas
        {
            get
            {
                return (_sumarioavaliacaomedicacoexamefisico.Membrana == Membranas.Rotas);
            }

            set
            {
                _sumarioavaliacaomedicacoexamefisico.Membrana = null;
                if (value)
                    _sumarioavaliacaomedicacoexamefisico.Membrana = Membranas.Rotas;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.integras));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.rotas));
            }
        }

        public DateTime? DiaMembrana
        {
            get
            {
                if (this._diamembrana.IsNull() && this._sumarioavaliacaomedicacoexamefisico.DataMembrana.HasValue)
                    this._diamembrana = this._sumarioavaliacaomedicacoexamefisico.DataMembrana.Value;
                return this._diamembrana;
            }
            set
            {
                this._diamembrana = value;
                if (_horahoramembrana.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicacoexamefisico.DataMembrana = DateTime.Parse(value.Value.ToShortDateString() + " " + _horahoramembrana.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicacoexamefisico.DataMembrana = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.DiaMembrana));
            }
        }
        public DateTime? HoraMembrana
        {
            get
            {
                if (this._horahoramembrana.IsNull() && this._sumarioavaliacaomedicacoexamefisico.DataMembrana.HasValue)
                    this._horahoramembrana = this._sumarioavaliacaomedicacoexamefisico.DataMembrana.Value;
                return this._horahoramembrana;
            }
            set
            {
                this._horahoramembrana = value;
                if (this._diamembrana.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicacoexamefisico.DataMembrana = DateTime.Parse(this._diamembrana.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicacoexamefisico.DataMembrana = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.HoraMembrana));
            }
        }

        public string LiquidoAmnioticoObservacao
        {
            get
            {
                return this._sumarioavaliacaomedicacoexamefisico.LiquidoAmnioticoObservacao;
            }

            set
            {
                this._sumarioavaliacaomedicacoexamefisico.LiquidoAmnioticoObservacao = value;
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.LiquidoAmnioticoObservacao));
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.Outros));
            }
        }

        public bool SangramentoSim
        {
            get
            {
                return (this._sumarioavaliacaomedicacoexamefisico.IsSangramento == SimNao.Sim);
            }

            set
            {
                this._sumarioavaliacaomedicacoexamefisico.IsSangramento = null;
                if (value)
                    this._sumarioavaliacaomedicacoexamefisico.IsSangramento = SimNao.Sim;
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.SangramentoSim));
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.SangramentoNao));
            }
        }

        public bool SangramentoNao
        {
            get
            {
                return (this._sumarioavaliacaomedicacoexamefisico.IsSangramento == SimNao.Nao);
            }

            set
            {
                this._sumarioavaliacaomedicacoexamefisico.IsSangramento = null;
                if (value)
                    this._sumarioavaliacaomedicacoexamefisico.IsSangramento = SimNao.Nao;
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.SangramentoSim));
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoSumarioAvaliacaoMedicaCO>(x => x.SangramentoNao));
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
