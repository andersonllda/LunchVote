using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winPassagemPlantaoHistorico.xaml
    /// </summary>
    public partial class winPassagemPlantaoHistorico : WindowBase
    {     
        public winPassagemPlantaoHistorico(vmPassagemPlantaoEmergencia pvm)
            : base(pvm)
        {
            InitializeComponent();
            base.CancelaCloseVM = true;
        }

        private void btnFechar_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            this.Close();
        }      
    }
}
