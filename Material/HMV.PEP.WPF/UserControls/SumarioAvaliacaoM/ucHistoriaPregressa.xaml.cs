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
using HMV.PEP.Consult;
using StructureMap;
using System.Linq;
using HMV.PEP.DTO;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucHistoriaPregressa.xaml
    /// </summary>
    public partial class ucHistoriaPregressa : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }
        IList<ProcedimentosRealizadosDTO> procedimentosRealizados;

        public ucHistoriaPregressa()
        {
            InitializeComponent();
        }

        private void buscaProcedimentoRealizados()
        {
            HistoriaPregressaPEP hisPre = (this.DataContext as HistoriaPregressaPEP);

            ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();
            procedimentosRealizados = (from T in consult.carregaProcedimentosRealizados(hisPre.SumarioAvaliacaoMedica.Paciente)
                                                       orderby T.DataAtendimento descending, T.IdAtendimento descending
                                                       select T).ToList();
            
            hisPre.ItensProcedimentoRealizados = new List<ItensProcedimentoRealizadosDTO>();

            foreach (var item in procedimentosRealizados)
            {
                hisPre.ItensProcedimentoRealizados.Add(
                    new ItensProcedimentoRealizadosDTO()
                    {
                        Atendimento = item.IdAtendimento,
                        //DataAtendimento = item.DataAtendimento.ToString(),
                        Procedimento = item.Procedimento,
                        Profissional = item.NomePrestador,
                        DataProcedimento = item.DataProcedimentoRealizado.HasValue ? item.DataProcedimentoRealizado.Value.ToShortDateString() : string.Empty
                    });
            }

         /*   if (procedimentosRealizados.Count > 0)
            {
                hisPre.ItensHistoriaPregressa[0].PodeNegar = false;
                hisPre.ItensHistoriaPregressa[1].PodeNegar = false;
            }*/

            //this.DataContext = hisPre;

        }
        
        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).HistoriaPregressa;
            buscaProcedimentoRealizados();
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdHistoriaPregressa.ItemsSource)
                item.SemParticularidades = string.IsNullOrWhiteSpace(item.Observacoes) ? ((CheckEdit)sender).IsChecked.Value : item.SemParticularidades;

            (this.DataContext as HistoriaPregressaPEP).ItensHistoriaPregressa = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdHistoriaPregressa.ItemsSource;
            HistoriaPregressaPEP histPre = (HistoriaPregressaPEP)this.DataContext;
            this.DataContext = null;
            this.DataContext = histPre;
            gdHistoriaPregressa.RefreshData();
            buscaProcedimentoRealizados();

        }

        private void gdHistoriaPregressa_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdHistoriaPregressa.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdHistoriaPregressa.View).BestFitColumns()));
        }

        private void txtOutros_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutros.Text))
                label1.Content = string.Format(txtOutros.Text.Length < 2999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 3000 - txtOutros.Text.Length);
        }

        private bool pcontrolaobs;

        private void viewHistoriaPregressa_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
           /* if (procedimentosRealizados.Count > 0)
            {
                if (gdHistoriaPregressa.GetCellValue(e.RowHandle, "Descricao").ToString() == "Cirurgias")
                {

                    ((TableView)sender).CancelRowEdit();// .Grid.SetCellValue(e.RowHandle, e.Column, false);
                    //e.Handled = false;
                    //e = CellValueEventArgs.Empty;
                }
            }
            else*/
            {
                if (!pcontrolaobs)
                    if ((sender as TableView).Grid.ItemsSource != null)
                    {
                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            (this.DataContext as HistoriaPregressaPEP).ItensHistoriaPregressa = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                            (sender as TableView).Grid.ItemsSource = (this.DataContext as HistoriaPregressaPEP).ItensHistoriaPregressa;
                            (sender as TableView).FocusedRow = e.Row;
                        }));
                        e.Handled = true;
                    }
            }
        }

        private void viewHistoriaPregressa_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.SemParticularidades)
                               || e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.NaoAvaliado))
            {
                (sender as TableView).CommitEditing();

                // GAMBA não permitir marca nega cirurgia se tiver alguma cirurgia realizada. 
                if (procedimentosRealizados.Count > 0)
                {
                    if (gdHistoriaPregressa.GetCellValue(e.RowHandle, "Descricao").ToString() == "Cirurgias")
                    {
                        foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdHistoriaPregressa.ItemsSource)
                        {
                            if (item.ID == 6) // desmarca cirurgias 
                            {
                                item.Nega = false;
                                item.SemParticularidades = false;
                            }
                        }

                        (this.DataContext as HistoriaPregressaPEP).ItensHistoriaPregressa = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdHistoriaPregressa.ItemsSource;
                        HistoriaPregressaPEP histPre = (HistoriaPregressaPEP)this.DataContext;
                        this.DataContext = null;
                        this.DataContext = histPre;
                        gdHistoriaPregressa.RefreshData();


                    }
                }
            }
            else if (e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.Observacoes))
            {
                pcontrolaobs = true;
                ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
                pcontrolaobs = false;
            }
        }

        private void gdHistoriaPregressa_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            viewHistoriaPregressa.CommitEditing();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedicaItensDetalheDTO item = gdHistoriaPregressa.GetFocusedRow() as SumarioAvaliacaoMedicaItensDetalheDTO;
            winObservacao win = new winObservacao(item.Observacoes);
            if (win.ShowDialog(base.OwnerBase).Value == true)
            {
                if (item != null)
                {
                    viewHistoriaPregressa.Grid.SetCellValue(viewHistoriaPregressa.GetSelectedRowHandles()[0], "Observacoes", win.Texto);
                    viewHistoriaPregressa.CommitEditing();
                }
            }
        }


    }
}
