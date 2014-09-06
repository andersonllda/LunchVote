using System.Windows;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using DevExpress.Xpf.Editors;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for winComentarios.xaml
    /// </summary>
    public partial class winComentarios : WindowBase
    {
        public winComentarios(ViewModelBase pVm) : base(pVm)
        {
            InitializeComponent();
            MaximoCaracter();
        }

        public winComentarios()
        {
            InitializeComponent();
            MaximoCaracter();
        }

        private void MaximoCaracter()
        {
            int qtd = ((txtComentario.Text == null ? 0 : txtComentario.Text.Length) + (txtDescricao.Text == null ? 0 : txtDescricao.Text.Length)) + 25 + App.Usuario.nm_usuario.Length;
            qtd = qtd > 4000 ? 4000 : qtd;
            label1.Content = string.Format(qtd < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - qtd);
            qtd = 4000 - (txtComentario.Text == null ? 25 + App.Usuario.nm_usuario.Length : txtComentario.Text.Length + 25 + App.Usuario.nm_usuario.Length);
            qtd = qtd < 0 ? 0 : qtd;
            if (qtd == 0)
                txtDescricao.IsEnabled = false;
            txtDescricao.MaxLength = qtd;
        }

        private void txtDescricao_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            MaximoCaracter();
        }
    }
}
