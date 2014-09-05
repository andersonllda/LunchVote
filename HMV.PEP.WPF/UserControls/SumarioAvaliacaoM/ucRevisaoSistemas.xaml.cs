using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.Core.DTO;
using HMV.Core.Interfaces;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoM;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucRevisaoSistemas.xaml
    /// </summary>
    public partial class ucRevisaoSistemas : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucRevisaoSistemas()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).RevisaoDeSistemas;
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdRevisaoSistema.ItemsSource)
                item.SemParticularidades = string.IsNullOrWhiteSpace(item.Observacoes) ? ((CheckEdit)sender).IsChecked.Value : item.SemParticularidades;

            (this.DataContext as RevisaoDeSistemasPEP).ItensRevisaoDeSistemas = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdRevisaoSistema.ItemsSource;
            RevisaoDeSistemasPEP revisao = (RevisaoDeSistemasPEP)this.DataContext;
            this.DataContext = null;
            this.DataContext = revisao;
            gdRevisaoSistema.RefreshData();
        }

        private void gdRevisaoSistema_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdRevisaoSistema.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdRevisaoSistema.View).BestFitColumns()));
        }

        private void txtObs_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtObs.Text))
                label1.Content = string.Format(txtObs.Text.Length < 2999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 3000 - txtObs.Text.Length);
        }

        private bool pcontrolaobs;

        private void viewRevisaoSistema_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!pcontrolaobs)
                if ((sender as TableView).Grid.ItemsSource != null)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        (this.DataContext as RevisaoDeSistemasPEP).ItensRevisaoDeSistemas = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                        (sender as TableView).Grid.ItemsSource = (this.DataContext as RevisaoDeSistemasPEP).ItensRevisaoDeSistemas;
                        (sender as TableView).FocusedRow = e.Row;
                    }));
                    e.Handled = true;
                }
        }

        private void viewRevisaoSistema_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.SemParticularidades)
                                 || e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.NaoAvaliado))
            {
                (sender as TableView).CommitEditing();
            }
            else if (e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.Observacoes))
            {
                pcontrolaobs = true;
                ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
                pcontrolaobs = false;
            }
        }

        private void gdRevisaoSistema_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            viewRevisaoSistema.CommitEditing();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedicaItensDetalheDTO item = gdRevisaoSistema.GetFocusedRow() as SumarioAvaliacaoMedicaItensDetalheDTO;
            winObservacao win = new winObservacao(item.Observacoes);
            if (win.ShowDialog(base.OwnerBase).Value == true)
            {
                if (item != null)
                {
                    viewRevisaoSistema.Grid.SetCellValue(viewRevisaoSistema.GetSelectedRowHandles()[0], "Observacoes", win.Texto);
                    viewRevisaoSistema.CommitEditing();
                }
            }
        }
    }
}
