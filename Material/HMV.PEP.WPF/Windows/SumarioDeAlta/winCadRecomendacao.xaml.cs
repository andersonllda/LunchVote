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
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winCadRecomendacao.xaml
    /// </summary>
    public partial class winCadRecomendacao : WindowBase
    {
        public winCadRecomendacao(vmRecomendacaoPadrao pData)
        {
            InitializeComponent();
            this.DataContext = pData;
        }

       
        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }      

    }
}
