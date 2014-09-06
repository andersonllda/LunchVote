using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.Helper;
using StructureMap.Pipeline;
using System.Windows.Navigation;
using System.Diagnostics;
using HMV.PEP.WPF.Report.MinhaListaPacientes;
using System.Configuration;
using HMV.PEP.WPF.Report;
using HMV.PEP.Services;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.WPF.InicioPEP;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for ucListaPacientesInternados.xaml
    /// </summary>
    public partial class ucTabMinhaListaDePaciente : UserControlBase, IUserControl
    {
        #region --- Construtor ---
        public ucTabMinhaListaDePaciente(WindowBase winPep)
        {
            InitializeComponent();
            _winPep = winPep;
            this.DataContext = this;
            loadMinhaListaDePacientes();
        }

        #endregion

        #region --- Propriedades Privadas ---
        private string _keys = string.Empty;
        private readonly static TimerCallback timer = new TimerCallback(ExecuteDelayedAction);
        private WindowBase _winPep;
        private bool _temDadosParaImprimir = false;
        private ucPacienteAgendados _ucPacienteAgendados;
        #endregion

        #region --- Propriedades Púplicas ---
        public bool CancelClose { get; set; }
        public event EventHandler<EventArgs> DoubleClick;
        public MinhaListaDTO PacienteSelecionado
        {
            get
            {
                return (MinhaListaDTO)gdMinhaLista.GetFocusedRow();
            }
        }

        public bool BotaoImprimeMinhaLista
        {
            get
            {
                return _temDadosParaImprimir;
            }
        }

        #endregion

        #region --- Métodos Privados ---
        #endregion

        #region --- Métodos Públicos ---
        public void AtualizaDados()
        {
            loadMinhaListaDePacientes();
        }

        public void SetData(object pData)
        {
            throw new NotImplementedException();
        }

        private void loadMinhaListaDePacientes()
        {
            if (App.UsuarioDTO.Prestador != null)
            {
                //IMeusPacientesService srvMeus = ObjectFactory.GetInstance<IMeusPacientesService>();
                //IList<MinhaListaDTO> lista = srvMeus.ListaMeusPacientes(App.UsuarioDTO.Prestador.ID).ToList();
                InicializacaoService serv = new InicializacaoService();
                IList<MinhaListaDTO> lista = serv.ListaMeusPacientes(App.UsuarioDTO.Prestador.ID);

                gdMinhaLista.ItemsSource = lista;
                this._temDadosParaImprimir = lista.Count > 0;

                if (_ucPacienteAgendados == null)
                {
                    _ucPacienteAgendados = new ucPacienteAgendados();
                    gridPanelAgendados.Children.Add(_ucPacienteAgendados);
                }

                IList<TaxaOcupacaoDTO> lst = serv.TaxaOcupacaoInicio();
                if (lst.Count() > 0)
                {
                    pb0.DataContext = new VMTaxaOcupacao(lst.Where(x => x.ID.Equals(1)).FirstOrDefault(), true);
                    pb1.DataContext = new VMTaxaOcupacao(lst.Where(x => x.ID.Equals(2)).FirstOrDefault());
                    pb2.DataContext = new VMTaxaOcupacao(lst.Where(x => x.ID.Equals(3)).FirstOrDefault());
                    pb3.DataContext = new VMTaxaOcupacao(lst.Where(x => x.ID.Equals(4)).FirstOrDefault());
                }


            }
        }
        #endregion

        #region --- Eventos ---
        private void gdMinhaLista_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdMinhaLista.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdMinhaLista.View).BestFitColumns()));
        }

        private void viewMinhaLista_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if ((gdMinhaLista.GetFocusedRow() != null))
                btnRemoverPaciente.IsEnabled = (gdMinhaLista.GetFocusedRow() as MinhaListaDTO).PacienteAdicionadoLista;
        }

        private void btnRemoverPaciente_Click(object sender, RoutedEventArgs e)
        {
            MinhaListaDTO paciente = gdMinhaLista.GetFocusedRow() as MinhaListaDTO;
            if (paciente != null)
            {
                if (DXMessageBox.Show("Confirma exclusão do paciente da lista ? ", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Atendimento atendimento = ObjectFactory.GetInstance<IInternacaoService>().FiltraPorId(paciente.Atendimento);
                    IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
                    Prestador prestador = serv.FiltraPorId(App.Usuario.Prestador.Id);
                    prestador.removeAtendimentoDaMinhaListaDePacientes(atendimento);
                    serv.Save(prestador);
                    loadMinhaListaDePacientes();
                }
            }
        }

        private void gdMinhaLista_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "MINHA_LISTA");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "MINHA_LISTA");
            }
        }

        public void SetKeys(string pkey, GridControl pGrid, string QualGrid)
        {
            if (this._keys.IsEmpty())
                Do(() => SetFocusedRow(pGrid, QualGrid), 1000);

            this._keys += pkey;
            Do(() => this._keys = string.Empty, 1200);
        }

        private void SetFocusedRow(GridControl pGrid, string QualGrid)
        {
            if (QualGrid.Equals("MINHA_LISTA"))
            {
                IMeusPacientesService srvMeusPacientes = ObjectFactory.GetInstance<IMeusPacientesService>();
                IList<MinhaListaDTO> MinhaListaPac = srvMeusPacientes.ListaMeusPacientes(App.Usuario.Prestador.Id).ToList();
                pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<MinhaListaDTO>).Where(x => x.Paciente.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();
            }

            this._keys = string.Empty;
        }

        public static void Do(Action action, int delay, int interval = Timeout.Infinite)
        {
            Do(action, TimeSpan.FromMilliseconds(delay), interval);
            return;
        }

        public static void Do(Action action, TimeSpan delay, int interval = Timeout.Infinite)
        {
            new Timer(timer, action, Convert.ToInt32(delay.TotalMilliseconds), interval);
            return;
        }

        private static void ExecuteDelayedAction(object o)
        {
            App.Current.Dispatcher.Invoke((o as Action));
            return;
        }

        private void viewMinhaLista_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(null, null);
        }

        private void btnImprimirListaPacientes_Click(object sender, RoutedEventArgs e)
        {
            if (App.Usuario.IsNull())
                return;

            rptMinhaListaPacientes report = new rptMinhaListaPacientes();
            report.txtProfissional.Text = App.Usuario.NomeExibicao;
            report.subPacientes.ReportSource.DataSource = RelMinhaListaPacientes;

            winRelatorio win = new winRelatorio(report, true, "Minha Lista de Pacientes.", false);
            win.ShowDialog(_winPep);
        }

        public List<PacienteMedico> RelMinhaListaPacientes
        {
            get
            {
                List<PacienteMedico> retorno = new List<PacienteMedico>();
                for (int i = 0; i < ((IList<MinhaListaDTO>)gdMinhaLista.ItemsSource).ToList().Count(); i++)
                {
                    MinhaListaDTO lst = (MinhaListaDTO)gdMinhaLista.GetRow(i);
                    if (lst.IsNotNull())
                        retorno.Add(new PacienteMedico()
                             {
                                 Nome = lst.Paciente,
                                 Prontuario = lst.Prontuario.ToString(),
                                 QuartoLeito = lst.QuartoLeito,
                                 Internacao = lst.Data.ToString("dd/MM/yyyy"),
                                 Convenio = lst.Convenio
                             }
                            );
                }
                return retorno;
            }
        }
        #endregion

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            winTaxaOcupacao win = new winTaxaOcupacao();
            win.ShowDialog(null);

        }
    }

    #region Relatorio MinhaListaPacientes
    public class PacienteMedico
    {
        public string Nome { get; set; }
        public string Prontuario { get; set; }
        public string QuartoLeito { get; set; }
        public string Internacao { get; set; }
        public string Convenio { get; set; }
    }
    #endregion
}
