using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.WPF.Report;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucAlergias.xaml
    /// </summary>
    public partial class ucAlergias : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucAlergias()
        {
            InitializeComponent();
            if (App.Usuario != null)
            {
                if (App.Usuario.Prestador == null)
                {
                    DesabilitaBotoes();
                }
                else if (!App.Usuario.Prestador.IsCorpoClinico)
                {
                    DesabilitaBotoes();
                }
            }
        }

        private void DesabilitaBotoes()
        {
            btnIncluir.IsEnabled = false;
            btnExcluir.IsEnabled = false;
            btnAlterar.IsEnabled = false;
            btnComentarios.IsEnabled = false;
            chkSemAlergia.IsEnabled = false;
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmAlergias).NovoRegistro();

            Windows.Alergia.winCadAlergia win = new Windows.Alergia.winCadAlergia(this.DataContext as vmAlergias);
            win.ShowDialog(base.OwnerBase);

            (this.DataContext as vmAlergias).AtualizaListaAlergias();
            this.gdAlergias.RefreshData();
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {

            Windows.Alergia.winCadAlergia win = new Windows.Alergia.winCadAlergia(this.DataContext as vmAlergias);
            win.ShowDialog(base.OwnerBase);

            (this.DataContext as vmAlergias).AtualizaListaAlergias();
            this.gdAlergias.RefreshData();
        }

        private void btnComentarios_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmAlergias).AlergiaSelecionada != null)
            {
                Windows.Alergia.winComentarios win = new Windows.Alergia.winComentarios(this.DataContext as vmAlergias);
                win.ShowDialog(base.OwnerBase);
                this.gdAlergias.RefreshData();
            }
            else
                DXMessageBox.Show("Selecione uma 'Alergia'", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Deseja realmente Excluir a Alergia?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Windows.Alergia.winJustificaExclusao win = new Windows.Alergia.winJustificaExclusao(this.DataContext as vmAlergias);
                win.ShowDialog(base.OwnerBase);
                this.gdAlergias.RefreshData();
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            vmAlergias alergias = (vmAlergias)this.DataContext;
            winRelListaProblema win = new winRelListaProblema(alergias.Paciente.DomainObject, (this.DataContext as vmAlergias).Atendimento);
            win.ShowDialog();
        }

        public void SetStyleToNull()
        {
            this.grPrincipal.Style = null;
        }

        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                this.DataContext = new vmAlergias((pData as Atendimento).Paciente, App.Usuario, new GetSettings().IsCorpoClinico, pData as Atendimento);
            }
            else if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {
                this.DataContext = new vmAlergias((pData as Paciente), App.Usuario, new GetSettings().IsCorpoClinico);
            }
            else if (typeof(SumarioAvaliacaoMedica) == pData.GetType() || typeof(SumarioAvaliacaoMedica) == pData.GetType().BaseType)
            {
                this.DataContext = new vmAlergias((pData as SumarioAvaliacaoMedica).Paciente, App.Usuario, new GetSettings().IsCorpoClinico, (pData as SumarioAvaliacaoMedica).Atendimento);
            }
        }

        private void gdAlergias_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                if (btnAlterar.IsEnabled)
                    btnAlterar_Click(this, null);
        }
    }
}
