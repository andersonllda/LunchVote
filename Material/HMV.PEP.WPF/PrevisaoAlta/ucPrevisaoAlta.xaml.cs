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
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.Evolucao;
using HMV.PEP.WPF.PrevisaoAlta;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucPrevisaoAlta.xaml
    /// </summary>
    public partial class ucPrevisaoAlta : UserControlBase, IUserControl
    {
        public ucPrevisaoAlta()
        {
            InitializeComponent();
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetData(object pData)
        {
            this.DataContext = new VMPrevisaoAltaConsulta(App.Usuario, pData as Atendimento);
        }

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new VMPrevisaoAlta(App.Usuario, (this.DataContext as VMPrevisaoAltaConsulta).Atendimento, false, true));
            win.ShowDialog(null);
            Atendimento atendimento = (this.DataContext as VMPrevisaoAltaConsulta).Atendimento;
            this.DataContext = null;
            this.DataContext = new VMPrevisaoAltaConsulta(App.Usuario, atendimento);
        }
    }
}
