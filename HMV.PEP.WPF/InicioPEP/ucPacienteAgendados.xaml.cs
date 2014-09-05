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
using HMV.Core.Framework.Exception;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.Helper;
using StructureMap.Pipeline;
using System.Windows.Navigation;
using System.Diagnostics;
using HMV.PEP.Services;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for ucListaPacientesInternados.xaml
    /// </summary>
    public partial class ucPacienteAgendados : UserControlBase, IUserControl
    {
        #region --- Construtor ---
        public ucPacienteAgendados()
        {
            InitializeComponent();
            CarregaAgendamentos(DateTime.Now.Date, DateTime.Now.Date, false);
        }
        #endregion

        #region --- Propriedades Privadas ---
        private bool jaCarregouAgenda = false;
        private Point startPosition;
        private string _keys = string.Empty;
        private readonly static TimerCallback timer = new TimerCallback(ExecuteDelayedAction);
        #endregion

        #region --- Propriedades Púplicas ---
        public bool CancelClose { get; set; }
        #endregion

        #region --- Métodos Privados ---
        private void CarregaAgendamentos(DateTime dtInicio, DateTime dtFim, bool pForce)
        {
            if (!jaCarregouAgenda || pForce)
            {

                if (dtInicio > dtFim)
                {
                    DXMessageBox.Show("Data inicial deve ser menor que a final", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (DateTimeHelper.dateDiff(0, dtInicio, dtFim) > 365)
                {
                    DXMessageBox.Show("Período de pesquisa deve ser de no maximo 1 ano", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                /*ExplicitArguments args = new ExplicitArguments();
                args.SetArg("cd_prestador", App.UsuarioDTO.Prestador.ID);
                IAgendaMedicaService repAgenda = ObjectFactory.GetInstance<IAgendaMedicaService>(args);

                IList<AgendaMedica> agenda = repAgenda.FiltraPorPeriodo(dtInicio, dtFim)
                    .CarregarAgendaSemPaginacao();*/

                InicializacaoService serv = new InicializacaoService();
                IList<AgendaMedicaDTO> agenda = serv.CarregarAgenda(App.UsuarioDTO.Prestador.ID, dtInicio, dtFim);
                gdAgendados.ItemsSource = agenda;
                jaCarregouAgenda = true;
            }
        }
        #endregion

        #region --- Métodos Públicos ---
        public void SetData(object pData)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region --- Eventos ---
        private void btnAgenda_Click(object sender, RoutedEventArgs e)
        {
            CarregaAgendamentos(deInicial.DateTime.Date, deFinal.DateTime.Date, true);
        }

        private void gdAgendados_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point location = e.GetPosition(gdAgendados);
            double hOffset = location.X - startPosition.X;
            if (FlowDirection == System.Windows.FlowDirection.RightToLeft)
                hOffset = -hOffset;

            hitInfoPopup.HorizontalOffset = hOffset;
            hitInfoPopup.VerticalOffset = location.Y - startPosition.Y;
        }

        private void hitInfoPopup_Opened(object sender, EventArgs e)
        {
            startPosition = Mouse.GetPosition(gdAgendados);
        }

        private void gdAgendados_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdAgendados.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdAgendados.View).BestFitColumns()));
        }

        private void gdAgendados_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "AGENDADOS");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "AGENDADOS");
            }
        }

        private void SetFocusedRow(GridControl pGrid, string QualGrid)
        {
            if (QualGrid.Equals("AGENDADOS"))
            {
                if (pGrid.ItemsSource.GetType() is IList<AgendaMedica> 
                    && (pGrid.ItemsSource as IList<AgendaMedica>).Count(x => x.PacienteOuNomeAgenda.ToUpper().StartsWith(this._keys.ToUpper())) > 0)
                    pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<AgendaMedica>).Where(x => x.PacienteOuNomeAgenda.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();
            }

            this._keys = string.Empty;
        }

        public void SetKeys(string pkey, GridControl pGrid, string QualGrid)
        {
            if (this._keys.IsEmpty())
                Do(() => SetFocusedRow(pGrid, QualGrid), 1000);

            this._keys += pkey;
            Do(() => this._keys = string.Empty, 1200);
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)); } //abre com o navegador padrao
            catch { Process.Start("IExplore.exe", e.Uri.AbsoluteUri); } //se não tiver navegador padrao abre com o IE
            e.Handled = true;
        }

        #endregion

    }
}
