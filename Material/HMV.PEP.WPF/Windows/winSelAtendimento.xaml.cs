using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.Cadastros
{
    /// <summary>
    /// Interaction logic for winSelAtendimento.xaml
    /// </summary>
    public partial class winSelAtendimento : WindowBase
    {
        public Atendimento Atendimento;

        public winSelAtendimento(Paciente pPaciente, TipoAtendimentoSumario pTipoAtendimentoSumario, bool pAtendimentosComAlta)
        {
            InitializeComponent();
            this.DataContext = new vmSumarioAtendimento(pPaciente, TipoAtendimentoSumario.Todos, true, pAtendimentosComAlta,App.Usuario, null);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            //(this.DataContext as vmSumarioAtendimento).Atendimento();
            this.Atendimento = (this.DataContext as vmSumarioAtendimento).Atendimento;
            if (this.Atendimento == null)
                DXMessageBox.Show("Selecione o Atendimento maior que a Data de 01/05/2007.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                this.Atendimento = (this.DataContext as vmSumarioAtendimento).Atendimento;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
