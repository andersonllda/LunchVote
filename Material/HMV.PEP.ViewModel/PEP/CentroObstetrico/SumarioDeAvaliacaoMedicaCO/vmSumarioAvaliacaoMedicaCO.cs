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
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.AdmAssistencial;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using StructureMap;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Repository;
using HMV.PEP.Consult;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmSumarioAvaliacaoMedicaCO : ViewModelBase
    {
        #region Enum
        public enum TabsSumarioAvaliacaoMedicaCO
        {
            [Description("Anamnese")]
            Anamnese,
            [Description("Exames Realizados")]
            ExamesRealizados,
            [Description("Exame Físico")]
            ExameFisico,
            [Description("Diagnósticos / Hipóteses Diagnósticas")]
            DiagnosticosHipotesesDiagnosticas,
            [Description("Plano Diagnóstico e Terapêutico")]
            PlanoDiagnosticoTerapeutico,
            [Description("Finalizar/Imprimir")]
            FinalizarImprimir
        }
        #endregion

        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaCO(Atendimento pAtendimento, Usuarios pUsuario, bool pIsCorpoClinico)
        {
            this._usuario = new wrpUsuarios(pUsuario);

            this._atendimento = new wrpAtendimento(pAtendimento);
            this._paciente = this._atendimento.Paciente;
            this._iscorpoclinico = pIsCorpoClinico;

            IRepositorioDeSumarioAvaliacaoMedicaCO rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCO>();
            var ret = rep.OndeCodigoAtendimentoIgual(pAtendimento).Single();
            if (ret.IsNotNull())
                this._sumarioavaliacaomedicaco = new wrpSumarioAvaliacaoMedicaCO(ret);

            //VALIDA SE O USUARIO É MEDICO OU ENFERMEIRA            
            if (this._usuario.IsNotNull() && this._usuario.Prestador.IsNotNull() && !this._usuario.Prestador.IsNurse)
            {
                IRepositorioDeEventoSumarioAvaliacaoMedicaCO repe = ObjectFactory.GetInstance<IRepositorioDeEventoSumarioAvaliacaoMedicaCO>();
                this._evento = new wrpEventoSumarioAvaliacaoMedicaCO(repe.Single());

                if (this._sumarioavaliacaomedicaco.IsNull())
                {
                    this._sumarioavaliacaomedicaco = new wrpSumarioAvaliacaoMedicaCO(this._atendimento, this._usuario, this._paciente, DateTime.Now);
                    this._sumarioavaliacaomedicaco.Save();
                    this._copiardadosultimaadmissao();
                    this.Novo = true;
                }
            }
        }

        private void _copiardadosultimaadmissao()
        {
            wrpAdmissaoAssistencialCO ultadm = null;

            IRepositorioDeAdmissaoAssistencialCO rep = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCO>();
            var ret = rep.OndeCodigoAtendimentoIgual(this._atendimento.DomainObject);
            if (ret.List().Count(x => x.DataEncerramento.IsNotNull() && x.DataExclusao.IsNull()) > 0)
            {
                ultadm = new wrpAdmissaoAssistencialCO(ret.List().Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.DataEncerramento).FirstOrDefault());
                rep.Refresh(ultadm.DomainObject);
            }


            if (ultadm.IsNotNull())
            {
                if (ultadm.Admissao.IsNotNull())
                {
                    //this._sumarioavaliacaomedicaco.MotivoInternacao = ultadm.Admissao.MotivoInternacao;
                    //this._sumarioavaliacaomedicaco.HistoriaAtual = ultadm.Admissao.HistoriaAtual;
                    this._sumarioavaliacaomedicaco.Procedencia = ultadm.Admissao.Procedencia;
                    this._sumarioavaliacaomedicaco.ProcedenciaOutros = ultadm.Admissao.Outros;
                    this._sumarioavaliacaomedicaco.UltimaMenstruacao = ultadm.Admissao.UltimaMenstruacao;
                    this._sumarioavaliacaomedicaco.TipagemPaciente = ultadm.Admissao.TipagemPaciente;
                    this._sumarioavaliacaomedicaco.RHPaciente = ultadm.Admissao.RHPaciente;
                    this._sumarioavaliacaomedicaco.IdadeDesconhecida = ultadm.Admissao.IdadeGestacional.Desconhecido;
                    this._sumarioavaliacaomedicaco.IdadeSemana = ultadm.Admissao.IdadeGestacional.IdadeSemana;
                    this._sumarioavaliacaomedicaco.IdadeDias = ultadm.Admissao.IdadeGestacional.IdadeDia;
                    if (ultadm.Admissao.Eco.HasValue)
                        this._sumarioavaliacaomedicaco.EcografiaData = ultadm.Admissao.Eco.Value;

                    if (ultadm.GestacaoAtual != null)
                    {
                        this._sumarioavaliacaomedicaco.ExamesRealizadosObservacao = ultadm.GestacaoAtual.AchadosClinicos != null ? ultadm.GestacaoAtual.AchadosClinicos.Outros : "";
                        this._sumarioavaliacaomedicaco.PatologiaObservacao = ultadm.GestacaoAtual.Patologias != null ? ultadm.GestacaoAtual.Patologias.Outros : "";
                    }

                    if (ultadm.HistoriaPregressa != null)
                    {
                        this._sumarioavaliacaomedicaco.GestacaoAnteriorObservacao = ultadm.HistoriaPregressa.GestacaoAnterior != null ? ultadm.HistoriaPregressa.GestacaoAnterior.Outros : "";
                        this._sumarioavaliacaomedicaco.PatologiaPreviaObservacao = ultadm.HistoriaPregressa.DoencasPrevias != null ? ultadm.HistoriaPregressa.DoencasPrevias.Outros : "";
                    }

                    this._sumarioavaliacaomedicaco.PerfilObservacao = ultadm.AvaliacaoSocioEconomica != null ? ultadm.AvaliacaoSocioEconomica.Outros : "";

                    if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.IsNull())
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico = new wrpSumarioAvaliacaoMedicaCOExameFisico(this._sumarioavaliacaomedicaco);

                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.PAAlta = ultadm.Admissao.PAAlta;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.PABaixa = ultadm.Admissao.PABaixa;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.TAX = ultadm.Admissao.Tax;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.FC = ultadm.Admissao.FC;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.FR = ultadm.Admissao.FR;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.DOR = ultadm.Admissao.Dor;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.BCF = ultadm.Admissao.BCF;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.Peso = ultadm.Admissao.Peso;
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.Altura = ultadm.Admissao.Altura;

                    if (ultadm.GestacaoAtual.IsNotNull() && ultadm.GestacaoAtual.AchadosClinicos.IsNotNull())
                    {
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.Dinamica = ultadm.GestacaoAtual.AchadosClinicos.Dinamica;
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.Membrana = ultadm.GestacaoAtual.AchadosClinicos.DomainObject.MembranasAmnioticas;
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.DataMembrana = ultadm.GestacaoAtual.AchadosClinicos.DataMembranas;
                        this._sumarioavaliacaomedicaco.Gesta = ultadm.GestacaoAtual.AchadosClinicos.Gestacao.Gesta;
                        this._sumarioavaliacaomedicaco.Para = ultadm.GestacaoAtual.AchadosClinicos.Gestacao.Para;
                        this._sumarioavaliacaomedicaco.Aborto = ultadm.GestacaoAtual.AchadosClinicos.Gestacao.Aborto;
                        this._sumarioavaliacaomedicaco.Cesarea = ultadm.GestacaoAtual.AchadosClinicos.Gestacao.Cesarea;
                        this._sumarioavaliacaomedicaco.Ectopica = ultadm.GestacaoAtual.AchadosClinicos.Gestacao.Ectopica;
                        this._sumarioavaliacaomedicaco.GravidezMultipla = ultadm.GestacaoAtual.AchadosClinicos.GravidezMultipla;
                    }
                }

                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Clear();

                if (ultadm.ItensPatologia.HasItems())
                    foreach (var item in ultadm.ItensPatologia)
                    {
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(
                            new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, item.Item)
                            {
                                Observacao = item.Observacoes
                            });
                    }
                else
                    this._sumarioavaliacaomedicaco.IsPatologia = SimNao.Sim;


                if (ultadm.ItensGestacaoAnterior.HasItems())
                    foreach (var item in ultadm.ItensGestacaoAnterior)
                    {
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(
                            new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, item.Item)
                            {
                                Observacao = item.Observacoes
                            });
                    }
                else
                {
                    this._sumarioavaliacaomedicaco.IsGestacaoAnterior = ultadm.HistoriaPregressa.GestacaoAnterior.Intercorrencias;
                    this._sumarioavaliacaomedicaco.IsPrimeiraGestacao = ultadm.HistoriaPregressa.GestacaoAnterior.IsPrimeiraGestacao;
                }

                if (ultadm.ItensDoencasPrevias.HasItems())
                    foreach (var item in ultadm.ItensDoencasPrevias)
                    {
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(
                            new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, item.Item)
                            {
                                Observacao = item.Observacoes
                            });
                    }
                else
                    this._sumarioavaliacaomedicaco.NegaPatologiaPrevia = SimNao.Sim;

                if (ultadm.ItensHabitoDeVida.HasItems())
                    foreach (var item in ultadm.ItensHabitoDeVida)
                    {
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(
                            new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, item.Item)
                            {
                                Observacao = item.Observacoes
                            });
                    }

                if (ultadm.ItensSorologia.HasItems())
                    foreach (var item in ultadm.ItensSorologia)
                    {
                        var novo = new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, item.Item);

                        if (item.Resposta == SimNaoNA.Sim)
                            novo.Resultado = ResultadoItemCO.Positivo;
                        else if (item.Resposta == SimNaoNA.Nao)
                            novo.Resultado = ResultadoItemCO.Negativo;
                        else
                            novo.Resultado = ResultadoItemCO.NaoDisponivel;
                        this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(novo);
                    }


                if (ultadm.Alergias.HasItems())
                    foreach (var ale in ultadm.Alergias)
                    {
                        this._evento.AlergiaEventos.Add(new wrpAlergiaEvento
                        {
                            Alergia = ale.Alergia,
                            Chave = this._sumarioavaliacaomedicaco.Id,
                            Atendimento = this._atendimento,
                            Evento = new wrpEvento(this._evento.DomainObject),
                            Data = DateTime.Now,
                            Usuario = this._usuario
                        });
                    }

                if (ultadm.MedicamentosEmUso.HasItems())
                {
                    foreach (var ale in ultadm.MedicamentosEmUso)
                    {
                        var eventoMed = new wrpMedicamentoEmUsoEvento
                        {
                            MedicamentosEmUso = ale.MedicamentosEmUso,
                            Chave = this._sumarioavaliacaomedicaco.Id,
                            Atendimento = this._atendimento,
                            Evento = new wrpEvento(this._evento.DomainObject),
                            Data = DateTime.Now,
                            Usuario = this._usuario
                        };

                        IRepositorioDeEventoMedicamentosEmUso repEventoMedUso = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                        repEventoMedUso.Save(eventoMed.DomainObject);
                    }


                    /*foreach (var ale in ultadm.MedicamentosEmUso)
                    {
                        this._evento.MedicamentosEmUsoEventos.Add(new wrpMedicamentoEmUsoEvento
                        {
                            MedicamentosEmUso = ale.MedicamentosEmUso,
                            Chave = this._sumarioavaliacaomedicaco.Id,
                            Atendimento = this._atendimento,
                            Evento = new wrpEvento(this._evento.DomainObject),
                            Data = DateTime.Now,
                            Usuario = this._usuario
                        });
                    }*/
                }
            }

        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsSumarioAvaliacaoMedicaCO>> _tabs;
        private TabsSumarioAvaliacaoMedicaCO _tipotabselecionada;
        private wrpAtendimento _atendimento;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        private bool _iscorpoclinico;
        private bool _tabatualdiagnosticohipotese = false;

        private wrpSumarioAvaliacaoMedicaCO _sumarioavaliacaomedicaco;
        private wrpEventoSumarioAvaliacaoMedicaCO _evento;

        //VMs
        private vmAnamneseSumarioAvaliacaoMedicaCO _vmanamnesesumarioavaliacaoco;
        private vmExamesRealizadosSumarioAvaliacaoMedicaCO _vmexamesrealizadossumarioavaliacaomedicaco;
        private vmExameFisicoSumarioAvaliacaoMedicaCO _vmexamefisicosumarioavaliacaomedicaco;
        private vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO _vmdiagnosticohipotesesumarioavaliacaomedicaco;
        private vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO _vmplanodiagnosticosumarioavaliacaomedicaco;
        #endregion

        #region ----- Propriedades Públicas -----
        public bool Novo { get; set; }

        public bool SalvaDiagnosticoHipotese { get { return _tabatualdiagnosticohipotese; } set { _tabatualdiagnosticohipotese = value; } }      

        public bool boolImprimir
        {
            get { return this.SumarioAvaliacaoMedicaCO.IsNotNull(); }
        }

        public wrpSumarioAvaliacaoMedicaCO SumarioAvaliacaoMedicaCO
        {
            get
            {
                return this._sumarioavaliacaomedicaco;
            }
            set
            {
                this._sumarioavaliacaomedicaco = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.SumarioAvaliacaoMedicaCO);
            }
        }

        public vmAnamneseSumarioAvaliacaoMedicaCO vmAnamneseSumarioAvaliacaoMedicaCO
        {
            get
            {
                if (this._vmanamnesesumarioavaliacaoco.IsNull())
                    this._vmanamnesesumarioavaliacaoco = new vmAnamneseSumarioAvaliacaoMedicaCO(this._sumarioavaliacaomedicaco, this._paciente, this._usuario, this._iscorpoclinico, this);

                return this._vmanamnesesumarioavaliacaoco;

            }
        }

        public vmExamesRealizadosSumarioAvaliacaoMedicaCO vmExamesRealizadosSumarioAvaliacaoMedicaCO
        {
            get
            {
                if (this._vmexamesrealizadossumarioavaliacaomedicaco.IsNull())
                    this._vmexamesrealizadossumarioavaliacaomedicaco = new vmExamesRealizadosSumarioAvaliacaoMedicaCO(this._sumarioavaliacaomedicaco);

                return this._vmexamesrealizadossumarioavaliacaomedicaco;
            }
        }

        public vmExameFisicoSumarioAvaliacaoMedicaCO vmExameFisicoSumarioAvaliacaoMedicaCO
        {
            get
            {
                if (this._vmexamefisicosumarioavaliacaomedicaco.IsNull())
                    this._vmexamefisicosumarioavaliacaomedicaco = new vmExameFisicoSumarioAvaliacaoMedicaCO(this._sumarioavaliacaomedicaco);

                return this._vmexamefisicosumarioavaliacaomedicaco;
            }
        }

        public vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO
        {
            get
            {
                if (this._vmdiagnosticohipotesesumarioavaliacaomedicaco.IsNull())
                    this._vmdiagnosticohipotesesumarioavaliacaomedicaco = new vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO(this._sumarioavaliacaomedicaco);

                return this._vmdiagnosticohipotesesumarioavaliacaomedicaco;

            }
        }

        public vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO
        {
            get
            {
                if (this._vmplanodiagnosticosumarioavaliacaomedicaco.IsNull())
                    this._vmplanodiagnosticosumarioavaliacaomedicaco = new vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO(this._sumarioavaliacaomedicaco);


                return this._vmplanodiagnosticosumarioavaliacaomedicaco;
            }
        }

        public IList<RuntimeTab<TabsSumarioAvaliacaoMedicaCO>> Tabs
        {
            get
            {
                if (this._tabs.IsNull())
                    this._montatabs();
                return this._tabs;
            }
        }

        public TabsSumarioAvaliacaoMedicaCO? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.MostraFinalizar);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.TipoTabSelecionada);
            }
        }

        public bool MostraMarcaDaguaRelatorio
        {
            get
            {
                return this.SumarioAvaliacaoMedicaCO.DataEncerramento.IsNull();
            }
        }

        public bool MostraRelatorioFinalizado
        {
            get
            {
                return this.SumarioAvaliacaoMedicaCO.DataEncerramento.IsNotNull();
            }
        }

        public bool MostraFinalizar
        {
            get { return _mostrabotao && this._tipotabselecionada == TabsSumarioAvaliacaoMedicaCO.FinalizarImprimir && this._sumarioavaliacaomedicaco.DataEncerramento.IsNull(); }
        }

        public bool MostraImprimir
        {
            get { return _mostrabotao && this._tipotabselecionada == TabsSumarioAvaliacaoMedicaCO.FinalizarImprimir && this._sumarioavaliacaomedicaco.DataEncerramento.IsNotNull(); }
        }
        #endregion

        bool _mostrabotao;

        #region ----- Métodos Privados -----

        private bool PodeAbrirSumario()
        {
            if (this._usuario.IsNotNull() && this._usuario.Prestador.IsNotNull() && !this._usuario.Prestador.IsNurse)
            {
                if (this._sumarioavaliacaomedicaco.Usuario.cd_usuario != this._usuario.cd_usuario)
                {
                    if (DXMessageBox.Show("Este sumário foi iniciado pelo Profissional: "
                        + this._sumarioavaliacaomedicaco.Usuario.nm_usuario
                        + ". Deseja editar este sumário ? ", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this._sumarioavaliacaomedicaco.Usuario = this._usuario;
                        _mostrabotao = true;
                        return true;
                    }
                    else
                    {
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.MostraFinalizar);
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.MostraImprimir);
                        return false;
                    }
                }
                else
                    _mostrabotao = true;
                return true;
            }
            else
                return false;
        }

        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsSumarioAvaliacaoMedicaCO>>();

            if (this._sumarioavaliacaomedicaco.DataEncerramento.IsNull() && PodeAbrirSumario())
            {
                //Anamnese
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCO.Anamnese,
                    Descricao = TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucAnamnese.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCO>(x => x.vmAnamneseSumarioAvaliacaoMedicaCO)),
                        Source = this
                    }
                });

                //ExamesRealizados
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCO.ExamesRealizados,
                    Descricao = TabsSumarioAvaliacaoMedicaCO.ExamesRealizados.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucExamesRealizados.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCO>(x => x.vmExamesRealizadosSumarioAvaliacaoMedicaCO)),
                        Source = this
                    }
                });

                //ExameFisico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCO.ExameFisico,
                    Descricao = TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucExameFisico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCO>(x => x.vmExameFisicoSumarioAvaliacaoMedicaCO)),
                        Source = this
                    }
                });

                //DiagnosticosHipotesesDiagnosticas
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCO.DiagnosticosHipotesesDiagnosticas,
                    Descricao = TabsSumarioAvaliacaoMedicaCO.DiagnosticosHipotesesDiagnosticas.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucDiagnosticoHipotese.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCO>(x => x.vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO)),
                        Source = this
                    }
                });

                //PlanoDiagnosticoTerapeutico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCO.PlanoDiagnosticoTerapeutico,
                    Descricao = TabsSumarioAvaliacaoMedicaCO.PlanoDiagnosticoTerapeutico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucPlanoDiagnostico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCO>(x => x.vmPlanoDiagnosticoSumarioAvaliacaoMedicaCO)),
                        Source = this
                    }
                });
            }

            //FinalizarImprimir
            this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsSumarioAvaliacaoMedicaCO.FinalizarImprimir,
                Descricao = TabsSumarioAvaliacaoMedicaCO.FinalizarImprimir.GetEnumDescription(),
                Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucResumo.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });
        }
        private void _salva()
        {
            if (this.SalvaDiagnosticoHipotese)
                if (this._vmdiagnosticohipotesesumarioavaliacaomedicaco.IsNotNull() && this._vmdiagnosticohipotesesumarioavaliacaomedicaco.CID.IsNotNull())
                    ObjectFactory.GetInstance<HMV.PEP.Interfaces.ICidService>().verificaSeOCIDJaEstaNaListaDeProblemas(this._vmdiagnosticohipotesesumarioavaliacaomedicaco.CID.DomainObject, this._vmdiagnosticohipotesesumarioavaliacaomedicaco.Atendimento, this._usuario.DomainObject);

            if (this._vmanamnesesumarioavaliacaoco.IsNotNull())
            {
                if (this._vmanamnesesumarioavaliacaoco.CollectionPerfilPsicoSocial.HasItems())
                    foreach (var item in this._vmanamnesesumarioavaliacaoco.CollectionPerfilPsicoSocial)
                    {
                        var jaexiste = this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao
                                });
                            //this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.AdicionaDomain(new SumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco.DomainObject, item.ItemCO)
                            //{
                            //    Observacao = item.Observacao
                            //});
                        }
                        else
                        {
                            if (item.Selecionado)
                                jaexiste.Observacao = item.Observacao;
                            else
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Remove(jaexiste);
                        }
                    }

                if (this._vmanamnesesumarioavaliacaoco.CollectionHistoriaPregressa.HasItems())
                    foreach (var item in this._vmanamnesesumarioavaliacaoco.CollectionHistoriaPregressa)
                    {
                        var jaexiste = this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao
                                });
                            //this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.AdicionaDomain(new SumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco.DomainObject, item.ItemCO)
                            //{
                            //    Observacao = item.Observacao
                            //});
                        }
                        else
                        {
                            if (item.Selecionado)
                                jaexiste.Observacao = item.Observacao;
                            else
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Remove(jaexiste);
                        }
                    }

                if (this._vmanamnesesumarioavaliacaoco.CollectionGestacaoAnterior.HasItems())
                    foreach (var item in this._vmanamnesesumarioavaliacaoco.CollectionGestacaoAnterior)
                    {
                        var jaexiste = this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao
                                });
                            //this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.AdicionaDomain(new SumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco.DomainObject, item.ItemCO)
                            //{
                            //    Observacao = item.Observacao
                            //});
                        }
                        else
                        {
                            if (item.Selecionado)
                                jaexiste.Observacao = item.Observacao;
                            else
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Remove(jaexiste);
                        }
                    }

                if (this._vmanamnesesumarioavaliacaoco.CollectionGest.HasItems())
                    foreach (var item in this._vmanamnesesumarioavaliacaoco.CollectionGest)
                    {
                        var jaexiste = this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            if (item.Selecionado)
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao
                                });
                            //this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.AdicionaDomain(new SumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco.DomainObject, item.ItemCO)
                            //{
                            //    Observacao = item.Observacao
                            //});
                        }
                        else
                        {
                            if (item.Selecionado)
                                jaexiste.Observacao = item.Observacao;
                            else
                                this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Remove(jaexiste);
                        }
                    }
            }

            if (this._vmexamesrealizadossumarioavaliacaomedicaco.IsNotNull())
                if (this._vmexamesrealizadossumarioavaliacaomedicaco.CollectionResultados.HasItems())
                    foreach (var item in this._vmexamesrealizadossumarioavaliacaomedicaco.CollectionResultados)
                    {
                        var jaexiste = this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).SingleOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.Add(new wrpSumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco, new wrpItensCO(item.ItemCO))
                            {
                                IsTrimestre1 = item.IsTrimestre1,
                                IsTrimestre2 = item.IsTrimestre2,
                                IsTrimestre3 = item.IsTrimestre3,
                                Resultado = item.Resultado
                            });
                            //this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOItens.AdicionaDomain(new SumarioAvaliacaoMedicaCOItem(this._sumarioavaliacaomedicaco.DomainObject, item.ItemCO)
                            //{
                            //    IsTrimestre1 = item.IsTrimestre1,
                            //    IsTrimestre2 = item.IsTrimestre2,
                            //    IsTrimestre3 = item.IsTrimestre3,
                            //    Resultado = item.Resultado
                            //});
                        }
                        else
                        {
                            jaexiste.IsTrimestre1 = item.IsTrimestre1;
                            jaexiste.IsTrimestre2 = item.IsTrimestre2;
                            jaexiste.IsTrimestre3 = item.IsTrimestre3;
                            jaexiste.Resultado = item.Resultado;
                        }
                    }

            this._sumarioavaliacaomedicaco.Paciente.Save();
            this._sumarioavaliacaomedicaco.Save();

            //Salva AlergiasEvento
            if (this._vmanamnesesumarioavaliacaoco.IsNotNull())
            {
                this._vmanamnesesumarioavaliacaoco.vmAlergiasEvento.SalvarNovo();
                //foreach (var ale in this._vmanamnesesumarioavaliacaoco.vmAlergiasEvento.AlergiaCollection)
                //{
                //    var jaexiste = this._evento.AlergiaEventos.Where(x => x.Chave == this._sumarioavaliacaomedicaco.Id && x.Alergia.ID == ale.ID).SingleOrDefault();
                //    if (jaexiste.IsNull())
                //    {
                //        if (ale.Selecionado)
                //            this._evento.AlergiaEventos.Add(new wrpAlergiaEvento
                //            {
                //                Alergia = ale,
                //                Chave = this._sumarioavaliacaomedicaco.Id,
                //                Atendimento = this._atendimento,
                //                Evento = new wrpEvento(this._evento.DomainObject),
                //                Data = DateTime.Now,
                //                Usuario = this._usuario
                //            });
                //    }
                //    else
                //    {
                //        if (ale.Selecionado)
                //        {
                //            jaexiste.Data = DateTime.Now;
                //            jaexiste.Usuario = this._usuario;
                //        }
                //        else
                //            this._evento.AlergiaEventos.Remove(jaexiste);
                //    }
                //}

                //Salva MedicamentosEmUsoEvento  
                this._vmanamnesesumarioavaliacaoco.vmMedicamentosEmUsoEvento.SalvarNovo();

                /*foreach (var med in this._vmanamnesesumarioavaliacaoco.vmMedicamentosEmUsoEvento.MedicamentosCollection)
                {
                    var jaexiste = this._evento.MedicamentosEmUsoEventos.Where(x => x.Chave == this._sumarioavaliacaomedicaco.Id && x.MedicamentosEmUso.ID == med.ID).SingleOrDefault();
                    if (jaexiste.IsNull())
                    {
                        if (med.Selecionado)
                        {
                            jaexiste = new wrpMedicamentoEmUsoEvento
                            {
                                MedicamentosEmUso = med,
                                Chave = this._sumarioavaliacaomedicaco.Id,
                                Atendimento = this._atendimento,
                                Evento = new wrpEvento(this._evento.DomainObject),
                                Data = DateTime.Now,
                                Usuario = this._usuario
                            };
                            
                            this._evento.MedicamentosEmUsoEventos.Add(jaexiste);

                            IRepositorioDeEventoMedicamentosEmUso rep = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                            rep.Save(jaexiste.DomainObject);
                        }
                    }
                    else
                    {
                        if (med.Selecionado)
                        {
                            jaexiste.Data = DateTime.Now;
                            jaexiste.Usuario = this._usuario;
                            IRepositorioDeEventoMedicamentosEmUso rep = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                            rep.Save(jaexiste.DomainObject);
                        }
                        else
                        {
                            IRepositorioDeEventoMedicamentosEmUso rep = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                            rep.Delete(jaexiste.DomainObject);
                            //this._evento.MedicamentosEmUsoEventos.Remove(jaexiste);
                        }
                    }

                }*/
            }
            //if (this._evento.IsNotNull())
            //{

                //this._evento.Save();
            //}
        }
        #endregion

        #region ----- Métodos Públicos -----
        public bool Imprimir()
        {
            if (DXMessageBox.Show("Deseja finalizar o Sumário?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;

            List<string> erros = new List<string>();

            bool setou = false;

            if (!this._sumarioavaliacaomedicaco.IsValid)
            {
                erros.Add("É necessário preencher os dados corretamente na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                    setou = true;
                }
            }

            if (this.vmAnamneseSumarioAvaliacaoMedicaCO.IsNotNull())
                if (!this.vmAnamneseSumarioAvaliacaoMedicaCO.IsValid)
                {
                    erros.Add("É necessário preencher os dados corretamente na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                        setou = true;
                    }
                }

            if (this._sumarioavaliacaomedicaco.MotivoInternacao.IsEmptyOrWhiteSpace())
            {
                erros.Add("É necessário informar o Motivo de Internação na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                if (!setou)
                {
                    this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.MotivoInternacao;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicaco.HistoriaAtual.IsEmptyOrWhiteSpace())
            {
                erros.Add("É necessário informar a História Atual na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                if (!setou)
                {
                    this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.MotivoInternacao;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicaco.Procedencia.IsNull())
            {
                erros.Add("É necessário informar a Procedência na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                if (!setou)
                {
                    this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                    setou = true;
                }
            }
            else
                if (this._sumarioavaliacaomedicaco.Procedencia.Outros == SimNao.Sim)
                    if (this._sumarioavaliacaomedicaco.ProcedenciaOutros.IsEmptyOrWhiteSpace())
                    {
                        erros.Add("É necessário informar o campo 'Procedência Outros' na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                            setou = true;
                        }
                    }

            if (this._sumarioavaliacaomedicaco.Gesta.IsNull())
            {
                erros.Add("É necessário informar a Gesta na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                if (!setou)
                {
                    this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual;
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                    setou = true;
                }
            }

            //if (!this._sumarioavaliacaomedicaco.UltimaMenstruacao.HasValue)
            //{
            //    erros.Add("É necessário informar a data da Última Mestruaçao na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
            //    if (!setou)
            //    {
            //        this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual;
            //        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
            //        setou = true;
            //    }
            //}

            if (this._sumarioavaliacaomedicaco.IdadeDesconhecida == SimNao.Nao || this._sumarioavaliacaomedicaco.IdadeDesconhecida.IsNull())
                if (this._sumarioavaliacaomedicaco.IdadeSemana.IsNull())
                {
                    erros.Add("É necessário informar a Idade Gestacional Semanas na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                        setou = true;
                    }
                }

            if (this._sumarioavaliacaomedicaco.IsPatologia == SimNao.Nao || this._sumarioavaliacaomedicaco.IsPatologia.IsNull())
                if (this.vmAnamneseSumarioAvaliacaoMedicaCO.CollectionGest.Count(x => x.Selecionado == true) == 0 && this._sumarioavaliacaomedicaco.PatologiaObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário informar as Patologias na Gravidez, Marcar a opção Nenhum ou Informar Outros/Observações na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                        setou = true;
                    }
                }

            if ((this._sumarioavaliacaomedicaco.IsGestacaoAnterior == SimNao.Nao || this._sumarioavaliacaomedicaco.IsGestacaoAnterior.IsNull())
                && (this._sumarioavaliacaomedicaco.IsPrimeiraGestacao == SimNao.Nao || this._sumarioavaliacaomedicaco.IsPrimeiraGestacao.IsNull()))
                if (this.vmAnamneseSumarioAvaliacaoMedicaCO.CollectionGestacaoAnterior.Count(x => x.Selecionado == true) == 0 && this._sumarioavaliacaomedicaco.GestacaoAnteriorObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário informar as Gestações Anteriores ou Marcar a opção Sem Intercorrências na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.GestacoesAnteriores;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
                        setou = true;
                    }
                }

            if (this._sumarioavaliacaomedicaco.NegaPatologiaPrevia == SimNao.Nao || this._sumarioavaliacaomedicaco.NegaPatologiaPrevia.IsNull())
                if (this.vmAnamneseSumarioAvaliacaoMedicaCO.CollectionHistoriaPregressa.Count(x => x.Selecionado == true) == 0 && this._sumarioavaliacaomedicaco.PatologiaPreviaObservacao.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário informar as Patologias Prévias ou Marcar a opção Nega na aba de " + TabsSumarioAvaliacaoMedicaCO.Anamnese.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmAnamneseSumarioAvaliacaoMedicaCO.TipoTabSelecionada = vmAnamneseSumarioAvaliacaoMedicaCO.TabsAnamneseSumarioAvaliacaoMedicaCO.HistoriaPregressa;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.Anamnese;
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

            //if (this._sumarioavaliacaomedicaco.Coombs.IsNull())
            //{
            //    if (!setou)
            //    {
            //        erros.Add("É necessário informar o Commbs na aba de " + TabsSumarioAvaliacaoMedicaCO.ExamesRealizados.GetEnumDescription() + ".");
            //        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExamesRealizados;
            //        setou = true;
            //    }
            //}

            if (this.vmExameFisicoSumarioAvaliacaoMedicaCO.IsNotNull())
                if (!this.vmExameFisicoSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.IsValid)
                {
                    erros.Add("É necessário preencher os dados corretamente na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                        setou = true;
                    }
                }

            if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.IsNotNull())
            {
                if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.Membrana.IsNull())
                {
                    erros.Add("É necessário informar a Membrana na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                        setou = true;
                    }
                }
                if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.Membrana == Membranas.Rotas)
                {
                    if (!this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.DataMembrana.HasValue)
                    {
                        erros.Add("É necessário informar a Data Membrana na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                            setou = true;
                        }
                    }

                    if (!this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmniotico.HasValue)
                    {
                        erros.Add("É necessário informar o Líquido Amniótico na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                            setou = true;
                        }
                    }
                    else if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmniotico == LiquidoAmniotico.Outros)
                        if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmnioticoObservacao.IsEmptyOrWhiteSpace())
                        {
                            erros.Add("É necessário informar o Líquido Amniótico Outros na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                            if (!setou)
                            {
                                this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                                setou = true;
                            }
                        }
                }

                if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.ToqueVaginal.IsEmptyOrWhiteSpace())
                {
                    erros.Add("É necessário informar o Toque Vaginal na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                        setou = true;
                    }
                }

                if (!this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOExameFisico.IsSangramento.HasValue)
                {
                    erros.Add("É necessário informar o Sangramento na aba de " + TabsSumarioAvaliacaoMedicaCO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExameFisico;
                        setou = true;
                    }
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //if (this._sumarioavaliacaomedicaco.TipagemPaciente.IsNull())
            //{
            //    erros.Add("É necessário informar a Tipagem na aba de " + TabsSumarioAvaliacaoMedicaCO.ExamesRealizados.GetEnumDescription() + ".");
            //    if (!setou)
            //    {
            //        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExamesRealizados;
            //        setou = true;
            //    }
            //}

            if (!this._sumarioavaliacaomedicaco.HIV.HasValue)
            {
                erros.Add("É necessário informar o Teste Rápido na aba de " + TabsSumarioAvaliacaoMedicaCO.ExamesRealizados.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.ExamesRealizados;
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

            if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOPlano.IsNotNull())
            {
                if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaEletiva ||
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaUrgencia)
                    if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOPlano.Justificativa.IsEmptyOrWhiteSpace())
                    {
                        erros.Add("É necessário informar justificativa na aba de " + TabsSumarioAvaliacaoMedicaCO.PlanoDiagnosticoTerapeutico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.PlanoDiagnosticoTerapeutico;
                            setou = true;
                        }
                    }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (this._sumarioavaliacaomedicaco.Atendimento.Cid.IsNull())
            {
                erros.Add("É necessário informar o CID Principal do Atendimento na aba de " + TabsSumarioAvaliacaoMedicaCO.DiagnosticosHipotesesDiagnosticas.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCO.DiagnosticosHipotesesDiagnosticas;
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
                this._sumarioavaliacaomedicaco.DataEncerramento = DateTime.Now;
                this._tabs = null;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.MostraFinalizar);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.MostraImprimir);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCO>(x => x.MostraMarcaDaguaRelatorio);
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
            public ResultadoItemCO? Resultado { get; set; }
            public SimNao IsTrimestre1 { get; set; }
            public SimNao IsTrimestre2 { get; set; }
            public SimNao IsTrimestre3 { get; set; }
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
                        this._selecionado = true;

                    base.OnPropertyChanged<Item>(x => x.Observacao);
                    base.OnPropertyChanged<Item>(x => x.Selecionado);
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

            public bool IsTriPrimeiro
            {
                get
                {
                    return (IsTrimestre1 == SimNao.Sim);
                }
                set
                {
                    if (value)
                    {
                        IsTrimestre1 = SimNao.Sim;
                        //IsTrimestre2 = SimNao.Nao;
                        //IsTrimestre3 = SimNao.Nao;
                    }
                    else
                        IsTrimestre1 = SimNao.Nao;
                    //base.OnPropertyChanged<Item>(x => x.IsTriSegundo);
                    //base.OnPropertyChanged<Item>(x => x.IsTriTerceiro);
                    base.OnPropertyChanged<Item>(x => x.IsTriPrimeiro);
                }
            }
            public bool IsTriSegundo
            {
                get
                {
                    return (IsTrimestre2 == SimNao.Sim);
                }
                set
                {
                    if (value)
                    {
                        IsTrimestre2 = SimNao.Sim;
                        //IsTrimestre1 = SimNao.Nao;
                        //IsTrimestre3 = SimNao.Nao;
                    }
                    else
                        IsTrimestre2 = SimNao.Nao;
                    //base.OnPropertyChanged<Item>(x => x.IsTriPrimeiro);
                    //base.OnPropertyChanged<Item>(x => x.IsTriTerceiro);
                    base.OnPropertyChanged<Item>(x => x.IsTriSegundo);
                }
            }
            public bool IsTriTerceiro
            {
                get
                {
                    return (IsTrimestre3 == SimNao.Sim);
                }
                set
                {
                    if (value)
                    {
                        IsTrimestre3 = SimNao.Sim;
                        //IsTrimestre2 = SimNao.Nao;
                        //IsTrimestre1 = SimNao.Nao;
                    }
                    else
                        IsTrimestre3 = SimNao.Nao;
                    //base.OnPropertyChanged<Item>(x => x.IsTriPrimeiro);
                    //base.OnPropertyChanged<Item>(x => x.IsTriSegundo);
                    base.OnPropertyChanged<Item>(x => x.IsTriTerceiro);
                }
            }

            public bool ResultadoPositivo
            {
                get
                {
                    return (Resultado == ResultadoItemCO.Positivo);
                }
                set
                {
                    if (value)
                        Resultado = ResultadoItemCO.Positivo;
                    base.OnPropertyChanged<Item>(x => x.ResultadoPositivo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoNegativo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoIndisponivel);
                }
            }
            public bool ResultadoNegativo
            {
                get
                {
                    return (Resultado == ResultadoItemCO.Negativo);
                }
                set
                {
                    if (value)
                        Resultado = ResultadoItemCO.Negativo;
                    base.OnPropertyChanged<Item>(x => x.ResultadoPositivo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoNegativo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoIndisponivel);
                }
            }
            public bool ResultadoIndisponivel
            {
                get
                {
                    return (Resultado == ResultadoItemCO.NaoDisponivel);
                }
                set
                {
                    if (value)
                        Resultado = ResultadoItemCO.NaoDisponivel;
                    base.OnPropertyChanged<Item>(x => x.ResultadoPositivo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoNegativo);
                    base.OnPropertyChanged<Item>(x => x.ResultadoIndisponivel);
                }
            }
        }
        #endregion


    }
}
