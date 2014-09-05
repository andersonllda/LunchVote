using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.Core.Interfaces;
using HMV.PEP.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using NHibernate.Validator.Engine;
using StructureMap;
using DevExpress.Xpf.Grid;
using System;
using HMV.PEP.WPF.Windows.SumarioDeAlta;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucAltaMedica.xaml
    /// </summary>
    public partial class ucAltaMedica : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucAltaMedica()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmSumarioAlta).vmAltaMedica;
            ucrListaProblemaSumario.SetData((pData as vmSumarioAlta).vmListaProblemaSumario);
        }

        private void btnSetorObito_Click(object sender, RoutedEventArgs e)
        {
            winSelMedicamentos win = new winSelMedicamentos((vmAltaMedica)this.DataContext, winSelMedicamentos.enumAltaMedica.SetorObito);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnProcedimentoAMB_Click(object sender, RoutedEventArgs e)
        {
            winSelMedicamentos win = new winSelMedicamentos((vmAltaMedica)this.DataContext, winSelMedicamentos.enumAltaMedica.ProcedimentoAMB);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnCidPrincipal_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(true);
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
            {
                (this.DataContext as vmAltaMedica).SetaCidPrincipal(win.CID);
            }
        }

        private void btnProcedimentoSUS_Click(object sender, RoutedEventArgs e)
        {
            winSelMedicamentos win = new winSelMedicamentos((vmAltaMedica)this.DataContext, winSelMedicamentos.enumAltaMedica.ProcedimentoSUS);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(true);
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
            {
                (this.DataContext as vmAltaMedica).SetaCidSelecionado(win.CID);
                (this.DataContext as vmAltaMedica).AddCidCommand.Execute(null);

            }
        }

        private void btnMotivoAlta_Click(object sender, RoutedEventArgs e)
        {
            winSelMedicamentos win = new winSelMedicamentos((vmAltaMedica)this.DataContext, winSelMedicamentos.enumAltaMedica.MotivoAlta);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnProcedimentoAMB_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                (this.DataContext as vmAltaMedica).ProcedimentoAMB = null;
                btnProcedimentoAMB.Text = string.Empty;
                //if (e.NewValue.ToString().Length > 7 || string.IsNullOrEmpty(e.NewValue.ToString()))
                //    (this.DataContext as vmAltaMedica).ProcedimentoAMB = (this.DataContext as vmAltaMedica).ProcedimentosAMBPossiveis.Where(x => x.ID == e.NewValue.ToString()).FirstOrDefault();               
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void btnCidPrincipal_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                Cid cid;
                if (e.NewValue.IsNotNull())
                {
                    ICidService serv = ObjectFactory.GetInstance<ICidService>();
                    cid = serv.FiltraPorCid10(e.NewValue.ToString());
                }
                else
                {
                    cid = null;
                }

                (this.DataContext as vmAltaMedica).SetaCidPrincipal(cid);
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void btnProcedimentoSUS_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            /* try
             {
                 if ((this.DataContext as vmAltaMedica).ProcedimentosSUSPossiveis != null)
                     (this.DataContext as vmAltaMedica).ProcedimentoSUS = (this.DataContext as vmAltaMedica).ProcedimentosSUSPossiveis.Where(x => x.ID == e.NewValue.ToString()).FirstOrDefault();
             }
             catch (BusinessValidatorException err)
             {
                 IList<InvalidValue> erros = err.GetErros();
                 DXMessageBox.Show(erros[0].Message);
             }*/
        }

        private void btnMotivoAlta_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                int lID = 0;
                if (e.NewValue.IsNotNull() && !string.IsNullOrWhiteSpace(e.NewValue.ToString()) && int.TryParse(e.NewValue.ToString(), out lID))
                    (this.DataContext as vmAltaMedica).MotivoAlta = (this.DataContext as vmAltaMedica).MotivosAltaPossiveis.Where(x => x.Id == lID).FirstOrDefault();
                else
                    (this.DataContext as vmAltaMedica).MotivoAlta = null;
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void btnSetorObito_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                int lID = 0;
                if (!string.IsNullOrWhiteSpace(e.NewValue.ToString()) && int.TryParse(e.NewValue.ToString(), out lID))
                    (this.DataContext as vmAltaMedica).SetorObito = (this.DataContext as vmAltaMedica).SetoresObitoPossiveis.Where(x => x.ID == lID).FirstOrDefault();
                else
                    (this.DataContext as vmAltaMedica).SetorObito = null;
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmAltaMedica).ValidaAlta())
            {
                winConfirmaAlta win = new winConfirmaAlta(this.DataContext as vmAltaMedica);
                win.ShowDialog(base.OwnerBase);
            }
        }

        private void gdCID_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as GridControl).View.FocusedRow = (sender as GridControl).View.FocusedRow;
        }

        private void btnProcedimentoAMB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnProcedimentoAMB.Text) && string.IsNullOrWhiteSpace(txtDescricaoCIDPrincipal.Text))
            {
                DXMessageBox.Show("Código do 'Procedimento AMB' inexistente ou inativo.", "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Warning);
                btnProcedimentoAMB.Text = string.Empty;
            }
        }

        private void btnCidPrincipal_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnCidPrincipal.Text) && string.IsNullOrWhiteSpace(txtCidPrincipal.Text))
            {
                string msbox = "Cid Inexistente!" + Environment.NewLine;
                msbox += "Este CID não foi localizado conforme a última atualização do CID 10." + Environment.NewLine;
                msbox += "Selecione um CID válido!";
                btnCidPrincipal.Text = "";
                DXMessageBox.Show(msbox, "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnProcedimentoSUS_LostFocus(object sender, RoutedEventArgs e)
        {
            /*if (!string.IsNullOrWhiteSpace(btnProcedimentoSUS.Text) && string.IsNullOrWhiteSpace(txtProcessoSUS.Text))
            {
                DXMessageBox.Show("Código do 'Procedimento SUS' inexistente ou inativo.", "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Warning);
                btnProcedimentoSUS.Text = string.Empty;
            }*/
        }

        private void btnMotivoAlta_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnMotivoAlta.Text) && string.IsNullOrWhiteSpace(txtMotivoAlta.Text))
            {
                DXMessageBox.Show("Motivo de alta não cadastrado.", "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Warning);
                btnMotivoAlta.Text = string.Empty;
            }
        }

        private void btnSetorObito_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(btnSetorObito.Text) && string.IsNullOrWhiteSpace(txtSetorObito.Text))
                DXMessageBox.Show("Setor não cadastrado.", "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void cbDataAlta_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((this.DataContext as vmAltaMedica).HoraSelecionada == null)
                (this.DataContext as vmAltaMedica).HoraSelecionada = DateTime.Now.ToShortTimeString();
        }

        private void btnProcedimentoAMB_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }
    }
}
