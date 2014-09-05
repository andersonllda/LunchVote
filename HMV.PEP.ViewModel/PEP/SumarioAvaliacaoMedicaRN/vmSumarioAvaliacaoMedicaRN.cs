using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.CentroObstetrico;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.AdmAssistencial;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using StructureMap;
using HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN
{
    public class vmSumarioAvaliacaoMedicaRN : ViewModelBase
    {
        #region Enum
        public enum TabsSumarioAvaliacaoMedicaRN
        {
            [Description("Sumário Obstétrico")]
            SumarioObstetrico,
            [Description("Atendimento ao Recém-Nascido em sala de parto")]
            AtendimentoRecemNascidoSalaParto,
            [Description("Exame Clínico")]
            ExameClinico,
            [Description("Finalizar/Imprimir")]
            FinalizarImprimir
        }
        #endregion

        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaRN(Atendimento pAtendimento, Usuarios pUsuario, bool pIsCorpoClinico)
        {
            this._usuario = new wrpUsuarios(pUsuario);
            this._atendimento = new wrpAtendimento(pAtendimento);
            this._paciente = this._atendimento.Paciente;
            this._iscorpoclinico = pIsCorpoClinico;

            IRepositorioDeSumarioDeAvaliacaoMedicaRN rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
            var ret = rep.OndeCodigoAtendimentoIgual(pAtendimento).Single();
            if (ret.IsNotNull())
                this._sumarioavaliacaomedicarn = new wrpSumarioAvaliacaoMedicaRN(ret);

            if (this._sumarioavaliacaomedicarn.IsNull())
            {
                this._sumarioavaliacaomedicarn = new wrpSumarioAvaliacaoMedicaRN(this._atendimento, this._usuario, this._paciente, DateTime.Now);
                this._sumarioavaliacaomedicarn.Save();
                this.Novo = true;
            }
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsSumarioAvaliacaoMedicaRN>> _tabs;
        private TabsSumarioAvaliacaoMedicaRN _tipotabselecionada;
        private wrpAtendimento _atendimento;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        private bool _iscorpoclinico;
        private wrpSumarioAvaliacaoMedicaRN _sumarioavaliacaomedicarn;
        private bool _nEdicao;

        //VMs
        private vmSumarioObstetrico _vmSumarioObstetrico;
        private vmSumarioAvaliacaoMedicaRNExameClinico _vmSumarioAvaliacaoMedicaRNExameClinico;
        private vmSumarioAvaliacaoMedicaRNRecemNascido _vmSumarioAvaliacaoMedicaRNRecemNascido;
        #endregion

        #region ----- Propriedades Públicas -----
        public vmSumarioObstetrico vmSumarioObstetrico
        {
            get
            {
                if (this._vmSumarioObstetrico.IsNull())
                    this._vmSumarioObstetrico = new vmSumarioObstetrico(this._sumarioavaliacaomedicarn, this);
                return this._vmSumarioObstetrico;
            }
        }

        public vmSumarioAvaliacaoMedicaRNExameClinico vmSumarioAvaliacaoMedicaRNExameClinico
        {
            get
            {
                if (this._vmSumarioAvaliacaoMedicaRNExameClinico.IsNull())
                    this._vmSumarioAvaliacaoMedicaRNExameClinico = new vmSumarioAvaliacaoMedicaRNExameClinico(this._sumarioavaliacaomedicarn, this);
                return this._vmSumarioAvaliacaoMedicaRNExameClinico;
            }
        }

        public vmSumarioAvaliacaoMedicaRNRecemNascido vmSumarioAvaliacaoMedicaRNRecemNascido
        {
            get
            {
                if (this._vmSumarioAvaliacaoMedicaRNRecemNascido.IsNull())
                    this._vmSumarioAvaliacaoMedicaRNRecemNascido = new vmSumarioAvaliacaoMedicaRNRecemNascido(this._sumarioavaliacaomedicarn, this);
                return this._vmSumarioAvaliacaoMedicaRNRecemNascido;
            }
        }

        public bool Novo { get; set; }

        public bool boolImprimir
        {
            get { return this._sumarioavaliacaomedicarn.IsNotNull(); }
        }

        public wrpSumarioAvaliacaoMedicaRN SumarioAvaliacaoMedicaRN
        {
            get
            {
                return this._sumarioavaliacaomedicarn;
            }
            set
            {
                this._sumarioavaliacaomedicarn = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.SumarioAvaliacaoMedicaRN);
            }
        }

        public IList<RuntimeTab<TabsSumarioAvaliacaoMedicaRN>> Tabs
        {
            get
            {
                if (this._tabs.IsNull())
                    this._montatabs();
                return this._tabs;
            }
        }

        public TabsSumarioAvaliacaoMedicaRN? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.MostraFinalizar);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.TipoTabSelecionada);
            }
        }

        public bool MostraMarcaDaguaRelatorio
        {
            get
            {
                return this._sumarioavaliacaomedicarn.DataEncerramento.IsNull();
            }
        }

        public bool MostraRelatorioFinalizado
        {
            get
            {
                return this._sumarioavaliacaomedicarn.DataEncerramento.IsNotNull();
            }
        }

        public bool MostraFinalizar
        {
            get { return this._tipotabselecionada == TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir && this._sumarioavaliacaomedicarn.DataEncerramento.IsNull() && _nEdicao; }
        }

        public bool MostraImprimir
        {
            get { return this._tipotabselecionada == TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir && this._sumarioavaliacaomedicarn.DataEncerramento.IsNotNull() && _nEdicao; }
        }
        #endregion

        #region ----- Métodos Privados -----
        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsSumarioAvaliacaoMedicaRN>>();

            if (this._sumarioavaliacaomedicarn.DataEncerramento.IsNull() && _nEdicao)
            {
                //SumarioObstetrico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaRN>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico,
                    Descricao = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucObstetrico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaRN>(x => x.vmSumarioObstetrico)),
                        Source = this
                    }
                });

                //AtendimentoRecemNascidoSalaParto
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaRN>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto,
                    Descricao = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucRecemNascido.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaRN>(x => x.vmSumarioAvaliacaoMedicaRNRecemNascido)),
                        Source = this
                    }
                });

                //ExameClinico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaRN>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaRN.ExameClinico,
                    Descricao = TabsSumarioAvaliacaoMedicaRN.ExameClinico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucExameClinico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaRN>(x => x.vmSumarioAvaliacaoMedicaRNExameClinico)),
                        Source = this
                    }
                });
            }

            //FinalizarImprimir
            this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaRN>
            {
                TipoTab = TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir,
                Descricao = TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucResumoRN.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });
        }

        private void _salva()
        {
            if (this._vmSumarioObstetrico.IsNotNull())
            {
                if (this._vmSumarioObstetrico.CollectionGestacaoAnterior.HasItems())
                    foreach (var item in this._vmSumarioObstetrico.CollectionGestacaoAnterior)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                                jaexiste.Observacoes = item.Observacao;
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }

                if (this._vmSumarioObstetrico.CollectionGest.HasItems())
                    foreach (var item in this._vmSumarioObstetrico.CollectionGest)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                                jaexiste.Observacoes = item.Observacao;
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }

                if (this._vmSumarioObstetrico.CollectionResultados.HasItems())
                    foreach (var item in this._vmSumarioObstetrico.CollectionResultados)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                            {
                                ResultadoRN = item.Resultado
                            });
                        }
                        else
                        {
                            jaexiste.ResultadoRN = item.Resultado;
                        }
                    }
            }

            if (this._vmSumarioAvaliacaoMedicaRNExameClinico.IsNotNull())
            {
                if (this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionCabecaPescoco.HasItems())
                    foreach (var item in this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionCabecaPescoco)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                            {
                                jaexiste.Observacoes = item.Observacao;
                                jaexiste.IsNormal = item.IsNormal;
                            }
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }

                if (this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionOutros.HasItems())
                    foreach (var item in this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionOutros)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                            {
                                jaexiste.Observacoes = item.Observacao;
                                jaexiste.IsNormal = item.IsNormal;
                            }
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }

                if (this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionCardioVascular.HasItems())
                    foreach (var item in this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionCardioVascular)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                            {
                                jaexiste.Observacoes = item.Observacao;
                                jaexiste.IsNormal = item.IsNormal;
                            }
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }

                if (this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionRespiratorio.HasItems())
                    foreach (var item in this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionRespiratorio)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                            {
                                jaexiste.Observacoes = item.Observacao;
                                jaexiste.IsNormal = item.IsNormal;
                            }
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }

                if (this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionOsteoArticular.HasItems())
                    foreach (var item in this._vmSumarioAvaliacaoMedicaRNExameClinico.CollectionOsteoArticular)
                    {
                        var jaexiste = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Add(new wrpSumarioAvaliacaoMedicaRNItem(this._sumarioavaliacaomedicarn, new wrpItensCO(item.ItemCO))
                                {
                                    Observacoes = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            if (item.Selecionado)
                            {
                                jaexiste.Observacoes = item.Observacao;
                                jaexiste.IsNormal = item.IsNormal;
                            }
                            else
                                this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Remove(jaexiste);
                        }
                    }
            }

            if (this._vmSumarioAvaliacaoMedicaRNRecemNascido.IsNotNull())
            {
                if (this._vmSumarioAvaliacaoMedicaRNRecemNascido.APGARCollection.HasItems())
                {
                    foreach (var item in this._vmSumarioAvaliacaoMedicaRNRecemNascido.APGARCollection)
                    {
                        var jaexiste1 = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Where(x => x.Ordem == item.Ordem && x.Minuto == MinutoApgarRN.Primeiro).SingleOrDefault();
                        var jaexiste5 = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Where(x => x.Ordem == item.Ordem && x.Minuto == MinutoApgarRN.Quinto).SingleOrDefault();
                        var jaexiste10 = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Where(x => x.Ordem == item.Ordem && x.Minuto == MinutoApgarRN.Dessimo).SingleOrDefault();

                        if (jaexiste1.IsNull())
                        {
                            if (item.FirstMinute)
                            {
                                var novo = new wrpSumarioAvaliacaoMedicaRNApgar(this._sumarioavaliacaomedicarn);
                                novo.Ordem = item.Ordem;
                                novo.Minuto = MinutoApgarRN.Primeiro;
                                if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.COR)
                                    novo.Cor = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.ER)
                                    novo.Esforco = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.FC)
                                    novo.FrequenciaCardiaca = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.IR)
                                    novo.Irritabilidade = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.TM)
                                    novo.Tonus = item.PontuacaoValor;
                                if (item.Marcado)
                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Add(novo);
                            }
                        }
                        else
                        {
                            if (jaexiste1.IsNotNull())
                            {
                                if (item.FirstMinute)
                                {
                                    if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.COR)
                                        jaexiste1.Cor = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.ER)
                                        jaexiste1.Esforco = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.FC)
                                        jaexiste1.FrequenciaCardiaca = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.IR)
                                        jaexiste1.Irritabilidade = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.TM)
                                        jaexiste1.Tonus = item.PontuacaoValor;
                                }
                                else
                                {
                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Remove(jaexiste1);
                                    this._sumarioavaliacaomedicarn.DomainObject.SumarioAvaliacaoMedicaRNApgar.Remove(jaexiste1.DomainObject);
                                }
                            }

                        }

                        if (jaexiste5.IsNull())
                        {
                            if (item.FifthMinute)
                            {
                                var novo = new wrpSumarioAvaliacaoMedicaRNApgar(this._sumarioavaliacaomedicarn);
                                novo.Ordem = item.Ordem;
                                novo.Minuto = MinutoApgarRN.Quinto;
                                if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.COR)
                                    novo.Cor = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.ER)
                                    novo.Esforco = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.FC)
                                    novo.FrequenciaCardiaca = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.IR)
                                    novo.Irritabilidade = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.TM)
                                    novo.Tonus = item.PontuacaoValor;
                                if (item.Marcado)
                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Add(novo);
                            }
                        }
                        else
                        {
                            if (jaexiste5.IsNotNull())
                            {
                                if (item.FifthMinute)
                                {
                                    if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.COR)
                                        jaexiste5.Cor = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.ER)
                                        jaexiste5.Esforco = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.FC)
                                        jaexiste5.FrequenciaCardiaca = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.IR)
                                        jaexiste5.Irritabilidade = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.TM)
                                        jaexiste5.Tonus = item.PontuacaoValor;
                                }
                                else
                                {
                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Remove(jaexiste5);
                                    this._sumarioavaliacaomedicarn.DomainObject.SumarioAvaliacaoMedicaRNApgar.Remove(jaexiste5.DomainObject);
                                }
                            }
                        }

                        if (jaexiste10.IsNull())
                        {
                            if (item.TenthtMinute)
                            {
                                var novo = new wrpSumarioAvaliacaoMedicaRNApgar(this._sumarioavaliacaomedicarn);
                                novo.Ordem = item.Ordem;
                                novo.Minuto = MinutoApgarRN.Dessimo;
                                if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.COR)
                                    novo.Cor = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.ER)
                                    novo.Esforco = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.FC)
                                    novo.FrequenciaCardiaca = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.IR)
                                    novo.Irritabilidade = item.PontuacaoValor;
                                else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.TM)
                                    novo.Tonus = item.PontuacaoValor;
                                if (item.Marcado)
                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Add(novo);
                            }
                        }
                        else
                        {
                            if (jaexiste10.IsNotNull())
                            {
                                if (item.TenthtMinute)
                                {
                                    if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.COR)
                                        jaexiste10.Cor = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.ER)
                                        jaexiste10.Esforco = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.FC)
                                        jaexiste10.FrequenciaCardiaca = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.IR)
                                        jaexiste10.Irritabilidade = item.PontuacaoValor;
                                    else if (item.ItemAPGAR == vmSumarioAvaliacaoMedicaRNRecemNascido.ItemAPGAR.TM)
                                        jaexiste10.Tonus = item.PontuacaoValor;
                                }
                                else
                                {
                                    this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNApgar.Remove(jaexiste10);
                                    this._sumarioavaliacaomedicarn.DomainObject.SumarioAvaliacaoMedicaRNApgar.Remove(jaexiste10.DomainObject);
                                }
                            }
                        }
                    }
                }
            }
            this._sumarioavaliacaomedicarn.Paciente.Save();
            this._sumarioavaliacaomedicarn.Save();
        }
        #endregion

        #region ----- Métodos Públicos -----
        public void PodeAbrir()
        {
            this._nEdicao = true;
            if (this._sumarioavaliacaomedicarn.DataEncerramento.IsNull())
                if (this._sumarioavaliacaomedicarn.Usuario.cd_usuario != this._usuario.cd_usuario)
                {
                    if (DXMessageBox.Show("Este sumário foi iniciado pelo Profissional: "
                        + this._sumarioavaliacaomedicarn.Usuario.nm_usuario
                        + ". Deseja editar este sumário ? ", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        this._sumarioavaliacaomedicarn.Usuario = this._usuario;
                    else
                        this._nEdicao = false;
                }
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.MostraFinalizar);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.MostraImprimir);
        }

        public bool Imprimir()
        {
            if (DXMessageBox.Show("Deseja finalizar o Sumário?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;

            bool setou = false;

            List<string> erros = new List<string>();

            if (this._sumarioavaliacaomedicarn.IsGestacaoAnterior.IsNull()
                || ((this._sumarioavaliacaomedicarn.IsGestacaoAnterior == SimNao.Nao || !this._sumarioavaliacaomedicarn.IsGestacaoAnterior.HasValue)
                && (this._sumarioavaliacaomedicarn.IsPrimeiraGestacao == SimNao.Nao || !this._sumarioavaliacaomedicarn.IsPrimeiraGestacao.HasValue)))
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.GestacaoAnterior == SimNao.Sim) == 0 && this._sumarioavaliacaomedicarn.GestacaoAnteriorObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário marcar um dos itens das Anomalias em Gestações Anteriores na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.IdentificacaoGestacoesAnteriores.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.IdentificacaoGestacoesAnteriores;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }

            if (this._sumarioavaliacaomedicarn.IsPatologia.IsNull() || this._sumarioavaliacaomedicarn.IsPatologia == SimNao.Nao)
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.Patologia == SimNao.Sim) == 0 && this._sumarioavaliacaomedicarn.PatologiaObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário marcar um dos itens de Patologias na Gravidez ou informar Outros na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.IdentificacaoGestacoesAnteriores.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }

            if (!this._sumarioavaliacaomedicarn.Gesta.HasValue)
            {
                erros.Add("É necessário preencher o campo Gesta na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.Gesta.Value > 20)
                {
                    erros.Add("É necessário preencher corretamente o campo Gesta na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.Para.HasValue)
            {
                if (this._sumarioavaliacaomedicarn.Para.Value > 20)
                {
                    erros.Add("É necessário preencher corretamente o campo Para na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.Ectopica.HasValue)
            {
                if (this._sumarioavaliacaomedicarn.Ectopica.Value > 20)
                {
                    erros.Add("É necessário preencher corretamente o campo Ectopica na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.Aborto.HasValue)
            {
                if (this._sumarioavaliacaomedicarn.Aborto.Value > 20)
                {
                    erros.Add("É necessário preencher corretamente o campo Aborto na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.IdadeSemanas.HasValue)
            {
                if (this._sumarioavaliacaomedicarn.IdadeSemanas.Value < 4 || this._sumarioavaliacaomedicarn.IdadeSemanas.Value > 42)
                {
                    erros.Add("É necessário preencher corretamente o campo Idade Dias na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.IdadeDias.HasValue)
            {
                if (this._sumarioavaliacaomedicarn.IdadeDias.Value > 6)
                {
                    erros.Add("É necessário preencher corretamente o campo Idade Semanas na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.IsMedicacao.IsNull())
            {
                erros.Add("É necessário preencher o campo Medicações na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.IsMedicacao == SimNao.Sim)
            {
                if (this._sumarioavaliacaomedicarn.MedicacaoObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário preencher o campo Observações das Medicações quando esta estiver marcada 'SIM' na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.Sorologia == SimNao.Sim && x.ItemCO.Exames == SimNao.Sim) > 0)
            {
                erros.Add("É necessário marcar um dos itens de Exames Laboratoriais na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.ExamesLaboratoriais.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.ExamesLaboratoriais;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.TipoParto.IsNull())
            {
                erros.Add("É necessário preencher o campo Tipo de Parto na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.TipoParto == TipoPartoRN.CesarianaUrgencia)
            {
                if (this._sumarioavaliacaomedicarn.MotivoCesarianaUrgencia.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário preencher o campo Motivo para este Tipo de Parto na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.Membrana == Membranas.Rotas)
            {
                if (this._sumarioavaliacaomedicarn.DataMembrana.IsNull())
                {
                    erros.Add("É necessário preencher o campo Data/Hora da Membranas Aminióticas quando esta estiver marcada 'rotas' na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.LiquidoAmniotico.IsNull())
            {
                erros.Add("É necessário preencher o campo Líquido Aminiótico na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.LiquidoAmniotico == LiquidoAmniotico.Outros)
            {
                if (this._sumarioavaliacaomedicarn.LiquidoAmnioticoObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário preencher o campo Observações do Liquido Aminiótico quando esta estiver marcada 'OUTROS' na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.Apresentacao.IsNull())
            {
                erros.Add("É necessário preencher o campo Apresentação na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.Apresentacao == ApresentacaoRN.Outros)
                if (this._sumarioavaliacaomedicarn.ApresentacaoObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário preencher o campo Observações da Apresentação quando esta estiver marcada 'OUTROS' na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }


            if (this._sumarioavaliacaomedicarn.IsCordao.IsNull())
            {
                erros.Add("É necessário preencher o campo Circular cordão na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.IsCordao == SimNao.Sim)
            {
                if (this._sumarioavaliacaomedicarn.CordaoObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário preencher o campo Observações do Circular cordao na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.TipoAnestesia.IsNull())
            {
                erros.Add("É necessário preencher o campo Tipo de Anestesia na aba de " + vmSumarioObstetrico.TabsSumarioObstetrico.Parto.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioObstetrico.TipoTabSelecionada = vmSumarioObstetrico.TabsSumarioObstetrico.Parto;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico;
                    setou = true;
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.DataNascimento.IsNull())
            {
                erros.Add("É necessário preencher o campo Data Nascimento na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Sexo.IsNull())
            {
                erros.Add("É necessário preencher o campo Sexo na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Cor.IsNull())
            {
                erros.Add("É necessário preencher o campo Cor na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Peso.IsNull())
            {
                erros.Add("É necessário preencher o campo Peso na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Peso.Value < 300 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Peso.Value > 8000)
                {
                    erros.Add("É necessário preencher corretamente o campo Peso na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Comprimento.IsNull())
            {
                erros.Add("É necessário preencher o campo Comprimento na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Comprimento.Value < 15 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Comprimento.Value > 70)
                {
                    erros.Add("É necessário preencher corretamente o campo Comprimento na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico.IsNull())
            {
                erros.Add("É necessário preencher o campo Perímetro Cefálico na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico.Value < 12 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.PerimentroCefalico.Value > 55)
                {
                    erros.Add("É necessário preencher corretamente o campo Perímetro Cefálico na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico.IsNull())
            {
                erros.Add("É necessário preencher o campo Perímetro Toráxico na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico.Value < 12 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.PerimetroToracico.Value > 55)
                {
                    erros.Add("É necessário preencher corretamente o campo Perímetro Toráxico na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsUrinou.IsNull())
            {
                erros.Add("É necessário preencher o campo Urinou na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsEvacuou.IsNull())
            {
                erros.Add("É necessário preencher o campo Evacuou na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.Classificacao.IsNull())
            {
                erros.Add("É necessário preencher o campo Classificação na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsNaoRealizadoCapurro.IsNull())
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.CapurroSemanas.IsNull())
                {
                    erros.Add("É necessário preencher o campo Capurro Semanas na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }

                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.CapurroSemanas.IsNotNull() && (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.CapurroSemanas.Value > 43 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.CapurroSemanas < 23))
                {
                    erros.Add("É necessário preencher corretamente o campo Capurro Semanas na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }

                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.CapurroDias.IsNotNull() && this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.CapurroDias.Value > 6)
                {
                    erros.Add("É necessário preencher corretamente o campo Capurro Dias na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacao.IsNull())
            {
                erros.Add("É necessário preencher o campo Reanimação na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacao == SimNao.Sim)
            {
                if ((this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoEntubacao.IsNull() || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoEntubacao == SimNao.Nao)
                    && (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoMassagem.IsNull() || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoMassagem == SimNao.Nao)
                    && (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoOxigenio.IsNull() || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoOxigenio == SimNao.Nao)
                    && (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoVentilacao.IsNull() || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsReanimacaoVentilacao == SimNao.Nao))
                {
                    erros.Add("É necessário preencher o campo Tipos de Reanimação na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsMedicamentos.IsNull())
            {
                erros.Add("É necessário preencher o campo Medicações na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }
            else if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.IsMedicamentos == SimNao.Sim)
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.MedicamentosObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário preencher o campo Observações de Medicações na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro.IsNull())
            {
                erros.Add("É necessário preencher o campo APGAR 1 na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto.IsNull())
            {
                erros.Add("É necessário preencher o campo APGAR 5 na aba de " + TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.AtendimentoRecemNascidoSalaParto;
                    setou = true;
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FC.IsNull())
            {
                erros.Add("É necessário preencher o campo FC na aba de " + TabsSumarioAvaliacaoMedicaRN.ExameClinico.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FC.Value < 30 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FC.Value > 300)
                {
                    erros.Add("É necessário preencher corretamente o campo FC na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FR.IsNull())
            {
                erros.Add("É necessário preencher o campo FR na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FR.Value < 5 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FR.Value > 100)
                {
                    erros.Add("É necessário preencher corretamente o campo FR na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                        setou = true;
                    }
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.TAX.IsNotNull())
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.TAX.Value < 34 || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.TAX.Value > 42)
                {
                    erros.Add("É necessário preencher corretamente o campo TAX na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                        setou = true;
                    }
                }
            }
            else
            {
                erros.Add("É necessário preencher o campo TAX na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.SAT.IsNotNull())
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.SAT.Value > 100)
                {
                    erros.Add("É necessário preencher corretamente o campo SAT na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                        setou = true;
                    }
                }

            if ((this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsFacies.IsNull() || this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsFacies == SimNao.Nao)
                && this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.FaciesObservacao.IsEmptyOrWhiteSpace())
            {
                erros.Add("É necessário preencher o campo Aspecto Geral na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.Atividade.IsNull())
            {
                erros.Add("É necessário preencher o campo Atividade na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            bool temcor = false;

            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianose.IsNotNull()
                && this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianose == SimNao.Sim)
                temcor = true;
            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianoseGeneralizada.IsNotNull()
                && this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianoseGeneralizada == SimNao.Sim)
                temcor = true;
            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorPalida.IsNotNull()
                && this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorPalida == SimNao.Sim)
                temcor = true;
            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorRosada.IsNotNull()
                && this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsCorRosada == SimNao.Sim)
                temcor = true;

            if (!temcor)
            {
                erros.Add("É necessário preencher um dos campos Cor na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
            rep.FiltraAtivos();
            rep.OndeItemIgualCabecaPescoco();
            if (rep.List().Count != this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.CabecaPescoco == SimNao.Sim))
            {
                erros.Add("É necessário preencher todos os itens Cabeça e Pescoço na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            if (!this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.Pele.HasValue)
            {
                erros.Add("É necessário informar a Pele na aba de " + TabsSumarioAvaliacaoMedicaRN.ExameClinico.GetEnumDescription() + ".");
                if (!setou)
                {
                    this.vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.Pele == Pele.Alteracoes)
                    if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.PeleOutros.IsEmptyOrWhiteSpace())
                    {
                        erros.Add("É necessário informar as alterações da Pele na aba de " + TabsSumarioAvaliacaoMedicaRN.ExameClinico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte1;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                            setou = true;
                        }
                    }
            }

            rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
            rep.FiltraAtivos();
            rep.OndeItemIgualRespiratorio();
            if (rep.List().Count != this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.Respiratorio == SimNao.Sim))
            {
                erros.Add("É necessário preencher todos os itens Respiratório na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte2.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte2;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
            rep.FiltraAtivos();
            rep.OndeItemIgualCardio();
            if (rep.List().Count != this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.Cardio == SimNao.Sim))
            {
                erros.Add("É necessário preencher todos os itens Cardio Vascular na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte2.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte2;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
            rep.FiltraAtivos();
            rep.OndeItemIgualOutros();
            if (rep.List().Count != this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.Outros == SimNao.Sim))
            {
                erros.Add("É necessário preencher todos os itens Outros na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte2.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte2;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
            rep.FiltraAtivos();
            rep.OndeItemIgualOsteoArticular();
            if (rep.List().Count != this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Count(x => x.ItemCO.OsteoArticular == SimNao.Sim))
            {
                erros.Add("É necessário preencher todos os itens Osteoarticular na aba de " + vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte3.GetEnumDescription() + ".");
                if (!setou)
                {
                    vmSumarioAvaliacaoMedicaRNExameClinico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaRNExameClinico.TabsExameClinico.Parte3;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaRN.ExameClinico;
                    setou = true;
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                this._sumarioavaliacaomedicarn.DataEncerramento = DateTime.Now;
                this._tabs = null;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.MostraFinalizar);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.MostraImprimir);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRN>(x => x.MostraMarcaDaguaRelatorio);
            }

            this._salva();
            DXMessageBox.Show("Finalizado com sucesso!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        #endregion

        #region ----- Commands -----
        protected override void CommandSalvar(object param)
        {
            this._salva();
        }
        #endregion

        #region ----- Classes -----
        public class Item : NotifyPropertyChanged
        {
            public ItensCO ItemCO { get; set; }
            public ResultadoItemRN? Resultado { get; set; }
            public SimNao? IsNormal { get; set; }
            private bool _selecionado { get; set; }
            private string _observacao { get; set; }

            public string Observacao
            {
                get
                {
                    return this._observacao;
                }
                set
                {
                    this._observacao = value;
                    if (value.IsNotEmptyOrWhiteSpace())
                    {                        
                        this.Normal = false;
                        this._selecionado = true;
                    }
                    else
                        this._selecionado = false;
                    base.OnPropertyChanged<Item>(x => x.Normal);
                    base.OnPropertyChanged<Item>(x => x.Selecionado);
                    base.OnPropertyChanged<Item>(x => x.Observacao);
                }
            }

            public bool Selecionado
            {
                get
                {
                    return this._selecionado;
                }

                set
                {
                    if (!value)
                        _observacao = string.Empty;

                    this._selecionado = value;
                    base.OnPropertyChanged<Item>(x => x.Selecionado);
                }
            }

            public bool ResultadoPositivo
            {
                get
                {
                    return (Resultado == ResultadoItemRN.Positivo);
                }
                set
                {
                    if (value)
                        Resultado = ResultadoItemRN.Positivo;
                    base.OnPropertyChanged<Item>(x => x.ResultadoPositivo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoNegativo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoIndisponivel);
                }
            }
            public bool ResultadoNegativo
            {
                get
                {
                    return (Resultado == ResultadoItemRN.Negativo);
                }
                set
                {
                    if (value)
                        Resultado = ResultadoItemRN.Negativo;
                    base.OnPropertyChanged<Item>(x => x.ResultadoPositivo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoNegativo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoIndisponivel);
                }
            }
            public bool ResultadoIndisponivel
            {
                get
                {
                    return (Resultado == ResultadoItemRN.NaoDisponivel);
                }
                set
                {
                    if (value)
                        Resultado = ResultadoItemRN.NaoDisponivel;
                    base.OnPropertyChanged<Item>(x => x.ResultadoPositivo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoNegativo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoIndisponivel);
                }
            }
            public bool Normal
            {
                get
                {
                    if (this.IsNormal.HasValue)
                        return (this.IsNormal.Value == SimNao.Sim);
                    return false;
                }
                set
                {
                    if (value)
                    {
                        this.IsNormal = SimNao.Sim;
                        this._selecionado = true;
                        this._observacao = string.Empty;
                    }
                    else
                    {
                        this.IsNormal = SimNao.Nao;
                        this._selecionado = false;
                    }
                    base.OnPropertyChanged<Item>(x => x.Observacao);
                    base.OnPropertyChanged<Item>(x => x.Selecionado);
                    base.OnPropertyChanged<Item>(x => x.Normal);
                }
            }
        }
        #endregion
    }
}
