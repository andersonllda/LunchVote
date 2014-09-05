using System.Windows;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.PEP.WPF.Cadastros;
using HMV.PEP.WPF.Cadastros.SumarioAvaliacaoM;
using HMV.PEP.WPF.Windows;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoM;

namespace HMV.PEP.WPF.UserControls.SumarioDeAvaliacaoMedicaCTINEO
{
    /// <summary>
    /// Interaction logic for ucDiagnosticoHipotese.xaml
    /// </summary>
    public partial class ucDiagnosticoHipotese : UserControlBase
    {
        public ucDiagnosticoHipotese()
        {
            InitializeComponent();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(false);
            //win.txtCodigoCid.Text = txtCIDPrincipal.Text;
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
                (this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO).CID = new Core.Wrappers.ObjectWrappers.wrpCid(win.CID);
        }

        private void btnCids_Click(object sender, RoutedEventArgs e)
        {
            winCidsSugeridos win = new winCidsSugeridos((this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO).Paciente);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadCID win = new winCadCID((this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO));
            win.ShowDialog(base.OwnerBase);
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO).RemoveDiagnostico();
        }

        private void btnIncluirCidListaProblema_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO).CID != null)
            {
                winCadCIDsNaListaDeProblemas win = new winCadCIDsNaListaDeProblemas((this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO), true);
                win.ShowDialog(base.OwnerBase);
            }
        }

        private void btnIncluirCidsListadeProblema_Click(object sender, RoutedEventArgs e)
        {
            winVisualizaListaProblema win = new winVisualizaListaProblema((this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO).Atendimento.Paciente);
            win.ShowDialog(null);
        }

        private void btnIncluirHipo_Click(object sender, RoutedEventArgs e)
        {
            winCadHiposteseDiagnostico win = new winCadHiposteseDiagnostico((this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO));
            win.ShowDialog(base.OwnerBase);

        }

        private void btnExcluirHipo_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO).RemoveHipotese();            
        }
    }
}
