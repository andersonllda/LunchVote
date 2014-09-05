using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.WPF.Windows;
using System.Windows.Media;
using HMV.Core.Framework.WPF;


namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucMedicamentosEmUsoPaciente.xaml
    /// </summary>
    public partial class ucMedicamentosEmUsoPaciente : UserControlBase, IUserControl
    {

        object Data;
        Paciente paciente;

        public ucMedicamentosEmUsoPaciente()
        {
            InitializeComponent();
            //HMV.Core.Framework.WPF.Helpers.BindingErrorTraceListener.SetTrace();
        }

        public bool CancelClose { get; set; }

        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                paciente = (pData as Atendimento).Paciente;
                this.DataContext = new vmMedicamentosEmUsoProntuario(paciente, App.Usuario);
                this.Data = pData;
            }

            else if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {
                this.DataContext = new vmMedicamentosEmUsoProntuario(pData as Paciente, App.Usuario);
                paciente = pData as Paciente;
                this.Data = pData;
            }

            else if (typeof(SumarioAvaliacaoMedica) == pData.GetType() || typeof(SumarioAvaliacaoMedica) == pData.GetType().BaseType)
            {
                this.DataContext = new vmMedicamentosEmUsoProntuario((pData as SumarioAvaliacaoMedica).Paciente, App.Usuario);
                paciente = (pData as SumarioAvaliacaoMedica).Paciente;
                this.Data = (pData as SumarioAvaliacaoMedica).Atendimento;
            }
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            if (this.Data is Paciente)
                paciente = (Data as Paciente);
            else
                paciente = (Data as Atendimento).Paciente;

            this.DataContext = new vmMedicamentosEmUsoProntuario(paciente, App.Usuario);
            winCadMedicamentosEmUso win = new winCadMedicamentosEmUso((this.DataContext as vmMedicamentosEmUsoProntuario), null);
            win.ShowDialog(base.OwnerBase);
            (this.DataContext as vmMedicamentosEmUsoProntuario).carrega();
            gdMedicamentos.RefreshData();
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado != null)
            {
                //(this.DataContext as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado.BeginEdit();
                winCadMedicamentosEmUso win = new winCadMedicamentosEmUso((this.DataContext as vmMedicamentosEmUsoProntuario), (this.DataContext as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado);
                if (win.ShowDialog(base.OwnerBase) == true)
                {
                    //(this.DataContext as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado.EndEdit();
                    (this.DataContext as vmMedicamentosEmUsoProntuario).carrega();
                }
                else
                    (this.DataContext as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado.DomainObject.Refresh();//.CancelEdit();
            }
            else
            {
                DXMessageBox.Show("Selecione um item.", "Aviso:", MessageBoxButton.OK);
            }
            gdMedicamentos.RefreshData();
        }

        private void viewMedicamentos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (btnAlterar.IsEnabled == true)
                if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                    btnAlterar_Click(this, null);
        }

        public void RefreshMedicamentosEmUsoDoPaciente()
        {
            (this.DataContext as vmMedicamentosEmUsoProntuario).Refresh(paciente);
        }

    }
}
