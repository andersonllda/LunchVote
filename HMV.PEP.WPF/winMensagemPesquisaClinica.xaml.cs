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
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.PEP.DTO;
using HMV.PEP.WPF.Report;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Windows;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winMensagemPesquisaClinica.xaml
    /// </summary>
    public partial class winMensagemPesquisaClinica : WindowBase
    {
        public winMensagemPesquisaClinica()
        {
            InitializeComponent();
        }

        public winMensagemPesquisaClinica(Paciente paciente)
        {
            InitializeComponent();
            this.DataContext = paciente;
        }

        private void btnEntrarProntuario_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPesquisa_Click(object sender, RoutedEventArgs e)
        {
            winPesquisaClinica win = new winPesquisaClinica(new ViewModel.PEP.PesqClinica.vmPesquisaClinica(new wrpPesquisaClinica((this.DataContext as Paciente).PesquisaClinicaAtiva)));
            win.ShowDialog(this);
            this.Close();
        }
    }
}
