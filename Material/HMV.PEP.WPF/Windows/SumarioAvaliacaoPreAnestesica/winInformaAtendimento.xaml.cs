using System.Windows;
using DevExpress.Xpf.Editors;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for winInformaAtendimento.xaml
    /// </summary>
    public partial class winInformaAtendimento : WindowBase
    {
        public winInformaAtendimento(ViewModelBase pVm)
            : base(pVm)
        {
            InitializeComponent();            
        }

        public winInformaAtendimento() 
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {

        }     
    }
}
