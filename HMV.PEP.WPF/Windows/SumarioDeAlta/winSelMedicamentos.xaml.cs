using System.Windows;
using System.Windows.Input;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.DTO;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Editors.Settings;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winSelMedicamentos.xaml
    /// </summary>
    public partial class winSelMedicamentos : WindowBase
    {
        public enum enumAltaMedica
        {
            ProcedimentoAMB,
            ProcedimentoSUS,
            MotivoAlta,
            SetorObito
        }

        private enumAltaMedica _enumAltaMedica { get; set; }

        public winSelMedicamentos(vmPosAlta pPosAlta)
        {
            InitializeComponent();
            GridColumn gridcolumn = new GridColumn() { Header = "Medicamento", FieldName = "DescricaoProduto", AllowEditing = DevExpress.Utils.DefaultBoolean.False };
            gdSelecao.Columns.Add(gridcolumn);
            this.DataContext = pPosAlta;
            gdSelecao.ItemsSource = pPosAlta.MedicamentosItens;
        }

        public winSelMedicamentos(vmFarmacos pFarmacos)
        {
            InitializeComponent();
            btnConfirmar.IsEnabled = false;
            var headerTemplate = (DataTemplate)gdSelecao.Resources["checkBoxColumnTemplate"];
            GridColumn gridcolumn = new GridColumn() { Header = " ", FieldName = "Selecionado", Width = 20, AllowEditing = DevExpress.Utils.DefaultBoolean.True, EditSettings = new CheckEditSettings(), HeaderTemplate = headerTemplate };

            gdSelecao.Columns.Add(gridcolumn);
            TableViewConsulta.NavigationStyle = GridViewNavigationStyle.Cell;
            gridcolumn = new GridColumn();
            gridcolumn.Header = "Produto";
            gridcolumn.FieldName = "Descricao";
            gridcolumn.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
            gridcolumn.FieldName = "Descricao";
            gdSelecao.Columns.Add(gridcolumn);

            this.DataContext = pFarmacos;

            gdSelecao.ItemsSource = pFarmacos.FarmacoItens;
        }

        public winSelMedicamentos(vmExames pExames)
        {
            InitializeComponent();
            GridColumn gridcolumn = new GridColumn() { Header = "Procedimento", FieldName = "Descricao", AllowEditing = DevExpress.Utils.DefaultBoolean.False };
            gdSelecao.Columns.Add(gridcolumn);
            this.DataContext = pExames;
            gdSelecao.ItemsSource = pExames.ExamesItens;
        }

        public winSelMedicamentos(vmAltaMedica pAltaMedica, enumAltaMedica penumAltaMedica)
        {
            InitializeComponent();
            this._enumAltaMedica = penumAltaMedica;
            GridColumn gridcolumn = new GridColumn();

            if (penumAltaMedica == enumAltaMedica.ProcedimentoAMB)
            {
                gridcolumn.Header = "Procedimentos";
                gridcolumn.FieldName = "Descricao";
                gridcolumn.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                gdSelecao.Columns.Add(gridcolumn);
                this.DataContext = pAltaMedica;
                gdSelecao.ItemsSource = pAltaMedica.ProcedimentosAMBPossiveis;
            }
            else if (penumAltaMedica == enumAltaMedica.ProcedimentoSUS)
            {
                gridcolumn.Header = "Procedimentos";
                gridcolumn.FieldName = "Descricao";
                gridcolumn.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                gdSelecao.Columns.Add(gridcolumn);
                this.DataContext = pAltaMedica;
                gdSelecao.ItemsSource = pAltaMedica.ProcedimentosSUSPossiveis;
            }
            else if (penumAltaMedica == enumAltaMedica.MotivoAlta)
            {
                gridcolumn.Header = "Motivo Alta";
                gridcolumn.FieldName = "Descricao";
                gridcolumn.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                gdSelecao.Columns.Add(gridcolumn);
                this.DataContext = pAltaMedica;
                gdSelecao.ItemsSource = pAltaMedica.MotivosAltaPossiveis;
            }
            else if (penumAltaMedica == enumAltaMedica.SetorObito)
            {
                gridcolumn.Header = "Setor Óbito";
                gridcolumn.FieldName = "Descricao";
                gridcolumn.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                gdSelecao.Columns.Add(gridcolumn);
                this.DataContext = pAltaMedica;
                gdSelecao.ItemsSource = pAltaMedica.SetoresObitoPossiveis;
            }
        }

        public winSelMedicamentos(vmProcedimento pProcedimentoAlta)
        {
            InitializeComponent();
            GridColumn gridcolumn = new GridColumn() { Header = "Cirurgia", FieldName = "ds_cirurgia", AllowEditing = DevExpress.Utils.DefaultBoolean.False };
            gdSelecao.Columns.Add(gridcolumn);
            this.DataContext = pProcedimentoAlta;
            gdSelecao.ItemsSource = pProcedimentoAlta.ProcedimentosAltaItens;
        }

        private void bEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((sender as ButtonEdit).Text != string.Empty)
            {
                if (this.DataContext.GetType() == typeof(vmPosAlta))
                    gdSelecao.FilterCriteria = (new BinaryOperator("DescricaoProduto", (sender as ButtonEdit).Text + "%", BinaryOperatorType.Like));
                else if (this.DataContext.GetType() == typeof(vmFarmacos) || this.DataContext.GetType() == typeof(vmExames) || this.DataContext.GetType() == typeof(vmAltaMedica))
                    gdSelecao.FilterCriteria = (new BinaryOperator("Descricao", (sender as ButtonEdit).Text + "%", BinaryOperatorType.Like));
                else if (this.DataContext.GetType() == typeof(vmProcedimento))
                    gdSelecao.FilterCriteria = new BinaryOperator("ds_cirurgia", (sender as ButtonEdit).Text + "%", BinaryOperatorType.Like);
            }
            else
                gdSelecao.FilterCriteria = null;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext.GetType() == typeof(vmPosAlta))
            {
                if ((this.DataContext as vmPosAlta).MedicamentoSelecionado != null)
                {
                    this.DialogResult = true;
                    (this.DataContext as vmPosAlta).SetaPlanoPosAlta();
                    this.Close();
                    winCadMedicamentos win = new winCadMedicamentos((this.DataContext as vmPosAlta));
                    win.ShowDialog(this.Owner);
                }
            }
            else if (this.DataContext.GetType() == typeof(vmFarmacos))
            {
                this.DialogResult = true;
                (this.DataContext as vmFarmacos).SetaFarmaco();
                this.Close();
            }
            else if (this.DataContext.GetType() == typeof(vmExames))
            {
                if ((this.DataContext as vmExames).ProcedimentoSelecionado != null)
                {
                    this.DialogResult = true;
                    (this.DataContext as vmExames).SetaExame();
                    this.Close();
                }
            }
            else if (this.DataContext.GetType() == typeof(vmProcedimento))
            {
                vmProcedimento vm = this.DataContext as vmProcedimento;
                if (vm.cirurgiaSelecionada != null)
                {
                    this.DialogResult = true;
                    vm.SetaProcedimento();
                    this.Close();
                    winCadProcedimentos win = new winCadProcedimentos(vm);
                    win.ShowDialog(this.Owner);
                }
            }
            else if (this.DataContext.GetType() == typeof(vmAltaMedica))
            {
                if (TableViewConsulta.FocusedRow != null)
                {
                    this.DialogResult = true;
                    if (this._enumAltaMedica == enumAltaMedica.ProcedimentoAMB)
                        (this.DataContext as vmAltaMedica).ProcedimentoAMB = (wrpProcedimento)TableViewConsulta.FocusedRow;
                    //if (this._enumAltaMedica == enumAltaMedica.ProcedimentoSUS)
                    //    (this.DataContext as vmAltaMedica).ProcedimentoSUS = (wrpProcedimentoSUS)TableViewConsulta.FocusedRow;
                    if (this._enumAltaMedica == enumAltaMedica.MotivoAlta)
                        (this.DataContext as vmAltaMedica).MotivoAlta = (wrpMotivoAlta)TableViewConsulta.FocusedRow;
                    if (this._enumAltaMedica == enumAltaMedica.SetorObito)
                        (this.DataContext as vmAltaMedica).SetorObito = (wrpSetor)TableViewConsulta.FocusedRow;
                    this.Close();
                }
            }
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext.GetType() != typeof(vmFarmacos))
                if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                    btnConfirmar_Click(this, null);
        }

        private void TableView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (sender != null)
                if (this.DataContext.GetType() == typeof(vmPosAlta))
                {
                    (this.DataContext as vmPosAlta).MedicamentoSelecionado = (MedicamentoPosAltaDTO)(sender as TableView).FocusedRow;
                }
                //else if (this.DataContext.GetType() == typeof(vmFarmacos))
                //{
                //    (this.DataContext as vmFarmacos).ProdutoSelecionado = (Produto)(sender as TableView).FocusedRow;
                //}
                else if (this.DataContext.GetType() == typeof(vmExames))
                {
                    (this.DataContext as vmExames).ProcedimentoSelecionado = (Procedimento)(sender as TableView).FocusedRow;
                }
                else if (this.DataContext.GetType() == typeof(vmProcedimento))
                {
                    (this.DataContext as vmProcedimento).cirurgiaSelecionada = (Cirurgia)(sender as TableView).FocusedRow;
                }
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmFarmacos).MarcaTodosProdutos();
            Habilita();
        }

        private void ckbSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmFarmacos).DesmarcaTodosProdutos();
            Habilita();
        }

        private void TableViewConsulta_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (this.DataContext.GetType() == typeof(vmFarmacos))
            {
                Habilita();
            }
        }

        private void Habilita()
        {
            btnFechar.Focus();
            gdSelecao.Focus();
            btnConfirmar.IsEnabled = false;
            if ((this.DataContext as vmFarmacos).HabilitarBotaoSelecionar())
                btnConfirmar.IsEnabled = true;
        }

        private void bEdit_KeyUp(object sender, KeyEventArgs e)
        {
            gdSelecao.Focus();
            if (e.Key == Key.Up)
                TableViewConsulta.MovePrevRow();
            else
                TableViewConsulta.MoveNextRow();

            bEdit.Focus();
        }      
    }
}
