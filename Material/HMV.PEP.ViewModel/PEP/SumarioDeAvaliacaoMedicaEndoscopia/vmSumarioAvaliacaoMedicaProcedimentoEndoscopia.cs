using HMV.Core.Domain.Enum;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmSumarioAvaliacaoMedicaProcedimentoEndoscopia : ViewModelBase
    {
        #region Contrutor
        public vmSumarioAvaliacaoMedicaProcedimentoEndoscopia(wrpSumarioAvaliacaoMedicaEndoscopia pSumarioAvaliacaoMedicaEndoscopia)
        {
            this._sumarioavaliacaomedicaendoscopia = pSumarioAvaliacaoMedicaEndoscopia;
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAvaliacaoMedicaEndoscopia SumarioAvaliacaoMedicaEndoscopia
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.SumarioAvaliacaoMedicaEndoscopia);
            }
        }

        public bool END
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.EndoscopiaDigestivaAlta)
                //    return true;
                //else
                //    return false;
                if (this._sumarioavaliacaomedicaendoscopia.EndoscopiaDigestivaAlta == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.EndoscopiaDigestivaAlta;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.EndoscopiaDigestivaAlta = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.EndoscopiaDigestivaAlta = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool COL
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.Colonoscopia)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.Colonoscopia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.Colonoscopia;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.Colonoscopia = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.Colonoscopia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool CPE
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.ColangiopancreatografiaEndoscopica)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.Colangiopancreatografia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.ColangiopancreatografiaEndoscopica;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.Colangiopancreatografia = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.Colangiopancreatografia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool ECOA
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.EcoendoscopiaAlta)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaAlta == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.EcoendoscopiaAlta;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaAlta = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaAlta = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool ECOB
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.EcoendoscopiaBaixa)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaBaixa == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.EcoendoscopiaBaixa;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaBaixa = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaBaixa = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool FIB
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.Fibrobroncoscopia)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.Fibrobroncospia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.Fibrobroncoscopia;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.Fibrobroncospia = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.Fibrobroncospia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool LAR
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.Laringoscopia)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.Laringoscopia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.Laringoscopia;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.Laringoscopia = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.Laringoscopia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool RET
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.Retossigmoidoscopia)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.Retossigmoidoscopia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.Retossigmoidoscopia;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.Retossigmoidoscopia = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.Retossigmoidoscopia = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }

        public bool ENE
        {
            get
            {
                //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado == ProcedimentoPlanejado.Enema)
                //    return true;
                //else
                //    return false;

                if (this._sumarioavaliacaomedicaendoscopia.Enema == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                //if (value)
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ProcedimentoPlanejado.Enema;
                //else
                //    this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = null;

                if (value)
                    this._sumarioavaliacaomedicaendoscopia.Enema = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicaendoscopia.Enema = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.COL);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.CPE);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.FIB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.LAR);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.END);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOA);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.RET);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ECOB);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaProcedimentoEndoscopia>(x => x.ENE);
            }
        }
        #endregion

        #region Métodos Privados

        #endregion

        #region Propriedades PrivadasSumarioAvaliacaoMedica
        private wrpSumarioAvaliacaoMedicaEndoscopia _sumarioavaliacaomedicaendoscopia;
        #endregion
    }
}
