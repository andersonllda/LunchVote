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
using HMV.PEP.ViewModel.SumarioDeAlta; 
using HMV.Core.Wrappers.ObjectWrappers;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.Receituario;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.Receituario
{
    /// <summary>
    /// Interaction logic for winSelReceituarioPadrao.xaml
    /// </summary>
    public partial class winSelReceituarioPadrao : WindowBase
    {
        public string Descricao { get; set; }

        public winSelReceituarioPadrao(vmReceituario pvm)
        {
            this.DataContext = pvm;
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            base.CancelaCloseVM = true;
            this.Close();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmReceituario).ReceituarioPadrao.IsNotNull())
            {
                (this.DataContext as vmReceituario).SetaReceituarioPadrao();
                base.CancelaCloseVM = true;
                this.Close();
            }
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                btnConfirmar_Click(this, null);
        }
        
        private void bEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((sender as ButtonEdit).Text != string.Empty)
                gridConsulta.FilterCriteria = (new BinaryOperator("Descricao", (sender as ButtonEdit).Text + "%", BinaryOperatorType.Like));
            else
                gridConsulta.FilterCriteria = null;
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmReceituario).DescricaoPadrao = string.Empty;
            (this.DataContext as vmReceituario).TextoPadrao = string.Empty;
            var win = new winCadReceituarioPadrao((this.DataContext as vmReceituario));
            win.ShowDialog(this);
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmReceituario).ExcluiReceituarioPadrao();
        }        
    }
}
