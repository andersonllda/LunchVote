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
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winSelEvolucaoPadrao.xaml
    /// </summary>
    public partial class winSelEvolucaoPadrao : WindowBase
    {
        public string Descricao { get; set; }

        public winSelEvolucaoPadrao(wrpSumarioAlta pSumarioAlta)
        {
            this.DataContext = new vmEvolucaoPadrao(App.Usuario.Prestador, pSumarioAlta);
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmEvolucaoPadrao).SetaEvolucaoPadrao();
            this.Close();
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                btnConfirmar_Click(this, null);
        }
        
        private void bEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((sender as ButtonEdit).Text != string.Empty)
                gridConsulta.FilterCriteria = (new BinaryOperator("Titulo", (sender as ButtonEdit).Text + "%", BinaryOperatorType.Like));
            else
                gridConsulta.FilterCriteria = null;
        }        
    }
}
