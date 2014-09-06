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
using HMV.Core.Framework.WPF;
using HMV.PEP.DTO;
using HMV.PEP.Services;
using HMV.PEP.ViewModel.PEP;

namespace HMV.PEP.WPF.InicioPEP
{
    /// <summary>
    /// Interaction logic for winTaxaOcupacao.xaml
    /// </summary>
    public partial class winTaxaOcupacao : WindowBase
    {
        public winTaxaOcupacao()
        {
            InitializeComponent();

            InicializacaoService serv = new InicializacaoService();
            IList<TaxaOcupacaoDTO> lst = serv.TaxaOcupacaoDetalhe();
            foreach (TaxaOcupacaoDTO item in lst)
            {
                ucTaxaOcupacao usr = new ucTaxaOcupacao();
                usr.DataContext = new VMTaxaOcupacao(item);
                usr.Width = 450;                
                wrpPanel.Children.Add(usr);
            }
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
