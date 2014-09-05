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
using HMV.Core.Interfaces;
using HMV.PEP.WPF.Windows.RegistroDor;
using HMV.Core.Domain.Model;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.RegistroDor
{
    /// <summary>
    /// Interaction logic for ucConsultadaDor.xaml
    /// </summary>
    public partial class ucRegistroDor : UserControlBase, IUserControl
    {
        public ucRegistroDor()
        {
            InitializeComponent();
        }

            #region SetData
            public void SetData(object pData)
            {

                if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
                    this.DataContext = new vmRegistrosDeDor((pData as Atendimento).Paciente);
                else
                    this.DataContext = new vmRegistrosDeDor(pData as Paciente);
            }
            #endregion

            public bool CancelClose { get; set; }

        private void btnEvolucao_Click(object sender, RoutedEventArgs e)
        {
            winEvolucao winform = new winEvolucao((vmRegistrosDeDor)this.DataContext);
            winform.ShowDialog(base.OwnerBase);
        }


    }
}
