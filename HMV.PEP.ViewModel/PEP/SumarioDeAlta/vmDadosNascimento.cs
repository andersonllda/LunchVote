using System;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.CentroObstetrico;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Types;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using StructureMap;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmDadosNascimento : ViewModelBase
    {
        #region Contrutor
        public vmDadosNascimento(wrpSumarioAlta pSumarioAlta)
        {
            this.SumarioAlta = pSumarioAlta;

            IRepositorioDeDadosNascimento rep = ObjectFactory.GetInstance<IRepositorioDeDadosNascimento>();
            var ret = rep.OndeCodigoAtendimentoIgual(pSumarioAlta.Atendimento.ID).Single();
            if (ret.IsNotNull())
                this._dadosnascimento = new wrpDadosNascimento(ret);
            else
            {
                this._dadosnascimento = new wrpDadosNascimento(pSumarioAlta.Atendimento, pSumarioAlta.Usuario);
                this._ImportaDadosRN();
            }
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAlta SumarioAlta { get; set; }

        public wrpDadosNascimento DadosNascimento
        {
            get
            {
                return _dadosnascimento;
            }
            set
            {
                this._dadosnascimento = value;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.DadosNascimento);
            }
        }

        public DateTime? DiaNascimento
        {
            get
            {
                if (this._dianascimento.IsNull() && this._dadosnascimento.DataNascimento.HasValue)
                    this._dianascimento = this._dadosnascimento.DataNascimento.Value;
                return this._dianascimento;
            }
            set
            {
                this._dianascimento = value;
                if (_horanascimento.HasValue && value.HasValue)
                    this._dadosnascimento.DataNascimento = DateTime.Parse(value.Value.ToShortDateString() + " " + _horanascimento.Value.ToShortTimeString());
                else
                    this._dadosnascimento.DataNascimento = null;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.DiaNascimento);
            }
        }
        public DateTime? HoraNascimento
        {
            get
            {
                if (this._horanascimento.IsNull() && this._dadosnascimento.DataNascimento.HasValue)
                    this._horanascimento = this._dadosnascimento.DataNascimento.Value;
                return this._horanascimento;
            }
            set
            {
                this._horanascimento = value;
                if (this._dianascimento.HasValue && value.HasValue)
                    this._dadosnascimento.DataNascimento = DateTime.Parse(this._dianascimento.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._dadosnascimento.DataNascimento = null;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.HoraNascimento);
            }
        }

        public List<string> CarregaPositivo
        {
            get
            {
                return Enum<FatorRH>.GetCustomDisplay().ToList();
            }
        }
        public List<string> CarregaTipagem
        {
            get
            {
                return Enum<Tipagem>.GetCustomDisplay().ToList();
            }
        }

        public bool Vaginal
        {
            get
            {
                if (this._dadosnascimento.TipoParto.HasValue)
                    return (this._dadosnascimento.TipoParto.Value == TipoPartoRN.Vaginal);
                return false;
            }
            set
            {
                if (value)
                {
                    this._dadosnascimento.TipoParto = TipoPartoRN.Vaginal;
                    this._dadosnascimento.MotivoCesarianaUrgencia = string.Empty;
                }
                else
                    this._dadosnascimento.TipoParto = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.CesarianaUrgencia);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.CesarianaEletiva);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.Vaginal);
            }
        }
        public bool CesarianaEletiva
        {
            get
            {
                if (this._dadosnascimento.TipoParto.HasValue)
                    return (this._dadosnascimento.TipoParto.Value == TipoPartoRN.CesarianaEletiva);
                return false;
            }
            set
            {
                if (value)
                {
                    this._dadosnascimento.TipoParto = TipoPartoRN.CesarianaEletiva;
                    this._dadosnascimento.MotivoCesarianaUrgencia = string.Empty;
                }
                else
                    this._dadosnascimento.TipoParto = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.Vaginal);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.CesarianaUrgencia);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.CesarianaEletiva);
            }
        }
        public bool CesarianaUrgencia
        {
            get
            {
                if (this._dadosnascimento.TipoParto.HasValue)
                    return (this._dadosnascimento.TipoParto.Value == TipoPartoRN.CesarianaUrgencia);
                return false;
            }
            set
            {
                if (value)
                    this._dadosnascimento.TipoParto = TipoPartoRN.CesarianaUrgencia;
                else
                    this._dadosnascimento.TipoParto = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.Vaginal);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.CesarianaEletiva);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.CesarianaUrgencia);
            }
        }

        public bool MembranasIntegras
        {
            get
            {
                if (this._dadosnascimento.Membrana.HasValue)
                    return (this._dadosnascimento.Membrana.Value == Membranas.Integras);
                return false;
            }
            set
            {
                if (value)
                {
                    this._dadosnascimento.Membrana = Membranas.Integras;
                    this._dadosnascimento.DataMembrana = null;
                    this._diamembrana = null;
                    this._horahoramembrana = null;
                }
                else
                    this._dadosnascimento.Membrana = null;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.MembranasRotas);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.MembranasIntegras);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.DiaMembrana);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.HoraMembrana);
            }
        }
        public bool MembranasRotas
        {
            get
            {
                if (this._dadosnascimento.Membrana.HasValue)
                    return (this._dadosnascimento.Membrana.Value == Membranas.Rotas);
                return false;
            }
            set
            {
                if (value)
                    this._dadosnascimento.Membrana = Membranas.Rotas;
                else
                    this._dadosnascimento.Membrana = null;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.MembranasIntegras);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.MembranasRotas);
            }
        }

        public DateTime? DiaMembrana
        {
            get
            {
                if (this._diamembrana.IsNull() && this._dadosnascimento.DataMembrana.HasValue)
                    this._diamembrana = this._dadosnascimento.DataMembrana.Value;
                return this._diamembrana;
            }
            set
            {
                this._diamembrana = value;
                if (_horahoramembrana.HasValue && value.HasValue)
                    this._dadosnascimento.DataMembrana = DateTime.Parse(value.Value.ToShortDateString() + " " + _horahoramembrana.Value.ToShortTimeString());
                else
                    this._dadosnascimento.DataMembrana = null;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.DiaMembrana);
            }
        }
        public DateTime? HoraMembrana
        {
            get
            {
                if (this._horahoramembrana.IsNull() && this._dadosnascimento.DataMembrana.HasValue)
                    this._horahoramembrana = this._dadosnascimento.DataMembrana.Value;
                return this._horahoramembrana;
            }
            set
            {
                this._horahoramembrana = value;
                if (this._diamembrana.HasValue && value.HasValue)
                    this._dadosnascimento.DataMembrana = DateTime.Parse(this._diamembrana.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._dadosnascimento.DataMembrana = null;
                this.OnPropertyChanged<vmDadosNascimento>(x => x.HoraMembrana);
            }
        }

        public bool LAClaro
        {
            get
            {
                if (this._dadosnascimento.LiquidoAmniotico.HasValue)
                    return (this._dadosnascimento.LiquidoAmniotico.Value == LiquidoAmniotico.Claro);
                return false;
            }
            set
            {
                if (value)
                {
                    this._dadosnascimento.LiquidoAmniotico = LiquidoAmniotico.Claro;
                    this._dadosnascimento.LiquidoAmnioticoObservacao = string.Empty;
                }
                else
                    this._dadosnascimento.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAOutros);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LATintoMeconio);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAMeconioEspesso);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAClaro);
            }
        }
        public bool LAMeconioEspesso
        {
            get
            {
                if (this._dadosnascimento.LiquidoAmniotico.HasValue)
                    return (this._dadosnascimento.LiquidoAmniotico.Value == LiquidoAmniotico.MeconioEspesso);
                return false;
            }
            set
            {
                if (value)
                {
                    this._dadosnascimento.LiquidoAmniotico = LiquidoAmniotico.MeconioEspesso;
                    this._dadosnascimento.LiquidoAmnioticoObservacao = string.Empty;
                }
                else
                    this._dadosnascimento.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAOutros);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LATintoMeconio);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAClaro);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAMeconioEspesso);
            }
        }
        public bool LATintoMeconio
        {
            get
            {
                if (this._dadosnascimento.LiquidoAmniotico.HasValue)
                    return (this._dadosnascimento.LiquidoAmniotico.Value == LiquidoAmniotico.TintoMeconio);
                return false;
            }
            set
            {
                if (value)
                {
                    this._dadosnascimento.LiquidoAmniotico = LiquidoAmniotico.TintoMeconio;
                    this._dadosnascimento.LiquidoAmnioticoObservacao = string.Empty;
                }
                else
                    this._dadosnascimento.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAOutros);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAMeconioEspesso);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAClaro);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LATintoMeconio);
            }
        }
        public bool LAOutros
        {
            get
            {
                if (this._dadosnascimento.LiquidoAmniotico.HasValue)
                    return (this._dadosnascimento.LiquidoAmniotico.Value == LiquidoAmniotico.Outros);
                return false;
            }
            set
            {
                if (value)
                    this._dadosnascimento.LiquidoAmniotico = LiquidoAmniotico.Outros;
                else
                    this._dadosnascimento.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.LATintoMeconio);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAMeconioEspesso);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAClaro);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.LAOutros);
            }
        }

        public bool ClassificacaoAIG
        {
            get
            {
                if (this._dadosnascimento.Classificacao.HasValue)
                    return (this._dadosnascimento.Classificacao.Value == ClassificacaoRN.AIG);
                return false;
            }
            set
            {
                if (value)
                    this._dadosnascimento.Classificacao = ClassificacaoRN.AIG;
                else
                    this._dadosnascimento.Classificacao = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoPIG);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoGIG);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoAIG);
            }
        }
        public bool ClassificacaoGIG
        {
            get
            {
                if (this._dadosnascimento.Classificacao.HasValue)
                    return (this._dadosnascimento.Classificacao.Value == ClassificacaoRN.GIG);
                return false;
            }
            set
            {
                if (value)
                    this._dadosnascimento.Classificacao = ClassificacaoRN.GIG;
                else
                    this._dadosnascimento.Classificacao = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoAIG);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoPIG);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoGIG);
            }
        }
        public bool ClassificacaoPIG
        {
            get
            {
                if (this._dadosnascimento.Classificacao.HasValue)
                    return (this._dadosnascimento.Classificacao.Value == ClassificacaoRN.PIG);
                return false;
            }
            set
            {
                if (value)
                    this._dadosnascimento.Classificacao = ClassificacaoRN.PIG;
                else
                    this._dadosnascimento.Classificacao = null;

                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoAIG);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoGIG);
                this.OnPropertyChanged<vmDadosNascimento>(x => x.ClassificacaoPIG);
            }
        }
        #endregion

        #region Commands

        #endregion

        #region Propriedades Privadas
        wrpDadosNascimento _dadosnascimento { get; set; }
        private DateTime? _dianascimento;
        private DateTime? _horanascimento;
        private DateTime? _diamembrana;
        private DateTime? _horahoramembrana;
        #endregion

        #region Metodos Privados
        private void _ImportaDadosRN()
        {
            wrpSumarioAvaliacaoMedicaRN ultsumariorn = null;

            IRepositorioDeSumarioDeAvaliacaoMedicaRN rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
            var ret = rep.OndePacienteIgual(this.SumarioAlta.Atendimento.DomainObject.Paciente);
            if (ret.List().Count(x => x.DataEncerramento.IsNotNull()) > 0)
            {
                ultsumariorn = new wrpSumarioAvaliacaoMedicaRN(ret.List().OrderByDescending(x => x.DataEncerramento).FirstOrDefault());
                //rep.Refresh(ultsumariorn.DomainObject);
            }

            if (ultsumariorn.IsNotNull())
            {
                if (ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.IsNotNull())
                {
                    this._dadosnascimento.DataNascimento = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.DataNascimento;
                    this._dadosnascimento.Peso = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.Peso;
                    this._dadosnascimento.ApgarPrimeiro = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro;
                    this._dadosnascimento.ApgarQuinto = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto;
                    this._dadosnascimento.ApgarDessimo = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo;
                    this._dadosnascimento.Classificacao = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.Classificacao;
                    this._dadosnascimento.Comprimento = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.Comprimento;
                    this._dadosnascimento.PerimentroCefalico = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico;
                    this._dadosnascimento.PerimetroToracico = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico;
                }

                this._dadosnascimento.DataMembrana = ultsumariorn.DataMembrana;

                this._dadosnascimento.IdadeSemanas = ultsumariorn.IdadeSemanas;
                this._dadosnascimento.IdadeDias = ultsumariorn.IdadeDias;
                this._dadosnascimento.IsIdadeDesconhecido = ultsumariorn.IsIdadeDesconhecido;

                this._dadosnascimento.LiquidoAmniotico = ultsumariorn.LiquidoAmniotico;
                this._dadosnascimento.LiquidoAmnioticoObservacao = ultsumariorn.LiquidoAmnioticoObservacao;

                this._dadosnascimento.Membrana = ultsumariorn.Membrana;
                this._dadosnascimento.MotivoCesarianaUrgencia = ultsumariorn.MotivoCesarianaUrgencia;

                this._dadosnascimento.RHPaciente = ultsumariorn.RHPaciente;
                this._dadosnascimento.TipagemPaciente = ultsumariorn.TipagemPaciente;
                this._dadosnascimento.TipoParto = ultsumariorn.TipoParto;

                int codOlhinho = 0;
                Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaCodigoTesteOlhinho().Single();
                if (par.IsNotNull())
                    codOlhinho = par.Valor.ToInt();
                if (codOlhinho > 0)
                    if (ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == codOlhinho).HasItems())
                    {
                        this._dadosnascimento.TesteOlhinho = ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == codOlhinho).FirstOrDefault().IsNormal;
                        this._dadosnascimento.TesteOlhinhoObservacao = ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == codOlhinho).FirstOrDefault().Observacoes;
                    }
            }
        }
        #endregion
    }
}
