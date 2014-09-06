using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.DTO;
using HMV.Core.Framework.Expression;
using HMV.Core.Interfaces;
using HMV.PEP.Interfaces;
using StructureMap;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoM;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucPerfilPsicoSocial.xaml
    /// </summary>
    public partial class ucPerfilPsicoSocial : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }
        Paciente paciente;

        public ucPerfilPsicoSocial()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).PerfilPsicoSocial;
            this.paciente = (pData as SumarioAvaliacaoMedica).Paciente;
            escondeColuna();
        }

        private void gdPerfilNega_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdPerfilNega.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdPerfilNega.View).BestFitColumns()));
        }
        private void gdPerfilSemParticularidades_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdPerfilSemParticularidades.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdPerfilSemParticularidades.View).BestFitColumns()));
        }

        private void ckbSelectAll_SemParticularidades(object sender, RoutedEventArgs e)
        {
            foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource)
                item.SemParticularidades = string.IsNullOrWhiteSpace(item.Observacoes) ? ((CheckEdit)sender).IsChecked.Value : item.SemParticularidades;

            (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialSemParticularidades = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource;
            PerfilPsicoSocialPEP perfil = (PerfilPsicoSocialPEP)this.DataContext;
            this.DataContext = null;
            this.DataContext = perfil;
            gdPerfilSemParticularidades.RefreshData();
        }

        private void ckbSelectAll_Nega(object sender, RoutedEventArgs e)
        {
            /*old
            foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource)
                item.SemParticularidades = string.IsNullOrWhiteSpace(item.Observacoes) ? ((CheckEdit)sender).IsChecked.Value : item.SemParticularidades;

            (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialNega = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource;
            PerfilPsicoSocialPEP perfil = (PerfilPsicoSocialPEP)this.DataContext;
            this.DataContext = null;
            this.DataContext = perfil;
            gdPerfilNega.RefreshData();*/

            if (((CheckEdit)sender).Name == "ckbSelectAll")
            {
                if (((CheckEdit)sender).IsChecked.Value)
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource)
                    {
                        item.SemParticularidades = !string.IsNullOrWhiteSpace(item.Observacoes) ? false : ((CheckEdit)sender).IsChecked.Value;
                        item.NaoAvaliado = false;
                    }
                }
                else
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource)
                        item.SemParticularidades = false;
            }
            else if (((CheckEdit)sender).Name == "ckbSelectAllN")
            {
                if (((CheckEdit)sender).IsChecked.Value)
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource)
                    {
                        item.NaoAvaliado = !string.IsNullOrWhiteSpace(item.Observacoes) ? false : ((CheckEdit)sender).IsChecked.Value;
                        item.SemParticularidades = false;
                    }
                }
                else
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource)
                    {
                        item.NaoAvaliado = false;
                    }
                    PerfilPsicoSocialPEP perfil = (PerfilPsicoSocialPEP)this.DataContext;
                    this.DataContext = null;
                    this.DataContext = perfil;
                }
            }
            (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialNega = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilNega.ItemsSource;

            gdPerfilNega.RefreshData();
        }

        private void ckbSelectAll_NaoAvaliado(object sender, RoutedEventArgs e)
        {
            if (((CheckEdit)sender).Name == "ckbSelectAll")
            {
                if (((CheckEdit)sender).IsChecked.Value)
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource)
                    {
                        item.SemParticularidades = !string.IsNullOrWhiteSpace(item.Observacoes) ? false : ((CheckEdit)sender).IsChecked.Value;
                        item.NaoAvaliado = false;
                    }
                }
                else
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource)
                    {
                        item.SemParticularidades = false;
                    }
                }
            }
            else if (((CheckEdit)sender).Name == "ckbSelectAllN")
            {
                if (((CheckEdit)sender).IsChecked.Value)
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource)
                    {
                        item.NaoAvaliado = !string.IsNullOrWhiteSpace(item.Observacoes) ? false : ((CheckEdit)sender).IsChecked.Value;
                        item.SemParticularidades = false;
                    }
                }
                else
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource)
                    {
                        item.NaoAvaliado = false;
                    }
                }
            }
            (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialSemParticularidades = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdPerfilSemParticularidades.ItemsSource;
            gdPerfilSemParticularidades.RefreshData();
        }

        private void txtOutros_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOutros.Text))
                label1.Content = "Máximo 3000 caracteres";
            else label1.Content = string.Format(txtOutros.Text.Length < 2999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 3000 - txtOutros.Text.Length);
        }


        private bool pcontrolaobs;

        private void viewPerfilSemParticularidades_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!pcontrolaobs)
                if ((sender as TableView).Grid.ItemsSource != null)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialSemParticularidades = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                        (sender as TableView).Grid.ItemsSource = (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialSemParticularidades;
                        (sender as TableView).FocusedRow = e.Row;
                    }));
                    e.Handled = true;
                }
        }

        private void viewPerfilNega_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!pcontrolaobs)
                if ((sender as TableView).Grid.ItemsSource != null)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialNega = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                        (sender as TableView).Grid.ItemsSource = (this.DataContext as PerfilPsicoSocialPEP).ItensPerfilPsicoSocialNega;
                        (sender as TableView).FocusedRow = e.Row;
                    }));
                    e.Handled = true;
                }
        }

        private void view_CellValueChanging(object sender, CellValueChangedEventArgs e)
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

        private void gdPerfilSemParticularidades_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            viewPerfilSemParticularidades.CommitEditing();
        }

        private void gdPerfilNega_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            viewPerfilNega.CommitEditing();
        }

        private void escondeColuna()
        {

            if (paciente != null)
            {
                if (paciente.TipoDoPaciente == Core.Domain.Enum.TipoPaciente.Adulto)
                {
                    gdPerfilSemParticularidades.Columns.GetColumnByFieldName("NaoAvaliado").Visible = false;
                    gdPerfilNega.Columns.GetColumnByFieldName("NaoAvaliado").Visible = false;
                }
            }

        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedicaItensDetalheDTO item = gdPerfilSemParticularidades.GetFocusedRow() as SumarioAvaliacaoMedicaItensDetalheDTO;
            winObservacao win = new winObservacao(item.Observacoes);
            if (win.ShowDialog(base.OwnerBase).Value == true)
            {
                if (item != null)
                {
                    viewPerfilSemParticularidades.Grid.SetCellValue(viewPerfilSemParticularidades.GetSelectedRowHandles()[0], "Observacoes", win.Texto);
                    viewPerfilSemParticularidades.CommitEditing();
                }
            }

        }

        private void ButtonInfo_ClickPerfil(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedicaItensDetalheDTO item = gdPerfilNega.GetFocusedRow() as SumarioAvaliacaoMedicaItensDetalheDTO;
            winObservacao win = new winObservacao(item.Observacoes);
            if (win.ShowDialog(base.OwnerBase).Value == true)
            {
                if (item != null)
                {
                    viewPerfilNega.Grid.SetCellValue(viewPerfilNega.GetSelectedRowHandles()[0], "Observacoes", win.Texto);
                    viewPerfilNega.CommitEditing();
                }
            }


        }

    }
}
