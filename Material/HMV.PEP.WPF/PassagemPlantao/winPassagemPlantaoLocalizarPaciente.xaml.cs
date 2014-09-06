using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winPassagemPlantaoLocalizarPaciente.xaml
    /// </summary>
    public partial class winPassagemPlantaoLocalizarPaciente : WindowBase
    {
        public winPassagemPlantaoLocalizarPaciente(vmPassagemPlantaoEmergencia pvm)
            : base(pvm)
        {
            InitializeComponent();
            base.CancelaCloseVM = true;
        }

        private void btnFechar_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            this.Close();
        }

        private void btnSalvar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
