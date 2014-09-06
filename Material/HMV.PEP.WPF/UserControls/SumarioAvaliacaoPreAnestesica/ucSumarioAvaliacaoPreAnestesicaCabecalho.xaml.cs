using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for ucSumarioAvaliacaoPreanestesicaCabecalho.xaml
    /// </summary>
    public partial class ucSumarioAvaliacaoPreAnestesicaCabecalho : UserControlBase
    {
        public ucSumarioAvaliacaoPreAnestesicaCabecalho()
        {
            InitializeComponent();           
        }

        private void btnCid_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(false);
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
                (this.DataContext as vmSumarioAvaliacaoPreAnestesicaCabecalho).IncluirCid(new wrpCid(win.CID));
        }

        private void btnRemover_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Confirma exclusão do CID ? ", "Alerta", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                (this.DataContext as vmSumarioAvaliacaoPreAnestesicaCabecalho).RemoveCid();
        }
    }
}
