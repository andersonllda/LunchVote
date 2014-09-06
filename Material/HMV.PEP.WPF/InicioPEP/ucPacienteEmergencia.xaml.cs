using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Framework.Extensions;
using System.Windows;
using HMV.PEP.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using HMV.Core.Framework.Windows;
using HMV.Core.Domain.Enum;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;

namespace HMV.PEP.WPF
{
    public partial class ucPacienteEmergencia : UserControlBase, IUserControl
    {
        #region --- Construtor ---

        public ucPacienteEmergencia(ucTabEmergenciaNova pUserControl, TipoTabEmergencia pTab, EmergenciaServiceBase pService)
        {
            InitializeComponent();
            if (pTab == TipoTabEmergencia.Obstetrica)
                grupoInternados.Header = "Pacientes Internados no Centro Obstétrico";
            else
                grupoInternados.Header = "Pacientes Internados na Emergência " + pTab.GetEnumCustomDisplay();
            _service = pService;

            // Intervalo para dar refresh na tela quando inativo
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["IntervaloRefreshGridEmergencia"], out intervalo))
                MessageBox.Show("Deve ser configurado o tempo de refresh da emergência, parametro da aplicação(IntervaloRefreshGridEmergencia).");

            gridPaciente.View.NavigationStyle = GridViewNavigationStyle.None;
            gridPacienteInternado.View.NavigationStyle = GridViewNavigationStyle.None;

            rbEmAtendimento.IsChecked = true;
            threadEmergencia();

            timerLabel = new DispatcherTimer();
            timerLabel.Interval = TimeSpan.FromSeconds(1.00);
            timerLabel.Tick += new EventHandler(timerLabel_Tick);
            timerLabel.Start();
            _userControl = pUserControl;
        }

        #endregion

        #region --- Propriedades Privadas ---
        private EmergenciaServiceBase _service;
        private IList<dynamic> _listaDePaciente;
        private bool carregouInternado;
        private dynamic gridRowIndex;
        private int intervalo = 60;
        private int tempo;
        private DispatcherTimer timerLabel;
        private IScrollInfo scrollInfo;
        private double offset = 0;
        //public event EventHandler<EventArgs> DoubleClick;
        private ucTabEmergenciaNova _userControl;
        #endregion

        #region --- Metodos Privados ---
        private IScrollInfo GetScrollInfo(GridControl grid)
        {
            VisualTreeEnumerator enumerator = new VisualTreeEnumerator(grid);
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is DataPresenter)
                    return enumerator.Current as IScrollInfo;
            }
            return null;
        }

        private void gridPaciente_Loaded(object sender, RoutedEventArgs e)
        {
            //gridPaciente.DataController.ImmediateUpdateRowPosition = false;
            scrollInfo = GetScrollInfo(gridPaciente);
        }

        private void guardaFocus()
        {
            if (scrollInfo != null)
                offset = scrollInfo.VerticalOffset;

            int top = int.Parse(offset.ToString());
            gridRowIndex = new { RowSelect = TableViewEmergencia.FocusedRowHandle, TopRowSelect = top };
        }

        private void setaFocus()
        {
            try
            {
                TableViewEmergencia.FocusedRowHandle = gridRowIndex.TopRowSelect;
                TableViewEmergencia.FocusedRowHandle = gridRowIndex.RowSelect;
            }
            catch
            {
                TableViewEmergencia.FocusedRowHandle = 0;
            }
        }

        private void threadEmergencia()
        {
            new Thread(buscaDadosEmergencia).Start();
        }

        private void populaGridEmergencia()
        {
            if (txtPaciente.Text.IsNotEmptyOrWhiteSpace() && _listaDePaciente.Where(x => x.Paciente != null && x.Paciente.StartsWith(txtPaciente.Text.ToUpper())).Count() > 0)
                gridPaciente.ItemsSource = _listaDePaciente.Where(x => x.Paciente != null && x.Paciente.StartsWith(txtPaciente.Text.ToUpper())).ToList();
            else
                gridPaciente.ItemsSource = _listaDePaciente;
            //lbTotalEmergencia.Content = gridPaciente.VisibleRowCount.ToString();
        }

        private void buscaDadosEmergencia()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                gridPaciente.AutoPopulateColumns = true;

                if (rbComAlta.IsChecked.HasValue && rbComAlta.IsChecked.Value)
                    _listaDePaciente = (IList<dynamic>)_service.BuscaComAlta(string.Empty);
                else if (rbEmAtendimento.IsChecked.HasValue && rbEmAtendimento.IsChecked.Value)
                {
                    _listaDePaciente = (IList<dynamic>)_service.BuscaEmAtendimento(string.Empty);
                }
                else if (rbSemAtendimento.IsChecked.HasValue && rbSemAtendimento.IsChecked.Value)
                    _listaDePaciente = (IList<dynamic>)_service.BuscaSemAtendimento(string.Empty);

                guardaFocus();
                confirguraColunas();
                populaGridEmergencia();
                setaFocus();
                gridPaciente.View.NavigationStyle = GridViewNavigationStyle.Row;

                if (!carregouInternado)
                    threadInternados();

            }));
        }

        private void threadInternados()
        {
            carregouInternado = true;
            new Thread(buscaDadosInternado).Start();
        }

        private void buscaDadosInternado()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                gridPacienteInternado.AutoPopulateColumns = true;
                gridPacienteInternado.ItemsSource = _service.BuscaPacientesInternados();
                //lbTotalInternado.Content = gridPacienteInternado.VisibleRowCount;
            }));
        }

        private void confirguraColunas()
        {

            //A REF Ë PEOLO FIELDNAME E NAO PELO NAME!!!
            if (rbSemAtendimento.IsChecked.HasValue && rbSemAtendimento.IsChecked.Value == true)
            {
                gridPaciente.Columns["Situacao"].Visible = false;
                //gridPaciente.Columns["Paciente"].Visible = true;
                gridPaciente.Columns["Idade"].Visible = false;
                gridPaciente.Columns["DataClassificacaoRisco"].Visible = false;
                gridPaciente.Columns["CodCor"].Visible = false;
                gridPaciente.Columns["IDPaciente"].Visible = false;
                gridPaciente.Columns["DataAtendimento"].Visible = false;
                gridPaciente.Columns["HoraAtendimento"].Visible = false;
                gridPaciente.Columns["HoraAtendimentoFim"].Visible = false;
                gridPaciente.Columns["Atendimento"].Visible = false;
                gridPaciente.Columns["DataInclusao"].Visible = true;
                gridPaciente.Columns["DataTriagem"].Visible = true;
                gridPaciente.Columns["Classificacao"].Visible = true;
                gridPaciente.Columns["MotivoConsulta"].Visible = true;

                gridPaciente.Columns["Paciente"].VisibleIndex = 0;
                gridPaciente.Columns["DataInclusao"].VisibleIndex = 1;
                gridPaciente.Columns["DataTriagem"].VisibleIndex = 2;
                gridPaciente.Columns["Classificacao"].VisibleIndex = 3;
                gridPaciente.Columns["MotivoConsulta"].VisibleIndex = 4;
            }
            else
            {
                gridPaciente.Columns["Situacao"].Visible = true;
                //gridPaciente.Columns["Paciente"].Visible = true;
                gridPaciente.Columns["Idade"].Visible = true;
                gridPaciente.Columns["DataClassificacaoRisco"].Visible = true;
                gridPaciente.Columns["CodCor"].Visible = true;
                gridPaciente.Columns["IDPaciente"].Visible = true;
                gridPaciente.Columns["DataAtendimento"].Visible = true;
                gridPaciente.Columns["HoraAtendimento"].Visible = true;
                gridPaciente.Columns["HoraAtendimentoFim"].Visible = true;
                gridPaciente.Columns["Atendimento"].Visible = true;
                gridPaciente.Columns["DataInclusao"].Visible = false;
                gridPaciente.Columns["DataTriagem"].Visible = false;
                gridPaciente.Columns["Classificacao"].Visible = false;
                gridPaciente.Columns["MotivoConsulta"].Visible = false;

                gridPaciente.Columns["Situacao"].VisibleIndex = 0;
                gridPaciente.Columns["Paciente"].VisibleIndex = 1;
                gridPaciente.Columns["Idade"].VisibleIndex = 2;
                gridPaciente.Columns["DataClassificacaoRisco"].VisibleIndex = 3;
                gridPaciente.Columns["CodCor"].VisibleIndex = 4;
                gridPaciente.Columns["IDPaciente"].VisibleIndex = 5;
                gridPaciente.Columns["DataAtendimento"].VisibleIndex = 6;
                gridPaciente.Columns["HoraAtendimento"].VisibleIndex = 7;
                gridPaciente.Columns["HoraAtendimentoFim"].VisibleIndex = 8;
                gridPaciente.Columns["Atendimento"].VisibleIndex = 9;
            }
        }

        #endregion

        #region --- Metodos Publicos ---
        public void SetData(object pData)
        {
            throw new System.NotImplementedException();
        }
        public bool CancelClose
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public void Refresh()
        {
            carregouInternado = false;
            threadEmergencia();
        }

        public int AtendimentoEmergencia
        {
            get
            {
                if (gridPaciente.View.NavigationStyle == GridViewNavigationStyle.Row && gridPaciente.GetFocusedRow() != null)
                    return Convert.ToInt32(gridPaciente.GetFocusedRowCellValue("Atendimento"));
                return 0;
            }
        }
        public int AtendimentoInternadoEmergencia
        {
            get
            {
                if (gridPacienteInternado.View.NavigationStyle == GridViewNavigationStyle.Row && gridPacienteInternado.GetFocusedRow() != null)
                    return Convert.ToInt32(gridPacienteInternado.GetFocusedRowCellValue("Atendimento"));
                return 0;
            }
        }

        public void DepoisDeEntrarNoProntuario()
        {
            if (App.BuscaChaveUltimoLog("TAB_EMERGENCIA") != (int)_service.TipoTab())
                App.Log(this.GetType().Assembly, "TAB_EMERGENCIA", (int)_service.TipoTab());

            txtPaciente.Text = "";

            if (gridPaciente.View.NavigationStyle == GridViewNavigationStyle.Row)
                rbEmAtendimento.IsChecked = true;

            //gridPaciente.View.NavigationStyle = GridViewNavigationStyle.None;
            //gridPacienteInternado.View.NavigationStyle = GridViewNavigationStyle.None;

            carregouInternado = false;
            //TODO VERIFICAR SE NAO VAI AFETAR O UPDATE DOS DADOS.
            //threadEmergencia();
            timerLabel.Start();
        }

        public void StopTimer()
        {
            timerLabel.Stop();
        }

        #endregion

        #region --- Eventos ---
        private void btnPesquisa_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            populaGridEmergencia();
        }

        private void rbEmAtendimento_Click(object sender, RoutedEventArgs e)
        {
            threadEmergencia();
        }

        private void timerLabel_Tick(object sender, EventArgs e)
        {
            int timer = Convert.ToInt32(Win32.GetIdleTime() / 1000);

            if (timer > 1)
                tempo = tempo - 1;
            else
                tempo = intervalo;

            if (tempo == 0)
            {
                threadEmergencia();
                tempo = intervalo + 1;
            }

            lblAtualiza.Content = "Atualizar em: " + (tempo).ToString();
        }

        private void txtPaciente_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            populaGridEmergencia();
        }

        private void TableViewEmergencia_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                _userControl.EventoDoubleClick();
        }

        private void gridPaciente_GotFocus(object sender, RoutedEventArgs e)
        {
            gridPaciente.View.NavigationStyle = GridViewNavigationStyle.Row;
            gridPacienteInternado.View.NavigationStyle = GridViewNavigationStyle.None;
        }

        private void gridPacienteInternado_GotFocus(object sender, RoutedEventArgs e)
        {
            gridPaciente.View.NavigationStyle = GridViewNavigationStyle.None;
            gridPacienteInternado.View.NavigationStyle = GridViewNavigationStyle.Row;
        }

        #endregion
    }
}
