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
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucDadosNascimento.xaml
    /// </summary>
    public partial class ucDadosNascimento : UserControlBase, IUserControl
    {
        public ucDadosNascimento()
        {
            InitializeComponent();           
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

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmSumarioAlta).vmDadosNascimento;
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
