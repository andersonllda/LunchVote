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
using System.Windows.Shapes;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.Receituario;

namespace HMV.PEP.WPF.Receituario
{
    /// <summary>
    /// Interaction logic for winCadReceituarioPadrao.xaml
    /// </summary>
    public partial class winCadReceituarioPadrao : WindowBase
    {
        public winCadReceituarioPadrao(vmReceituario pVm)
        {            
            InitializeComponent();
            this.DataContext = pVm;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            base.CancelaCloseVM = true;
            this.Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmReceituario).SalvaNovoReceituarioPadrao();
            base.CancelaCloseVM = true;
            this.Close();
        }         
    }
}
