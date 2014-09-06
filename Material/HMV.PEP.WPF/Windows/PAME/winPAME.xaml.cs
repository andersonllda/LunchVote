using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.Interfaces;
using HMV.PEP.Services;
using StructureMap;
using StructureMap.Pipeline;

namespace HMV.PEP.WPF.Windows.PAME
{
    /// <summary>
    /// Interaction logic for winPAME.xaml
    /// </summary>
    public partial class winPAME : WindowBase
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private bool _liberaBotao = false;
        public bool fCarregouDataAcess;

        public winPAME()
        {
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();

            string nome = App.UsuarioDTO.NomeExibicaoProfissional;
            if (string.IsNullOrWhiteSpace(nome))
                nome = App.UsuarioDTO.Prestador.NomeExibicao;
            lblSaudacao.Text = DateTimeHelper.Saudacao() + ", " + nome;

            App.IsPAME = true;
            LoadEmergencia();
            InicializaDataAccess();
        }

        private void InicializaDataAccess()
        {
            btnEntrarProntuario.IsEnabled = false;

            ThreadStart ts1 = delegate
            {
                String stringConexao = System.Configuration.ConfigurationManager.ConnectionStrings["BANCO"].ToString();
                HMV.Core.DataAccess.SessionManager.ConfigureDataAccess(stringConexao.Replace("@BANCO", App.Banco),
                System.Configuration.ConfigurationManager.AppSettings["ConfigNHibernate"].ToString() + this.GetType().Assembly.GetName().Version.ToString().Replace(".", null));

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
                delegate
                {
                    HMV.PEP.IoC.IoCWorker.ConfigureWIN();
                    IUsuariosService serv = ObjectFactory.GetInstance<IUsuariosService>();
                    App.Usuario = serv.FiltraPorID(App.UsuarioDTO.ID);
                    fCarregouDataAcess = true;
                    // Libera Acesso aos Médicos a Prescrição
                    liberaAcessoPrescricao(true);
                    btnEntrarProntuario.IsEnabled = true;
                    _liberaBotao = true;
                    btnPAME.IsEnabled = true;
                }, null, null);
            };

            ts1.BeginInvoke(delegate(IAsyncResult aysncResult) { ts1.EndInvoke(aysncResult); }, null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            liberaAcessoPrescricao(false);
            Application.Current.Shutdown(-1);
        }

        private void btnEntrarProntuario_Click(object sender, RoutedEventArgs e)
        {
            if ((!btnEntrarProntuario.IsEnabled) || (gdPAME.GetFocusedRowCellValue("Atendimento").IsNull()))
                return;

            HMV.PEP.Interfaces.IAtendimentoService srv = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IAtendimentoService>();
            Atendimento atendimento = srv.FiltraPorID(Convert.ToInt32(gdPAME.GetFocusedRowCellValue("Atendimento")));
            winProntuario win = new winProntuario(atendimento);
            dispatcherTimer.Stop();
            win.ShowDialog(this.Owner);
            LoadEmergencia();
            dispatcherTimer.Start();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            liberaAcessoPrescricao(false);
            Application.Current.Shutdown(-1);
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnEntrarProntuario_Click(this, null);
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            LoadEmergencia();
        }

        private int TotalRegistroEncontrado(GridControl gc)
        {
            int _total = gc.VisibleRowCount - gc.GroupCount;
            return (_total >= 0 ? _total : 0);
        }

        private void gdEmergencia_FilterChanged(object sender, RoutedEventArgs e)
        {
            lbQtdRegis.Label = TotalRegistroEncontrado(gdPAME).ToString();
        }

        private void logEntradaInicialPAME()
        {
            ISistemaService srvsis = ObjectFactory.GetInstance<ISistemaService>();
            Sistemas sis = srvsis.FiltraPorId(int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString()));
            ExplicitArguments args = new ExplicitArguments();
            args.SetArg("sistema", sis.ID);
            IAcessoSistemaLogService srvlog = ObjectFactory.GetInstance<IAcessoSistemaLogService>(args);

            SistemasLog log = new SistemasLog
            {
                Sistema = sis
              ,
                Acao = Acao.Inserir
              ,
                Usuarios = App.Usuario
              ,
                Data = DateTime.Now
              ,
                Tabela = "ACESSO_PAME"
              ,
                Dispositivo = Environment.MachineName
              ,
                Observacao = new GetSettings().Versao
            };

            srvlog.Gravar(log);
        }

        private void LoadEmergencia()
        {
            /*IRepositoriovEmergenciaPAME rep = ObjectFactory.GetInstance<IRepositoriovEmergenciaPAME>();
            rep.FiltraDataAtendimentoParaEmergencia();
            rep.OrdenarPorOrdemCor();
            rep.OndeNomePacienteIgual(txtNomePaciente.Text);
            IList<vEmergenciaPAME> dados = rep.List();*/

            InicializacaoService serv = new InicializacaoService();
            IList<vEmergenciaPAME> dados = serv.BuscaPacienteEmergenciaPAME(txtNomePaciente.Text);

            int Contregistros = 0;
            if (chkAltas.IsChecked.Equals(false))
            {
                gdPAME.ItemsSource = dados.Where(x => x.DataHoraAltaBoletim.IsNull()).ToList();
                Contregistros = dados.Where(x => x.DataHoraAltaBoletim.IsNull()).ToList().Count;
            }
            else
            {
                gdPAME.ItemsSource = dados.Where(x => x.DataHoraAltaBoletim.IsNotNull()).ToList();
                Contregistros = dados.Where(x => x.DataHoraAltaBoletim.IsNotNull()).ToList().Count;
            }
            gdPAME.AutoPopulateColumns = true;
            gdPAME.RefreshData();
            lbQtdRegis.Label = (Contregistros >= 0 ? Contregistros : 0);

            if (this._liberaBotao)
                btnEntrarProntuario.IsEnabled = Contregistros > 0 ? true : false;
            gdPAME.View.MoveFocusedRow(1);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LoadEmergencia();
            gdPAME.RefreshData();
        }

        private void txtNomePaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
                LoadEmergencia();
        }

        private void txtNomePaciente_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (e.IsNotNull() && e.NewValue.IsNotNull())
                if (string.IsNullOrWhiteSpace(e.NewValue.ToString()))
                    LoadEmergencia();
        }

        private void chkAltas_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            LoadEmergencia();
        }

        private void btnPAME_Click(object sender, RoutedEventArgs e)
        {
            //if (this._liberaBotao)
            //{
            //    winPesquisaPacientePAME window = new winPesquisaPacientePAME();
            //    window.ShowDialog(this);
            //}

            winPesquisaPacientePAME wpac = new winPesquisaPacientePAME();
            wpac.ShowDialog(this);

            if (wpac.Paciente.IsNotNull())
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
                winp.ShowDialog(this);
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
    }
}
