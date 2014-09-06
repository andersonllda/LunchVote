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
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using DevExpress.Data.Filtering;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winSelRecomendacao.xaml
    /// </summary>
    public partial class winSelRecomendacao : WindowBase
    {
        public winSelRecomendacao(vmRecomendacaoPadrao pData)
        {
            InitializeComponent();
            this.DataContext = pData;
            gdRecomendacoes.AutoPopulateColumns = true;
            gdRecomendacoes.ItemsSource = pData.consultaTitulos;
            gdRecomendacoes.Columns[0].Header = "Título";
            gdRecomendacoes.Columns[0].FieldName = "Descricao";
            gdRecomendacoes.SortBy(gdRecomendacoes.Columns[0], DevExpress.Data.ColumnSortOrder.Ascending);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtFiltro_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if ((sender as TextEdit).Text != string.Empty)
            {
                gdRecomendacoes.FilterCriteria = new BinaryOperator("Descricao", (sender as TextEdit).Text + "%", BinaryOperatorType.Like);
            }
            else
                gdRecomendacoes.FilterCriteria = null;
        }

    }
}
