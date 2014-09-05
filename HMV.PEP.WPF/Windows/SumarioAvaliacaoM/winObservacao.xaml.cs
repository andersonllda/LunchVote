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
using DevExpress.Xpf.Docking;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Base;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoM;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for winNotasAdicionais.xaml
    /// </summary>
    public partial class winObservacao : WindowBase
    {
        public winObservacao(string texto)
        {
            InitializeComponent();
            txtObs.Text = texto;
            Texto = texto;
            txtObs.Focus();
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            Texto = txtObs.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public string Texto { get; set; }

    }
}
