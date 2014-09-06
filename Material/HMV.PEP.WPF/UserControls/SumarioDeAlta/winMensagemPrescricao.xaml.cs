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
    /// Interaction logic for winMensagemPrescricao.xaml
    /// </summary>
    public partial class winMensagemPrescricao : WindowBase
    {
        public winMensagemPrescricao()
        {
            InitializeComponent();
        }
        
        private void btnEntrarProntuario_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }      
    }
}
