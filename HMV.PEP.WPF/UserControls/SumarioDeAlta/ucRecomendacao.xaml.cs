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
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucRecomendacao.xaml
    /// </summary>
    public partial class ucRecomendacao : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucRecomendacao()
        {
            InitializeComponent();
        }
        private bool _IsRN;
        public void SetData(object pData)
        {
            this._IsRN = (pData as vmSumarioAlta).IsRN;
            this.DataContext = new vmRecomendacao((pData as vmSumarioAlta).SumarioAlta, App.Usuario, this._IsRN);
        } 

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadRecomendacao win = new winCadRecomendacao(new vmRecomendacaoPadrao((this.DataContext as vmRecomendacao), this._IsRN));
            win.ShowDialog(base.OwnerBase);
        }
                      
        private void txtObservacao_EditValueChanged(object sender,EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCasoEmergencia.Text))
                label1.Content = "Máximo 2000 caracteres";
            else label1.Content = string.Format(txtCasoEmergencia.Text.Length < 1999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 2000 - txtCasoEmergencia.Text.Length); 
        }

        private void btnPadrao_Click(object sender, RoutedEventArgs e)
        {
            winSelRecomendacao win = new winSelRecomendacao(new vmRecomendacaoPadrao((this.DataContext as vmRecomendacao),this._IsRN));
            win.ShowDialog(base.OwnerBase);
        }

        private void viewRecomendacao_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Value.IsNotNull())
                (e.Row as wrpSumarioRecomendacao).Descricao = e.Value.ToString();
            else
                (e.Row as wrpSumarioRecomendacao).Descricao = string.Empty;

            (this.DataContext as vmRecomendacao).SetaListaRecomendacao((e.Row as wrpSumarioRecomendacao));
        }                
    }
}
