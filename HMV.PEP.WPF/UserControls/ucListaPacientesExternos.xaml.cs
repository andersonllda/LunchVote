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
using HMV.PEP.Services;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucListaPacientesExternos.xaml
    /// </summary>
    public partial class ucListaPacientesExternos : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }
        public event EventHandler<EventArgs> DoubleClick;

        public ucListaPacientesExternos()
        {
            InitializeComponent();
        }

        public void Inicializa()
        {
            txtNomePaciente.Text = "";
            txtProntuario.Text = "";
            cboSetor.SelectedIndex = -1;
            LoadPacientesExternos();
        }

        private void LoadSetores()
        {
            if (cboSetor.ItemsSource == null)
            {
                InicializacaoService serv = new InicializacaoService();

                List<UnidadeInternacaoDTO> dto = new List<UnidadeInternacaoDTO>();
                UnidadeInternacaoDTO uidto = new UnidadeInternacaoDTO() { Descricao = "", ID = 0 };
                dto.Add(uidto);
                var lista = serv.ListaSetoresPacientesExternos();
                if (lista.IsNotNull())
                    foreach (var item in lista)
                    {
                        dto.Add(new UnidadeInternacaoDTO() { Descricao = item.Descricao, ID = item.ID });
                    }
                cboSetor.ItemsSource = dto;
                cboSetor.ValueMember = ExpressionEx.PropertyName<UnidadeInternacaoDTO>(x => x.ID);
                cboSetor.DisplayMember = ExpressionEx.PropertyName<UnidadeInternacaoDTO>(x => x.Descricao);
            }
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            LoadPacientesExternos();
        }

        public void LoadPacientesExternos()
        {
            int IDPaciente = 0;
            int.TryParse(txtProntuario.Text, out IDPaciente);
            int IDSetor = 0;
            if (cboSetor.SelectedIndex != -1)
                IDSetor = ((UnidadeInternacaoDTO)cboSetor.SelectedItem).ID;

            string pNomePaciente = txtNomePaciente.Text;

            InicializacaoService serv = new InicializacaoService();
            IList<PacienteExternoDTO> lst = serv.ListaPacientesExternos(IDPaciente, pNomePaciente, IDSetor);
            dgAtendimento.ItemsSource = lst;
        }

        private void cboUnidade_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            LoadSetores();
        }

        private void txtNomePaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
                LoadPacientesExternos();
        }

        public PacienteExternoDTO GetFocusedRow()
        {
            return (PacienteExternoDTO)dgAtendimento.GetFocusedRow();
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

        private void cboUnidade_KeyDown(object sender, KeyEventArgs e)
        {
            LoadSetores();
        }

        private string _keys = string.Empty;
        public void SetKeys(string pkey, GridControl pGrid)
        {
            if (this._keys.IsEmpty())
                Do(() => SetFocusedRow(pGrid), 1000);

            this._keys += pkey;
            Do(() => this._keys = string.Empty, 1200);
        }

        private void dgAtendimento_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl));
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl));
            }
        }

        private void SetFocusedRow(GridControl pGrid)
        { 
            int IDPaciente = 0;
            int.TryParse(txtProntuario.Text, out IDPaciente);
            int IdSetor = 0;
            if (cboSetor.SelectedIndex != -1)
                IdSetor = ((UnidadeInternacaoDTO)cboSetor.SelectedItem).ID;
            string pNomePaciente = txtNomePaciente.Text;

            InicializacaoService serv = new InicializacaoService();
            IList<PacienteExternoDTO> lst = serv.ListaPacientesExternos(IDPaciente, pNomePaciente, IdSetor);

            pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<PacienteExternoDTO>).Where(x => x.Paciente.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();

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
    }
}
