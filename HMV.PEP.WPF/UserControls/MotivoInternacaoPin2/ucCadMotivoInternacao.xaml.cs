using HMV.Core.Framework.WPF;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.WPF.UserControls.MotivoInternacaoPin2
{
    /// <summary>
    /// Interaction logic for ucMotivoInternacao.xaml
    /// </summary>
    public partial class ucCadMotivoInternacao : UserControlBase
    {
        public ucCadMotivoInternacao()
        {
            InitializeComponent();            
        }

        private void TextBlock_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)    
        {
            EditGridCellData ts = (EditGridCellData)(sender as TextBlock).DataContext;
            (this.DataContext as vmMotivoInternacao).MotivoItem = (ts.RowData.Row as wrpMotivoSubItem);
        }       
    }
}
