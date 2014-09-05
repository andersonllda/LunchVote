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
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using System.Threading;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winConfirmaAlta.xaml
    /// </summary>
    public partial class winConfirmaAlta : WindowBase
    {
        public winConfirmaAlta(vmAltaMedica pvmAltaMedica)
        {
            InitializeComponent();
            this.DataContext = pvmAltaMedica;

            (this.DataContext as vmAltaMedica).DataSelecionada = DateTime.Now.ToString("dd/MM/yyyy");
            (this.DataContext as vmAltaMedica).HoraSelecionada = DateTime.Now.ToShortTimeString();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Width = 0;
            Height = 0;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            ShowActivated = false;

            if ((this.DataContext as vmAltaMedica).RealizaAlta())
            {                               
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                Width = 290;
                Height = 167;
                WindowStyle = WindowStyle.ToolWindow;
                ShowInTaskbar = true;
                ShowActivated = true;
            }
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void cbDataAlta_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((this.DataContext as vmAltaMedica).HoraSelecionada == null && (this.DataContext as vmAltaMedica).DataSelecionada == DateTime.Now.ToString("dd/MM/yyyy"))
                (this.DataContext as vmAltaMedica).HoraSelecionada = DateTime.Now.ToShortTimeString();
        }
    }
}
