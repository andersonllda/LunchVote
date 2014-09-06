using System;
using System.Windows;
using HMV.Core.Framework.WPF;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{    
    public partial class ucGestacaoAtual : UserControlBase
    {
        public ucGestacaoAtual()
        {
            InitializeComponent();
            //dtEco.MaxValue = DateTime.Now;
            //dtUltimaMenstruacao.MaxValue = DateTime.Now;
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
        }                         

        private void dtUltimaMenstruacao_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (e.Source.IsNull())
                return;

            DateTime date = (e.Source as DevExpress.Xpf.Editors.DateEdit).DateTime;
            if (date > DateTime.Now.Date)
            {
                DXMessageBox.Show("Data da Última Mestruação deve ser igual ou menor que a data atual.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);                
                (e.Source as DevExpress.Xpf.Editors.DateEdit).EditValue = null;
                e.Handled = true;                
            }            
        }

        private void dtEco_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (e.Source.IsNull())
                return;

            DateTime date = (e.Source as DevExpress.Xpf.Editors.DateEdit).DateTime;
            if (date > DateTime.Now.Date)
            {
                DXMessageBox.Show("Data da Eco deve ser igual ou menor que a data atual.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                (e.Source as DevExpress.Xpf.Editors.DateEdit).EditValue = null;
                e.Handled = true;
            }   
        }
    }
}
