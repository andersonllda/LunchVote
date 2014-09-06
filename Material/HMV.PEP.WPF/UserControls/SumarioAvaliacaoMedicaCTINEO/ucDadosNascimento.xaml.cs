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

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaCTINEO
{
    /// <summary>
    /// Interaction logic for ucDadosNascimento.xaml
    /// </summary>
    public partial class ucDadosNascimento : UserControlBase
    {
        public ucDadosNascimento()
        {
            InitializeComponent();
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
