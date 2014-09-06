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
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaRN
{
    /// <summary>
    /// Interaction logic for ucParto.xaml
    /// </summary>
    public partial class ucParto : UserControlBase
    {
        public ucParto()
        {
            InitializeComponent();
            //dtMembrana.MaxValue = DateTime.Today;
        }

        private void Mem_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (e.Source.IsNull())
                return;

            DateTime date = (e.Source as DevExpress.Xpf.Editors.DateEdit).DateTime;
            if (date > DateTime.Now.Date)
            {
                DXMessageBox.Show("Data das Membranas deve ser igual ou menor que a data atual.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                (e.Source as DevExpress.Xpf.Editors.DateEdit).EditValue = null;
                e.Handled = true;
            }
        }
    }
}
