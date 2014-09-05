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
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.WPF;
using HMV.PEP.Interfaces;
using HMV.PEP.ViewModel.PEP;
using NHibernate.Validator.Engine;
using StructureMap;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winCIDListaProblema.xaml
    /// </summary>
    public partial class winCIDListaProblema : WindowBase
    {
        public winCIDListaProblema(vmCIDListaProblemaPrescricao pVM)
        {
            InitializeComponent();
            this.DataContext = pVM;
            (this.DataContext as vmCIDListaProblemaPrescricao).GoClose += this.Close;

        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(false);
          //  win.txtCodigoCid.Text = txtCIDPrincipal.Text;
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
                (this.DataContext as vmCIDListaProblemaPrescricao).CID = new Core.Wrappers.ObjectWrappers.wrpCid(win.CID);

        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !(this.DataContext as vmCIDListaProblemaPrescricao).CancelaGrava();
        }

        private void txtCIDPrincipal_EditValueChanging(object sender, DevExpress.Xpf.Editors.EditValueChangingEventArgs e)
        {
            try
            {
                if (this.DataContext != null)
                {
                    if (e.NewValue != null)
                    {
                        ICidService serv = ObjectFactory.GetInstance<ICidService>();
                        Cid cid = serv.FiltraPorCid10(e.NewValue.ToString());

                        if (cid != null)
                        {
                            (this.DataContext as vmCIDListaProblemaPrescricao).CID = new Core.Wrappers.ObjectWrappers.wrpCid(cid);
                            this.txtDescricaoCIDPrincipal.Text = cid.Descricao;
                        }
                        else
                        {
                            (this.DataContext as vmCIDListaProblemaPrescricao).CID = null;
                            this.txtDescricaoCIDPrincipal.Text = string.Empty;
                        }
                    }
                    else
                    {
                        (this.DataContext as vmCIDListaProblemaPrescricao).CID = null;
                        this.txtDescricaoCIDPrincipal.Text = string.Empty;
                    }
                }
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

    }
}
