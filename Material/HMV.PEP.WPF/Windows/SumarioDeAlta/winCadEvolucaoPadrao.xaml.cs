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
using System.Windows.Shapes;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winCadEvolucaoPadrao.xaml
    /// </summary>
    public partial class winCadEvolucaoPadrao : WindowBase
    {
        public winCadEvolucaoPadrao(string pDescricao)
        {            
            InitializeComponent();
            this.DataContext = new vmEvolucaoPadrao(App.Usuario.Prestador, pDescricao);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtDescricao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtObservacao.Text))
                label1.Content = "Máximo 8000 caracteres";
            else label1.Content = string.Format(txtObservacao.Text.Length < 7999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 8000 - txtObservacao.Text.Length); 
        }            
    }
}
