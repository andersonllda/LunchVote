using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.Core.Interfaces;
using HMV.PEP.Interfaces;
using HMV.PEP.WPF.Cadastros;
using HMV.PEP.WPF.Cadastros.SumarioAvaliacaoM;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoM;
using StructureMap;
using NHibernate.Validator.Engine;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Windows;


namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for UserControlDiagnosticoHipoteses.xaml
    /// </summary>
    /// 
    public partial class ucDiagnosticoHipoteses : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucDiagnosticoHipoteses()
        {

            InitializeComponent();
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadCID win = new winCadCID((this.DataContext as SumarioAvaliacaoMedica));
            if (win.ShowDialog(base.OwnerBase).Equals(true))
                Refresh();
        }

        private void Refresh()
        {
            var x = this.DataContext as SumarioAvaliacaoMedica;
            this.DataContext = null;
            this.DataContext = x;
            gdDiagnostico.RefreshData();
            HabilitaDesabilitaBotoes();
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            //if (gdDiagnostico.VisibleRowCount > 0)
            //{

            if (DXMessageBox.Show("Deseja realmente Excluir o CID do Diagnóstico?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                (this.DataContext as SumarioAvaliacaoMedica).RemoveDiagnostico((Diagnostico)gdDiagnostico.GetFocusedRow());
                gdDiagnostico.RefreshData();
                HabilitaDesabilitaBotoes();
            }
            //}
            //else
            //    DXMessageBox.Show("Não há item para excluir!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnIncluirHipo_Click(object sender, RoutedEventArgs e)
        {
            winCadHiposteseDiagnostico win = new winCadHiposteseDiagnostico((this.DataContext as SumarioAvaliacaoMedica));
            if (win.ShowDialog(base.OwnerBase).Equals(true))
                Refresh();
        }

        private void btnExcluirHipo_Click(object sender, RoutedEventArgs e)
        {
            //if (gdHipotese.VisibleRowCount > 0)
            //{
            if (DXMessageBox.Show("Deseja realmente Excluir a hipótese diagnostica?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                (this.DataContext as SumarioAvaliacaoMedica).RemoveHipotese((Hipotese)gdHipotese.GetFocusedRow());
                gdHipotese.RefreshData();
                HabilitaDesabilitaBotoes();
            }

            //}
            //else
            //    DXMessageBox.Show("Não há item para excluir!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica);
            HabilitaDesabilitaBotoes();
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
                            (this.DataContext as SumarioAvaliacaoMedica).Atendimento.Cid = cid;
                            this.txtDescricaoCIDPrincipal.Text = cid.Descricao;
                        }
                        else
                        {
                            (this.DataContext as SumarioAvaliacaoMedica).Atendimento.Cid = null;
                            this.txtDescricaoCIDPrincipal.Text = string.Empty;
                        }
                    }
                    else
                    {
                        (this.DataContext as SumarioAvaliacaoMedica).Atendimento.Cid = null;
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

        private void btnIncluirCidListaProblema_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ICidService serv = ObjectFactory.GetInstance<ICidService>();
                Cid cid = serv.FiltraPorCid10(txtCIDPrincipal.Text);

                if (cid != null)
                {
                    winCadCIDsNaListaDeProblemas win = new winCadCIDsNaListaDeProblemas((this.DataContext as SumarioAvaliacaoMedica), true);
                    win.ShowDialog(base.OwnerBase);
                }
                else
                    txtDescricaoCIDPrincipal.Text = "";
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(false);
            //win.txtCodigoCid.Text = txtCIDPrincipal.Text;
            win.ShowDialog(base.OwnerBase);
            if (win.CID != null)
            {
                txtCIDPrincipal.Text = win.CID.Id.ToString();
                txtDescricaoCIDPrincipal.Text = win.CID.Descricao;
                //ProblemaJaExistente(txtCIDPrincipal.Text);
            }
        }

        //private void ProblemaJaExistente(string CIDPrincipal)
        //{
        //    if (this.DataContext != null)
        //        this.btnIncluirCidListaProblema.IsEnabled = (this.DataContext as SumarioAvaliacaoMedica).Paciente.ProblemasPaciente.Where(x => x.CID.Id.Equals(CIDPrincipal) && x.Status.Equals(StatusAlergiaProblema.Ativo)).Count().Equals(0);
        //}

        private void btnCids_Click(object sender, RoutedEventArgs e)
        {
            winCidsSugeridos win = new winCidsSugeridos((this.DataContext as SumarioAvaliacaoMedica).Paciente);
            win.ShowDialog(base.OwnerBase);
        }

        private void HabilitaDesabilitaBotoes()
        {
            btnExcluirHipo.IsEnabled = false;
            btnExcluir.IsEnabled = false;
            //btnIncluirCidsListadeProblema.IsEnabled = false;
            if ((this.DataContext as SumarioAvaliacaoMedica).Diagnosticos != null)
                if ((this.DataContext as SumarioAvaliacaoMedica).Diagnosticos.Count > 0)
                {
                    btnExcluir.IsEnabled = true;
                  //  btnIncluirCidsListadeProblema.IsEnabled = true;
                }
            if ((this.DataContext as SumarioAvaliacaoMedica).Hipoteses != null)
                if ((this.DataContext as SumarioAvaliacaoMedica).Hipoteses.Count > 0)
                    btnExcluirHipo.IsEnabled = true;
        }

        private void btnIncluirCidsListadeProblema_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as SumarioAvaliacaoMedica).Atendimento.Cid == null)
                DXMessageBox.Show("Inclua o Cid principal primeiro!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                winCadCIDsNaListaDeProblemas win = new winCadCIDsNaListaDeProblemas((this.DataContext as SumarioAvaliacaoMedica), false);
                win.ShowDialog(base.OwnerBase);
            }
        }

        private void btnIncluir_Click_1(object sender, RoutedEventArgs e)
        {
            winVisualizaListaProblema win = new winVisualizaListaProblema((this.DataContext as SumarioAvaliacaoMedica).Paciente);
            win.ShowDialog(null);
        }
    }
}
