using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucAvaliacaoClinica.xaml
    /// </summary>
    public partial class ucAvaliacaoClinica : UserControlBase
    {
        public ucAvaliacaoClinica()
        {
            InitializeComponent();
        }

        private void LayoutGroup_SelectedTabChildChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<System.Windows.FrameworkElement> e)
        {

        }

        public override void SetData(Core.Framework.ViewModelBaseClasses.ViewModelBase pVMObject)
        {
            vmAvaliacaoClinica vm = (this.DataContext as vmAvaliacaoClinica);
            //ucIdadeGestacional.SetData(vm.AvaliacaoClinica.GestacaoIdade);
            //ucGesta.SetData(vm.AvaliacaoClinica.Gestacao);
            ucIdadeGestacional.SetData(vm);
            ucGesta.SetData(vm);
        }
      
    }
}
