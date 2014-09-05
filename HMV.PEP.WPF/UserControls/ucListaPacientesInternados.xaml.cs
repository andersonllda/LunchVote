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
    /// Interaction logic for ucListaPacientesInternados.xaml
    /// </summary>
    public partial class ucListaPacientesInternados : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }
        public event EventHandler<EventArgs> DoubleClick;

        public ucListaPacientesInternados()
        {
            InitializeComponent();
        }

        public void Inicializa()
        {
            txtNomePaciente.Text = "";
            txtProntuario.Text = "";
            cboUnidade.SelectedIndex = -1;
            cboMedico.SelectedIndex = -1;
            LoadPacientesInternados();
        }

        private void LoadUnidadeDeInternacao()
        {
            if (cboUnidade.ItemsSource == null)
            {
                //IInternacaoService serv = ObjectFactory.GetInstance<IInternacaoService>();
                InicializacaoService serv = new InicializacaoService();
                
                List<UnidadeInternacaoDTO> dto = new List<UnidadeInternacaoDTO>();
                UnidadeInternacaoDTO uidto = new UnidadeInternacaoDTO() { Descricao = "", ID = 0 };
                dto.Add(uidto);
                foreach (var item in serv.ListaUnidadesInternacaoComPacientesInternados())
                {
                    dto.Add(new UnidadeInternacaoDTO() { Descricao = item.Descricao, ID = item.ID });
                }
                cboUnidade.ItemsSource = dto;
                cboUnidade.ValueMember = ExpressionEx.PropertyName<UnidadeInternacaoDTO>(x => x.ID);
                cboUnidade.DisplayMember = ExpressionEx.PropertyName<UnidadeInternacaoDTO>(x => x.Descricao);
            }
        }

        private void LoadMedicoAssistente()
        {
            if (cboMedico.ItemsSource == null)
            {
                //IInternacaoService srvInt = ObjectFactory.GetInstance<IInternacaoService>();
                InicializacaoService serv = new InicializacaoService();
                
                List<MedicoAssistenteDTO> DTO = new List<MedicoAssistenteDTO>();
                MedicoAssistenteDTO me = new MedicoAssistenteDTO() { Nome = "", ID = 0 };
                DTO.Add(me);
                foreach (var item in serv.ListaMedicosComPacientesInternados())
                {
                    DTO.Add(new MedicoAssistenteDTO() { ID = item.ID, Nome = item.Nome });
                }
                cboMedico.ItemsSource = DTO;
                cboMedico.ValueMember = ExpressionEx.PropertyName<MedicoAssistenteDTO>(x => x.ID);
                cboMedico.DisplayMember = ExpressionEx.PropertyName<MedicoAssistenteDTO>(x => x.Nome);
            }
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            LoadPacientesInternados();
        }

        public void LoadPacientesInternados()
        {
            int IDPaciente = 0;
            int.TryParse(txtProntuario.Text, out IDPaciente);
            int IDUnidadeInternacao = 0;
            if (cboUnidade.SelectedIndex != -1)
                IDUnidadeInternacao = ((UnidadeInternacaoDTO)cboUnidade.SelectedItem).ID;
            int IDPrestador = 0;
            if (cboMedico.SelectedIndex != -1)
                IDPrestador = ((MedicoAssistenteDTO)cboMedico.SelectedItem).ID;
            string pNomePaciente = txtNomePaciente.Text;

            //dgAtendimento.Visibility = System.Windows.Visibility.Collapsed;
            //IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();
            //IList<vPacienteInternado> lst = srv.ListaPacientesInternados(IDPaciente, pNomePaciente, IDUnidadeInternacao, IDPrestador);
            InicializacaoService serv = new InicializacaoService();
            IList<vPacienteInternado> lst = serv.ListaPacientesInternados(IDPaciente, pNomePaciente, IDUnidadeInternacao, IDPrestador);
            dgAtendimento.ItemsSource = lst;
            //gAtendimento.Visibility = System.Windows.Visibility.Visible;
        }

        private void cboMedico_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            LoadMedicoAssistente();
        }

        private void cboUnidade_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            LoadUnidadeDeInternacao();
        }

        private void txtNomePaciente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
                LoadPacientesInternados();
        }

        public vPacienteInternado GetFocusedRow()
        {
            return (vPacienteInternado)dgAtendimento.GetFocusedRow();
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

        private void cboMedico_KeyDown(object sender, KeyEventArgs e)
        {
            LoadMedicoAssistente();
        }

        private void cboUnidade_KeyDown(object sender, KeyEventArgs e)
        {
            LoadUnidadeDeInternacao();
        }
        
        private string _keys = string.Empty;
        public void SetKeys(string pkey, GridControl pGrid, string QualGrid)
        {
            if (this._keys.IsEmpty())
                Do(() => SetFocusedRow(pGrid, QualGrid), 1000);

            this._keys += pkey;
            Do(() => this._keys = string.Empty, 1200);
        }

        private void dgAtendimento_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "INTERNADOS");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "INTERNADOS");
            }
        }

        private void SetFocusedRow(GridControl pGrid, string QualGrid)
        {
            if (QualGrid.Equals("INTERNADOS"))
            {
                int IDPaciente = 0;
                int.TryParse(txtProntuario.Text, out IDPaciente);
                int IDUnidadeInternacao = 0;
                if (cboUnidade.SelectedIndex != -1)
                    IDUnidadeInternacao = ((UnidadeInternacaoDTO)cboUnidade.SelectedItem).ID;
                int IDPrestador = 0;
                if (cboMedico.SelectedIndex != -1)
                    IDPrestador = ((MedicoAssistenteDTO)cboMedico.SelectedItem).ID;
                string pNomePaciente = txtNomePaciente.Text;

                //IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();
                //IList<vPacienteInternado> lst = srv.ListaPacientesInternados(IDPaciente, pNomePaciente, IDUnidadeInternacao, IDPrestador);
                InicializacaoService serv = new InicializacaoService();
                IList<vPacienteInternado> lst = serv.ListaPacientesInternados(IDPaciente, pNomePaciente, IDUnidadeInternacao, IDPrestador);

                pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<vPacienteInternado>).Where(x => x.Paciente.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();
            }
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
