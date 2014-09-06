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
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.PEP.Interfaces;
using HMV.PEP.ViewModel.PEP;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.Alergia
{
    /// <summary>
    /// Interaction logic for winJustificaExclusao.xaml
    /// </summary>
    public partial class winJustificaExclusao : WindowBase
    {
        public winJustificaExclusao(vmAlergias pData) 
        {
            InitializeComponent();
            this.DataContext = pData;
            (this.DataContext as vmAlergias).BeginEdit();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {           
            (this.DataContext as vmAlergias).ComentarioSelecionado = null;
            this.Close();
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {   
            this.Close();
        }

        private void txtJustificativa_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (txtJustificativa.Text != null)
                label1.Content = string.Format(txtJustificativa.Text.Length < 127 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 128 - txtJustificativa.Text.Length);
        }
    }
}
