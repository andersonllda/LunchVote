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
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.PEP.WPF.Windows;
using HMV.Core.Framework.Extensions;
using HMV.PEP.DTO;
using HMV.Core.Framework.Helper;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucProcedimentosRealizados.xaml
    /// </summary>
    public partial class ucProcedimentosRealizados : UserControl
    {
        public ucProcedimentosRealizados()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = pData;
        }      

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            UIHelper.SetBusyState();
            (this.DataContext as vmProcedimentosRealizados).SelectByAviso((int)(sender as Hyperlink).Tag);            
            winDescricaoCirurgica win = new winDescricaoCirurgica((this.DataContext as vmProcedimentosRealizados));
            UIHelper.SetBusyState();
            win.ShowDialog(null);
        }
    }
}
