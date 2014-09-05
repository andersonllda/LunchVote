using System.Windows;
using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using DevExpress.Xpf.Grid;
using System.Windows.Input;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucCausaExterna.xaml
    /// </summary>
    public partial class ucCausaExterna : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucCausaExterna()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {            
            this.DataContext = new vmCausaExterna((pData as vmSumarioAlta).SumarioAlta);
        }        

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadCausaExterna win = new winCadCausaExterna((this.DataContext as vmCausaExterna));
            win.ShowDialog(base.OwnerBase);
            gdCausaExterna.RefreshData();
        }

        private void gdCausaExterna_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as GridControl).View.FocusedRow = (sender as GridControl).View.FocusedRow;
        }

        private void viewCausaExterna_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            (e.Row as wrpCausaExterna).Observacao = e.Value.ToString();
            //(this.DataContext as vmCausaExterna).SetaCausaExternaSelecionada((e.Row as wrpCausaExterna).Cid.DomainObject);
        }       
    }
}
