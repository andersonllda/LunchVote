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
    /// Interaction logic for ucHistoriaFamiliar.xaml
    /// </summary>
    public partial class ucHistoriaFamiliar : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucHistoriaFamiliar()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).HistoriaFamiliar;
        }

        public void Refresh()
        {
            if (this.DataContext != null)
            {
                HistoriaFamiliarPEP histFami = (HistoriaFamiliarPEP)this.DataContext;
                this.DataContext = null;
                this.DataContext = histFami;
            }
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdHistoriaFamiliar.ItemsSource)
                item.SemParticularidades = string.IsNullOrWhiteSpace(item.Observacoes) ? ((CheckEdit)sender).IsChecked.Value : item.SemParticularidades && !((CheckEdit)sender).IsChecked.Value;

            (this.DataContext as HistoriaFamiliarPEP).ItensHistoriaFamiliar = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdHistoriaFamiliar.ItemsSource;
            HistoriaFamiliarPEP histFami = (HistoriaFamiliarPEP)this.DataContext;
            this.DataContext = null;
            this.DataContext = histFami;
            gdHistoriaFamiliar.RefreshData();
        }

        private void gdHistoriaFamiliar_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdHistoriaFamiliar.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdHistoriaFamiliar.View).BestFitColumns()));
        }

        private void txtOutros_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutros.Text))
                label1.Content = "Máximo 3000 caracteres";
            else label1.Content = string.Format(txtOutros.Text.Length < 2999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 3000 - txtOutros.Text.Length);
        }


        private bool pcontrolaobs;

        private void viewHistoriaFamiliar_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!pcontrolaobs)
                if ((sender as TableView).Grid.ItemsSource != null)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        (this.DataContext as HistoriaFamiliarPEP).ItensHistoriaFamiliar = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                        (sender as TableView).Grid.ItemsSource = (this.DataContext as HistoriaFamiliarPEP).ItensHistoriaFamiliar;
                        (sender as TableView).FocusedRow = e.Row;
                    }));
                    e.Handled = true;
                }
        }

        private void viewHistoriaFamiliar_CellValueChanging(object sender, CellValueChangedEventArgs e)
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

        private void gdHistoriaFamiliar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            viewHistoriaFamiliar.CommitEditing();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedicaItensDetalheDTO item = gdHistoriaFamiliar.GetFocusedRow() as SumarioAvaliacaoMedicaItensDetalheDTO;
            winObservacao win = new winObservacao(item.Observacoes);
            if (win.ShowDialog(base.OwnerBase).Value == true)
            {
                if (item != null)
                {
                    viewHistoriaFamiliar.Grid.SetCellValue(viewHistoriaFamiliar.GetSelectedRowHandles()[0], "Observacoes", win.Texto);
                    viewHistoriaFamiliar.CommitEditing();
                }
            }

        }
    }
}
