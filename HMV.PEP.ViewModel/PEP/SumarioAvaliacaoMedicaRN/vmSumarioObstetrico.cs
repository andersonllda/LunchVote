using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.CentroObstetrico;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Types;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN
{
    public class vmSumarioObstetrico : ViewModelBase
    {
        #region Enum
        public enum TabsSumarioObstetrico
        {
            [Description("Identificação / Gestações Anteriores")]
            IdentificacaoGestacoesAnteriores,
            [Description("Gestação Atual")]
            GestacaoAtual,
            [Description("Sorologia/Exames")]
            ExamesLaboratoriais,
            [Description("Parto")]
            Parto
        }
        #endregion

        #region ----- Construtor -----
        public vmSumarioObstetrico(wrpSumarioAvaliacaoMedicaRN pSumarioRN, vmSumarioAvaliacaoMedicaRN pVm)
        {
            this._sumarioavaliacaomedicarn = pSumarioRN;
            this._vm = pVm;

            //IMPORTACAO DOS DADOS DO SUMARIO CO
            if (this._sumarioavaliacaomedicarn.Atendimento.AtendimentoPai.IsNotNull() && pVm.Novo)
            {
                var pai = this._sumarioavaliacaomedicarn.Atendimento.AtendimentoPai;
                IRepositorioDeSumarioAvaliacaoMedicaCO rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCO>();
                var ret = rep.OndeCodigoAtendimentoIgual(pai.DomainObject).Single();
                if (ret.IsNotNull())
                {
                    var sumco = new wrpSumarioAvaliacaoMedicaCO(ret);
                    this._sumarioavaliacaomedicarn.Gesta = sumco.Gesta;
                    this._sumarioavaliacaomedicarn.Para = sumco.Para;
                    this._sumarioavaliacaomedicarn.Cesarea = sumco.Cesarea;
                    this._sumarioavaliacaomedicarn.Aborto = sumco.Aborto;
                    this._sumarioavaliacaomedicarn.Ectopica = sumco.Ectopica;

                    this._sumarioavaliacaomedicarn.TipagemPaciente = sumco.TipagemPaciente;
                    this._sumarioavaliacaomedicarn.RHPaciente = sumco.RHPaciente;

                    this._sumarioavaliacaomedicarn.IdadeDias = sumco.IdadeDias;
                    this._sumarioavaliacaomedicarn.IdadeSemanas = sumco.IdadeSemana;
                    this._sumarioavaliacaomedicarn.IsIdadeDesconhecido = sumco.IdadeDesconhecida;

                    if (sumco.SumarioAvaliacaoMedicaCOPlano.IsNotNull())
                    {
                        if (sumco.SumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaEletiva)
                            this._sumarioavaliacaomedicarn.TipoParto = TipoPartoRN.CesarianaEletiva;
                        else if (sumco.SumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaUrgencia)
                        {
                            this._sumarioavaliacaomedicarn.TipoParto = TipoPartoRN.CesarianaUrgencia;                            
                        }
                        this._sumarioavaliacaomedicarn.MotivoCesarianaUrgencia = sumco.SumarioAvaliacaoMedicaCOPlano.Justificativa;
                    }

                    if (sumco.SumarioAvaliacaoMedicaCOExameFisico.IsNotNull())
                    {
                        this._sumarioavaliacaomedicarn.Membrana = sumco.SumarioAvaliacaoMedicaCOExameFisico.Membrana;
                        this._sumarioavaliacaomedicarn.DataMembrana = sumco.SumarioAvaliacaoMedicaCOExameFisico.DataMembrana;
                        this._sumarioavaliacaomedicarn.LiquidoAmniotico = sumco.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmniotico;
                        this._sumarioavaliacaomedicarn.LiquidoAmnioticoObservacao = sumco.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmnioticoObservacao;
                    }

                    if (sumco.SumarioAvaliacaoMedicaCOItens.Count(x => x.ItemCO.GestacaoAnterior == SimNao.Sim) > 0)
                        foreach (var item in sumco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.GestacaoAnterior == SimNao.Sim).ToList())
                        {
                            this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, item.ItemCO)
                                {
                                    Observacoes = item.Observacao
                                });
                        }
                    else
                    {
                        this._sumarioavaliacaomedicarn.IsGestacaoAnterior = sumco.IsGestacaoAnterior;
                        this._sumarioavaliacaomedicarn.IsPrimeiraGestacao = sumco.IsPrimeiraGestacao;
                    }

                    if (sumco.SumarioAvaliacaoMedicaCOItens.Count(x => x.ItemCO.Patologia == SimNao.Sim) > 0)
                        foreach (var item in sumco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.Patologia == SimNao.Sim).ToList())
                        {
                            this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, item.ItemCO)
                            {
                                Observacoes = item.Observacao
                            });
                        }
                    else
                        this._sumarioavaliacaomedicarn.IsPatologia = SimNao.Sim;

                    foreach (var item in sumco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.Exames == SimNao.Sim || x.ItemCO.Sorologia == SimNao.Sim).ToList())
                    {
                        var novo = new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, item.ItemCO);
                        if (item.Resultado.HasValue)
                        {
                            if (item.Resultado.Value == ResultadoItemCO.NaoDisponivel)
                                novo.ResultadoRN = ResultadoItemRN.NaoDisponivel;
                            else if (item.Resultado.Value == ResultadoItemCO.Negativo)
                                novo.ResultadoRN = ResultadoItemRN.Negativo;
                            else
                                novo.ResultadoRN = ResultadoItemRN.Positivo;
                        }
                        this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(novo);
                    }

                    this._sumarioavaliacaomedicarn.GestacaoAnteriorObservacao = sumco.GestacaoAnteriorObservacao;
                    this._sumarioavaliacaomedicarn.PatologiaObservacao = sumco.PatologiaObservacao;
                    this._sumarioavaliacaomedicarn.SorologiaObservacao = sumco.ExamesRealizadosObservacao;
                }
            }
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsSumarioObstetrico>> _tabs;
        private TabsSumarioObstetrico _tipotabselecionada;
        private wrpSumarioAvaliacaoMedicaRN _sumarioavaliacaomedicarn;
        private DateTime? _diamembrana;
        private DateTime? _horahoramembrana;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectiongestacaoanterior;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectiongest;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectionExames;
        //VMs       
        private vmSumarioAvaliacaoMedicaRN _vm;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoMedicaRN SumarioAvaliacaoMedicaRN
        {
            get
            {
                return this._sumarioavaliacaomedicarn;
            }
        }

        public string NomeMae
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsNotNull())
                {
                    return this._sumarioavaliacaomedicarn.Paciente.NomeMae;
                }
                return string.Empty;
            }
        }
        public string IdadeMae
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsNotNull())
                {
                    var pai = this._sumarioavaliacaomedicarn.Atendimento.AtendimentoPai;
                    if (pai.IsNotNull())
                        return pai.Paciente.Idade.GetDate(this._sumarioavaliacaomedicarn.Atendimento.AtendimentoPai.HoraAtendimento);
                }
                return string.Empty;
            }
        }
        public string CorMae
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsNotNull())
                {
                    var pai = this._sumarioavaliacaomedicarn.Atendimento.AtendimentoPai;
                    if (pai.IsNotNull())
                        return pai.Paciente.Cor.ToString();
                }
                return string.Empty;
            }
        }
        public string MedicoMae
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsNotNull())
                {
                    var pai = this._sumarioavaliacaomedicarn.Atendimento.AtendimentoPai;
                    if (pai.IsNotNull())
                        return pai.Prestador.NomeExibicao;
                            //.NomeExibicaoPrestador;
                }
                return string.Empty;
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

        public bool MedicacaoNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsMedicacao.HasValue)
                    return (this._sumarioavaliacaomedicarn.IsMedicacao.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.IsMedicacao = SimNao.Nao;
                    this._sumarioavaliacaomedicarn.MedicacaoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.IsMedicacao = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MedicacaoSim);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MedicacaoNao);
            }
        }
        public bool MedicacaoSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsMedicacao.HasValue)
                    return (this._sumarioavaliacaomedicarn.IsMedicacao.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.IsMedicacao = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarn.IsMedicacao = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MedicacaoNao);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MedicacaoSim);
            }
        }

        public bool Vaginal
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoParto.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoParto.Value == TipoPartoRN.Vaginal);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.TipoParto = TipoPartoRN.Vaginal;
                    this._sumarioavaliacaomedicarn.MotivoCesarianaUrgencia = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.TipoParto = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CesarianaUrgencia);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CesarianaEletiva);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.Vaginal);
            }
        }
        public bool CesarianaEletiva
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoParto.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoParto.Value == TipoPartoRN.CesarianaEletiva);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.TipoParto = TipoPartoRN.CesarianaEletiva;
                    this._sumarioavaliacaomedicarn.MotivoCesarianaUrgencia = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.TipoParto = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.Vaginal);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CesarianaUrgencia);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CesarianaEletiva);
            }
        }
        public bool CesarianaUrgencia
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoParto.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoParto.Value == TipoPartoRN.CesarianaUrgencia);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoParto = TipoPartoRN.CesarianaUrgencia;
                else
                    this._sumarioavaliacaomedicarn.TipoParto = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.Vaginal);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CesarianaEletiva);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CesarianaUrgencia);
            }
        }

        public bool SituacaoFetalNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora.HasValue)
                    return (this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora = SimNao.Nao;
                    this._sumarioavaliacaomedicarn.SituacaoFetalNaoTraquilizadoraObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.SituacaoFetalSim);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.SituacaoFetalNao);
            }
        }
        public bool SituacaoFetalSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora.HasValue)
                    return (this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarn.IsSituacaoFetalNaoTraquilizadora = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.SituacaoFetalNao);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.SituacaoFetalSim);
            }
        }

        public bool MembranasIntegras
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.Membrana.HasValue)
                    return (this._sumarioavaliacaomedicarn.Membrana.Value == Membranas.Integras);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.Membrana = Membranas.Integras;
                    this._sumarioavaliacaomedicarn.DataMembrana = null;
                    this._diamembrana = null;
                    this._horahoramembrana = null;
                }
                else
                    this._sumarioavaliacaomedicarn.Membrana = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MembranasRotas);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MembranasIntegras);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.DiaMembrana);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.HoraMembrana);
            }
        }
        public bool MembranasRotas
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.Membrana.HasValue)
                    return (this._sumarioavaliacaomedicarn.Membrana.Value == Membranas.Rotas);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.Membrana = Membranas.Rotas;
                else
                    this._sumarioavaliacaomedicarn.Membrana = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MembranasIntegras);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.MembranasRotas);
            }
        }

        public DateTime? DiaMembrana
        {
            get
            {
                if (this._diamembrana.IsNull() && this._sumarioavaliacaomedicarn.DataMembrana.HasValue)
                    this._diamembrana = this._sumarioavaliacaomedicarn.DataMembrana.Value;
                return this._diamembrana;
            }
            set
            {
                this._diamembrana = value;
                if (_horahoramembrana.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicarn.DataMembrana = DateTime.Parse(value.Value.ToShortDateString() + " " + _horahoramembrana.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicarn.DataMembrana = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.DiaMembrana);
            }
        }
        public DateTime? HoraMembrana
        {
            get
            {
                if (this._horahoramembrana.IsNull() && this._sumarioavaliacaomedicarn.DataMembrana.HasValue)
                    this._horahoramembrana = this._sumarioavaliacaomedicarn.DataMembrana.Value;
                return this._horahoramembrana;
            }
            set
            {
                this._horahoramembrana = value;
                if (this._diamembrana.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicarn.DataMembrana = DateTime.Parse(this._diamembrana.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicarn.DataMembrana = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.HoraMembrana);
            }
        }

        public bool LAClaro
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.LiquidoAmniotico.HasValue)
                    return (this._sumarioavaliacaomedicarn.LiquidoAmniotico.Value == LiquidoAmniotico.Claro);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = LiquidoAmniotico.Claro;
                    this._sumarioavaliacaomedicarn.LiquidoAmnioticoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAOutros);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LATintoMeconio);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAMeconioEspesso);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAClaro);
            }
        }
        public bool LAMeconioEspesso
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.LiquidoAmniotico.HasValue)
                    return (this._sumarioavaliacaomedicarn.LiquidoAmniotico.Value == LiquidoAmniotico.MeconioEspesso);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = LiquidoAmniotico.MeconioEspesso;
                    this._sumarioavaliacaomedicarn.LiquidoAmnioticoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAOutros);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LATintoMeconio);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAClaro);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAMeconioEspesso);
            }
        }
        public bool LATintoMeconio
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.LiquidoAmniotico.HasValue)
                    return (this._sumarioavaliacaomedicarn.LiquidoAmniotico.Value == LiquidoAmniotico.TintoMeconio);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = LiquidoAmniotico.TintoMeconio;
                    this._sumarioavaliacaomedicarn.LiquidoAmnioticoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAOutros);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAMeconioEspesso);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAClaro);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LATintoMeconio);
            }
        }
        public bool LAOutros
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.LiquidoAmniotico.HasValue)
                    return (this._sumarioavaliacaomedicarn.LiquidoAmniotico.Value == LiquidoAmniotico.Outros);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = LiquidoAmniotico.Outros;
                else
                    this._sumarioavaliacaomedicarn.LiquidoAmniotico = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LATintoMeconio);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAMeconioEspesso);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAClaro);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.LAOutros);
            }
        }

        public bool ACefalica
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.Apresentacao.HasValue)
                    return (this._sumarioavaliacaomedicarn.Apresentacao.Value == ApresentacaoRN.Cefalica);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.Apresentacao = ApresentacaoRN.Cefalica;
                    this._sumarioavaliacaomedicarn.ApresentacaoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.Apresentacao = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.AOutros);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.APelvica);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.ACefalica);
            }
        }
        public bool APelvica
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.Apresentacao.HasValue)
                    return (this._sumarioavaliacaomedicarn.Apresentacao.Value == ApresentacaoRN.Pelvica);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.Apresentacao = ApresentacaoRN.Pelvica;
                    this._sumarioavaliacaomedicarn.ApresentacaoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.Apresentacao = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.AOutros);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.ACefalica);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.APelvica);
            }
        }
        public bool AOutros
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.Apresentacao.HasValue)
                    return (this._sumarioavaliacaomedicarn.Apresentacao.Value == ApresentacaoRN.Outros);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.Apresentacao = ApresentacaoRN.Outros;
                else
                    this._sumarioavaliacaomedicarn.Apresentacao = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.APelvica);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.ACefalica);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.AOutros);
            }
        }

        public bool CordaoNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsCordao.HasValue)
                    return (this._sumarioavaliacaomedicarn.IsCordao.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarn.IsCordao = SimNao.Nao;
                    this._sumarioavaliacaomedicarn.CordaoObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarn.IsCordao = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CordaoSim);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CordaoNao);
            }
        }
        public bool CordaoSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsCordao.HasValue)
                    return (this._sumarioavaliacaomedicarn.IsCordao.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.IsCordao = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarn.IsCordao = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CordaoSim);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CordaoNao);
            }
        }

        public bool TANenhuma
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Nenhuma);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Nenhuma;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
            }
        }
        public bool TAGeral
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Geral);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Geral;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
            }
        }
        public bool TAInfiltrativa
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Infiltrativa);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Infiltrativa;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
            }
        }
        public bool TAParacervical
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Paracervical);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Paracervical;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
            }
        }
        public bool TAPeridural
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Peridural);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Peridural;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
            }
        }
        public bool TAPudendos
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Pudendos);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Pudendos;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
            }
        }
        public bool TARaqui
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Raqui);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Raqui;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
            }
        }
        public bool TASedativa
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.TipoAnestesia.HasValue)
                    return (this._sumarioavaliacaomedicarn.TipoAnestesia.Value == TipoAnestesiaRN.Sedativa);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarn.TipoAnestesia = TipoAnestesiaRN.Sedativa;
                else
                    this._sumarioavaliacaomedicarn.TipoAnestesia = null;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TARaqui);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPudendos);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAPeridural);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAParacervical);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAGeral);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TANenhuma);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TAInfiltrativa);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TASedativa);
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionGestacaoAnterior
        {
            get
            {
                if (this._collectiongestacaoanterior.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualGestacaoAnterior();
                    var lista = rep.List();
                    this._collectiongestacaoanterior = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectiongestacaoanterior.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                        });
                    });
                }
                return this._collectiongestacaoanterior;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionGest
        {
            get
            {
                if (this._collectiongest.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualPatologia();
                    var lista = rep.List();
                    this._collectiongest = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();

                    lista.Each(x =>
                    {
                        this._collectiongest.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,

                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false

                        });
                    });
                }
                return this._collectiongest;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionResultados
        {
            get
            {
                if (this._collectionExames.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    var lista = rep.List().Where(x => x.Sorologia == SimNao.Sim);//|| x.Exames == SimNao.Sim;
                    this._collectionExames = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectionExames.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Resultado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                            this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                            this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().ResultadoRN
                            : new Nullable<ResultadoItemRN>()
                            : new Nullable<ResultadoItemRN>(),

                            ResultadoPositivo = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().ResultadoRN == ResultadoItemRN.Positivo ? true
                                                : false : false : false,

                            ResultadoNegativo = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().ResultadoRN == ResultadoItemRN.Negativo ? true
                                                : false : false : false,

                            ResultadoIndisponivel = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().ResultadoRN == ResultadoItemRN.NaoDisponivel ? true
                                                    : false : false : false
                        });
                    });
                }


                return this._collectionExames;
            }
        }

        public IList<RuntimeTab<TabsSumarioObstetrico>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
        }
        public TabsSumarioObstetrico? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TipoTabSelecionada);
            }
        }

        public SimNao? IsPatologia
        {
            get
            {
                return this._sumarioavaliacaomedicarn.IsPatologia;
            }
            set
            {
                if (value.HasValue)
                    if (value.Value == SimNao.Sim)
                        if (this._collectiongest.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                            base.OnPropertyChanged<vmSumarioObstetrico>(x => x.CollectionGest);
                            base.OnPropertyChanged<vmSumarioObstetrico>(x => x.IsPatologia);
                            return;
                        }

                this._sumarioavaliacaomedicarn.IsPatologia = value;
                base.OnPropertyChanged<vmSumarioObstetrico>(x => x.CollectionGest);
                base.OnPropertyChanged<vmSumarioObstetrico>(x => x.IsPatologia);
            }
        }
        public SimNao? IsGestacaoAnterior
        {
            get
            {
                return this._sumarioavaliacaomedicarn.IsGestacaoAnterior;
            }
            set
            {
                if (value.HasValue)
                    if (value.Value == SimNao.Sim)
                        if (this._collectiongestacaoanterior.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                            base.OnPropertyChanged<vmSumarioObstetrico>(x => x.CollectionGestacaoAnterior);
                            base.OnPropertyChanged<vmSumarioObstetrico>(x => x.IsGestacaoAnterior);
                            return;
                        }
                this._sumarioavaliacaomedicarn.IsGestacaoAnterior = value;
                base.OnPropertyChanged<vmSumarioObstetrico>(x => x.CollectionGestacaoAnterior);
                base.OnPropertyChanged<vmSumarioObstetrico>(x => x.IsGestacaoAnterior);
            }
        }
        public bool IsPrimeiraGestacao
        {
            get
            {
                if (this._sumarioavaliacaomedicarn.IsPrimeiraGestacao == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (this._collectiongestacaoanterior.HasItems())
                    {
                        if (this._collectiongestacaoanterior.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                        else
                        {
                            this._sumarioavaliacaomedicarn.IsPrimeiraGestacao = SimNao.Sim;
                            this._sumarioavaliacaomedicarn.GestacaoAnteriorObservacao = string.Empty;
                            this._sumarioavaliacaomedicarn.IsGestacaoAnterior = SimNao.Nao;
                        }
                    }
                    else
                    {
                        this._sumarioavaliacaomedicarn.IsPrimeiraGestacao = SimNao.Sim;
                        this._sumarioavaliacaomedicarn.GestacaoAnteriorObservacao = string.Empty;
                        this._sumarioavaliacaomedicarn.IsGestacaoAnterior = SimNao.Nao;
                    }
                }
                else
                    this._sumarioavaliacaomedicarn.IsPrimeiraGestacao = SimNao.Nao;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.IsGestacaoAnterior);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.CollectionGestacaoAnterior);
                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.IsPrimeiraGestacao);
            }
        }
        #endregion

        #region ----- Métodos Privados -----
        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsSumarioObstetrico>>();

            //IdentificacaoGestacoesAnteriores
            this._tabs.Add(new RuntimeTab<TabsSumarioObstetrico>
            {
                TipoTab = TabsSumarioObstetrico.IdentificacaoGestacoesAnteriores,
                Descricao = TabsSumarioObstetrico.IdentificacaoGestacoesAnteriores.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucGestacoesAnteriores.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //GestacaoAtual
            this._tabs.Add(new RuntimeTab<TabsSumarioObstetrico>
            {
                TipoTab = TabsSumarioObstetrico.GestacaoAtual,
                Descricao = TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucGestacaoAtual.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //ExamesLaboratoriais
            this._tabs.Add(new RuntimeTab<TabsSumarioObstetrico>
            {
                TipoTab = TabsSumarioObstetrico.ExamesLaboratoriais,
                Descricao = TabsSumarioObstetrico.ExamesLaboratoriais.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucExamesLaboratoriais.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //Parto
            this._tabs.Add(new RuntimeTab<TabsSumarioObstetrico>
            {
                TipoTab = TabsSumarioObstetrico.Parto,
                Descricao = TabsSumarioObstetrico.Parto.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucParto.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });
        }
        #endregion

        #region ----- Métodos Públicos -----
        public void ChamaSalvar()
        {
            this._vm.Commands.ExecuteCommand(enumCommand.CommandSalvar, null);
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
