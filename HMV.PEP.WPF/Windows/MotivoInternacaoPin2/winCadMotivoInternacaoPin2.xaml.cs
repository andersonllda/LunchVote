using System.Windows;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.MotivoInternacaoPin2
{
    /// <summary>
    /// Interaction logic for winCadPin.xaml
    /// </summary>
    public partial class winCadMotivoInternacaoPin2 : WindowBase 
    {
        public winCadMotivoInternacaoPin2(vmMotivoInternacaoPim2 pvmMotivoInternacaoPin2, int TabIndex) : base(pvmMotivoInternacaoPin2)
        {         
            InitializeComponent();
            groupContainer.SelectedTabIndex = TabIndex;
            (this.DataContext as vmMotivoInternacaoPim2).vmMotivoDeInternacao.Inicializa();
        }
  
        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {           
           (this.DataContext as vmMotivoInternacaoPim2).RefreshMotivoInternacao();
            this.Close();           
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if ((DataContext as vmMotivoInternacaoPim2).Atendimento.MotivoInternacao.Count == 0)
            {
                DXMessageBox.Show("Selecione o Motivo de Internação do Paciente", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                groupContainer.SelectedTabIndex = 0;
                return;
            }
            (this.DataContext as vmMotivoInternacaoPim2).RefreshMotivoInternacao();
            this.Close();
        }
    }
}
