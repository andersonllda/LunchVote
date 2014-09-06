using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using HMV.PEP.WPF.Windows;
using StructureMap;
using StructureMap.Pipeline;
using HMV.Core.Framework.DevExpress.v12._1.Assets.Resources;
using HMV.Core.Framework.WPF.Helpers;
using HMV.Core.WCF.Interfaces.Acesso;
using HMV.Core.DTO;
using HMV.Core.Framework.Windows;
using HMV.PEP.WPF.Windows.SCI;
using HMV.PEP.WPF.PrevisaoAlta;
using HMV.PEP.ViewModel.PEP.Evolucao;
using HMV.PEP.WPF.InicioPEP;
using DevExpress.XtraSpellChecker;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winPEP.xaml
    /// </summary>
    public partial class winPEP : WindowBase
    {
        #region --- Construtor ---
        public winPEP()
        {
            if (App.AtivaLog)
            {
                ExportFileHelper.ExportStringToTXT(App.PathLog, "Iniciou o InitializeComponent - " + DateTime.Now + Environment.NewLine +
                                                   "Duração: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoParcialLog).Minutes, (DateTime.Now - App.TempoParcialLog).Seconds, (DateTime.Now - App.TempoParcialLog).Milliseconds) + Environment.NewLine +
                                                   "Total Parcial: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoInicialLog).Minutes, (DateTime.Now - App.TempoInicialLog).Seconds, (DateTime.Now - App.TempoInicialLog).Milliseconds) + Environment.NewLine);
                App.TempoParcialLog = DateTime.Now;
            }

            InitializeComponent();

            if (App.AtivaLog)
            {
                ExportFileHelper.ExportStringToTXT(App.PathLog, "Abriu a tela inicial - " + DateTime.Now + Environment.NewLine +
                                                   "Duração: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoParcialLog).Minutes, (DateTime.Now - App.TempoParcialLog).Seconds, (DateTime.Now - App.TempoParcialLog).Milliseconds) + Environment.NewLine +
                                                   "Total Parcial: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoInicialLog).Minutes, (DateTime.Now - App.TempoInicialLog).Seconds, (DateTime.Now - App.TempoInicialLog).Milliseconds) + Environment.NewLine);
                App.TempoParcialLog = DateTime.Now;
            }

            if (App.UsuarioDTO.Prestador.IsNull())
            {
                DXMessageBox.Show("Usuário não configurado como Prestador no sistema MV, favor contate a equipe de informática", "Prontuário Eletrônico", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
                return;
            }
            else if (App.UsuarioDTO.Prestador.Registro.IsNull())
            {
                DXMessageBox.Show("Usuário configurado como Prestador no sistema MV está sem o registro, favor contate a equipe de informática", "Prontuário Eletrônico", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
                return;
            }

            string nome = App.UsuarioDTO.NomeExibicaoProfissional;
            if (string.IsNullOrWhiteSpace(nome))
                nome = App.UsuarioDTO.Prestador.NomeExibicao;
            lblSaudacao.Text = DateTimeHelper.Saudacao() + ", " + nome;

            //_ultimaTab = App.BuscaChaveUltimoLog("TAB_PEP");
            //tabMain.SelectedIndex = _ultimaTab;

            Inicializa();

            if (App.AtivaLog)
            {
                ExportFileHelper.ExportStringToTXT(App.PathLog, "Iniciou a carregar a grid principal - " + DateTime.Now + Environment.NewLine +
                                                   "Duração: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoParcialLog).Minutes, (DateTime.Now - App.TempoParcialLog).Seconds, (DateTime.Now - App.TempoParcialLog).Milliseconds) + Environment.NewLine +
                                                   "Total Parcial: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoInicialLog).Minutes, (DateTime.Now - App.TempoInicialLog).Seconds, (DateTime.Now - App.TempoInicialLog).Milliseconds) + Environment.NewLine);
                App.TempoParcialLog = DateTime.Now;
            }

            InicializaDataAccess();
            fReabrirAba = ReabrirAba.Nenhuma;

            //Abre uma nova Thread para carregar o dicionário.
            Thread tdic = new Thread(delegate()
            {
                App.SPChecker.SpellingFormType = SpellingFormType.Word;
            });
            tdic.Start();                       
        }
        #endregion

        #region --- Propriedades Privadas ---
        private int idClinicaDefault;
        private DispatcherTimer dispatcherLoad = new DispatcherTimer();
        private ucTabPesquisaPacientes _ucTabPesquisaPaciente;
        private ucTabMinhaListaDePaciente _ucTabMinhaListaDePaciente;
        //private ucTabEmergencia _ucTabEmergencia;
        private ucTabEmergenciaNova _ucTabEmergenciaNova;
        private ucTabMinhaEquipe _ucTabMinhaEquipe;
        private winProntuario prontuarioAberto;
        //private int _ultimaTab;
        private enum TabItem
        {
            MinhaLista = 0,
            PesquisaPaciente = 1,
            EmergenciaNova = 2,
            //Emergencia = 2,
            MinhaEquipe = 3,
        }
        
        public ReabrirAba fReabrirAba { get; set; }
        public Atendimento fAtendimentoReabrir { get; set; }
        public Paciente fPacienteReabrir { get; set; }
        public bool fCarregouDataAcess;
        #endregion

        #region --- Propriedades Públicas ---
        #endregion

        #region --- Métodos Privados ---
        private void BuscaIntervaloTimeOut()
        {
            //Verifica se o TimeOut está liberado
            Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeLiberaTimeOutPEP().Single();
            if (par.IsNotNull())
                if (par.Valor == "S")
                {
                    //Busca o intervalo para timeout
                    Parametro parT = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaTempoReloginBoletim().Single();
                    if (parT.IsNotNull())
                        intervalo = parT.Valor.ToInt();
                    timerLabel = new DispatcherTimer();
                    timerLabel.Interval = TimeSpan.FromSeconds(1.00);
                    timerLabel.Tick += new EventHandler(timerLogin_Tick);
                    timerLabel.Start();
                }
        }

        private void InicializaDataAccess()
        {
            btnAdd.IsEnabled = false;
            btnEntrarProntuario.IsEnabled = false;
            btnSelPaciente.IsEnabled = false;
            tabMinhaEquipe.IsEnabled = false;

            if (this._ucTabMinhaEquipe != null)
            {
                this._ucTabMinhaEquipe.btnAdicionaEquipeMedica.IsEnabled = false;
                this._ucTabMinhaEquipe.btnExcluirEquipeMedica.IsEnabled = false;
            }

            ThreadStart ts1 = delegate
            {

                String stringConexao = System.Configuration.ConfigurationManager.ConnectionStrings["BANCO"].ToString();
                HMV.Core.DataAccess.SessionManager.ConfigureDataAccess(stringConexao.Replace("@BANCO", App.Banco),
                System.Configuration.ConfigurationManager.AppSettings["ConfigNHibernate"].ToString()
                + this.GetType().Assembly.GetName().Version.ToString().Replace(".", null)
                );

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
                delegate
                {

                    HMV.PEP.IoC.IoCWorker.ConfigureWIN();
                    IUsuariosService serv = ObjectFactory.GetInstance<IUsuariosService>();
                    App.Usuario = serv.FiltraPorID(App.UsuarioDTO.ID);
                    fCarregouDataAcess = true;
                    // Libera Acesso aos Médicos a Prescrição
                    liberaAcessoPrescricao(true);

                    if (!App.UsuarioDTO.AcessoTotalProntuario)
                        btnAdd.Visibility = System.Windows.Visibility.Hidden;

                    btnAdd.IsEnabled = true;
                    btnEntrarProntuario.IsEnabled = true;
                    btnSelPaciente.IsEnabled = true;
                    tabMinhaEquipe.IsEnabled = true;
                    btnEvolucoes.IsEnabled = true;

                    if (this._ucTabMinhaEquipe != null)
                    {
                        this._ucTabMinhaEquipe.btnAdicionaEquipeMedica.IsEnabled = true;
                        this._ucTabMinhaEquipe.btnExcluirEquipeMedica.IsEnabled = true;

                    }

                    BuscaIntervaloTimeOut();
                    
                    if (App.Usuario.Prestador.IsNotNull() && (App.Usuario.Prestador.IsNurse || App.Usuario.Prestador.IsCorpoClinico))
                        btnEvolucoes.Visibility = tabMain.SelectedItem == tabEmergenciaNova ? Visibility.Visible : Visibility.Collapsed;
                    else
                        btnEvolucoes.Visibility = System.Windows.Visibility.Collapsed;

                    if (this._ucTabEmergenciaNova.IsNotNull())
                        if (App.Usuario.Prestador.IsNotNull() && (App.Usuario.Prestador.IsNurse || App.Usuario.Prestador.IsCorpoClinico))
                            if (this._ucTabEmergenciaNova.TipoTab == (int)TipoTabEmergencia.Obstetrica)
                                btnEvolucoes.Visibility = System.Windows.Visibility.Collapsed;
                            else
                                btnEvolucoes.Visibility = System.Windows.Visibility.Visible;

                }, null, null);

            };

            ts1.BeginInvoke(delegate(IAsyncResult aysncResult) { ts1.EndInvoke(aysncResult); }, null);
        }

        private void Inicializa()
        {
            //Log("Acesso PEP", null);

            GridControlLocalizer.Active = new DXGridDictionary();

            if (!int.TryParse(ConfigurationManager.AppSettings["ClinicaDefault"], out idClinicaDefault))
                throw new System.Configuration.SettingsPropertyNotFoundException("Clinica não configurada.");

            // verifica qual aba deve abrir para o usuario 
            gerenciaAbas();

            tabMain.SelectionChanged += new TabControlSelectionChangedEventHandler(tabMain_SelectionChanged);
            this.DataContext = this;

            tabMain.Visibility = Visibility.Visible;
            grdBottom.Visibility = System.Windows.Visibility.Visible;

            // Carrega aba selecionada 
            tabMain_SelectionChanged(null, null);

            if (App.AtivaLog)
            {
                ExportFileHelper.ExportStringToTXT(App.PathLog, "Terminou de carregar a grid, fim da inicialização - " + DateTime.Now + Environment.NewLine +
                                                   "Duração: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoParcialLog).Minutes, (DateTime.Now - App.TempoParcialLog).Seconds, (DateTime.Now - App.TempoParcialLog).Milliseconds) + Environment.NewLine + Environment.NewLine +
                                                   "Tempo Total: " + string.Format("{0:D2}:{1:D2}:{2:D3}", (DateTime.Now - App.TempoInicialLog).Minutes, (DateTime.Now - App.TempoInicialLog).Seconds, (DateTime.Now - App.TempoInicialLog).Milliseconds));
            }
        }

        private void cadastraUsuario()
        {
            if (App.Usuario.Usuario.IsNull())
            {
                IRepositorioDeUsuario rep = ObjectFactory.GetInstance<IRepositorioDeUsuario>();
                IRepositorioDeRecurso repre = ObjectFactory.GetInstance<IRepositorioDeRecurso>();
                IRepositorioDeClinica repcl = ObjectFactory.GetInstance<IRepositorioDeClinica>();
                IRepositorioDeSiga_Profissional repsp = ObjectFactory.GetInstance<IRepositorioDeSiga_Profissional>();

                string cdusu = string.Empty;
                var listusu = rep.List();
                while (cdusu.IsEmpty())
                {
                    var tentar = GeneratingRandomHelper.RandomString(3, false);
                    if (listusu.Count(x => x.cd_usuario == tentar) == 0)
                        cdusu = tentar;
                }

                var usuario = new Usuario()
                {
                    cd_usuario = cdusu,
                    nome = App.Usuario.Nome,
                    status = 'A',
                    username_oracle = "OPS$" + cdusu,
                    login_sigla = App.Usuario.cd_usuario,
                    senha = App.Usuario.cd_usuario
                };

                var clin = repcl.OndeIDIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).Single();
                var recurso = new Recurso()
                {
                    Descricao = App.Usuario.Prestador.Nome,
                    Clinica = clin,
                    Status = Status.Ativo,
                    TempoPadrao = 0,
                    cod_tprecurso = 1
                };

                var profissional = new Siga_Profissional()
                {
                    conselho = App.Usuario.Prestador.Conselho.ds_conselho,
                    nome = App.Usuario.Prestador.Nome,
                    Recurso = recurso,
                    registro = App.Usuario.Prestador.Registro,
                    status = "A",
                    //tratamento = "Dr.",
                    Usuario = usuario,
                    cod_tpprofissional = "ME"
                };

                Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeCodigosDoConselhoParaEnfermeiras().Single();

                if (!par.IsNull() && par.Valor.Split(',').Contains(App.Usuario.Prestador.Conselho.cd_conselho.ToString()))
                {
                    //profissional.tratamento = "Enf.";
                    profissional.cod_tpprofissional = "EN";
                }

                usuario.Profissionais = new List<Siga_Profissional>();
                usuario.Profissionais.Add(profissional);

                repre.Save(recurso);
                rep.Save(usuario);
                repsp.Save(profissional);

                IRepositorioDeUsuarios repusu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>();
                repusu.Refresh(App.Usuario);
            }
            else
            {
                if (!App.Usuario.Usuario.ExisteProfissionalVinculado(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])))
                {
                    IRepositorioDeRecurso repre = ObjectFactory.GetInstance<IRepositorioDeRecurso>();
                    IRepositorioDeClinica repcl = ObjectFactory.GetInstance<IRepositorioDeClinica>();
                    IRepositorioDeUsuario rep = ObjectFactory.GetInstance<IRepositorioDeUsuario>();
                    IRepositorioDeSiga_Profissional repsp = ObjectFactory.GetInstance<IRepositorioDeSiga_Profissional>();

                    var clin = repcl.OndeIDIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).Single();
                    var recurso = new Recurso()
                    {
                        Descricao = App.Usuario.Prestador.Nome,
                        Clinica = clin,
                        Status = Status.Ativo,
                        TempoPadrao = 0,
                        cod_tprecurso = 1
                    };

                    var profissional = new Siga_Profissional()
                    {
                        conselho = App.Usuario.Prestador.Conselho.ds_conselho,
                        nome = App.Usuario.Prestador.Nome,
                        Recurso = recurso,
                        registro = App.Usuario.Prestador.Registro,
                        status = "A",
                        //tratamento = "Dr.",
                        Usuario = App.Usuario.Usuario,
                        cod_tpprofissional = "ME"
                    };

                    Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeCodigosDoConselhoParaEnfermeiras().Single();

                    if (!par.IsNull() && par.Valor.Split(',').Contains(App.Usuario.Prestador.Conselho.cd_conselho.ToString()))
                    {
                        //profissional.tratamento = "Enf.";
                        profissional.cod_tpprofissional = "EN";
                    }

                    App.Usuario.Usuario.Profissionais.Add(profissional);

                    repre.Save(recurso);
                    rep.Save(App.Usuario.Usuario);
                    repsp.Save(profissional);

                    IRepositorioDeUsuarios repusu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>();
                    repusu.Refresh(App.Usuario);
                }
            }
        }

        private void gerenciaAbas()
        {
            tabMain.SelectedIndex = (int)TabItem.PesquisaPaciente;

            if (App.UsuarioDTO.AcessoTotalProntuario)
            {
                tabMinhaListaDePaciente.Visibility = System.Windows.Visibility.Visible;
                tabMinhaEquipe.Visibility = System.Windows.Visibility.Visible;
                btnAdd.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                tabMinhaListaDePaciente.Visibility = System.Windows.Visibility.Collapsed;
                tabMinhaEquipe.Visibility = System.Windows.Visibility.Collapsed;
                btnAdd.Visibility = System.Windows.Visibility.Hidden;
            }

            if (!App.UsuarioDTO.IsPlantonista)
            {
                tabMain.SelectedIndex = (int)TabItem.MinhaLista;
            }
        }

        private void AbreVinculoWeb(Atendimento pAtendimento)
        {
            ISumarioAvaliacaoMedicaService serv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
            var suma = serv.carregaSumarioAvaliacoesWeb(App.Usuario);

            if (suma.Count > 0)
            {
                winVinculoWeb win = new winVinculoWeb(pAtendimento);
                win.ShowDialog(this);
            }
        }

        private void liberaAcessoPrescricao(bool pLibera)
        {
            if (!fCarregouDataAcess) return;

            if (App.Usuario.IsNotNull())
            {
                IRepositorioDeSeguranca rep = ObjectFactory.GetInstance<IRepositorioDeSeguranca>();
                rep.LiberaPrescricaoParaMedicosPEP(App.Usuario.cd_usuario, pLibera);
            }
            else if (App.UsuarioDTO.IsNotNull())
            {
                IRepositorioDeSeguranca rep = ObjectFactory.GetInstance<IRepositorioDeSeguranca>();
                rep.LiberaPrescricaoParaMedicosPEP(App.UsuarioDTO.ID, pLibera);
            }
        }

        private static void msgSelecionePaciente()
        {
            DXMessageBox.Show("Selecione um Paciente", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void win_ExecuteMethod(object sender, EventArgs e)
        {
            UIHelper.SetBusyState();
            Memory.MinimizeMemory();
            //winProntuario win;

            if (fAtendimentoReabrir.IsNotNull())
                winPront = new winProntuario(fAtendimentoReabrir, fReabrirAba);
            else
                winPront = new winProntuario(fPacienteReabrir, fReabrirAba);

            prontuarioAberto = winPront;

            fReabrirAba = ReabrirAba.Nenhuma;
            fAtendimentoReabrir = null;
            fPacienteReabrir = null;
            winPront.ExecuteMethod += win_ExecuteMethod;
            UIHelper.SetBusyState();
            winPront.ShowDialog(this);

            Recarrega();
        }

        private void Recarrega()
        {
            string nome = App.UsuarioDTO.NomeExibicaoProfissional;
            if (string.IsNullOrWhiteSpace(nome))
                nome = App.UsuarioDTO.Prestador.NomeExibicao;
            lblSaudacao.Text = DateTimeHelper.Saudacao() + ", " + nome;
            Inicializa();
            InicializaDataAccess();
            fReabrirAba = ReabrirAba.Nenhuma;
            tabMain.SelectedItem = tabEmergenciaNova;
        }

        #region Timer
        //Timer Login Boletim de Emergencia        
        private DispatcherTimer timerLabel;
        private int intervalo;
        private int tempo;

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
            var win = new winLoginBoletimEmergencia();
            var ret = win.ShowDialog(this);
            if (ret.HasValue && ret.Value)
            {
                UIHelper.SetBusyState();
                Memory.MinimizeMemory();
                Recarrega();
                UIHelper.SetBusyState();
            }

            if (winPront.IsNotNull())
            {
                if (winPront.WindowState == WindowState.Minimized)
                    this.WindowState = WindowState.Minimized;

                winPront.Activate();
                winPront.Topmost = true;
                winPront.Topmost = false;
                winPront.Focus();
            }
        }
        //
        #endregion
        #endregion

        #region --- Métodos Públicos ---
        #endregion

        #region --- Eventos ---
        private void tabMain_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            btnAdd.Visibility = tabMain.SelectedItem == tabPesquisaPaciente && tabMinhaEquipe.Visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            btnEntrarProntuario.Visibility = tabMain.SelectedItem == tabMinhaEquipe ? Visibility.Collapsed : Visibility.Visible;
            btnSelPaciente.Visibility = tabMain.SelectedItem == tabMinhaEquipe ? Visibility.Collapsed : Visibility.Visible;
            btnEvolucoes.Visibility = tabMain.SelectedItem == tabEmergenciaNova ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            //if (this._ucTabEmergencia != null)
            //    this._ucTabEmergencia.StopTimer(); 

            if (tabMain.SelectedIndex == (int)TabItem.PesquisaPaciente)
            {
                if (this._ucTabPesquisaPaciente == null)
                {
                    this._ucTabPesquisaPaciente = new ucTabPesquisaPacientes(App.UsuarioDTO.AcessoTotalProntuario);
                    this._ucTabPesquisaPaciente.DoubleClick += new EventHandler<EventArgs>(_ucTabPesquisaPaciente_DoubleClick);
                    this._ucTabPesquisaPaciente.RadioButtonClick += new EventHandler<EventArgs>(_ucTabPesquisaPaciente_RadioButtonClick);
                    _ucTabPesquisaPaciente_RadioButtonClick(null, null);
                    gridTabPesquisaPaciente.Children.Add(this._ucTabPesquisaPaciente);
                }
                else
                    this._ucTabPesquisaPaciente.AtualizaDados();
            }

            if (tabMain.SelectedIndex == (int)TabItem.EmergenciaNova)
            {
                if (this._ucTabEmergenciaNova == null)
                {
                    this._ucTabEmergenciaNova = new ucTabEmergenciaNova();
                    this._ucTabEmergenciaNova.DoubleClick += new EventHandler<EventArgs>(_ucTabEmergencia_DoubleClick);
                    this._ucTabEmergenciaNova.TabChildChanged += _ucTabEmergenciaNova_TabChildChanged;
                    gridTabEmergenciaNova.Children.Add(this._ucTabEmergenciaNova);
                }
            }

            if (tabMain.SelectedIndex == (int)TabItem.MinhaEquipe)
            {
                if (this._ucTabMinhaEquipe == null)
                {
                    this._ucTabMinhaEquipe = new ucTabMinhaEquipe(this);
                    gridTabMinhaEquipe.Children.Add(this._ucTabMinhaEquipe);
                }
            }

            if (tabMain.SelectedIndex == (int)TabItem.MinhaLista)
            {
                if (this._ucTabMinhaListaDePaciente == null)
                {
                    this._ucTabMinhaListaDePaciente = new ucTabMinhaListaDePaciente(this);
                    this._ucTabMinhaListaDePaciente.DoubleClick += new EventHandler<EventArgs>(_ucTabMinhaListaDePaciente_DoubleClick);
                    gridTabMinhaListaDePaciente.Children.Add(this._ucTabMinhaListaDePaciente);
                }
                else
                    this._ucTabMinhaListaDePaciente.AtualizaDados();
            }
        }

        private void _ucTabEmergenciaNova_TabChildChanged(object sender, EventArgs e)
        {
            if (App.Usuario.IsNotNull())
                if (App.Usuario.Prestador.IsNotNull() && (App.Usuario.Prestador.IsNurse || App.Usuario.Prestador.IsCorpoClinico))
                    if (this._ucTabEmergenciaNova.TipoTab == (int)TipoTabEmergencia.Obstetrica)
                        btnEvolucoes.Visibility = System.Windows.Visibility.Collapsed;
                    else
                        btnEvolucoes.Visibility = System.Windows.Visibility.Visible;
        }

        private void _ucTabEmergencia_DoubleClick(object sender, EventArgs e)
        {
            btnEntrarProntuario_Click(null, null);
        }

        private void _ucTabPesquisaPaciente_DoubleClick(object sender, EventArgs e)
        {
            btnEntrarProntuario_Click(null, null);
        }

        private void _ucTabPesquisaPaciente_RadioButtonClick(object sender, EventArgs e)
        {
            if (this._ucTabPesquisaPaciente.Internado)
            {
                if (this._ucTabPesquisaPaciente.AcessoTotalProntuario)
                    btnAdd.Visibility = System.Windows.Visibility.Visible;
            }
            else
                btnAdd.Visibility = System.Windows.Visibility.Hidden;
        }

        private void _ucTabMinhaListaDePaciente_DoubleClick(object sender, EventArgs e)
        {
            btnEntrarProntuario_Click(null, null);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (App.Usuario.Prestador == null)
                return;

            IRepositorioDeUsuarios rep = ObjectFactory.GetInstance<IRepositorioDeUsuarios>();
            rep.Refresh(App.Usuario);

            IPrestadorService servPrestador = ObjectFactory.GetInstance<IPrestadorService>();

            vPacienteInternado pacientesDTO = this._ucTabPesquisaPaciente.pacientesDTO;
            if (pacientesDTO == null)
                return;

            IInternacaoService serv = ObjectFactory.GetInstance<IInternacaoService>();
            Atendimento atendimento = serv.FiltraPorId(pacientesDTO.Atendimento);

            try
            {
                if (App.Usuario.Prestador.getEquipeMedica().Count(x => x.Id == pacientesDTO.IDMedicoAssistente) > 0)
                {
                    App.Usuario.Prestador.addAtendimentoNaMinhaListaDePacientes(atendimento);
                    servPrestador.Save(App.Usuario.Prestador);
                }
                else
                {
                    winMsgAdicionaMinhaLista win = new winMsgAdicionaMinhaLista();
                    if (win.ShowDialog(this).Equals(true))
                    {
                        App.Usuario.Prestador.addAtendimentoNaMinhaListaDePacientes(atendimento);
                        servPrestador.Save(App.Usuario.Prestador);
                    }
                }
            }
            catch (BusinessValidatorException err)
            {
                DXMessageBox.Show(err.GetErros()[0].Message, "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelPaciente_Click(object sender, RoutedEventArgs e)
        {
            winPesquisaPaciente wpac = new winPesquisaPaciente();
            wpac.ShowDialog(this);

            if (wpac.Paciente != null)
            {
                HMV.PEP.Interfaces.IPacienteService rep = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IPacienteService>();
                Paciente pac = rep.FiltraPorID(wpac.Paciente.ID);

                IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();
                IList<vPacienteInternado> lst = srv.ListaPacientesInternados(pac.ID);

                winProntuario winp;
                if (lst.Count > 0)
                {
                    Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(lst.First().Atendimento).Single();
                    winp = new winProntuario(ate);
                }
                else
                    winp = new winProntuario(pac);

                prontuarioAberto = winp;

                winp.ShowDialog(this);
            }
        }


        winProntuario winPront;
        private void WindowBase_Activated(object sender, EventArgs e)
        {
            if (winPront.IsNotNull())
            {
                if (winPront.WindowState == WindowState.Minimized)               
                    this.WindowState = WindowState.Minimized;               
               
                winPront.Activate();
                winPront.Topmost = true;
                winPront.Topmost = false;
                winPront.Focus();                
            }            
        }

        void winPront_Closed(object sender, EventArgs e)
        {
            winPront = null;
            //grdGridGeral.IsEnabled = true;
            //this.ShowInTaskbar = true;
            //Inicia o Timer do TimeOut novamente
            timerLabel.Start();
        }

        private void WindowBase_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (winPront.IsNotNull())
            {
                if (winPront.WindowState == WindowState.Minimized)
                    this.WindowState = WindowState.Minimized;

                winPront.Activate();
                winPront.Topmost = true;
                winPront.Topmost = false;
                winPront.Focus();
            }
        }

        private void btnEntrarProntuario_Click(object sender, RoutedEventArgs e)
        {
            if (!btnEntrarProntuario.IsEnabled)
                return;

            if (winPront.IsNotNull())
            {
                winPront.Activate();
                return;
            }

            //Força o GC a limpar a memória.
            Memory.MinimizeMemory();

            UIHelper.SetBusyState();
            this.cadastraUsuario();

            //Para o Timer do TimeOut
            if (timerLabel.IsNotNull())
                timerLabel.Stop();

            HMV.PEP.Interfaces.IAtendimentoService srv = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IAtendimentoService>();

            if (tabMain.SelectedItem == tabMinhaListaDePaciente)
            {
                MinhaListaDTO currentRow = this._ucTabMinhaListaDePaciente.PacienteSelecionado;
                if (currentRow == null)
                {
                    UIHelper.SetBusyState();
                    msgSelecionePaciente();
                    return;
                }
                Atendimento atendimento = srv.FiltraPorID(currentRow.Atendimento);

                //winProntuario win = new winProntuario(atendimento);
                //win.ExecuteMethod += win_ExecuteMethod;
                winPront = new winProntuario(atendimento);
                
                prontuarioAberto = winPront;

                winPront.ExecuteMethod += win_ExecuteMethod;
                winPront.ExecuteFechou += new EventHandler(winPront_Closed);
                AbreVinculoWeb(atendimento);

                App.Log(this.GetType().Assembly, "ENTROU_PRONTUARIO", atendimento.Paciente.ID);

                winPront.Show();
                //win.ShowDialog(this);
            }
            else if (tabMain.SelectedItem == tabPesquisaPaciente)
            {
                Atendimento atendimento;

                if (this._ucTabPesquisaPaciente.Atendimento == 0)
                {
                    UIHelper.SetBusyState();
                    msgSelecionePaciente();
                    return;
                }
                else
                    atendimento = srv.FiltraPorID(this._ucTabPesquisaPaciente.Atendimento);

                Prestador prestador = ObjectFactory.GetInstance<IPrestadorService>().FiltraPorId(App.Usuario.Prestador.Id);

                if (prestador != null && prestador.Conselho.isMedico())
                {
                    if (App.Usuario.Prestador != atendimento.Prestador)
                    {
                        UIHelper.SetBusyState();
                        if (DXMessageBox.Show("O Sr(a) esta consultando a lista de pacientes de outro Médico Assistente:" + Environment.NewLine +
                                           "- O acesso a esta lista restringe-se a necessidade de atendimento desses pacientes;" + Environment.NewLine +
                                           "- O Médico Assistente será informado sobre o acesso a sua lista;" + Environment.NewLine +
                                           "Esta consulta será auditada e gravada.", "Aviso!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                            return;
                    }
                }

                //winProntuario win = new winProntuario(atendimento);
                //win.ExecuteMethod += win_ExecuteMethod;
                winPront = new winProntuario(atendimento);
               
                prontuarioAberto = winPront;

                winPront.ExecuteMethod += win_ExecuteMethod;
                winPront.ExecuteFechou += new EventHandler(winPront_Closed);
                App.Log(this.GetType().Assembly, "ENTROU_PRONTUARIO", atendimento.Paciente.ID);

                AbreVinculoWeb(atendimento);
                //win.ShowDialog(this);
                winPront.Show();
            }
            // Nova aba emergencia  
            else if (tabMain.SelectedItem == tabEmergenciaNova)
            {
                if (this._ucTabEmergenciaNova.AtendimentoEmergencia != 0)
                {
                    Atendimento atendimento = srv.FiltraPorID(this._ucTabEmergenciaNova.AtendimentoEmergencia);

                    if (atendimento == null)
                    {
                        UIHelper.SetBusyState();
                        DXMessageBox.Show("Este paciente ainda não foi admitido. Não será possivel entrar no Prontuário sem admitir o paciente.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }

                    this._ucTabEmergenciaNova.StopTimer();

                    //winProntuario win = new winProntuario(atendimento);
                    //win.ExecuteMethod += win_ExecuteMethod;
                    winPront = new winProntuario(atendimento);

                    prontuarioAberto = winPront;

                    winPront.ExecuteMethod += win_ExecuteMethod;
                    winPront.ExecuteFechou += new EventHandler(winPront_Closed);
                    App.Log(this.GetType().Assembly, "ENTROU_PRONTUARIO", atendimento.Paciente.ID);

                    AbreVinculoWeb(atendimento);
                    //win.ShowDialog(this);
                    winPront.Show();

                    this._ucTabEmergenciaNova.DepoisDeEntrarNoProntuario();
                }
                else if (this._ucTabEmergenciaNova.AtendimentoInternadoEmergencia != 0)
                {
                    Atendimento atendimento = srv.FiltraPorID(this._ucTabEmergenciaNova.AtendimentoInternadoEmergencia);
                    this._ucTabEmergenciaNova.StopTimer();

                    //winProntuario win = new winProntuario(atendimento);
                    //win.ExecuteMethod += win_ExecuteMethod;
                    winPront = new winProntuario(atendimento);
               
                    prontuarioAberto = winPront;

                    winPront.ExecuteMethod += win_ExecuteMethod;
                    winPront.ExecuteFechou += new EventHandler(winPront_Closed);
                    App.Log(this.GetType().Assembly, "ENTROU_PRONTUARIO", atendimento.Paciente.ID);

                    AbreVinculoWeb(atendimento);
                    //win.ShowDialog(this);
                    winPront.Show();
                    this._ucTabEmergenciaNova.DepoisDeEntrarNoProntuario();
                    //this._ucTabEmergenciaNova.DepoisDeEntrarNoProntuarioInternado();
                }
                else
                    msgSelecionePaciente();


                
                //grdGridGeral.IsEnabled = false;
                //this.ShowInTaskbar = false;                                                       
            }
        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            liberaAcessoPrescricao(false);
            Application.Current.Shutdown(-1);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            liberaAcessoPrescricao(false);
            this.Close();
        }

        winPassagemPlantaoEmergencia winPassagem;
        private void btnEvolucoes_Click(object sender, RoutedEventArgs e)
        {
            if (winPassagem.IsNull())
            {
                Memory.MinimizeMemory();
                //timerLabel.Stop();           
                winPassagem = new winPassagemPlantaoEmergencia((this._ucTabEmergenciaNova.TipoTab == (int)TipoTabEmergencia.Pediatrica));
                winPassagem.ExecuteFechou += new EventHandler(winPassagem_Closed);
                winPassagem.Show();
                //timerLabel.Start();
                Memory.MinimizeMemory();
            }
            else
                winPassagem.Activate();
        }

        void winPassagem_Closed(object sender, EventArgs e)
        {
            winPassagem = null;
        }
        #endregion

        private void btnSCI_Click(object sender, RoutedEventArgs e)
        {
            /*winSCI window = new winSCI();
            window.ShowDialog(this);*/
        }

        private void btnCienteMedicoSCI_Click(object sender, RoutedEventArgs e)
        {
            /*winSCICienteMedico window = new winSCICienteMedico();
            window.ShowDialog(this);*/
        }

        private void btnJustificatica_Click(object sender, RoutedEventArgs e)
        {
            /* winJustificativa window = new winJustificativa();
             window.ShowDialog(this);*/
        }
    }
}
