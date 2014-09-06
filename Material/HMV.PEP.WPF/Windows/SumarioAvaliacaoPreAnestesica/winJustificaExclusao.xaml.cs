using System.Windows;
using DevExpress.Xpf.Editors;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for winJustificaExclusao.xaml
    /// </summary>
    public partial class winJustificaExclusao : WindowBase
    {
        public winJustificaExclusao(ViewModelBase pVm)
            : base(pVm)
        {
            InitializeComponent();            
        }

        public winJustificaExclusao() 
        {
            InitializeComponent();
        }

        private void txtJustificativa_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (txtJustificativa.Text != null)
                label1.Content = string.Format(txtJustificativa.Text.Length < 127 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 128 - txtJustificativa.Text.Length);
        }
    }
}
