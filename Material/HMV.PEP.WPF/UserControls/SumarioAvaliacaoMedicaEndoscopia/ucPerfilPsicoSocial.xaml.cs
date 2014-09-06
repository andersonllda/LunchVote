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
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Types;
using HMV.Core.Framework.WPF;
using DevExpress.Xpf.Grid;


namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaEndoscopia
{
    /// <summary>
    /// Interaction logic for ucPerfilPsicoSocial.xaml
    /// </summary>
    public partial class ucPerfilPsicoSocial : UserControlBase
    {
        public ucPerfilPsicoSocial()
        {            
            InitializeComponent();
        }

        private void View_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
        } 
    }
}
