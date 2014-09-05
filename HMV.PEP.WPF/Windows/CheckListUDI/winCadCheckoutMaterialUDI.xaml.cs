using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;
using HMV.ProcessosEnfermagem.ViewModel;

namespace HMV.PEP.WPF.Windows.CheckListUDI
{
    /// <summary>
    /// Interaction logic for winCadCheckoutMaterialUDI.xaml
    /// </summary>
    public partial class winCadCheckoutMaterialUDI : WindowBase
    {
        public winCadCheckoutMaterialUDI(vmCheckOut pvm)
            : base(pvm)
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGravar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as vmCheckOut).Salva(); 
            this.Close();
        }       
    }
}
