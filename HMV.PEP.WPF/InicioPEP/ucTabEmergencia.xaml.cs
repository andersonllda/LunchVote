using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.PEP.Services;
using System.Net;
using HMV.Core.Framework.Exception;

namespace HMV.PEP.WPF
{
    public partial class ucTabEmergencia : UserControlBase, IUserControl
    {
        #region --- Construtor ---

        public ucTabEmergencia()
        {
            InitializeComponent();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);

            rdTodos.IsChecked = true;
            rdAltas.IsChecked = false;
            rdObstetrico.IsChecked = false;
            rdSemAtendimento.IsChecked = false;
            gdEmergencia.DataController.VisibleRowCountChanged += new EventHandler(DataController_VisibleRowCountChanged);

            txtEmergenciaPorNome.Text = "";
            rdTodos.IsChecked = true;
            LoadEmergencia(true);

            gdInternadosEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
            gdEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
            LoadEmergencia(false);
        }

        #endregion

        #region --- Propriedades Privadas ---
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        #endregion

        #region --- Propriedades Púplicas ---
        public bool CancelClose { get; set; }
        public event EventHandler<EventArgs> DoubleClick;
        public int AtendimentoEmergencia
        {
            get
            {
                if (gdEmergencia.View.NavigationStyle == GridViewNavigationStyle.Row && gdEmergencia.GetFocusedRow() != null)
                    return Convert.ToInt32(gdEmergencia.GetFocusedRowCellValue("Atendimento"));
                return 0;
            }
        }
        public int AtendimentoInternadoEmergencia
        {
            get
            {
                if (gdInternadosEmergencia.View.NavigationStyle == GridViewNavigationStyle.Row && gdInternadosEmergencia.GetFocusedRow() != null)
                    return Convert.ToInt32(gdInternadosEmergencia.GetFocusedRowCellValue("Atendimento"));
                return 0;
            }
        }
        #endregion

        #region --- Métodos Privados ---
        private void ConfirguraColunas(Boolean _CheckSemAtend)
        {
            if (!_CheckSemAtend)
            {
                gdEmergencia.Columns["TipoDoPaciente"].Visible = true;
                gdEmergencia.Columns["Paciente"].Visible = true;
                gdEmergencia.Columns["DataClassificacaoRisco"].Visible = true;
                gdEmergencia.Columns["CodCor"].Visible = true;
                gdEmergencia.Columns["IDPaciente"].Visible = true;
                gdEmergencia.Columns["DataAtendimento"].Visible = true;
                gdEmergencia.Columns["Atendimento"].Visible = true;

                gdEmergencia.Columns["DataInclusao"].Visible = false;
                gdEmergencia.Columns["DataTriagem"].Visible = false;
                gdEmergencia.Columns["Classificacao"].Visible = false;
                gdEmergencia.Columns["MotivoConsulta"].Visible = false;

                gdEmergencia.Columns["HoraAtendimento"].Visible = true;
                gdEmergencia.Columns["HoraAtendimentoFim"].Visible = true;
            }
            else
            {
                gdEmergencia.Columns["TipoDoPaciente"].Visible = false;
                gdEmergencia.Columns["DataClassificacaoRisco"].Visible = false;
                gdEmergencia.Columns["CodCor"].Visible = false;
                gdEmergencia.Columns["IDPaciente"].Visible = false;
                gdEmergencia.Columns["DataAtendimento"].Visible = false;
                gdEmergencia.Columns["Atendimento"].Visible = false;

                gdEmergencia.Columns["Paciente"].Visible = true;
                gdEmergencia.Columns["DataInclusao"].Visible = true;
                gdEmergencia.Columns["DataTriagem"].Visible = true;
                gdEmergencia.Columns["Classificacao"].Visible = true;
                gdEmergencia.Columns["MotivoConsulta"].Visible = true;

                gdEmergencia.Columns["HoraAtendimento"].Visible = false;
                gdEmergencia.Columns["HoraAtendimentoFim"].Visible = false;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            LoadEmergencia(false);
            gdEmergencia.RefreshData();
        }

        private void MontaGridAtendimento(bool _CheckAlta, bool _CheckObstetrico, bool _CheckTodos)
        {
            /*IRepositoriovEmergenciaPEP rep = ObjectFactory.GetInstance<IRepositoriovEmergenciaPEP>();
                                    rep.FiltraDataAtendimentoParaEmergencia();
                                    rep.FiltraObstetrico(_CheckObstetrico);
                                    rep.FiltraAltaBoletim(_CheckAlta);
                                    rep.OrdenarPorOrdemCor();
                                    IList<vEmergenciaPEP> dados = rep.List().Where(x => x.Paciente.StartsWith(txtEmergenciaPorNome.Text.ToUpper())).ToList();*/

            //TFS3149:
            //if (_CheckAlta)
               //_CheckObstetrico = true;

            InicializacaoService serv = new InicializacaoService();
            IList<vEmergenciaPEP> dados = new List<vEmergenciaPEP>();

            if (_CheckAlta)
            {
                // busca altas não obstetricos 
                List<vEmergenciaPEP> lista = new List<vEmergenciaPEP>(serv.BuscaPacientesNaEmergencia(false, _CheckAlta, txtEmergenciaPorNome.Text.ToUpper()));

                // busca altas obstetricos 
                lista.AddRange(serv.BuscaPacientesNaEmergencia(true, _CheckAlta, txtEmergenciaPorNome.Text.ToUpper()));

                dados = lista.ToList();
            }
            else
            {
                dados = serv.BuscaPacientesNaEmergencia(_CheckObstetrico, _CheckAlta, txtEmergenciaPorNome.Text.ToUpper());
            }
            
            gdEmergencia.AutoPopulateColumns = true;
            gdEmergencia.ItemsSource = dados;

            int Contregistros = dados.Count();
            lbQtdRegis.Content = (Contregistros >= 0 ? Contregistros : 0);
        }

        private void MontaSemAtendimento()
        {
            InicializacaoService serv = new InicializacaoService();
            IList<PacienteEmergenciaDTO> lstAtend = serv.BuscaSemAtendimentos();

            /*IEmergenciaService emerServ = ObjectFactory.GetInstance<IEmergenciaService>();
                                    IList<PacienteEmergencia> lstAtend = emerServ.BuscaSemAtendimentos();

                                    var atendEmerg = (from x in lstAtend
                                                      select new
                                                      {
                                                          Paciente = x.Nome,
                                                          DataInclusao = x.DataInclusao,
                                                          DataTriagem = x.DataTriagem,
                                                          Classificacao = x.CorRisco.Descricao,
                                                          MotivoConsulta = x.MotivoInternacao,
                                                          OrdemCor = x.CorRisco.Ordem
                                                      });*/

            gdEmergencia.AutoPopulateColumns = true;
            gdEmergencia.ItemsSource = lstAtend;

            int Contregistros = lstAtend.Count();
            lbQtdRegis.Content = (Contregistros >= 0 ? Contregistros : 0);
        }
        private void LoadEmergencia(Boolean boolEmer)
        {
            //loadPacEmerg.Visibility = System.Windows.Visibility.Visible;
            //
            Boolean _CheckAlta = rdAltas.IsChecked.Value;
            Boolean _CheckObstetrico = rdObstetrico.IsChecked.Value;
            Boolean _CheckSemAtend = rdSemAtendimento.IsChecked.Value;
            Boolean _CheckTodos = rdTodos.IsChecked.Value;
            //
            ConfirguraColunas(_CheckSemAtend);


            if (!_CheckSemAtend)
                MontaGridAtendimento(_CheckAlta, _CheckObstetrico, _CheckTodos);
            else
                MontaSemAtendimento();

            if (!boolEmer)
            {
                //IEmergenciaService emerServ = ObjectFactory.GetInstance<IEmergenciaService>();

                InicializacaoService emerServ = new InicializacaoService();
                IList<vPacienteInternado> lstInternadosAtend = emerServ.BuscaAtendimentosInternados();
                gdInternadosEmergencia.AutoPopulateColumns = true;
                gdInternadosEmergencia.ItemsSource = lstInternadosAtend;
                lbQtdPIE.Content = TotalRegistroEncontrado(gdInternadosEmergencia).ToString();
                gdInternadosEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
            }

            gdEmergencia.View.NavigationStyle = GridViewNavigationStyle.Row;
        }

        private void gdEmergencia_FilterChanged(object sender, RoutedEventArgs e)
        {
            lbQtdRegis.Content = TotalRegistroEncontradoEmergencia(gdEmergencia).ToString();
        }

        private void gdInternadosEmergencia_FilterChanged(object sender, RoutedEventArgs e)
        {
            lbQtdPIE.Content = TotalRegistroEncontrado(gdInternadosEmergencia).ToString();
        }

        private int TotalRegistroEncontrado(GridControl gc)
        {
            int _total = gc.VisibleRowCount - gc.GroupCount;
            return (_total >= 0 ? _total : 0);
        }

        private int TotalRegistroEncontradoEmergencia(GridControl gc)
        {
            int _total = gc.VisibleRowCount;
            return (_total >= 0 ? _total : 0);
        }

        #endregion

        #region --- Métodos Públicos ---
        public void StartTimer()
        {
            dispatcherTimer.Start();
        }
        public void StopTimer()
        {
            dispatcherTimer.Stop();
        }

        public void SetData(object pData)
        {
            throw new NotImplementedException();
        }
        public void DepoisDeEntrarNoProntuario()
        {
            gdEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
            dispatcherTimer.Start();
            txtEmergenciaPorNome.Text = "";
            rdTodos.IsChecked = true;
            LoadEmergencia(true);
        }
        public void DepoisDeEntrarNoProntuarioInternado()
        {
            gdInternadosEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
        }
        #endregion

        #region --- Eventos ---
        void DataController_VisibleRowCountChanged(object sender, EventArgs e)
        {

            lbQtdRegis.Content = gdEmergencia.VisibleRowCount.ToString();
        }

        private void gdEmergencia_GotFocus(object sender, RoutedEventArgs e)
        {
            gdEmergencia.View.NavigationStyle = GridViewNavigationStyle.Row;
            gdInternadosEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
        }

        private void gdInternadosEmergencia_GotFocus(object sender, RoutedEventArgs e)
        {
            gdEmergencia.View.NavigationStyle = GridViewNavigationStyle.None;
            gdInternadosEmergencia.View.NavigationStyle = GridViewNavigationStyle.Row;
        }

        private void gdEmergencia_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdEmergencia.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdEmergencia.View).BestFitColumns()));
        }

        private void gdInternadosEmergencia_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdInternadosEmergencia.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdInternadosEmergencia.View).BestFitColumns()));
        }

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEmergencia(true);
        }

        private void txtEmergenciaPorNome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
                LoadEmergencia(true);
        }

        private void TableViewEmergencia_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(null, null);
        }

        private void dockManager_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        #region SETFOCUS por Tecla!
        private string _keys = string.Empty;
        public void SetKeys(string pkey, GridControl pGrid, string QualGrid)
        {
            if (this._keys.IsEmpty())
                Do(() => SetFocusedRow(pGrid, QualGrid), 1000);

            this._keys += pkey;
            Do(() => this._keys = string.Empty, 1200);
        }

        private void SetFocusedRow(GridControl pGrid, string QualGrid)
        {
            if (QualGrid.Equals("EMERGENCIA"))
                pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<vEmergenciaPEP>).Where(x => x.Paciente.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();
            else
                if (QualGrid.Equals("INTERNADOS_EMERGENCIA"))
                    pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<vPacienteInternado>).Where(x => x.Paciente.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();
            this._keys = string.Empty;
        }

        #region DelayAction
        private readonly static TimerCallback timer = new TimerCallback(ExecuteDelayedAction);

        private static void ExecuteDelayedAction(object o)
        {
            App.Current.Dispatcher.Invoke((o as Action));
            return;
        }

        public static void Do(Action action, TimeSpan delay, int interval = Timeout.Infinite)
        {
            new Timer(timer, action, Convert.ToInt32(delay.TotalMilliseconds), interval);
            return;
        }

        public static void Do(Action action, int delay, int interval = Timeout.Infinite)
        {
            Do(action, TimeSpan.FromMilliseconds(delay), interval);
            return;
        }

        #endregion

        #endregion

        private void gdInternadosEmergencia_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "INTERNADOS_EMERGENCIA");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "INTERNADOS_EMERGENCIA");
            }
        }

        private void gdEmergencia_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            e.Result = Comparer<GridColumn>.Default.Compare((sender as GridControl).Columns["OrdemCor"], (sender as GridControl).Columns["OrdemCor"]);
            e.Handled = true;
        }

        private void gdEmergencia_EndSorting(object sender, RoutedEventArgs e)
        {
            (sender as GridControl).View.FocusedRowHandle = 0;
        }

        private void gdEmergencia_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "EMERGENCIA");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "EMERGENCIA");
            }
        }

        private void rdPesquisa_Checked(object sender, RoutedEventArgs e)
        {
            LoadEmergencia(true);
        }

        #endregion
    }
}
