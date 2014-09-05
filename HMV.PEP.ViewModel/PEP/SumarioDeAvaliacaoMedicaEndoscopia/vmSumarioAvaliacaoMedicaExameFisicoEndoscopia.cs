using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Framework.Validations;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using StructureMap;
using HMV.Core.Domain.Model;
using System;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmSumarioAvaliacaoMedicaExameFisicoEndoscopia : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaExameFisicoEndoscopia(wrpSumarioAvaliacaoMedicaEndoscopia pSumarioAvaliacaoMedicaEndoscopia)
        {
            this._sumarioavaliacaomedicaendoscopia = pSumarioAvaliacaoMedicaEndoscopia;
            this._usuario = pSumarioAvaliacaoMedicaEndoscopia.Usuario;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaEndoscopia _sumarioavaliacaomedicaendoscopia;
        private wrpUsuarios _usuario;
        #endregion

        #region ----- Propriedades Públicas -----
        [ValidaMaximoEMinimo(1, 300)]
        public int Altura
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.Altura;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.Altura = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.IMC);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.SC);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.Altura);
            }
        }

        [ValidaMaximoEMinimo(1, 200)]
        public double Peso
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.Peso;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.Peso = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.IMC);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.SC);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.Peso);
            }
        }        

        [ValidaCampoObrigatorio()]        
        public int? PAAlta
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.PA.Alta;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.PA = new wrpPressaoArterial(value, this._sumarioavaliacaomedicaendoscopia.PA.Baixa);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.PAAlta);
            }
        }

        [ValidaCampoObrigatorio()]        
        public int? PABaixa
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.PA.Baixa;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.PA = new wrpPressaoArterial(this._sumarioavaliacaomedicaendoscopia.PA.Alta, value);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.PABaixa);
            }
        }

        [ValidaMaximoEMinimo(34, 42)]
        public double TAX
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.TAX;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.TAX = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.TAX);
            }
        }

        [ValidaMaximoEMinimo(30, 300)]
        public double FC
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.FC;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.FC = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.FC);
            }
        }

        [ValidaMaximoEMinimo(0, 100)]
        public double SAT
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.SAT;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.SAT = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.SAT);
            }
        }

        [ValidaMaximoEMinimo(5, 100)]
        public double FR
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.FR;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.FR = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.FR);
            }
        }


        public double IMC
        {
            get
            {
                return Math.Round(this._sumarioavaliacaomedicaendoscopia.Peso / Math.Pow((double)this._sumarioavaliacaomedicaendoscopia.Altura / 100, 2), 2);
            }
        }

        public double SC
        {
            get
            {
                return Math.Round(0.007184 * Math.Pow(this._sumarioavaliacaomedicaendoscopia.Peso, 0.425) * Math.Pow((double)this._sumarioavaliacaomedicaendoscopia.Altura, 0.725), 2);
            }
        }


        public string Observacao
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.ExameFisicoObservacao;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia.ExameFisicoObservacao = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.Observacao);
            }
        }

        public bool EGBom
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.EstadoGeral == EstadoGeralEndoscopia.Bom);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.EstadoGeral = EstadoGeralEndoscopia.Bom;
                else
                    this._sumarioavaliacaomedicaendoscopia.EstadoGeral = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGMal);

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGRegular);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGBom);
            }
        }

        public bool EGRegular
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.EstadoGeral == EstadoGeralEndoscopia.Regular);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.EstadoGeral = EstadoGeralEndoscopia.Regular;
                else
                    this._sumarioavaliacaomedicaendoscopia.EstadoGeral = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGMal);

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGBom);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGRegular);
            }
        }

        public bool EGMal
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.EstadoGeral == EstadoGeralEndoscopia.Mal);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.EstadoGeral = EstadoGeralEndoscopia.Mal;
                else
                    this._sumarioavaliacaomedicaendoscopia.EstadoGeral = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGBom);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGRegular);

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.EGMal);
            }
        }

        public bool MCUmidas
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.MucosaSituacao == MucosasSituacao.Hidratado);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.MucosaSituacao = MucosasSituacao.Hidratado;
                else
                    this._sumarioavaliacaomedicaendoscopia.MucosaSituacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCSecas);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCUmidas);
            }
        }

        public bool MCSecas
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.MucosaSituacao == MucosasSituacao.Desidratado);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.MucosaSituacao = MucosasSituacao.Desidratado;
                else
                    this._sumarioavaliacaomedicaendoscopia.MucosaSituacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCUmidas);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCSecas);
            }
        }

        public bool MCCoradas
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.MucosaEstado == MucosasEstado.Corado);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.MucosaEstado = MucosasEstado.Corado;
                else
                    this._sumarioavaliacaomedicaendoscopia.MucosaEstado = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCDescoradas);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCCoradas);
            }
        }

        public bool MCDescoradas
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.MucosaEstado == MucosasEstado.Hipocorado);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.MucosaEstado = MucosasEstado.Hipocorado;
                else
                    this._sumarioavaliacaomedicaendoscopia.MucosaEstado = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCCoradas);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.MCDescoradas);
            }
        }

        public bool NCLucido
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.NivelConsciencia == NivelConsciencia.Lucido);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.NivelConsciencia = NivelConsciencia.Lucido;
                else
                    this._sumarioavaliacaomedicaendoscopia.NivelConsciencia = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCConfuso);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCComatoso);

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCLucido);
            }
        }

        public bool NCConfuso
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.NivelConsciencia == NivelConsciencia.Confuso);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.NivelConsciencia = NivelConsciencia.Confuso;
                else
                    this._sumarioavaliacaomedicaendoscopia.NivelConsciencia = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCComatoso);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCLucido);

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCConfuso);
            }
        }

        public bool NCComatoso
        {
            get
            {
                return (this._sumarioavaliacaomedicaendoscopia.NivelConsciencia == NivelConsciencia.Comatoso);
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicaendoscopia.NivelConsciencia = NivelConsciencia.Comatoso;
                else
                    this._sumarioavaliacaomedicaendoscopia.NivelConsciencia = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCConfuso);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCLucido);

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaExameFisicoEndoscopia>(x => x.NCComatoso);
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----
        public override bool IsValid
        {
            get
            {
                if (this._sumarioavaliacaomedicaendoscopia.EstadoGeral.IsNull()
                    || this._sumarioavaliacaomedicaendoscopia.MucosaSituacao.IsNull()
                    || this._sumarioavaliacaomedicaendoscopia.MucosaEstado.IsNull()
                    || this._sumarioavaliacaomedicaendoscopia.NivelConsciencia.IsNull())
                    return false;
                return base.IsValid;
            }
        }
        #endregion
    }
}
