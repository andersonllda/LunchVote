using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.DataAccess;
using HMV.PEP.WPF.Cadastros;
using HMV.PEP.Interfaces;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;


namespace HMV.PEP.WPF.Windows.Alergia
{
    /// <summary>
    /// Interaction logic for winProblemaComentarios.xaml
    /// </summary>
    public partial class winComentarios : WindowBase
    {
        public winComentarios(vmAlergias pData)
        {
            InitializeComponent();
            this.DataContext = pData;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            txtDescricao.Text = null;
            this.Close();
        }

        private void txtDescricao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
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

        private void txtComentario_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            MaximoCaracter();
        }
    }
}
