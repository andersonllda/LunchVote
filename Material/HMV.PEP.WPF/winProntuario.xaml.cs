using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.NavBar;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.AdmAssistencial;
using HMV.Core.Domain.Model.PesqClinica;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Repository.PEP;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Domain.Repository.PEP.PrevAlta;
using HMV.Core.DTO;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.Windows;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using HMV.PEP.Services;
using HMV.PEP.ViewModel;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.PEP.Evolucao;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.WPF.Cadastros;
using HMV.PEP.WPF.PrevisaoAlta;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.Pim2;
using HMV.PEP.WPF.Report.Termos;
using HMV.PEP.WPF.UserControls;
using HMV.PEP.WPF.UserControls.BoletimDeEmergencia;
using HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia;
using HMV.PEP.WPF.UserControls.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoM;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaCTINEO;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaEndoscopia;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaRN;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica;
using HMV.PEP.WPF.Windows;
using HMV.PEP.WPF.Windows.SCI;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoPreAnestesica;
using StructureMap;
using StructureMap.Pipeline;

namespace HMV.PEP.WPF
{
    public partial class winProntuario : WindowBase
    {
        private bool ControlaGroupExpand;
        const string SistemaMicroData = "MICRODATA";
        const string Prescricao = "PRESCRICAO";
        const string DescricaoCirurgica = "DESCRICAO_CIRURGICA";
        const string DocumentosEletronicos = "DOCUMENTOS_ELETRONICOS";
        const string PACs = "PACS";
        const string PACs2 = "PACS2";
        const string DLLCSharp = "DLL;";
        const string ADMISSAO = "ADMISSAO_ASSISTENCIAL";
        const string SUMARIOAVALIACAOMEDICA = "SUMARIO_AVALIACAO_MEDICA";
        const string SUMARIOAVALIACAOMEDICACTINEO = "UserControls/SumarioAvaliacaoMedicaCTINEO/ucSumarioCTINEO.xaml";
        const string BOLETIM = "BOLETIM_EMERGENCIA";
        const string PROCESSOSENFERMAGEM = "PROCESSOS_DE_ENFERMAGEM";
        const string PRONTUARIO = "PRONTUARIO";
        const string SUMARIOPREANESTESICO = "UserControls/SumarioAvaliacaoPreAnestesica/ucSumarioAvaliacaoPreAnestesica.xaml";
        const string CHECKLIST = "/UserControls/CheckListCirurgiaSegura/ucCheckListCirurgiaSegura.xaml";
        const string CHECKLISTUDI = "/UserControls/CheckListUDI/ucCheckListUDI.xaml";
        const string ONCOLOGIA = "ONCOLOGIA";
        const string PEDIATRIA = "PEDIATRIA";
        const string PEDIATRIAITENS = "UserControls/ucItensPediatria.xaml";
        const string GED = "UserControls/ucGED.xaml";
        const string EVOLUCAO = "Evolucao/ucEvolucao.xaml";
        const string PREVISAOALTA = "PrevisaoAlta/ucPrevisaoAlta.xaml";
        const string AVALIACAORISCO = "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.AvaliacaoRisco.ucAvaliacaoRisco";
        const string RESTRICAODEPACIENTE = "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.RestricaoPaciente.ucVisitanteRestricao";
        const string TRANSFASSISTENCIAL = "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.TransferenciaAssistencial.ucTelaTransferencia";
        const string FOLHAPARADAUTINEO = "UserControls/ucUTINEOFolhaParada.xaml";
        const string SUMARIOALTA = "UserControls/ucSumarioAlta.xaml";

        private bool _admissaoCO;
        private bool _admissaoUTINEO;
        private bool _admissaoEndoscopia;
        private bool _admissaoUrodinamica;
        private bool _sumarioCO;
        private bool _sumarioRN;
        private bool _sumarioUTINEO;
        private bool _sumarioEndoscopia;
        private bool _pediatria, _pediatriaitens;
        private bool _boletim, _boletimCO;
        private bool _checklist, _checklistUDI;
        private bool _GED;
        private bool _Evolucao;
        private bool _PrevisaoAlta;
        private bool _FolhaParadaUTINEO;

        int _idPaciente;
        Atendimento fAtendimentoCTINEOAnterior;
        public event EventHandler ExecuteFechou;
        public bool ProcessandoTermo { get; set; }
        protected virtual void Fechou()
        {
            if (ExecuteFechou != null) ExecuteFechou(this, EventArgs.Empty);
        }
        public winProntuario(Atendimento pAtendimento, ReabrirAba pAba = ReabrirAba.Nenhuma)
        {
            fRecarregarPEP = false;
            App.Usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID(App.Usuario.cd_usuario);
            _idPaciente = pAtendimento.Paciente.ID;

            ControlaGroupExpand = true;

            InitializeComponent();           

            _verificaGermeMultiResistente(pAtendimento.Paciente.ID);
          
            this.DataContext = pAtendimento;

            grpAtendimento.Visibility = Visibility.Visible;          
            atualizaPrevisaAlta();

            //Atualiza o Atendimento
            ObjectFactory.GetInstance<IRepositorioDeAtendimento>().Refresh(pAtendimento);
            //

            DocumentPanel pan = (DocumentPanel)mdiContainer.Items["ListadeProblemas"];
            (pan.Control as IUserControl).SetData(this.DataContext);
            menuDockLayoutManager.Activate(pan);
            ControlaGroupExpand = false;

            IParametroPEPService serv = ObjectFactory.GetInstance<IParametroPEPService>();
            serv = ObjectFactory.GetInstance<IParametroPEPService>();

            if ((pAtendimento.Paciente.TipoDoPaciente == TipoPaciente.Pediatrico)
                || ((this.DataContext as Atendimento) != null && (this.DataContext as Atendimento).Leito != null
                && (this.DataContext as Atendimento).Leito.UnidadeInternacao != null
                && serv.UnidadeValidaPIM2Igual().Count(x => x.Valor == (this.DataContext as Atendimento).Leito.UnidadeInternacao.ID.ToString()) > 0))
            {             
                _pediatria = true;
            }
            else
            {               
                _pediatria = false;
            }

            //este só aparece quando nao há atendimento...           
            _pediatriaitens = false;

            #region --- Admissao CO ---

            //tviAdmissaoAssistencialCO.Visibility = System.Windows.Visibility.Visible;
            //tviAdmissaoAssistencial.Visibility = System.Windows.Visibility.Collapsed;

            IRepositorioDeParametrosInternet repCO = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
            ParametroInternet parametroCO = repCO.OndeOrigemParaAdmissaoCO().Single();
            if (parametroCO.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCO.valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(pAtendimento.OrigemAtendimento.ID))
                    {                       
                        _admissaoCO = true;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO deve ser inteiro e separado por virgula.");
                }
            }
            #endregion

            #region --- Sumario CO ---
            IRepositorioDeParametrosInternet repCO2 = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
            ParametroInternet parametroCO2 = repCO2.OndeOrigemParaSumarioCO().Single();
            if (parametroCO2.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCO2.valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(pAtendimento.OrigemAtendimento.ID) && pAtendimento.SumarioAvaliacaoMedica.IsNull())
                    {                      
                        _sumarioCO = true;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO_SUMARIO deve ser inteiro e separado por virgula.");
                }
            }
            #endregion

            #region --- Sumario RN ---
            if (pAtendimento.AtendimentoPai.IsNotNull())
                if (serv.ObrigaRN())
                {                    
                    _sumarioRN = true;
                    _sumarioCO = false;
                }
            #endregion

            #region --- Boletim ---           
            if (pAtendimento.TipoDeAtendimento == TipoAtendimento.Urgencia)
            {
                if (pAtendimento.OrigemAtendimento.ID.ToString() == System.Configuration.ConfigurationManager.AppSettings["OrigemAtendimentoCO"])
                {                   
                    _boletimCO = true;
                }
                else
                {                    
                    _boletim = true;
                }
            }
            #endregion            

            #region --- Sumario/Admissao UTINEO ---
            fAtendimentoCTINEOAnterior = null;           
            _FolhaParadaUTINEO = false;
            IRepositorioDeParametrosClinicas repCTI = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
            Parametro parametroCTI = repCTI.OndeUnidadeCTINEO().Single();
            if (parametroCTI.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCTI.Valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (pAtendimento.Leito.IsNotNull() && codigos.Contains(pAtendimento.Leito.UnidadeInternacao.ID))
                    {
                        //SUMARIO                      
                        _sumarioCO = false;
                        _sumarioRN = true;
                        _sumarioUTINEO = true;

                        //ADMISSAO                       
                        _admissaoCO = false;
                        _admissaoUTINEO = true;

                        int _horasctineo = 0;
                        Parametro parC = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaHorasValidadeSumarioCTINEO().Single();
                        if (parC.IsNotNull())
                            _horasctineo = parC.Valor.ToInt();

                        var repA = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>();
                        var ret = repA.OndeCodigoPacienteIgual(pAtendimento.Paciente.ID).List();
                        if (ret.IsNotNull())
                        {
                            var atendmentosanteriores = ret.Where(x => x.DataAlta.IsNotNull() && x.DataAlta.Value.AddHours(_horasctineo) >= pAtendimento.DataAtendimento).ToList();
                            if (atendmentosanteriores.HasItems())
                                if (atendmentosanteriores.Count(x => x.AtendimentoPai.IsNotNull()) > 0)
                                    fAtendimentoCTINEOAnterior = atendmentosanteriores.FirstOrDefault();
                        }

                        #region UTINEO Folha Parada
                        if (pAtendimento.TipoDeAtendimento == TipoAtendimento.Internacao)
                        {                          
                            _FolhaParadaUTINEO = true;
                        }
                        #endregion
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO_SUMARIO deve ser inteiro e separado por virgula.");
                }
            }
            #endregion

            #region --- Admissao/Sumario Endoscopia --           

            Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OrigemEndoscopia().Single();
            if (par.IsNotNull())
            {
                try
                {
                    IList<int> codigos = par.Valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(pAtendimento.OrigemAtendimento.ID))
                    {                       
                        _admissaoCO = false;
                        _admissaoUTINEO = false;
                        _admissaoEndoscopia = true;                       
                        _sumarioCO = false;
                        _sumarioRN = false;
                        _sumarioUTINEO = true;
                        _sumarioEndoscopia = true;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro ORIGEM_ENDOSCOPIA deve ser inteiro e separado por virgula.");
                }
            }
            #endregion

            #region --- Admissao Urodinamica --           
            Parametro param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OrigemUrodinamica().Single();
            if (param.IsNotNull())
            {
                try
                {
                    IList<int> codigos = param.Valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(pAtendimento.OrigemAtendimento.ID))
                    {                        
                        _admissaoCO = false;
                        _admissaoUTINEO = false;
                        _admissaoEndoscopia = false;
                        _admissaoUrodinamica = true;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro ORIGEM_URODINAMICA deve ser inteiro e separado por virgula.");
                }
            }
            #endregion

            #region --- CheckList UDI ---         
            Parametro parUDI = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaCentroCirurgicoCheckListUDI().Single();
            if (parUDI.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parUDI.Valor.Split(',').Select(x => int.Parse(x)).ToList();
                    foreach (var item in pAtendimento.DescricaoCirurgica)
                    {
                        if (codigos.Contains(item.CentroCirurgico.Id))
                        {
                            _checklistUDI = true;
                            _checklist = false;
                        }
                        else
                        {
                            _checklist = true;
                            _checklistUDI = false;
                        }
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CENTRO_CIRURGICO_CHECKLIST_UDI deve ser inteiro e separado por virgula.");
                }
            }
            else
            {
                _checklist = true;                
            }
            #endregion

            mostraGED();

            mostraMensagemPesquisaClinica(pAtendimento.Paciente);
           
            IRepositorioDeParametrosClinicas repp = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
            Parametro _habilita = repp.OndePEPHabilitaEvolucao().Single();
            if (_habilita.Valor == "S")
            {               
                _Evolucao = true;
            }

            //Verifica se vem do Relogin
            if (pAba == ReabrirAba.Boletim)
            {
                AddDocumentPanel("Boletim de Emergência", "UserControls/BoletimDeEmergencia/ucBoletimDeEmergencia.xaml");
            }
            else if (pAba == ReabrirAba.BoletimCO)
            {
                AddDocumentPanel("Boletim de Emergência", "UserControls/CentroObstetrico/BoletimDeEmergencia/ucBoletimCO.xaml");
            }

            //Busca o intervalo para bloquear os boletins
            Parametro parT = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaTempoReloginBoletim().Single();
            if (parT.IsNotNull())
                intervalo = parT.Valor.ToInt();
           
            #region  --- Previsao De Alta ---
            IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();
            int ID = rep.UltimoAtendimento(pAtendimento.ID);
            if (ID > 0)
            {               
                _PrevisaoAlta = true;
            }
            #endregion
           
            montaMenu();
        }

        public winProntuario(Paciente pPaciente, ReabrirAba pAba = ReabrirAba.Nenhuma)
        {
            fRecarregarPEP = false;

            App.Usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID(App.Usuario.cd_usuario);
            _idPaciente = pPaciente.ID;

            ControlaGroupExpand = true;
            InitializeComponent();

            _verificaGermeMultiResistente(pPaciente.ID);           

            this.DataContext = pPaciente;           
            
            // Dentro do processo de enfermagem so foi habilitato o item Plano Educacional para funcionar sem atendimento.                        
            _pediatria = false;          

            if (pPaciente.TipoDoPaciente.Equals(TipoPaciente.Pediatrico))
            {
                List<Atendimento> AtendimentosComPimOuMotivo = pPaciente.Atendimentos.Where(x => (x.PIN2.Count > 0) || (x.MotivosInternacao.Count > 0)).ToList();
                List<UrgenciaPediatricaAtendimento> ListaUrgenciaPediatricaAtendimento = pPaciente.Atendimentos.SelectMany(x => x.UrgenciasPediatricas).ToList();
                Paciente pac = ((AtendimentosComPimOuMotivo.Count > 0) || (ListaUrgenciaPediatricaAtendimento.Count > 0)) ? pPaciente : null;

                if (pac.IsNotNull())
                {                    
                    _pediatriaitens = true;
                }
                else
                {                   
                    _pediatriaitens = false;
                }
            }
            else
            {               
                _pediatriaitens = false;
            }

            btnImprimir.Visibility = System.Windows.Visibility.Collapsed;

            DocumentPanel pan = (DocumentPanel)mdiContainer.Items["ListadeProblemas"];
            (pan.Control as IUserControl).SetData(this.DataContext);
            menuDockLayoutManager.Activate(pan);
            ControlaGroupExpand = false;

            mostraGED();

            mostraMensagemPesquisaClinica(pPaciente);

            //Busca o intervalo para bloquear os boletins
            Parametro parT = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaTempoReloginBoletim().Single();
            if (parT.IsNotNull())
                intervalo = parT.Valor.ToInt();

            montaMenu();
        }

        #region Monta Menu
        private vmMenu _vmMenu;
        IList<HMV.PEP.ViewModel.ProcessoDTO> menuFilho;
        private void montaMenu()
        {
            _vmMenu = new vmMenu(App.Usuario);

            ControlaGroupExpand = true;
            (menuNavBarControlRaiz.View as NavigationPaneView).IsExpanded = false;
            this.menuNavigationPaneView.GroupHeaderTemplate = App.Current.FindResource("GroupHeaderTemplateHiddenButtonExpand") as ControlTemplate;

            menuFilho = _vmMenu.Processos.Where(x => x.Nivel > 1).ToList();

            foreach (var i in _vmMenu.Processos.Where(x => x.Nivel == 1).OrderBy(x => x.Ordem))
            {
                NavBarGroup nav = new NavBarGroup();

                montaMenuFilho(nav, i.ProcessoFilho.ID);

                nav.Content = i;
                nav.Header = i.ProcessoPai.Descricao;

                if (i.Componente != PRONTUARIO)
                    nav.IsExpanded = false;
                else
                    nav.IsExpanded = true;

                Style style = (Style)gridMenu.Resources["styleNavBarGroup"];
                nav.Style = style;

                if (!String.IsNullOrEmpty(i.PathIco))
                    nav.ImageSource = loadImage(i.PathIco);

                if (i.Componente == PRONTUARIO)
                {
                    StackPanel panel = new StackPanel();
                    TreeView tv = new TreeView();
                    tv.MouseUp += TreeViewItem_MouseUp;

                    foreach (var p in _vmMenu.ProcessosProntuario.OrderBy(x => x.Ordem))
                    {
                        TreeViewItem tvi = new TreeViewItem();

                        if (p.Componente == GED)
                        {
                            if (!_GED)
                                continue;
                        }
                        
                        montaMenuTreeViewFilho(tvi, p);

                        tvi.Header = p.Processo.Descricao;

                        if (p.ProcessosFilhos.HasItems())
                        {
                            Style styletvi = (Style)gridMenu.Resources["styleTreeViewItemPai"];
                            tvi.Style = styletvi;
                            tvi.IsExpanded = true;
                        }
                        else
                        {
                            Style styletvi = (Style)gridMenu.Resources["styleTreeViewItem"];
                            tvi.Style = styletvi;
                            tvi.Tag = p.Componente;
                        }

                        DataTemplate template = (DataTemplate)gridMenu.Resources["ImageTreeViewItemTemplate"];
                        tv.ItemTemplate = template;
                        tv.Items.Add(tvi);
                    }
                    panel.Children.Add(tv);
                    nav.Content = panel;
                    nav.DisplaySource = DisplaySource.Content;
                }
                else if (i.Componente == PROCESSOSENFERMAGEM)
                {
                    StackPanel panel = new StackPanel();
                    TreeView tv = new TreeView();
                    tv.MouseUp += TreeViewItem_MouseUp;

                    foreach (var p in _vmMenu.ProcessosEnfermagem.OrderBy(x => x.Ordem))
                    {
                        TreeViewItem tvi = new TreeViewItem();

                        if (p.Componente == CHECKLIST)
                        {
                            if (!_checklist || !(this.DataContext is Atendimento))
                                continue;
                        }
                        else if (p.Componente == CHECKLISTUDI)
                        {
                            if (!_checklistUDI || !(this.DataContext is Atendimento))
                                continue;
                        }
                        else if (p.Componente == AVALIACAORISCO
                          || p.Componente == RESTRICAODEPACIENTE
                          || p.Componente == TRANSFASSISTENCIAL)
                        {
                            if (!(this.DataContext is Atendimento))
                                continue;
                        }

                        montaMenuTreeViewFilho(tvi, p);

                        tvi.Header = p.Processo.Descricao;

                        if (p.ProcessosFilhos.HasItems())
                        {
                            Style styletvi = (Style)gridMenu.Resources["styleTreeViewItemPai"];
                            tvi.Style = styletvi;
                            tvi.IsExpanded = true;
                        }
                        else
                        {
                            Style styletvi = (Style)gridMenu.Resources["styleTreeViewItem"];
                            tvi.Style = styletvi;
                            tvi.Tag = p.Componente;
                        }

                        DataTemplate template = (DataTemplate)gridMenu.Resources["ImageTreeViewItemTemplate"];
                        tv.ItemTemplate = template;
                        tv.Items.Add(tvi);
                    }
                    panel.Children.Add(tv);
                    nav.Content = panel;
                    nav.DisplaySource = DisplaySource.Content;
                }
                else if (i.Componente == SUMARIOPREANESTESICO
                      || i.Componente == SUMARIOAVALIACAOMEDICA
                      || i.Componente == DocumentosEletronicos
                      || i.Componente == ADMISSAO)
                {
                    nav.Tag = i.Componente;                   
                    if (!(this.DataContext is Atendimento))
                        continue;
                }
                else if (i.Componente == BOLETIM)
                {
                    nav.Tag = i.Componente;
                    if ((!_boletimCO && !_boletim) || !(this.DataContext is Atendimento))
                        continue;
                }
                else if (i.Componente == EVOLUCAO)
                {
                    nav.Tag = i.Componente;
                    if (!_Evolucao || !(this.DataContext is Atendimento))
                        continue;
                }
                else if (i.Componente == PREVISAOALTA)
                {
                    nav.Tag = i.Componente;
                    if (!_PrevisaoAlta)
                        continue;
                }
                else if (i.Componente == SUMARIOAVALIACAOMEDICACTINEO)
                {
                    nav.Tag = i.Componente;
                    if (!_sumarioUTINEO)
                        continue;
                }
                else if (i.Componente == SUMARIOALTA)
                {
                    nav.Tag = i.Componente;
                    if (App.Usuario.Prestador == null)                    
                        continue;                    
                }
                else
                    nav.Tag = i.Componente;

                menuNavBarControl2.Groups.Add(nav);
            }
            (menuNavBarControlRaiz.View as NavigationPaneView).IsExpanded = true;
            ControlaGroupExpand = false;
        }

        private void montaMenuFilho(NavBarGroup nav, int pProcesso)
        {
            foreach (var i in menuFilho.Where(x => x.ProcessoPai.ID == pProcesso).OrderBy(x => x.Ordem))
            {
                if (menuFilho.Where(x => x.ProcessoPai.ID == i.ProcessoFilho.ID).Count() > 0)
                {
                    NavBarControl control = new NavBarControl();
                    //control.Style = (Style)gridMenu.Resources["controlStyleFilho"];

                    NavBarGroup group = new NavBarGroup();
                    group.Header = i.ProcessoFilho.Descricao;
                    group.Content = i;

                    if (!String.IsNullOrEmpty(i.PathIco))
                    {
                        BitmapImage img = loadImage(i.PathIco);
                        group.ImageSource = img;
                        i.Imagem = img;
                    }

                    montaMenuFilho(group, i.ProcessoFilho.ID);
                    control.Groups.Add(group);
                    nav.Items.Add(control);
                    control.UpdateLayout();
                }
                else
                {

                    NavBarItem item = new NavBarItem();
                    item.Content = i.ProcessoFilho.Descricao;
                    item.Tag = i;

                    if (!String.IsNullOrEmpty(i.PathIco))
                    {
                        BitmapImage img = loadImage(i.PathIco);
                        item.ImageSource = img;
                        i.Imagem = img;
                    }
                    nav.Items.Add(item);
                }

            }
        }

        private void montaMenuTreeViewFilho(TreeViewItem tvi, HMV.PEP.ViewModel.ProcessoDTO pProcesso)
        {
            foreach (var i in pProcesso.ProcessosFilhos.OrderBy(x => x.Ordem))
            {
                if (i.ProcessosFilhos.HasItems())
                {
                    TreeViewItem tvin = new TreeViewItem();
                    tvin.Header = i.Processo.Descricao;
                    //tvin.Tag = i.Componente;

                    Style styletvi = (Style)gridMenu.Resources["styleTreeViewItemPai"];
                    tvin.Style = styletvi;
                    if (i.Componente != ONCOLOGIA)
                        tvin.IsExpanded = true;

                    if (i.Componente == PEDIATRIA)
                    {
                        tvin.IsExpanded = false;
                        if (!_pediatria)
                            continue;
                    }                                
                    
                    montaMenuTreeViewFilho(tvin, i);
                    tvi.Items.Add(tvin);
                }
                else
                {
                    TreeViewItem tvin = new TreeViewItem();
                    tvin.Header = i.Processo.Descricao;
                    tvin.Tag = i.Componente;

                    Style styletvi = (Style)gridMenu.Resources["styleTreeViewItem"];
                    tvin.Style = styletvi;

                    if (i.Componente == PEDIATRIAITENS)
                    {
                        if (!_pediatriaitens)
                            continue;
                    }
                    if (i.Componente == FOLHAPARADAUTINEO)
                    {
                        if (!_FolhaParadaUTINEO)
                            continue;
                    }                       
                    tvi.Items.Add(tvin);
                }
            }
        }

        private BitmapImage loadImage(string Path)
        {
            try
            {
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                myBitmapImage.UriSource = new Uri(System.Configuration.ConfigurationManager.AppSettings["PathIconeMenu"].Combine(Path), UriKind.Absolute);
                myBitmapImage.EndInit();
                return myBitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void menuNavigationPaneView_Click(object sender, RoutedEventArgs e)
        {
            NavBarGroup group = menuNavBarControlRaiz.View.GetNavBarGroup(e);
            if (e.OriginalSource is NavBarGroupHeader)
                group.IsExpanded = !group.IsExpanded;

            if (group != null && group.Header != null && group.Tag != null)
                AddDocumentPanel(group.Header.ToString(), group.Tag.ToString());
        }

        private void ExplorerBarView2_GroupExpandedChanging(object sender, NavBarGroupExpandedChangingEventArgs e)
        {
            if (!ControlaGroupExpand)
            {
                //if (e.Group.Tag.ToString() != "PRONTUARIO" && e.Group.Tag.ToString() != "PROCESSOS_DE_ENFERMAGEM")
                if (e.Group.Tag.IsNotNull())
                    e.Cancel = true;
            }
        }
        #endregion

        private void mostraMensagemPesquisaClinica(Paciente pPaciente)
        {
            if (pPaciente.PesquisaClinicaAtiva.IsNotNull())
            {
                winMensagemPesquisaClinica win = new winMensagemPesquisaClinica(pPaciente);
                win.ShowDialog(null);
            }
        }

        private void mostraGED()
        {           
            IRepositorioDeProntuario repProntuario = ObjectFactory.GetInstance<IRepositorioDeProntuario>();
            Paciente pac = null;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
                pac = (this.DataContext as Atendimento).Paciente;

            else if (typeof(Paciente) == this.DataContext.GetType() || typeof(Paciente) == this.DataContext.GetType().BaseType)
                pac = (this.DataContext as Paciente);

            bool temGED = repProntuario.VerificaSeDeveHabilitarOpcaoGED(0, pac.ID);

            if (temGED)
            {
                Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndePodeMostrarGED().Single();

                if (par.IsNotNull() && par.Valor.Split(',').Contains(App.Usuario.cd_usuario.ToString()))
                {                   
                    _GED = true;
                }
                else if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
                {                   
                    _GED = true;
                }
            }
        }        

        private void TreeViewItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender as TreeView != null)
            {
                TreeViewItem item = (TreeViewItem)(sender as TreeView).SelectedItem;
                if (item != null)
                {
                    item.IsExpanded = item.IsExpanded ? false : true;
                    if (item.Header != null && item.Tag != null)
                        AddDocumentPanel(item.Header.ToString(), item.Tag.ToString());
                }
            }
        }   

        private winSelAtendimento winSelAtendimentoDoSumarioDeAlta;
        private void AddDocumentPanel(string pHeader, string pComponente)
        {
            App.Log(this.GetType().Assembly, "ACESSO_MENU_PRONTUARIO", null, pHeader);

            bool pSumarioAtendimentoI = false;
            bool pSumarioAtendimentoA = false;

            if (pComponente != string.Empty)
            {
                if (pHeader.Replace(" ", "") == "Internações")
                {
                    pHeader = "Sumário de Atendimentos";
                    pSumarioAtendimentoI = true;
                }
                else if (pHeader.Replace(" ", "") == "Ambulatoriais")
                {
                    pHeader = "Sumário de Atendimentos";
                    pSumarioAtendimentoA = true;
                }
                else if (pHeader.Replace(" ", "") == "SumáriodeAlta")
                {
                    if (winSelAtendimentoDoSumarioDeAlta == null)
                    {
                        if (this.DataContext.GetType() == typeof(Paciente) || (this.DataContext.GetType().BaseType == typeof(Paciente)))
                        {
                            winSelAtendimentoDoSumarioDeAlta = new winSelAtendimento((this.DataContext as Paciente), TipoAtendimentoSumario.Todos, true);
                            if (winSelAtendimentoDoSumarioDeAlta.ShowDialog(this) != true && winSelAtendimentoDoSumarioDeAlta.Atendimento == null)
                            {
                                winSelAtendimentoDoSumarioDeAlta = null;
                                return;
                            }
                        }
                    }
                }

                DocumentPanel pan = (DocumentPanel)mdiContainer.Items[pHeader.Replace(" ", "")];
                if (pan != null)
                {
                    menuDockLayoutManager.Activate(pan);
                }
                else if (pan == null)
                {
                    try
                    {
                        if (pComponente.Equals(SistemaMicroData))
                            chamaSistemaMicroData();
                        else if (pComponente.Equals(Prescricao))
                            chamaPrescricao();
                        else if (pComponente.Equals(DescricaoCirurgica))
                            chamaDescricaoCirurgica();
                        else if (pComponente.Equals(DocumentosEletronicos))
                            chamaDocumentosEletronicos();
                        else if (pComponente.Equals(PACs))
                            chamaPACs();
                        else if (pComponente.Equals(PACs2))
                            chamaPACs2();
                        else if (pComponente.StartsWith(DLLCSharp))
                            chamaDLLCSharp(pComponente, pHeader);
                        else if (pComponente.Equals(ADMISSAO))
                            chamaAdmissao(pHeader);
                        else
                        {
                            if (menuDockLayoutManager == null || menuDockLayoutManager.DockController == null)
                                return;

                            if (pHeader.Replace(" ", "").Equals("SumáriodeAvaliaçãoMédica") && App.Usuario.Prestador.IsNurse && (DataContext as Atendimento).SumarioAvaliacaoMedica.IsNull())
                            {
                                DXMessageBox.Show("Não existe sumário para este paciente", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }

                            DocumentPanel panel = null;

                            if (pComponente.Equals(SUMARIOAVALIACAOMEDICA))
                                panel = chamaSumario(pHeader);
                            else if (pComponente.Equals(BOLETIM))
                                panel = chamaBoletim(pHeader);
                            else
                                panel = addDocumentoPanel(pHeader, pComponente);

                            if (panel.Control.GetType() == typeof(ucSumarioAvaliacaoMedica) || panel.Control.GetType() == typeof(ucSumarioAvaliacaoMedicaEndoscopia) || panel.Control.GetType() == typeof(ucSumarioCTINEO) || panel.Control.GetType() == typeof(ucSumarioRN))
                            {
                                verificaPrevisaoAlta();
                            }

                            if (panel.Name == "SumáriodeAlta")
                            {
                                MotivoAltaRelatorio.DescMotivoAltaRelatorio = string.Empty;
                                (panel.Control as ucSumarioAlta).ExecuteMethod += new EventHandler(FecharAba);
                                (panel.Control as ucSumarioAlta).ChamadaPrescricao += new EventHandler(ChamaPrescricaoEvento);
                                if (winSelAtendimentoDoSumarioDeAlta != null && winSelAtendimentoDoSumarioDeAlta.Atendimento != null)
                                {
                                    (panel.Control as IUserControl).SetData(winSelAtendimentoDoSumarioDeAlta.Atendimento);
                                    menuDockLayoutManager.Activate(panel);
                                    return;
                                }
                            }
                            else if (panel.Name == "SumáriodeAvaliaçãoMédica")
                            {
                                if (panel.Control.GetType() == typeof(ucSumarioAvaliacaoMedica))
                                    (panel.Control as ucSumarioAvaliacaoMedica).ExecuteMethod += new EventHandler(FecharAba);
                                else if (panel.Control.GetType() == typeof(ucSumarioAvalMedica))
                                    (panel.Control as ucSumarioAvalMedica).ExecuteMethod += new EventHandler(FecharAba);
                                else if (panel.Control.GetType() == typeof(ucSumarioAvaliacaoMedicaEndoscopia))
                                {
                                    Paciente paciente = ObjectFactory.GetInstance<IRepositorioDePacientes>().OndeCodigoIgual(_idPaciente).Single();
                                    Atendimento atendimento = null;

                                    if (this.DataContext.GetType() == typeof(Atendimento) || this.DataContext.GetType().HasBaseType(typeof(Atendimento)))
                                        atendimento = this.DataContext as Atendimento;
                                    (panel.Control as ucSumarioAvaliacaoMedicaEndoscopia).SetData(new vmSumarioAvaliacaoMedicaEndoscopia(atendimento, paciente, App.Usuario, new GetSettings().IsCorpoClinico));

                                    menuDockLayoutManager.Activate(panel);
                                    return;
                                }
                            }
                            else if (panel.Name == "SumáriodeAval.Pré-Anestésica")
                            {
                                UIHelper.SetBusyState();
                                (panel.Control as ucSumarioAvaliacaoPreAnestesica).ExecuteMethod += new EventHandler(FecharAba);
                                Paciente paciente = ObjectFactory.GetInstance<IRepositorioDePacientes>().OndeCodigoIgual(_idPaciente).Single();
                                Atendimento atendimento = null;
                                if (this.DataContext.GetType() == typeof(Atendimento) || this.DataContext.GetType().HasBaseType(typeof(Atendimento)))
                                    atendimento = this.DataContext as Atendimento;

                                vmSumarioAvaliacaoPreAnestesica vm = new vmSumarioAvaliacaoPreAnestesica(paciente, App.Usuario, atendimento);

                                UIHelper.SetBusyState();
                                if (vm.FechaSumario)
                                {
                                    menuDockLayoutManager.DockController.RemovePanel(panel);
                                    return;
                                }

                                if (vm.AbreSelecaoAvisoCirurgia)
                                {
                                    winSelAvisoCirurgia win = new winSelAvisoCirurgia(vm);
                                    win.ShowDialog(this);
                                    if (!vm.Selecionou)
                                    {
                                        menuDockLayoutManager.DockController.RemovePanel(panel);
                                        return;
                                    }
                                }

                                //TODOPRE
                                if (vm.AbreVinculoWeb)
                                {
                                    winVinculoPacientes winpac = new winVinculoPacientes(vm);
                                    winpac.ShowDialog(this);
                                    if (!vm.SelecionouWeb)
                                        vm.CriaNovo();
                                }

                                UIHelper.SetBusyState();
                                (panel.Control as IUserControl).SetData(vm);
                                UIHelper.SetBusyState();
                                menuDockLayoutManager.Activate(panel);
                                return;
                            }

                            if (pSumarioAtendimentoI)
                                (panel.Control as ucSumarioAtendimento).fTipoAtendimentoSumario = TipoAtendimentoSumario.Internações;
                            else if (pSumarioAtendimentoA)
                                (panel.Control as ucSumarioAtendimento).fTipoAtendimentoSumario = TipoAtendimentoSumario.Ambulatoriais;


                            if (panel.Name == "SumáriodeAvaliaçãoMédicaUTINEO")
                            {
                                if (fAtendimentoCTINEOAnterior.IsNotNull())
                                    (panel.Control as IUserControl).SetData(fAtendimentoCTINEOAnterior);
                                else
                                    (panel.Control as IUserControl).SetData(this.DataContext);
                            }
                            else
                                (panel.Control as IUserControl).SetData(this.DataContext);


                            if (panel.Name == "GED")
                            {
                                // verifica se o usuário cancelou a visualização. 
                                if ((panel.Control as ucGED).Abriu)
                                    menuDockLayoutManager.Activate(panel);
                                else
                                    menuDockLayoutManager.DockController.RemovePanel(panel);
                            }
                            else if (panel.Name == "BoletimdeEmergência")
                            {
                                if (panel.Control.GetType() == typeof(ucBoletimDeEmergencia))
                                {
                                    timerLabel = new DispatcherTimer();
                                    timerLabel.Interval = TimeSpan.FromSeconds(1.00);
                                    timerLabel.Tick += new EventHandler(timerLogin_Tick);
                                    timerLabel.Start();
                                }
                                menuDockLayoutManager.Activate(panel);
                            }
                            else
                                menuDockLayoutManager.Activate(panel);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (pComponente.Equals(SistemaMicroData) || pComponente.Equals("UserControls/VB6/ucCardiologia.xaml")
                            || pComponente.Equals("UserControls/ucAnalisesClinicas.xaml")) // || pComponente.Equals(Prescricao) || pComponente.Equals(DescricaoCirurgica)
                            DXMessageBox.Show("Não foi possivel chamar o componente: " + pComponente
                             + Environment.NewLine
                             + "Tente abrir novamente, se o problema persistir entre em contato com a Informática."
                             + Environment.NewLine
                             + ex.Message);
                        else
                            throw;
                    }
                }
            }
        }
        private DocumentPanel chamaBoletim(string pHeader)
        {
            DocumentPanel panel = null;

            if (_boletim)
                panel = addDocumentoPanel(pHeader, "UserControls/BoletimDeEmergencia/ucBoletimDeEmergencia.xaml");
            else if (_boletimCO)
                panel = addDocumentoPanel(pHeader, "UserControls/CentroObstetrico/BoletimDeEmergencia/ucBoletimCO.xaml");

            return panel;
        }
        private DocumentPanel addDocumentoPanel(string pHeader, string pComponente)
        {
            DocumentPanel panel = menuDockLayoutManager.DockController.AddDocumentPanel(mdiContainer, new Uri(pComponente, UriKind.Relative));
            panel.Name = pHeader.Replace(" ", "");
            panel.Caption = pHeader;
            panel.ShowCaption = true;
            panel.FloatOnDoubleClick = false;
            panel.ShowControlBox = false;
            panel.ShowRestoreButton = false;
            return panel;
        }
        private DocumentPanel chamaSumario(string pHeader)
        {
            DocumentPanel panel = null;

            if (_sumarioCO)
                panel = addDocumentoPanel(pHeader, "UserControls/CentroObstetrico/SumarioDeAvaliacaoMedicaCO/ucSumarioAvalMedica.xaml");
            else if (_sumarioEndoscopia)
                panel = addDocumentoPanel(pHeader, "UserControls/SumarioAvaliacaoMedicaEndoscopia/ucSumarioAvaliacaoMedicaEndoscopia.xaml");
            else if (_sumarioRN)
                panel = addDocumentoPanel(pHeader, "UserControls/SumarioAvaliacaoMedicaRN/ucSumarioRN.xaml");
            else
                panel = addDocumentoPanel(pHeader, "UserControls/ucSumarioAvaliacaoMedica.xaml");

            return panel;
        }
        private void chamaAdmissao(string pHeader)
        {          
            if (_admissaoCO)
                chamaDLLCSharp("DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.Views.CentroObstetrico.AdmissaoAssistencial.ucAdmissaoAssistencial", pHeader); 
            else if (_admissaoEndoscopia)
                chamaDLLCSharp("DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.Views.ProcessosEnfermagem.AdmissaoAssistencialEndoscopia.ucAdmissaoAssistencialEndoscopia", pHeader);
            else if (_admissaoUrodinamica)
                chamaDLLCSharp("DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.Views.ProcessosEnfermagem.AdmissaoAssistencialUrodinamica.ucAdmissaoAssistencialUrodinamica", pHeader);
            else if (_admissaoUTINEO)
                chamaDLLCSharp("DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.Views.CTINEO.AdmissaoAssistencial.ucAdmissaoAssistencial", pHeader);
            else
                chamaDLLCSharp("DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.AdmissaoAssistencial.ucAdmissaoAssistencial", pHeader);         
        }

        private void verificaPrevisaoAlta()
        {
            //if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
            //{
            //    Atendimento atendimento = this.DataContext as Atendimento;
            //    if (atendimento.TipoDeAtendimento.Equals(TipoAtendimento.Internacao))
            //    {
            //        Parametro _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).ExcecaoPrevisaoAlta().Single();
            //        if (_param.IsNotNull())
            //        {
            //            IList<string> lst = _param.Valor.Split(',');
            //            if (!lst.Contains(atendimento.Leito.UnidadeInternacao.ID.ToString()))
            //            {
            //                IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();
            //                int ID = rep.UltimoAtendimento(atendimento.ID);
            //                if (ID == 0)
            //                {
            //                    winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, false, false));
            //                    win.ShowDialog(this);
            //                    grpPrevisaoAlta.IsVisible = true;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        //private void verificaDiasPrevisaoAlta()
        //{
        //    if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
        //    {
        //        Atendimento atendimento = this.DataContext as Atendimento;
        //        if (atendimento.TipoDeAtendimento.Equals(TipoAtendimento.Internacao))
        //        {
        //            bool _abreconfirmacao = false;
        //            Parametro _param = null;

        //            VMPrevisaoAltaConsulta vm = new VMPrevisaoAltaConsulta(App.Usuario, atendimento);
        //            if (vm.HasPrevisao)
        //            {
        //                if (vm.HasDataPassada)
        //                {
        //                    winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, true, false));
        //                    win.ShowDialog(this);
        //                    return;
        //                }

        //                if (vm.Has7Dias)
        //                {
        //                    _abreconfirmacao = true;
        //                }
        //                else
        //                {
        //                    if (vm.Has48Horas)
        //                    {
        //                        _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).Excecao48PrevisaoAlta().Single();
        //                    }
        //                    if (vm.Has24Horas)
        //                    {
        //                        _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).Excecao24PrevisaoAlta().Single();
        //                    }

        //                    if (_param.IsNotNull())
        //                    {
        //                        IList<string> lst = _param.Valor.Split(',');
        //                        if (!lst.Contains(atendimento.Leito.UnidadeInternacao.ID.ToString()))
        //                        {
        //                            _abreconfirmacao = true;
        //                        }
        //                    }
        //                }
        //                if (_abreconfirmacao)
        //                {
        //                    winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
        //                    win.ShowDialog(this);
        //                }
        //            }

        //        }
        //    }


        //}


        private int dateDiff(char charInterval, DateTime dttFromDate, DateTime dttToDate)
        {
            TimeSpan tsDuration;
            tsDuration = dttToDate - dttFromDate;

            if (charInterval == 'd')
            {
                // Resultado em Dias
                return tsDuration.Days;
            }
            else if (charInterval == 'm')
            {
                // Resultado em Meses
                double dblValue = 12 * (dttFromDate.Year - dttToDate.Year) + dttFromDate.Month - dttToDate.Month;
                return Convert.ToInt32(Math.Abs(dblValue));
            }
            else if (charInterval == 'y')
            {
                // Resultado em Anos
                return Convert.ToInt32((tsDuration.Days) / 365);
            }
            else
            {
                return 0;
            }
        }

        private void verificaDiasPrevisaoAlta()
        {
            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
            {
                Parametro _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).ExcecaoPrevisaoAlta().Single();
                Atendimento atendimento = this.DataContext as Atendimento;

                if (_param.IsNotNull())
                {
                    if (atendimento.Leito.IsNull())
                        return;
                    IList<string> lst = _param.Valor.Split(',');
                    if (!lst.Contains(atendimento.Leito.UnidadeInternacao.ID.ToString()))
                    {
                        bool pTipoPacienteCirurgico = false;

                        if (atendimento.TipoDeAtendimento.Equals(TipoAtendimento.Internacao))
                        {
                            if (atendimento.TipoDePaciente.IsNotNull())
                            {
                                _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).TipoInternacaoPrevAlta().Single();
                                if (_param.IsNotNull())
                                {
                                    lst = _param.Valor.Split(',');
                                    if (lst.Contains(atendimento.TipoDePaciente.ID.ToString()))
                                    {
                                        pTipoPacienteCirurgico = true;
                                    }
                                }
                            }

                            VMPrevisaoAltaConsulta vm = new VMPrevisaoAltaConsulta(App.Usuario, atendimento);

                            if (vm.HasPrevisao.Equals(false) && (dateDiff('d', atendimento.DataAtendimento.Date, DateTime.Now.Date) == 2) && pTipoPacienteCirurgico)
                            {
                                winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
                                win.ShowDialog(this);
                                return;

                            }
                            if (vm.HasPrevisao.Equals(false) && (dateDiff('d', atendimento.DataAtendimento.Date, DateTime.Now.Date) == 5) && !pTipoPacienteCirurgico)
                            {
                                winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
                                win.ShowDialog(this);
                                return;
                            }

                            if (vm.HasPrevisaoNoDia.Equals(false) && (dateDiff('d', atendimento.DataAtendimento.Date, DateTime.Now.Date) == 14))
                            {

                                winPrevisaoAltaMotivo win = new winPrevisaoAltaMotivo(new VMPrevisaoAlta(App.Usuario, atendimento, true));
                                win.ShowDialog(this);
                                return;
                                
                                
                                //vm.Mensagem = "Paciente internado há 14 dias. Confirma previsão de alta nas próximas 48 horas?";
                                //winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
                                //win.ShowDialog(this);
                                //return;
                                //winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, true, false));
                                //win.ShowDialog(this);
                                //return;
                            }

                            if (vm.HasPrevisaoNoDia.Equals(false) && (dateDiff('d', atendimento.DataAtendimento.Date, DateTime.Now.Date) == 30))
                            {
                                winPrevisaoAltaMotivo win = new winPrevisaoAltaMotivo(new VMPrevisaoAlta(App.Usuario, atendimento, false));
                                win.ShowDialog(this);
                                return;
                                //vm.Mensagem = "Paciente internado há 30 dias. Confirma previsão de alta nas próximas 48 horas?";
                                //winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
                                //win.ShowDialog(this);
                                //return;

                                //winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, true, false));
                                //win.ShowDialog(this);
                                //return;
                            }
                            if (!vm.HasPrevisao && (dateDiff('d', atendimento.DataAtendimento.Date, DateTime.Now.Date) > 30))
                            {
                                winPrevisaoAltaMotivo win = new winPrevisaoAltaMotivo(new VMPrevisaoAlta(App.Usuario, atendimento, false));
                                win.ShowDialog(this);
                                return;
                                //vm.Mensagem = "Paciente internado há 30 dias. Confirma previsão de alta nas próximas 48 horas?";
                                //winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
                                //win.ShowDialog(this);
                                //return;

                                //winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, true, false));
                                //win.ShowDialog(this);
                                //return;
                            }

                            if (vm.HasPrevisaoNoDia.Equals(false) && vm.HasDataPassada)
                            {
                                winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, false, false));
                                win.ShowDialog(this);
                                return;
                                //vm.Mensagem = "Paciente internado há 30 dias. Confirma previsão de alta nas próximas 48 horas?";
                                //winMensagemPrevisaoAlta win = new winMensagemPrevisaoAlta(vm);
                                //win.ShowDialog(this);
                                //return;

                                //winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, atendimento, true, false));
                                //win.ShowDialog(this);
                                //return;
                            }

                            atualizaPrevisaAlta();
                        }
                    }
                }
            }
        }

        #region Timer
        //Timer Login Boletim de Emergencia
        public event EventHandler ExecuteMethod;
        private DispatcherTimer timerLabel;
        private int intervalo;
        private int tempo;
        public bool fRecarregarPEP;

        private void timerLogin_Tick(object sender, EventArgs e)
        {
            int timer = Convert.ToInt32(Win32.GetIdleTime() / 1000);

            if (timer > 1)
                tempo = tempo - 1;
            else
                tempo = intervalo;

            if (tempo == 0)
            {
                AbreLogin();
                tempo = intervalo + 1;
            }
        }

        private void AbreLogin()
        {
            try
            {
                if (menuDockLayoutManager.DockController.ActiveItem.IsNotNull())
                {
                    DocumentPanel document = menuDockLayoutManager.DockController.ActiveItem as DocumentPanel;

                    var win = new winLoginBoletimEmergencia();
                    var ret = win.ShowDialog(this);
                    if (ret.HasValue && ret.Value)
                    {
                        UIHelper.SetBusyState();
                        timerLabel.Stop();
                        FecharAba(null, null);
                        if (this.DataContext is Atendimento)
                            (this.Owner as winPEP).fAtendimentoReabrir = (this.DataContext as Atendimento);
                        else
                            (this.Owner as winPEP).fPacienteReabrir = (this.DataContext as Paciente);
                        if (document.Control.GetType() == typeof(ucBoletimDeEmergencia))
                        {
                            (this.Owner as winPEP).fReabrirAba = ReabrirAba.Boletim;
                        }
                        else if (document.Control.GetType() == typeof(ucBoletimCO))
                        {
                            (this.Owner as winPEP).fReabrirAba = ReabrirAba.BoletimCO;
                        }
                        this.Close();
                        Memory.MinimizeMemory();
                        this.OnExecuteMethod();
                        UIHelper.SetBusyState();
                    }
                }
            }
            catch { }
        }

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty);
        }
        //
        #endregion

        private void chamaDLLCSharp(string pComponente, string pHeader)
        {
            String[] lDados = pComponente.Split(';');

            if (lDados[1] == "HMV.Personalizados.FSCC.WF.exe")
            {
                try
                {
                    int processo = 0;
                    switch (App.Banco.ToUpper())
                    {
                        case "HOMOLOG_01":
                            processo = 1250;
                            break;
                        case "HOMOLOG_02":
                            processo = 1194;
                            break;
                        case "PRODUCAOMV":
                            processo = 1250;
                            break;
                    }
                    string pChamada = "\\\\HOH2307\\SII$\\UPDATE\\UPDATENET\\PERSONALIZADOS\\CORE\\HMV.PERSONALIZADOS.CORE.WF.EXE";
                    string pParametros = string.Format("{0}/{1}@{2},656,{3},{4}", App.Usuario.cd_usuario, App.Senha, App.Banco, processo, (DataContext as Atendimento).ID);
                    Process myProcess = System.Diagnostics.Process.Start(@pChamada, @pParametros);
                    myProcess.WaitForExit();
                }
                catch (Exception e)
                {
                    DXMessageBox.Show(e.Message + Environment.NewLine + "Não foi possível abrir o consentimento informado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (lDados[1] == "HMV.Personalizados.FSCC.WF.exeS")
            {
                try
                {
                    int processo = 0;
                    switch (App.Banco.ToUpper())
                    {
                        case "HOMOLOG_01":
                            processo = 1250;
                            break;
                        case "HOMOLOG_02":
                            processo = 1194;
                            break;
                        case "PRODUCAOMV":
                            processo = 1250;
                            break;
                    }
                    string pChamada = "\\\\HOH2307\\SII$\\UPDATE\\UPDATENET\\PERSONALIZADOS\\CORE\\HMV.PERSONALIZADOS.CORE.WF.EXE";
                    string pParametros = string.Format("{0}/{1}@{2},656,{3},{4},{5}", App.Usuario.cd_usuario, App.Senha, App.Banco, processo, (DataContext as Atendimento).ID, "S");
                    Process myProcess = System.Diagnostics.Process.Start(@pChamada, @pParametros);
                    myProcess.WaitForExit();
                }
                catch (Exception e)
                {
                    DXMessageBox.Show(e.Message + Environment.NewLine + "Não foi possível abrir o consentimento informado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                string pasta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Assembly MyAsm = Assembly.LoadFrom(Path.Combine(pasta, lDados[1]));

                System.Type lType = MyAsm.GetType(lDados[2]);
                object lRet;

                if (lType != null)
                {
                    ConexaoDTO conn = new ConexaoDTO();
                    conn.Banco = App.Banco;
                    conn.IdUsuario = App.Usuario.cd_usuario;
                    conn.Dados = this.DataContext;

                    lRet = Activator.CreateInstance(lType, BindingFlags.ExactBinding, null, null, null);

                    string lpasso = string.Empty;
                    try
                    {
                        lpasso = "Passo1";
                        UserControlBase usercontrol = (UserControlBase)lRet;
                        lpasso = "Passo2";
                        (usercontrol as IUserControl).SetData(conn);
                        lpasso = "Passo3";
                        DocumentPanel panel = addDocumentoPanel(pHeader, "UserControls/ucIntegraDll.xaml");
                        lpasso = "Passo4";
                        (panel.Control as IUserControl).SetData(usercontrol);
                        lpasso = "Passo5";
                        menuDockLayoutManager.Activate(panel);
                        lpasso = "Passo6";
                    }
                    catch (Exception e)
                    {
                        DXMessageBox.Show("Não foi possível abrir o componente " + pHeader + Environment.NewLine + lpasso + "  " + e.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        throw;
                    }
                }
            }
        }

        private void FecharAba(object sender, EventArgs e)
        {
            DocumentPanel document = menuDockLayoutManager.DockController.ActiveItem as DocumentPanel;
            if (document.Control.GetType() == typeof(ucSumarioAlta))
            {
                (document.Control as ucSumarioAlta).Save();
                menuDockLayoutManager.DockController.Close(menuDockLayoutManager.DockController.ActiveItem);
            }
            else if (document.Control.GetType() == typeof(ucSumarioAvaliacaoMedica))
            {
                (document.Control as ucSumarioAvaliacaoMedica).Save();
                menuDockLayoutManager.DockController.Close(menuDockLayoutManager.DockController.ActiveItem);
            }
            else if (document.Control.GetType() == typeof(ucSumarioAvalMedica))
            {
                //(document.Control as ucSumarioAvalMedica).Save();
                menuDockLayoutManager.DockController.Close(menuDockLayoutManager.DockController.ActiveItem);
            }
            else if (document.Control.GetType() == typeof(ucBoletimDeEmergencia))
            {
                (document.Control as ucBoletimDeEmergencia).Salva(false);
                menuDockLayoutManager.DockController.Close(menuDockLayoutManager.DockController.ActiveItem);
            }
            else if (document.Control.GetType() == typeof(ucBoletimCO))
            {
                (document.Control as ucBoletimCO).Salva();
                menuDockLayoutManager.DockController.Close(menuDockLayoutManager.DockController.ActiveItem);
            }
            else if (document.Control.GetType() == typeof(ucSumarioAvaliacaoPreAnestesica))
            {
                menuDockLayoutManager.DockController.Close(menuDockLayoutManager.DockController.ActiveItem);
            }
        }

        private void ChamaPrescricaoEvento(object sender, EventArgs e)
        {
            this.chamaPrescricao();
        }

        private int prescricaoID;
        Process PR;
        private void PR_Exited(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(
            delegate
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<winProntuario>(atualizaprescricao), this);
            }
            ));
            t.Start();
        }

        private void atualizaprescricao(winProntuario me)
        {
            string _log = string.Empty;
            try
            {
                UIHelper.SetBusyState();
                if (me.DataContext is Atendimento)
                {
                    _log += "SCI1 ";

                    // habilita justificativa para o SCI
                    if (ObjectFactory.GetInstance<IParametroPEPService>().ObrigaSCI())
                    {
                        Atendimento _atendimento = me.DataContext as Atendimento;
                        ISCIService serv = ObjectFactory.GetInstance<ISCIService>();

                        foreach (var item in serv.GeraParecer(_atendimento, App.Usuario))
                        {
                            // Habilita tela para o medico revisar a justificativa.
                            if (ObjectFactory.GetInstance<IParametroPEPService>().MostraTelaJustificativaMedicoSCI() || string.IsNullOrWhiteSpace(item.JustificativaMedica))
                            {
                                winSCI sci = new winSCI(item);
                                try
                                {
                                    sci.ShowDialog(this);
                                }
                                catch
                                {
                                    return;
                                }
                            }
                        }

                        _log += "SCI5 ";

                    }
                }

                if (prescricaoID > 0)
                {
                    _log += "Lista de Problema Atualiza1 ";

                    foreach (var tab in me.mdiContainer.Items)
                    {
                        if (tab.GetType() == typeof(DocumentPanel))
                        {
                            DocumentPanel document = tab as DocumentPanel;
                            if (document != null)
                            {
                                if (document.Control.GetType() == typeof(ucListaDeProblemas))
                                {
                                    (document.Control as ucListaDeProblemas).atualiza();
                                }
                            }
                        }
                    }

                    _log += "Lista de Problema Atualiza2 ";

                    if (me.DataContext is Atendimento)
                    {
                        _log += "SumarioDeAlta1 ";

                        ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>().Refresh((me.DataContext as Atendimento));
                        Atendimento ate = (me.DataContext as Atendimento);

                        IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
                        Parametro par = srv.ItemDeAltaPrescrito();

                        if (ate != null && !string.IsNullOrWhiteSpace(par.Valor) && ate.PrescricaoMedica.HasItems())
                        {
                            _log += "SumarioDeAlta2 ";
                            IList<int> itensDeAlta = par.Valor.ToListInt();

                            // busca os itens de alta da ultima prescricao.
                            if (ate.IsNotNull() && ate.PrescricaoMedica.HasItems())
                            {
                                // está dando erro as vezes !!!!!!!!!
                                /*IList<int> itensPrescritos = ate
                                    .PrescricaoMedica
                                    .Where(a => a.Id == ate.PrescricaoMedica.Max(m => m.Id))
                                    .SelectMany(x => x.ItensPrescricao.Select(a => a.TipoPrescricaoMedica.Id)).ToList();*/

                                PrescricaoMedica presc = ate.PrescricaoMedica
                                    .Where(a => a.Id == ate.PrescricaoMedica.Max(m => m.Id)).FirstOrDefault();

                                _log += "SumarioDeAlta3 ";
                                IRepositorioDeItemPrescricao repItem = ObjectFactory.GetInstance<IRepositorioDeItemPrescricao>();
                                IList<ItemPrescricaoMedica> itens = repItem.ondePrescricaoIgual(presc).List();

                                if (itens.HasItems())
                                {
                                    _log += "SumarioDeAlta4 ";
                                    IList<int> itensPrescritos = itens.Select(a => a.TipoPrescricaoMedica.Id).ToList();

                                    var itensDeAltaDaPrescricao = (from x in itensDeAlta
                                                                   join a in itensPrescritos on x equals a
                                                                   select a).ToList();

                                    if (itensDeAltaDaPrescricao.Count > 0)
                                        me.AddDocumentPanel("Sumário de Alta", "UserControls/ucSumarioAlta.xaml");
                                }
                            }
                            _log += "SumarioDeAlta5 ";
                        }
                    }
                    prescricaoID = 0;
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                Exception erro = new Exception(_log + " exception: " + ex.ToString());
                HMVEmail.SendErro(erro);

            }
            finally
            {
                if (this.Owner != null)
                    this.Owner.ShowInTaskbar = true;
                UIHelper.SetBusyState();
            }
        }

        public bool IsMedico
        {
            get
            {
                // Regra copiada do Metodo chama prescricao 
                int valid = 0;
                IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                Parametro param = rep.OndeTipoPrestadorEnfermeira().Single();
                if ((App.Usuario.Prestador.IsNurse && int.TryParse(param.Valor.Split(',').Where(x => int.Parse(x) == App.Usuario.Prestador.TipoPrestador.Id).FirstOrDefault(), out valid))
                    || App.Usuario.Prestador.Conselho.IsNutricionista)
                    return false;
                return true;
            }
        }

        private void chamaPrescricao()
        {
            /// ENFERMAGEM DEVE CHAMAR PRESCRIÇÃO DA ENFERMAGEM            
            if (!IsMedico)
            {
                if (App.Usuario.Prestador.Conselho.IsNutricionista)
                    chamaPrescricaoNutri();
                else
                    chamaPrescricaoEnfermagem();
            }
            else
                chamaPrescricaoMedica();
        }

        private void chamaPrescricaoNutri()
        {
            if (!CompartilhamentoMV())
                return;

            int ID = 0;

            ID = (this.DataContext as Atendimento).ID;
            try
            {
                killProcessosPrescricao();
                if (this.Owner != null)
                    this.Owner.ShowInTaskbar = false;
                ProcessStartInfo psi = new ProcessStartInfo(@"C:\ORANTMV\BIN\ifrun60.EXE");
                psi.WindowStyle = ProcessWindowStyle.Maximized;
                psi.WorkingDirectory = @"E:\MV2000\PAGU";
                psi.Arguments = @"E:\MV2000\PAGU\LEXMV.FMX USR=" + App.Usuario.cd_usuario
                            + " PSW=" + App.Usuario.SenhaSistemaMV
                            + " BD=" + App.Banco
                            + @" T=E:\MV2000\PAGU\M_PREMED.FMX";

                psi.Arguments += " PM=";

                if (ID > 0)
                    psi.Arguments += "CD_ATENDIMENTO=" + ID.ToString() + ";";

                psi.Arguments += "TP_PRE_MED=E";

                PR = Process.Start(psi);
                PR.EnableRaisingEvents = true;
                PR.Exited += new EventHandler(PR_Exited);
            }
            catch
            {
                DXMessageBox.Show("Erro ao Inicializar o componente Prescrição.\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chamaPrescricaoMedica()
        {
            if (!CompartilhamentoMV())
                return;

            int ID = 0;

            // NOVO PROCESSO DE PREVISÃO DE ALTA
            verificaDiasPrevisaoAlta();

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
            {
                IParametroPEPService serv = ObjectFactory.GetInstance<IParametroPEPService>();
                if ((this.DataContext as Atendimento) != null && (this.DataContext as Atendimento).Leito != null
                    && (this.DataContext as Atendimento).Leito.UnidadeInternacao != null && serv.UnidadeValidaPIM2Igual().Count(x => x.Valor == (this.DataContext as Atendimento).Leito.UnidadeInternacao.ID.ToString()) > 0)
                {
                    if ((this.DataContext as Atendimento).DataHoraAtendimento.AddDays(1) < DateTime.Now)
                    {
                        if (!(this.DataContext as Atendimento).PIN2.HasItems() || !(this.DataContext as Atendimento).MotivosInternacao.HasItems())
                        {
                            DXMessageBox.Show("É necessário incluir PIM 2 e Motivo Internação para fazer a prescrição.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                ID = (this.DataContext as Atendimento).ID;
                prescricaoID = ID;
                IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
                Parametro par = srv.UnidadesInternacaoLiberadas();
                IList<string> lst = par.Valor.Split(',');
                Atendimento atend = (this.DataContext as Atendimento);

                if (atend.TipoDeAtendimento == TipoAtendimento.Internacao)
                {
                    ISistemaService srvsis = ObjectFactory.GetInstance<ISistemaService>();
                    Sistemas sis = srvsis.FiltraPorId(int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString()));
                    ExplicitArguments args = new ExplicitArguments();
                    args.SetArg("sistema", sis.ID);

                    int QtdCidParaAtendimento = atend.Paciente.ProblemasPaciente.Where(x => x.Atendimento.IsNotNull() && x.Atendimento.ID.Equals(atend.ID)).Count();

                    IRepositorioLogsAcessoAoSistema repCID = ObjectFactory.GetInstance<IRepositorioLogsAcessoAoSistema>(args);
                    IList<SistemasLog> lstCIDListaProblema = repCID.OndeNomeTabelaIgual("LOG_CID_ATENDIMENTO_LISTA_PROBLEMA").OndeChaveIgual((this.DataContext as Atendimento).ID).List();
                    if (QtdCidParaAtendimento.Equals(0) && lstCIDListaProblema.Count.Equals(0))
                    {
                        winCIDListaProblema winCID = new winCIDListaProblema(new vmCIDListaProblemaPrescricao(new wrpAtendimento(this.DataContext as Atendimento), new wrpUsuarios(App.Usuario)));
                        winCID.ShowDialog(base.OwnerBase);
                        atend.Refresh();
                    }

                    //wrpAtendimento _atendimento = new wrpAtendimento(atend);
                    //wrpUsuarios _usuario = new wrpUsuarios(App.Usuario);
                    //winAdicionaCidListaProblema win = new winAdicionaCidListaProblema(_atendimento, _usuario);
                    //if (!win.NaoAbrir)
                    //{
                    //    win.ShowDialog(this);
                    //    atualizaprescricao(this);
                    //}
                }

                bool _abrevalidacao = false;

                if (atend.AtendimentoPai.IsNull())
                {
                    _abrevalidacao = true;
                }
                else
                {
                    serv = ObjectFactory.GetInstance<IParametroPEPService>();
                    if (serv.ObrigaRN())
                        _abrevalidacao = true;
                }

                if (atend.Leito != null && atend.Leito.UnidadeInternacao != null && !lst.Contains(atend.Leito.UnidadeInternacao.ID.ToString()) && _abrevalidacao)
                {
                    if (ID > 0)
                    {
                        winMensagemSumarioNaoIniciado win = new winMensagemSumarioNaoIniciado(this.DataContext as Atendimento);
                        var result = win.Inicializa(this);
                        if (result.Equals(winMensagemSumarioNaoIniciado.ActionResultPEP.SumarioAvaliacaoMedica))
                        {
                            AddDocumentPanel("Sumário de Avaliação Médica", "UserControls/ucSumarioAvaliacaoMedica.xaml");
                            return;
                        }
                        else if (result.Equals(winMensagemSumarioNaoIniciado.ActionResultPEP.SumarioAvaliacaoMedicaCO))
                        {
                            AddDocumentPanel("Sumário de Avaliação Médica", "UserControls/CentroObstetrico/SumarioDeAvaliacaoMedicaCO/ucSumarioAvalMedica.xaml");
                            return;
                        }
                        else if (result.Equals(winMensagemSumarioNaoIniciado.ActionResultPEP.SumarioAvaliacaoMedicaRN))
                        {
                            AddDocumentPanel("Sumário de Avaliação Médica", "UserControls/SumarioAvaliacaoMedicaRN/ucSumarioRN.xaml");
                            return;
                        }
                        else if (result.Equals(winMensagemSumarioNaoIniciado.ActionResultPEP.SumarioAvaliacaoMedicaCTINEO))
                        {
                            AddDocumentPanel("Sumário de Avaliação Médica UTINEO", "UserControls/SumarioAvaliacaoMedicaCTINEO/ucSumarioCTINEO.xaml");
                            return;
                        }
                    }
                }


                // habilita justificativa para o SCI
                if (ObjectFactory.GetInstance<IParametroPEPService>().ObrigaSCI())
                {
                    ISCIService servSCI = ObjectFactory.GetInstance<ISCIService>();

                    foreach (var item in servSCI.BuscaParecerPendentes(atend, App.Usuario))
                    {
                        // Habilita tela para o medico revisar a justificativa.
                        if (ObjectFactory.GetInstance<IParametroPEPService>().MostraTelaJustificativaMedicoSCI())
                        {
                            winCiente sci = new winCiente(item);
                            sci.ShowDialog(this);
                        }
                    }
                }

                //Mostra msg dos procedimentos realizados na classificacao de risco.
                IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                Parametro parametro = rep.OndeMostraMedicamentoProcedimentoAtendimento().Single();
                if (parametro.IsNotNull())
                    if (parametro.Valor == "S")
                        if ((this.DataContext as Atendimento).BoletinsDeEmergencia.HasItems())
                        {
                            var _boletim = new wrpBoletimDeEmergencia((this.DataContext as Atendimento).BoletinsDeEmergencia.OrderBy(x => x.Id).Last());
                            if (_boletim.DataCiente.IsNull() && _boletim.DescricaoMedicamentoProcedimento.IsNotEmpty())
                            {
                                var win = new winMsgProcedimentosRealizados(_boletim.PrestadorMedicamentoProcedimento.IsNotNull() ?
                                    _boletim.PrestadorMedicamentoProcedimento.NomeExibicao : string.Empty, _boletim.DescricaoMedicamentoProcedimento);
                                if (win.ShowDialog(this) == true)
                                {
                                    _boletim.DataCiente = DateTime.Now;
                                    _boletim.UsuarioCiente = new wrpUsuarios(App.Usuario);
                                    _boletim.Save();
                                }
                            }
                        }
            }
            try
            {
#if (RELEASE )

                killProcessosPrescricao();

                if (this.Owner != null)
                    this.Owner.ShowInTaskbar = false;
                ProcessStartInfo psi = new ProcessStartInfo(@"C:\ORANTMV\BIN\ifrun60.EXE");
                psi.WindowStyle = ProcessWindowStyle.Maximized;
                psi.WorkingDirectory = @"E:\MV2000\PAGU";
                psi.Arguments = @"E:\MV2000\PAGU\LEXMV.FMX USR=" + App.Usuario.cd_usuario
                            + " PSW=" + App.Usuario.SenhaSistemaMV
                            + " BD=" + App.Banco
                            + @" T=E:\MV2000\PAGU\M_PREMED.FMX";

                if (ID > 0)
                    psi.Arguments += " PM=CD_ATENDIMENTO=" + ID.ToString();

                PR = Process.Start(psi);
#else
                PR = Process.Start("notepad");

#endif
                PR.EnableRaisingEvents = true;
                PR.Exited += new EventHandler(PR_Exited);

            }
            catch (Exception err)
            {
                DXMessageBox.Show("Erro ao Inicializar o componente Prescrição.\nEntre em contato com o setor responsável e solicite a instalação." + Environment.NewLine + err.ToString(), "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImprimiTermoDeNaoCoberturaDoConvenio(Atendimento pAtendimento)
        {
            //  DXMessageBox.Show("xx 000");

            ProcessandoTermo = false;
            //PrescricaoMedica prescricao = pAtendimento.PrescricaoMedica.FirstOrDefault();
            //if (prescricao.IsNotNull() && prescricao.ItensPrescricao.HasItems())
            {
                // POG -> Não está carregando dentro da thread os itens.... 
                //var itens = prescricao.ItensPrescricao.Select(x => x.TipoPrescricaoMedica).First();
                vmTermoNaoCoberturaConvenio vm = null;
                Exception erro = null;

                //ThreadStart threadTermo = delegate
                {
                    try
                    {
                        //  DXMessageBox.Show("xx 1");
                        ProcessandoTermo = true;
                        vm = new vmTermoNaoCoberturaConvenio(pAtendimento, App.Usuario);
                        //   DXMessageBox.Show("xx 2");
                    }
                    catch (Exception err)
                    {
                        //   DXMessageBox.Show("xx 3");
                        //    DXMessageBox.Show(err.Message);
                        ProcessandoTermo = false;
                        erro = err;
                    }
                    // Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
                    // delegate
                    {
                        if (erro.IsNotNull())
                        {
                            // DXMessageBox.Show("xx 4");
                            HMVEmail.SendErro(erro);
                        }
                        //DXMessageBox.Show("xx 4.1");


                        if (vm.IsNotNull() && vm.DTO.HasItems())
                        {
                            //DXMessageBox.Show("xx 5");
                            rptNaoCoberturaDoConvenio rpt = new rptNaoCoberturaDoConvenio();
                            rpt.DataSource = vm.DTO;
                            //DXMessageBox.Show("xx 5.0.1");

                            //rpt.Print();
#if ( RELEASE )
                            rpt.Print();
#else
                            rpt.ShowPreviewDialog();
#endif
                        }
                        ProcessandoTermo = false;

                    }/*, null, null)*/;

                };
                // threadTermo.BeginInvoke(delegate(IAsyncResult aysncResult) { threadTermo.EndInvoke(aysncResult); }, null);

            }

            //  DXMessageBox.Show("xx 5.1");


        }

        private void chamaPrescricaoEnfermagem()
        {
            if (!CompartilhamentoMV())
                return;

            int ID = 0;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
            {
                int valid = 0;
                int? idsetor;
                bool ValidarAR = false, ValidarAA = false, ValidarPE = false, ValidaNAS = false;
                Atendimento _atendimentolocal = (this.DataContext as Atendimento);
                if (_atendimentolocal.TipoDeAtendimento == TipoAtendimento.Internacao)
                {
                    idsetor = _atendimentolocal.Leito.UnidadeInternacao.Setor.ID;
                    if (_atendimentolocal.Leito.IsNotNull() && _atendimentolocal.Leito.UnidadeInternacao.IsNotNull() && _atendimentolocal.Leito.UnidadeInternacao.Setor.IsNotNull())
                    {
                        IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                        Parametro parAR = rep.OndeLiberaSetorAvaliacaoRisco().Single();
                        ValidarAR = !(int.TryParse(parAR.Valor.Split(',').Where(x => int.Parse(x) == idsetor).FirstOrDefault(), out valid));

                        rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                        Parametro parAA = rep.OndeLiberaSetorAdmissaoAssistencial().Single();
                        ValidarAA = !(int.TryParse(parAR.Valor.Split(',').Where(x => int.Parse(x) == idsetor).FirstOrDefault(), out valid));

                        rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                        Parametro parPE = rep.OndeLiberaSetorPlanoEducacional().Single();
                        ValidarPE = !(int.TryParse(parAR.Valor.Split(',').Where(x => int.Parse(x) == idsetor).FirstOrDefault(), out valid));

                        rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                        Parametro parNAS = rep.BuscaSetorBloqueioNAS().Single();
                        ValidaNAS = (int.TryParse(parNAS.Valor.Split(',').Where(x => int.Parse(x) == idsetor).FirstOrDefault(), out valid));
                    }
                }
                else
                {
                    idsetor = _atendimentolocal.OrigemAtendimento.Setor.ID;
                    IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                    Parametro parAR = rep.OndeLiberaSetorAvaliacaoRiscoOutros().Single();
                    ValidarAR = (int.TryParse(parAR.Valor.Split(',').Where(x => int.Parse(x) == idsetor).FirstOrDefault(), out valid));
                }

                ID = (this.DataContext as Atendimento).ID;
                if (ID > 0)
                {
                    if (ValidarAR)
                    {
                        winMensagemAvaliacaoDeRiscoNaoIniciado win = new winMensagemAvaliacaoDeRiscoNaoIniciado(this.DataContext as Atendimento);
                        if (win.Inicializa(this).Equals(winMensagemAvaliacaoDeRiscoNaoIniciado.ActionResultPEP.AvaliacaoDeRisco))
                        {
                            AddDocumentPanel("Avaliação Risco", "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.AvaliacaoRisco.ucAvaliacaoRisco");
                            return;
                        }

                        //MensagemAvaliacaoDeRisco();
                        string msg = string.Empty;
                        if (App.Usuario.Prestador.TipoPrestador.IsNotNull())
                            msg = (this.DataContext as Atendimento).MensagemAvaliacaoDeRisco(false);
                        if (msg.IsNotEmptyOrWhiteSpace())
                            DXMessageBox.Show(msg, "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    if (ValidarAA && MensagemAdmissaoAssitencial())
                        return;

                    if (ValidarPE)
                    {
                        winMensagemPlanoEducacionalNaoIniciado winplano = new winMensagemPlanoEducacionalNaoIniciado(this.DataContext as Atendimento);
                        if (winplano.Inicializa(this).Equals(winMensagemPlanoEducacionalNaoIniciado.ActionResultPEP.PlanoEducacional))
                        {
                            AddDocumentPanel("Plano Educacional", "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.PlanoEducacional.ucPlanoEducacional");
                            return;
                        }
                    }

                    if (ValidaNAS)
                    {
                        winMensagemNASNaoIniciado winnas = new winMensagemNASNaoIniciado(this.DataContext as Atendimento);
                        if (winnas.Inicializa(this).Equals(winMensagemNASNaoIniciado.ActionResultPEP.NAS))
                        {
                            AddDocumentPanel("NAS", "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.NAS.ucNAS");
                            return;
                        }
                    }
                }
            }

            try
            {
                killProcessosPrescricao();
                if (this.Owner != null)
                    this.Owner.ShowInTaskbar = false;
                ProcessStartInfo psi = new ProcessStartInfo(@"C:\ORANTMV\BIN\ifrun60.EXE");
                psi.WindowStyle = ProcessWindowStyle.Maximized;
                psi.WorkingDirectory = @"E:\MV2000\PAGU";
                psi.Arguments = @"E:\MV2000\PAGU\LEXMV.FMX USR=" + App.Usuario.cd_usuario
                            + " PSW=" + App.Usuario.SenhaSistemaMV
                            + " BD=" + App.Banco
                            + @" T=E:\MV2000\PAGU\M_PREMED.FMX";

                psi.Arguments += " PM=";

                if (ID > 0)
                    psi.Arguments += "CD_ATENDIMENTO=" + ID.ToString() + ";";

                psi.Arguments += "TP_PRE_MED=E";

                PR = Process.Start(psi);
                PR.EnableRaisingEvents = true;
                PR.Exited += new EventHandler(PR_Exited);
            }
            catch
            {
                DXMessageBox.Show("Erro ao Inicializar o componente Prescrição.\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool MensagemAdmissaoAssitencial()
        {
            bool _temSumario = (this.DataContext as Atendimento).AdmissaoAssistencial.Where(x => x.Status != Status.Inativo).HasItems();

            IRepositorioDeParametrosInternet repCO = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
            ParametroInternet parametroCO = repCO.OndeOrigemParaAdmissaoCO().Single();
            if (parametroCO.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCO.valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains((this.DataContext as Atendimento).OrigemAtendimento.ID))
                    {
                        IRepositorioDeAdmissaoAssistencialCO rep = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCO>();
                        rep.OndeCodigoAtendimentoIgual((this.DataContext as Atendimento));
                        IList<AdmissaoAssistencialCO> lista = rep
                            .List().Where(x => !x.DataExclusao.HasValue)
                            .OrderByDescending(x => x.Data).ToList();
                        _temSumario = lista.HasItems();
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO deve ser inteiro e separado por virgula.");
                }
            }

            if (!_temSumario)
            {
                winMensagemAdmissaoAssitencialNaoIniciado win = new winMensagemAdmissaoAssitencialNaoIniciado((this.DataContext as Atendimento)
                    , (DateTime.Now.Subtract((this.DataContext as Atendimento).DataAtendimento).Days <= 1));
                win.ShowDialog(this.Owner);
                if (win.retAction.Equals(winMensagemAdmissaoAssitencialNaoIniciado.ActionResultPEP.Admissao))
                {
                    AddDocumentPanel("Admissão Assistencial", "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.AdmissaoAssistencial.ucAdmissaoAssistencial");
                    return true;
                }
                else if (win.retAction.Equals(winMensagemAdmissaoAssitencialNaoIniciado.ActionResultPEP.AdmissaoCO))
                {
                    AddDocumentPanel("Admissão Assistencial", "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.Views.CentroObstetrico.AdmissaoAssistencial.ucAdmissaoAssistencial");
                    return true;
                }
            }

            return false;
        }

        private static void killProcessosPrescricao()
        {
            try
            {
                //MATA o processo do relatorio MV!
                Process[] processes = Process.GetProcessesByName("RWRBE60");
                foreach (Process process in processes)
                {
                    process.Kill();
                }

                //MATA o processo da prescricao!
                Process[] processes1 = Process.GetProcessesByName("ifrun60");
                foreach (Process process in processes1)
                {
                    process.Kill();
                }
            }
            catch
            { // Nos Servidores o usuário nao tem permissão para dar KILL
            }
        }

        private void chamaDocumentosEletronicos()
        {
            if (!CompartilhamentoMV())
                return;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
            {
                Atendimento atend = this.DataContext as Atendimento;
                try
                {
                    killProcessosPrescricao();

                    IRepositorioDeSeguranca rep = ObjectFactory.GetInstance<IRepositorioDeSeguranca>();

                    ProcessStartInfo psi = new ProcessStartInfo(@"C:\ORANTMV\BIN\ifrun60.EXE");
                    psi.WindowStyle = ProcessWindowStyle.Maximized;
                    psi.WorkingDirectory = @"E:\mv2000\dbahmv";

                    psi.Arguments = @"E:\mv2000\dbahmv\HMV6130.fmx " + App.Usuario.cd_usuario + "/" + rep.ValidaSenhaMV(App.Usuario.cd_usuario, App.Usuario.SenhaSistemaMV) + "@" + App.Banco
                                + " pCd_Atendimento='" + atend.ID.ToString() + "'"
                                + " Tp_Documento='" + (atend.OrigemAtendimento.OrigemDocumento.Documento.IsNotEmptyOrWhiteSpace() ? atend.OrigemAtendimento.OrigemDocumento.Documento : string.Empty) + "'"
                                + " Tp_Uso_Resposta='" + (atend.OrigemAtendimento.OrigemDocumento.UsoDocumento.IsNotEmptyOrWhiteSpace() ? atend.OrigemAtendimento.OrigemDocumento.UsoDocumento : string.Empty) + "'";

                    PR = Process.Start(psi);
                    PR.EnableRaisingEvents = true;
                    PR.Exited += new EventHandler(PR_Exited);
                }
                catch
                {
                    DXMessageBox.Show("Erro ao Inicializar o componente Prescrição.\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void chamaDescricaoCirurgica()
        {
            if (!CompartilhamentoMV())
                return;

            killProcessosPrescricao();

            ProcessStartInfo psi = new ProcessStartInfo(@"C:\ORANTMV\BIN\ifrun60.EXE");
            psi.WindowStyle = ProcessWindowStyle.Maximized;
            psi.WorkingDirectory = @"E:\MV2000\FSCC";
            psi.Arguments = @"E:\MV2000\PAGU\LEXMV.FMX USR=" + App.Usuario.cd_usuario
                        + " PSW=" + App.Usuario.SenhaSistemaMV
                        + " BD=" + App.Banco
                        + @" T=E:\MV2000\fscc\M_DESC_CIRURGIA.FMX";

            Process PR = Process.Start(psi);
        }

        private void chamaSistemaMicroData()
        {
            if (!CompartilhamentoMV())
                return;

            int ID = 0;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
                ID = (this.DataContext as Atendimento).Paciente.ID;
            else
                ID = (this.DataContext as Paciente).ID;

            IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
            Parametro par = srv.PathMicroData();

            ProcessStartInfo psi = new ProcessStartInfo(par.Valor);
            psi.WindowStyle = ProcessWindowStyle.Maximized;
            psi.WorkingDirectory = new DirectoryInfo(par.Valor).Parent.FullName;
            psi.Arguments = ID.ToString();
            psi.UseShellExecute = false;
            Process PR = Process.Start(psi);
        }

        private void chamaPACs()
        {
            int ID = 0;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
                ID = (this.DataContext as Atendimento).Paciente.ID;
            else
                ID = (this.DataContext as Paciente).ID;

            try
            {
                //MATA o processo do CARESTREAM!
                Process[] processes = Process.GetProcessesByName("mp");
                foreach (Process process in processes)
                {
                    process.Kill();
                }

            }
            catch
            { // Nos Servidores o usuário nao tem permissão para dar KILL
            }

            IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
            Parametro srvPara = rep.OndeLinkPACs().Single();
            string uri = srvPara.Valor;
            uri = uri.Replace(":CD_PACIENTE", ID.ToString());

            //Process PR = Process.Start(uri);
            try
            {
                Process.Start("IExplore.exe", uri);
            }
            catch (Exception ex)
            {
                DXMessageBox.Show("Navegador não configurado, contate o setor responsável e solicite para que configure o Internet Explorer corretamente." + Environment.NewLine + ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chamaPACs2()
        {
            int ID = 0;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
                ID = (this.DataContext as Atendimento).Paciente.ID;
            else
                ID = (this.DataContext as Paciente).ID;

            try
            {
                //MATA o processo do CARESTREAM!
                Process[] processes = Process.GetProcessesByName("mp");
                foreach (Process process in processes)
                {
                    process.Kill();
                }

            }
            catch
            { // Nos Servidores o usuário nao tem permissão para dar KILL
            }

            IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
            Parametro srvPara = rep.OndeLinkPACs2().Single();
            string uri = srvPara.Valor;
            uri = uri.Replace(":ID_PACIENTE", ID.ToString());

            try
            {
                Process.Start("IExplore.exe", uri);
            }
            catch (Exception ex)
            {
                DXMessageBox.Show("Navegador não configurado, contate o setor responsável e solicite para que configure o Internet Explorer corretamente." + Environment.NewLine + ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuDockLayoutManager_DockItemClosed(object sender, DockItemClosedEventArgs e)
        {
            if (e.Item.GetType() == typeof(DocumentPanel))
            {
                DocumentPanel document = e.Item as DocumentPanel;
                if (document != null)
                {
                    if (document.Control.GetType() == typeof(ucSumarioAvaliacaoMedica))
                        (document.Control as ucSumarioAvaliacaoMedica).Save();
                    if (document.Control.GetType() == typeof(ucSumarioAlta))
                        (document.Control as ucSumarioAlta).Save();
                    if (document.Control.GetType() == typeof(ucImunizacao))
                        (document.Control as ucImunizacao).Save();
                    if (document.Control.GetType() == typeof(ucSumarioRN))
                        (document.Control as ucSumarioRN).Save();
                    if (document.Control.GetType() == typeof(ucSumarioCTINEO))
                        (document.Control as ucSumarioCTINEO).Save();

                    winSelAtendimentoDoSumarioDeAlta = null;
                }
            }
            HMV.Core.Framework.WPF.Memory.MinimizeMemory();
        }

        private void Window_Closing(object sender1, CancelEventArgs e)
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.Name == "winEvolucao")
                    if (DXMessageBox.Show("Há dados ainda não salvos na tela de Evolução." + Environment.NewLine + "Deseja Realmente fechar o Prontuário?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        window.Close();
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
            }

            if (IsMedico)
                if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
                    if (ObjectFactory.GetInstance<IParametroPEPService>().HabilitaTermoDeNaoCoberturaDoConvenio())
                        ImprimiTermoDeNaoCoberturaDoConvenio(this.DataContext as Atendimento);

            #region Fecha prontuario

            foreach (var tab in mdiContainer.Items)
            {
                if (tab.GetType() == typeof(DocumentPanel))
                {
                    DocumentPanel document = tab as DocumentPanel;
                    if (document != null)
                    {

                        if (document.Control.GetType() == typeof(ucSumarioAvaliacaoMedica))
                        {
                            (document.Control as ucSumarioAvaliacaoMedica).Save();
                            if ((document.Control as ucSumarioAvaliacaoMedica).CancelClose)
                            {
                                if (DXMessageBox.Show("Há dados ainda não salvos no Sumário de Avaliação Médica." + Environment.NewLine + "Deseja Continuar?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                                    e.Cancel = true;
                            }
                        }
                        else if (document.Control.GetType() == typeof(ucTelaInicio) && App.Usuario.isMedico())
                        {
                            IList<ProblemasPacienteDTO> lst = new List<ProblemasPacienteDTO>();
                            Paciente pac = null;

                            IProblemasPacienteService srv = ObjectFactory.GetInstance<IProblemasPacienteService>();
                            if (this.DataContext.GetType() == typeof(Atendimento) || this.DataContext.GetType().BaseType == typeof(Atendimento))
                            {
                                lst = srv.ListaDeProblemas((this.DataContext as Atendimento).Paciente, "ativos");
                                pac = (this.DataContext as Atendimento).Paciente;
                            }
                            else if (this.DataContext.GetType() == typeof(Paciente) || this.DataContext.GetType().BaseType == typeof(Paciente))
                            {
                                lst = srv.ListaDeProblemas((this.DataContext as Paciente), "ativos");
                                pac = this.DataContext as Paciente;
                            }

                            if (lst != null)
                            {
                                if (lst.Where(x => x.Impresso.Equals(SimNao.Nao)).Count() > 0)
                                {
                                    DXMessageBox.Show("A lista de problemas será impressa!" + Environment.NewLine + "Favor substituir a lista anterior no prontuário do paciente.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    if (pac != null)
                                    {
                                        foreach (ProblemasPaciente problema in pac.ProblemasPaciente)
                                            problema.Impresso = SimNao.Sim;

                                        IRepositorioDePacientes rep = ObjectFactory.GetInstance<IRepositorioDePacientes>();
                                        rep.Save(pac);
                                    }
                                    //winRptListaProblemas winRel = new winRptListaProblemas(lst, true, null);
                                    //winRel.Show();
                                    ImprimirListaProblemasResumo(true);
                                }
                            }
                        }
                        else if (document.Control.GetType() == typeof(ucSumarioAlta))
                        {
                            (document.Control as ucSumarioAlta).Save();
                            if ((document.Control as ucSumarioAlta).CancelClose)
                            {
                                if (DXMessageBox.Show("Há dados ainda não salvos no Sumário de Alta." + Environment.NewLine + "Deseja Continuar?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                                    e.Cancel = true;
                            }
                        }
                        else if (document.Control.GetType() == typeof(ucBoletimDeEmergencia))
                        {
                            if (((document.Control as ucBoletimDeEmergencia).DataContext as vmBoletimEmergencia).Editou)
                                if (DXMessageBox.Show("Há dados ainda não salvos no Boletim de Emergência." + Environment.NewLine + "Deseja Salvar?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    (document.Control as ucBoletimDeEmergencia).Salva(true);
                        }
                        else if (document.Control.GetType() == typeof(ucSumarioRN))
                        {
                            (document.Control as ucSumarioRN).Save();
                        }
                        else if (document.Control.GetType() == typeof(ucSumarioCTINEO))
                        {
                            (document.Control as ucSumarioCTINEO).Save();
                        }
                    }
                }
            }
            if (this.DataContext.GetType() == typeof(Atendimento) || this.DataContext.GetType().BaseType == typeof(Atendimento))
            {
                IParametroPEPService serv = ObjectFactory.GetInstance<ParametroPEPService>();

                if ((this.DataContext as Atendimento) != null && (this.DataContext as Atendimento).Leito != null && (this.DataContext as Atendimento).Leito.UnidadeInternacao != null &&
                    serv.UnidadeValidaPIM2Igual().Count(x => x.Valor == (this.DataContext as Atendimento).Leito.UnidadeInternacao.ID.ToString()) > 0)
                {
                    if ((this.DataContext as Atendimento).DataHoraAtendimento.AddDays(1) < DateTime.Now)
                    {
                        if (((this.DataContext as Atendimento).PIN2.HasItems() && (this.DataContext as Atendimento).PIN2.Count(x => x.Impresso.Equals(SimNao.Nao)) > 0) ||
                             ((this.DataContext as Atendimento).MotivosInternacao.HasItems() && (this.DataContext as Atendimento).MotivosInternacao.Count(x => x.Impresso.Equals(SimNao.Nao)) > 0))
                        {
                            DXMessageBox.Show("'PIM2' e 'Motivo de Internação' foram alterados e estão sendo impressos. " + Environment.NewLine + "Favor substituir o novo documento na pasta do paciente.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                            rptPim2 pim = new rptPim2();
                            vmMotivoInternacaoPim2 vm = new vmMotivoInternacaoPim2((this.DataContext as Atendimento), App.Usuario, true);
                            vm.ImprimiuORelatorio();
                            pim.sCabecalho.ReportSource.DataSource = vm.vmPin2.RelCabecalho;
                            pim.sListaValores.ReportSource.DataSource = vm.vmPin2.RelListaValores;
                            pim.sMotivoInternacao.ReportSource.DataSource = vm.vmMotivoDeInternacao.RelMotivoInternacao;
                            pim.sMotivoInternacao.Visible = true;
                            if (pim.sMotivoInternacao.ReportSource.DataSource == null)
                                pim.sMotivoInternacao.Visible = false;
                            pim.Print();
                        }
                    }
                }

                //ChamaImpressao evolucoes a cada X dias a partir da data de atendimento.
                IRepositorioDeParametrosClinicas repp = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                Parametro _habilita = repp.OndePEPHabilitaEvolucao().Single();
                if (_habilita.Valor == "S")
                {
                    IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                    Parametro srvPara = rep.OndePEPEvolucaoDiasImpressao().Single();
                    int maxdias = srvPara.Valor.ToInt();
                    var dias = DateTime.Now.Date.Subtract((this.DataContext as Atendimento).DataAtendimento).TotalDays;
                    if (dias > 0 && dias % maxdias == 0)
                    {
                        UIHelper.SetBusyState();
                        ChamaEvolucoes();
                        UIHelper.SetBusyState();
                    }
                }
            }

            if (mdiContainer != null && mdiContainer.Items != null && mdiContainer.Items.Count > 0 && !e.Cancel)
                mdiContainer.Items.Clear();

            HMV.Core.Framework.WPF.Memory.MinimizeMemory();
            if (timerLabel.IsNotNull())
                timerLabel.Stop();

            this.Fechou();

            #endregion
        }

        private void ChamaEvolucoes()
        {
            String[] lDados = "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.RelatorioEvolucoes".Split(';');

            string pasta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly MyAsm = Assembly.LoadFrom(Path.Combine(pasta, lDados[1]));

            System.Type lType = MyAsm.GetType(lDados[2]);
            object lRet;

            if (lType != null)
            {
                ConexaoDTO conn = new ConexaoDTO();
                conn.Banco = App.Banco;
                conn.IdUsuario = App.Usuario.cd_usuario;
                conn.Dados = new List<Atendimento>() { this.DataContext as Atendimento };

                lRet = Activator.CreateInstance(lType, BindingFlags.ExactBinding, null, null, null);

                string lpasso = string.Empty;
                try
                {
                    lpasso = "Passo1";
                    IUserControl obj = (IUserControl)lRet;
                    lpasso = "Passo2";
                    obj.SetData(conn);
                    lpasso = "Passo3";
                }
                catch (Exception e)
                {
                    DXMessageBox.Show("Não foi possível abrir o componente das Evolucoes" + Environment.NewLine + lpasso + "  " + e.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
            }
        }       

        private void ExplorerBarView_GroupExpandedChanging(object sender, NavBarGroupExpandedChangingEventArgs e)
        {
            if (!ControlaGroupExpand)
            {
                if (e.Group.Name != "grpProntuario" && e.Group.Name != "grpProcessoEnfermagem")
                    e.Cancel = true;
            }
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mdiContainer_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (btnImprimir.IsNotNull())
                btnImprimir.Visibility = System.Windows.Visibility.Collapsed;

            //
            atualizaPrevisaAlta();

            //
            if (e.Item != null)
                if (e.Item.GetType() == typeof(DocumentPanel))
                {
                    DocumentPanel document = e.Item as DocumentPanel;
                    if (document != null)
                        if (document.Control != null)
                        {
                            if (document.Control.GetType() == typeof(ucMedicamentosEmUsoPaciente))
                                (document.Control as ucMedicamentosEmUsoPaciente).RefreshMedicamentosEmUsoDoPaciente();
                            if (document.Control.GetType() == typeof(ucSumarioAvaliacaoMedica))
                                (document.Control as ucSumarioAvaliacaoMedica).RefreshMedicamentosEmUsoDoPaciente();
                            if (document.Control.GetType() == typeof(HMV.PEP.WPF.UserControls.MotivoInternacaoPin2.ucMotivoInternacao))
                                (document.Control as HMV.PEP.WPF.UserControls.MotivoInternacaoPin2.ucMotivoInternacao).RefreshMotivoInternacao();
                            if (document.Control.GetType() == typeof(ucListaDeProblemas))
                                (document.Control as ucListaDeProblemas).atualiza();
                            if (document.Control.GetType() == typeof(ucTelaInicio))
                            {
                                (document.Control as ucTelaInicio).atualiza();
                                if (btnImprimir.IsNotNull())
                                    btnImprimir.Visibility = System.Windows.Visibility.Visible;
                            }
                            //if (document.Control.GetType() == typeof(ucPin))
                            //    (document.Control as ucPin).RefreshMotivoInternacao();
                        }
                }
        }

        private bool CompartilhamentoMV()
        {
#if (DEBUG)
            return true;
#endif

            IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
            Parametro parametro = srv.PathCompartilhamentoMV();
            try
            {
                if (!Directory.Exists("E:\\"))
                {
                    DriveInfo E = new DriveInfo("E");
                    if (E.DriveType == DriveType.Network)
                    {
                        Directory.GetAccessControl(parametro.Valor);
                        Process.Start("net.exe", "use E: " + parametro.Valor);
                    }
                    else
                    {
                        DXMessageBox.Show("Entre em contato com a informatica, informando que a unidade 'E' não está mapeada para unidade de rede correta.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                DXMessageBox.Show("Entre em contato com a informatica, informando que a unidade 'E' não está mapeada.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void _verificaGermeMultiResistente(int pCodPaciente)
        {
            IGermeMultiResistenteConsult consult = ObjectFactory.GetInstance<IGermeMultiResistenteConsult>();
            bool isVisivel = consult.isGermeMultiResistente(pCodPaciente);
            lblGermeMultiResistente.SetVisible(isVisivel);

            if (isVisivel)
            {
                IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                lblGermeMultiResistente.Text = rep.OndeGermeMultiResistente().Single().Valor;
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            ImprimirListaProblemasResumo(false);
        }

        private void ImprimirListaProblemasResumo(bool pDireto)
        {
            Atendimento _atendimento = null;
            Paciente paciente = null;
            if (this.DataContext.GetType() == typeof(Atendimento) || this.DataContext.GetType().BaseType == typeof(Atendimento))
            {
                _atendimento = (Atendimento)this.DataContext;
                paciente = _atendimento.Paciente;
            }
            else
                paciente = (Paciente)DataContext;


            rptResumoPEP report = new rptResumoPEP();
            IList<RelatorioListaProblemaDTO> lista = new List<RelatorioListaProblemaDTO>();
            RelatorioListaProblemaDTO item = new RelatorioListaProblemaDTO();

            if (_atendimento.IsNotNull())
            {
                if (_atendimento.Paciente.IsNotNull())
                {
                    item.NomePaciente = _atendimento.Paciente.Nome;
                    item.IDPaciente = _atendimento.Paciente.ID;
                }

                item.NomeResumo = _atendimento.Leito.IsNotNull() ? _atendimento.Leito.Descricao : string.Empty;
                item.CodigoBarras = _atendimento.ID.ToString();

                if (_atendimento.Prestador.IsNotNull())
                {
                    item.NomePrestador = _atendimento.Prestador.Nome;
                    item.Registro = _atendimento.Prestador.Registro;
                }
            }
            else
            {
                report.BindCodigoBarras.Visible = false;
                report.BindIDPaciente.Visible = false;
            }

            item.Nome = paciente.Nome;
            item.Sexo = paciente.Sexo.ToString();
            item.Cor = paciente.Cor.HasValue ? paciente.Cor.ToString() : string.Empty;
            item.Idade = paciente.Idade.GetDate();
            item.Profissao = paciente.Profissao.IsNotNull() ? paciente.Profissao.Descricao : null;
            item.Prontuario = paciente.ID.ToString();

            item.ProblemasPaciente = paciente.ProblemasPaciente.ToList();

            var vmalergias = new vmAlergias(paciente, App.Usuario, new GetSettings().IsCorpoClinico, _atendimento);
            item.Alergias = vmalergias.ListaAlergias.ToList();

            var vmmedicamentos = new vmMedicamentosEmUsoProntuario(paciente, App.Usuario);
            item.MedicamentosEmUso = vmmedicamentos.ListaMedicamentosEmUso.ToList();

            ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();
            var procreal = (from T in consult.carregaProcedimentosRealizados(paciente)
                            orderby T.DataAtendimento descending, T.IdAtendimento descending
                            select T).ToList();
            item.ProcedimentosRealizados = procreal.ToList();
            if (paciente.PesquisaClinicaAtiva.IsNotNull())
            {
                item.listaPesquisaClinica = new List<PesquisaClinica>();
                item.listaPesquisaClinica.Add(paciente.PesquisaClinicaAtiva);
            }
            lista.Add(item);
            report.DataSource = lista;
            if (pDireto)
            {
                report.Imprime(1);
            }
            else
                report.ShowPreviewDialog();
        }

        private void atualizaPrevisaAlta()
        {
            txtPrevAlta.Visibility = System.Windows.Visibility.Hidden;
            txtPrevAltaT.Visibility = System.Windows.Visibility.Hidden;
            btnPRevAlta.Visibility = System.Windows.Visibility.Hidden;

            if (this.DataContext == null) return;

            if (typeof(Atendimento) == this.DataContext.GetType() || typeof(Atendimento) == this.DataContext.GetType().BaseType)
            {

                IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();
                int ID = rep.UltimoAtendimento((this.DataContext as Atendimento).ID);
                if (ID > 0)
                {
                    txtPrevAlta.Text = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>().FiltraPorID(ID).Single().DataPrevAlta.ToString("dd/MM/yyyy");
                    txtPrevAlta.Visibility = System.Windows.Visibility.Visible;
                    txtPrevAltaT.Visibility = System.Windows.Visibility.Visible;
                    btnPRevAlta.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void btnPRevAlta_Click(object sender, RoutedEventArgs e)
        {
            winDetalhesPrevAlta det = new winDetalhesPrevAlta(new VMPrevisaoAltaConsulta(App.Usuario, this.DataContext as Atendimento));
            det.ShowDialog(this);
        }
    }

    public class ImageTreeViewItem
    {
        public string Header { get; set; }
        public string ImageSource { get; set; }
    }

    public class ItensDaPediatria
    {
        public PIN2 Pims { get; set; }
        public MotivoInternacao Motivos { get; set; }
    }
}
