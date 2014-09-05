using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica;
using HMV.ProcessosEnfermagem.Relatorio;
using HMV.PEP.WPF.Report.SumarioDeAvaliacaoPreAnestesica;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winSumarioAtendimentoSelecionaPreAnestesica.xaml
    /// </summary>
    public partial class winSumarioAtendimentoSelecionaPreAnestesica : WindowBase
    {
        public winSumarioAtendimentoSelecionaPreAnestesica(vmSumarioAtendimento pvm)
        {
            InitializeComponent();
            this.DataContext = pvm;                     
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
              Relatorio();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            Relatorio();
        }

        public void Relatorio()
        {
            ucSumarioDeAvaliacaoPreAnestesica uc = new ucSumarioDeAvaliacaoPreAnestesica();            
            uc.SetData((this.DataContext as vmSumarioAtendimento).SumarioPreAnestesicoSelecionado);
            winRelatorio win = new winRelatorio(uc.report, true, "Sumário de Avaliação Pré-Anestésica", false);
            win.ShowDialog(null);
        }        
    }
}
