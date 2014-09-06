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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HMV.Core.Framework.WPF;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaRN
{
    /// <summary>
    /// Interaction logic for ucRecemNascido.xaml
    /// </summary>
    public partial class ucRecemNascido : UserControlBase
    {
        public ucRecemNascido()
        {
            InitializeComponent();
            //this.dtNascimento.MaxValue = DateTime.Now;
        }

        private void grid_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            e.Result = Comparer<int>.Default.Compare(e.ListSourceRowIndex1, e.ListSourceRowIndex2);
            e.Handled = true;
        }       

        private void dtNascimento_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.Source.IsNull())
                return;

            DateTime date = (e.Source as DevExpress.Xpf.Editors.DateEdit).DateTime;
            if (date > DateTime.Now.Date)
            {
                DXMessageBox.Show("Data de Nascimento deve ser igual ou menor que a data atual.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                (e.Source as DevExpress.Xpf.Editors.DateEdit).EditValue = null;
                e.Handled = true;
            }
        }
    }
}
