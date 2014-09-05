using System;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Docking;
using HMV.Core.Framework.WPF;
using System.Windows.Media;
using HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Framework.Extensions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;

namespace HMV.PEP.WPF.Windows.CheckListCirurgiaSegura
{
    /// <summary>
    /// Interaction logic for winCadCheckList.xaml
    /// </summary>
    public partial class winConfirmaDados : WindowBase
    {
        public winConfirmaDados(vmCheckList pvm)
        {
            InitializeComponent();
            this.DataContext = pvm;
        }      

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmCheckList).Novo();
            this.DialogResult = true;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
