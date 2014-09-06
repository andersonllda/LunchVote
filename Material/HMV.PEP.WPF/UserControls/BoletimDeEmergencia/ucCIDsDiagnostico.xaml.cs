using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.BoletimEmergencia;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucAlergias.xaml
    /// </summary>
    public partial class ucCIDsDiagnostico : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucCIDsDiagnostico()
        {
            InitializeComponent();
        }
       
        public void SetData(object pData)
        {
            this.DataContext = (pData as vmBoletimEmergencia).vmCidDiagnostico;
        }

        //private void txtCid_EditValueChanging(object sender, EditValueChangingEventArgs e)
        //{
        //    try
        //    {
        //        ICidService serv = ObjectFactory.GetInstance<ICidService>();
        //        Cid cid = serv.FiltraPorCid10(e.NewValue.ToString());

        //        if (cid != null)
        //        {
        //            txtDescricaoCIDPrincipal.Text = cid.Descricao;
        //        }
        //        else
        //            txtDescricaoCIDPrincipal.Text = "";
        //    }
        //    catch (BusinessValidatorException err)
        //    {
        //        IList<InvalidValue> erros = err.GetErros();
        //        DXMessageBox.Show(erros[0].Message);
        //    }
        //}

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            AbreListaCid();
        }

        private void AbreListaCid()
        {
            winSelCID win = new winSelCID(false);
           // win.txtCodigoCid.Text = txtCid.Text;
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
                txtCid.Text = win.CID.Id.ToString();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DXMessageBox.Show("Deseja realmente excluir este CID?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                (this.DataContext as vmCIDsAtendimento).DeletaCID(grdCids.View.FocusedRow);
        }
    }
}
