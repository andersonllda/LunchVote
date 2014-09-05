using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.Core.DTO;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for winCadImunizacaoDetalhe.xaml
    /// </summary>
    public partial class winCadImunizacaoDetalhe : WindowBase
    {
        Paciente Paciente;

        public winCadImunizacaoDetalhe(Paciente pPaciente)
        {
            InitializeComponent();

            Paciente = pPaciente;

            this.DataContext = new Imunizacao
                {
                    Paciente = pPaciente,
                    Usuario = App.Usuario,
                    DataInclusao = DateTime.Now
                };
        }  

        public winCadImunizacaoDetalhe(Imunizacao pImunizacao)
        {
            InitializeComponent();
            this.DataContext = pImunizacao;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {           

            Imunizacao imunizacao = (this.DataContext as Imunizacao);

            if (imunizacao.CalendarioCompleto == null)
            {
                DXMessageBox.Show("Selecione se o calendário está completo!", "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Exclamation);               
                return;
            }

            //if (imunizacao.ImunizacaoDetalhe == null)
            //{
            //    DXMessageBox.Show("Marque pelo menos uma Imunização!", App.win.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //    return;
            //}
            //else
            //{
            //    if (imunizacao.ImunizacaoDetalhe.Count == 0)
            //    {
            //        DXMessageBox.Show("Marque pelo menos uma Imunização!", App.win.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        return;
            //    }
            //}

            if (Paciente != null)
                imunizacao.Paciente.AddImunizacao(imunizacao);
            else
                imunizacao.Paciente.AlteraImunizacao(imunizacao);

            this.DialogResult = true;
            this.Close();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as Imunizacao);
        }

        public bool CancelClose()
        {
            return false;
        }

        private void txtObs_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtObs.Text))
                label1.Content = "Máximo 128 caracteres";
            else label1.Content = string.Format(txtObs.Text.Length < 127 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 128 - txtObs.Text.Length);
        }

        private void viewImunizacaoDetalhe_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if ((sender as TableView).Grid.ItemsSource != null)
            {               
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    (this.DataContext as Imunizacao).ImunizacaoTodas = (List<ImunizacaoDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                    (sender as TableView).Grid.ItemsSource = (this.DataContext as Imunizacao).ImunizacaoTodas;
                    (sender as TableView).FocusedRow = e.Row;
                }));
                e.Handled = true;
            } 
        }

        private void viewImunizacaoDetalhe_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == ExpressionEx.PropertyName<ImunizacaoDetalheDTO>(x => x.Ativo))
            {
                (sender as TableView).CommitEditing();
            }  
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {           
            foreach (ImunizacaoDetalheDTO item in (IList<ImunizacaoDetalheDTO>)gdImunizacaoDetalhe.ItemsSource)
                item.Ativo = ((CheckEdit)sender).IsChecked.Value;

            (this.DataContext as Imunizacao).ImunizacaoTodas = (List<ImunizacaoDetalheDTO>)gdImunizacaoDetalhe.ItemsSource;           
                
            Imunizacao imunizacoes = (Imunizacao)this.DataContext;
            this.DataContext = null;
            this.DataContext = imunizacoes;
            gdImunizacaoDetalhe.RefreshData();        
        }

        private void DateEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            (this.DataContext as Imunizacao).ImunizacaoTodas = (List<ImunizacaoDetalheDTO>)gdImunizacaoDetalhe.ItemsSource;
        }
        
    }
}
