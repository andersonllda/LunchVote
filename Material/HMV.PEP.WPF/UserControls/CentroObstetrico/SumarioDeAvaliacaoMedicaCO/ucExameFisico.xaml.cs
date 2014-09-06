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
using DevExpress.Xpf.Core;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    /// <summary>
    /// Interaction logic for ucExameFisico.xaml
    /// </summary>
    public partial class ucExameFisico : UserControlBase
    {
        public ucExameFisico()
        {
            InitializeComponent();            
        }        

        private void DtMembrana_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.Source.IsNull())
                return;

            DateTime date = (e.Source as DevExpress.Xpf.Editors.DateEdit).DateTime;
            if (date > DateTime.Now.Date)
            {
                DXMessageBox.Show("Data da das Membranas deve ser igual ou menor que a data atual.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                (e.Source as DevExpress.Xpf.Editors.DateEdit).EditValue = null;
                e.Handled = true;
            }
        }
    }
}
