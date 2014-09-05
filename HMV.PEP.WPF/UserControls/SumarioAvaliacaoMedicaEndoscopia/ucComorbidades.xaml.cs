using System.Windows.Input;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoMedicaEndoscopia;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaEndoscopia
{
    /// <summary>
    /// Interaction logic for ucComorbidades.xaml
    /// </summary>
    public partial class ucComorbidades : UserControlBase
    {
        public ucComorbidades()
        {            
            InitializeComponent();
            base.TelaIncluirType = new TelaType().SetType<winCadProcedimentoEndoscopia>();
        }

        private void View_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            base.ExecutaCommandAlterarSelecionar(null);
        }
    }
}
