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
using HMV.Core.Interfaces;
using HMV.Core.Wrappers;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucExames.xaml
    /// </summary>
    public partial class ucExames : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucExames()
        {
            InitializeComponent();
        }        

        public void SetData(object pData)
        {
            this.DataContext = new vmExames((pData as vmSumarioAlta).SumarioAlta, App.Usuario);
        }        
      
        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winSelMedicamentos win = new winSelMedicamentos((this.DataContext as vmExames));
            win.ShowDialog(base.OwnerBase);
        }       

        private void txtObservacao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
           if(string.IsNullOrEmpty(txtObservacao.Text))
                label1.Content = "Máximo 4000 caracteres";
           else label1.Content = string.Format(txtObservacao.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtObservacao.Text.Length);             
        }

        private void txtAnaliseClinica_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAnaliseClinica.Text))
                label2.Content = "Máximo 4000 caracteres";
            else label2.Content = string.Format(txtAnaliseClinica.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtAnaliseClinica.Text.Length); 
        }             
    }
}
