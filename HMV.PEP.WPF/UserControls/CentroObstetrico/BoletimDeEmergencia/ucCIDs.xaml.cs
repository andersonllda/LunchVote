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
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucCIDs.xaml
    /// </summary>
    public partial class ucCIDs : UserControlBase
    {
        public ucCIDs()
        {
            InitializeComponent();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(true);
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
            {
                (this.DataContext as vmCidDiagnostico).addCid(new wrpCid(win.CID), new wrpUsuarios(App.Usuario));
            }
        }

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            vmCidDiagnostico vm = (this.DataContext as vmCidDiagnostico);

            if (vm.CID == null)
            {
                DXMessageBox.Show("Informe um CID válido!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            vm.addCid(vm.CID, new wrpUsuarios(App.Usuario));
        }

        private void txtCid_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try
            {
                if (e.NewValue == null)
                    return; 

                ICidService serv = ObjectFactory.GetInstance<ICidService>();
                Cid cid = serv.FiltraPorCid10(e.NewValue.ToString());
                if (cid != null)
                {
                    vmCidDiagnostico vm = (this.DataContext as vmCidDiagnostico);
                    vm.CID = new wrpCid(cid);
                }
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void txtCid_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DXMessageBox.Show("Deseja realmente excluir este CID?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                (this.DataContext as vmCidDiagnostico).RemoveCID((wrpCid)grdCids.View.FocusedRow);
        }
    }
}
