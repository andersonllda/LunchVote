using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.PEP.ViewModel.BoletimEmergencia;

namespace HMV.PEP.WPF.Windows.CentroObstetrico.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for winAlta.xaml
    /// </summary>
    public partial class winAlta : WindowBase
    {
        public winAlta(vmAlta pVm)
            : base(pVm)
        {
            InitializeComponent();
        }

        private void btnSalvaEfinalizar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (this.DataContext as vmAlta);

            if (vm.Valida)
            {
                winConfirmaClassificacao win = new winConfirmaClassificacao( new vmClassificacaoRisco(vm.Boletim, App.Usuario));
                win.ShowDialog(this);
                if (win.DialogResult.HasValue)
                    if (win.DialogResult.Value)
                    {
                        vm.Commands.ExecuteCommand(Core.Framework.Commands.enumCommand.CommandSalvar, null);
                        //(this.DataContext as vmAlta).Salvar();                            
                    }
            }
        }
    }
}
