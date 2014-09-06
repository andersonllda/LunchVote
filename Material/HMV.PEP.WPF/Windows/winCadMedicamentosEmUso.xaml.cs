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
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Domain.Model.PEP;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winCadMedicamentosEmUso.xaml
    /// </summary>
    public partial class winCadMedicamentosEmUso : WindowBase
    {

        public winCadMedicamentosEmUso(vmMedicamentosEmUsoProntuario pData, wrpMedicamentosEmUsoProntuario pMedicametosEmUso)
        {
            InitializeComponent();

            dtInicioTratamento.MaxValue = DateTime.Today;
            dtInicioTratamento.EditValue = DateTime.Today; 

            if (pMedicametosEmUso == null)
            {
                pData.NovoRegistro();
                pData.EditaData = true;

            }else{

                pData.medicamentosEmUsoProntuarioSelecionado = pMedicametosEmUso;
                pData.EditaData = false;
            }

            this.DataContext = pData;
            txtMedicamentos.Focus();
        }


        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

    }
}
