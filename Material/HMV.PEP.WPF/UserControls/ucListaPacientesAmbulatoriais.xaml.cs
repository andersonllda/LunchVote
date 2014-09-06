using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HMV.Core.DataAccess;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.PEP.Services;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ucListaPacientesAmbulatoriais : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  
        public event EventHandler<EventArgs> DoubleClick;

        public ucListaPacientesAmbulatoriais()
        {
            InitializeComponent();
        }

        public void Inicializa()
        {
            //if (dgAmbulatorio.ItemsSource == null)
            txtNomePaciente.Text = "";
            txtProntuario.Text = "";
                LoadPacientes();
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            LoadPacientes();
        }

        public void LoadPacientes()
        {
            int IdPaciente;
            int.TryParse(txtProntuario.Text, out IdPaciente);
            string nomePaciente = txtNomePaciente.Text;

            InicializacaoService serv = new InicializacaoService();
            IList<PacienteAmbulatorialDTO> lst = serv.ListaPacientesAmbulatorial(IdPaciente, nomePaciente);
            dgAmbulatorio.ItemsSource = lst;            
        }

        private void txtNomePaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
                LoadPacientes();
        }

        public PacienteAmbulatorialDTO GetFocusedRow()
        {
            return (PacienteAmbulatorialDTO)dgAmbulatorio.GetFocusedRow();
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
                if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                    DoubleClick(null, null);
        }

        public void SetData(object pData)
        {

        }

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
            if (QualGrid.Equals("AMBULATORIAL"))
            {
                int IdPaciente;
                int.TryParse(txtProntuario.Text, out IdPaciente);
                string nomePaciente = txtNomePaciente.Text;

                IAmbulatorioService srv = ObjectFactory.GetInstance<IAmbulatorioService>();
                IList<PacienteAmbulatorialDTO> lst = srv.ListaPacientes(IdPaciente, nomePaciente);
                pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<PacienteAmbulatorialDTO>).Where(x => x.Paciente.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();
            }
            this._keys = string.Empty;
        }

        private void dgAmbulatorio_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "AMBULATORIAL");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "AMBULATORIAL");
            }
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
            // create a new thread timer to execute the method after the delay
            new Timer(timer, action, Convert.ToInt32(delay.TotalMilliseconds), interval);
            return;
        }

        public static void Do(Action action, int delay, int interval = Timeout.Infinite)
        {
            Do(action, TimeSpan.FromMilliseconds(delay), interval);

            return;
        }
        #endregion

    }
}
