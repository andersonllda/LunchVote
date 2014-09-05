using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.DTO;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using HMV.PEP.WPF.UserControls.SumarioDeAlta;
using HMV.PEP.WPF.Windows.SumarioDeAlta;
using StructureMap;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucSumarioAlta.xaml
    /// </summary>
    public partial class ucSumarioAlta : UserControlBase, IUserControl
    {
        private DocumentPanel panelN1;
      //  private DocumentPanel panelN1Exame;
        private Dictionary<HMV.PEP.ViewModel.SumarioDeAlta.vmSumarioAlta.TabsSumarioAlta, DocumentPanel> controles = new Dictionary<HMV.PEP.ViewModel.SumarioDeAlta.vmSumarioAlta.TabsSumarioAlta, DocumentPanel>();
         

        public bool CancelClose { get; set; }
        public bool ControlaSave { get; set; }
        public event EventHandler ExecuteMethod;
        public event EventHandler ChamadaPrescricao;

        public ucSumarioAlta() {
            controles = new Dictionary<HMV.PEP.ViewModel.SumarioDeAlta.vmSumarioAlta.TabsSumarioAlta, DocumentPanel>();
        }

        private void _ajustabotoes(bool pmostra)
        {
            
            if (pmostra)
            {
                if ((this.DataContext as vmSumarioAlta).JaTemAlta)
                {
                    this.btnImprimir.Visibility = Visibility.Visible;
                    this.btnConfirmar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.btnImprimir.Visibility = Visibility.Collapsed;
                    this.btnConfirmar.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.btnImprimir.Visibility = Visibility.Collapsed;
                this.btnConfirmar.Visibility = Visibility.Collapsed;
            }
        }

        public void SetData(object pData)
        {
            UIHelper.SetBusyState();

            this.DataContext = new vmSumarioAlta(pData as Atendimento, App.Usuario);

            InitializeComponent();

            CriarNiveis();
            if ((vmSumarioAlta.TabsSumarioAlta)documentGroupN1.SelectedItem.Tag == vmSumarioAlta.TabsSumarioAlta.Concluir || (vmSumarioAlta.TabsSumarioAlta)documentGroupN1.SelectedItem.Tag == vmSumarioAlta.TabsSumarioAlta.AltaMedica)
                this._ajustabotoes(true);
            else
                this._ajustabotoes(false);
            MotivoAltaRelatorio.DescMotivoAltaRelatorio = string.Empty;

            UIHelper.SetBusyState();
        }
        protected virtual void ChamaPrescricao()
        {
            if (ChamadaPrescricao != null) ChamadaPrescricao(this, EventArgs.Empty);
        }

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.OnExecuteMethod();
        }

        private void CriarNiveis()
        {
            var lstNivelI = (this.DataContext as vmSumarioAlta).Tabs;

            foreach (var NivelI in lstNivelI)
            {
                panelN1 = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, new Uri(@NivelI.Componente, UriKind.Relative));
                panelN1.Caption = NivelI.Descricao;
                panelN1.Tag = NivelI.TipoTab;
                if (NivelI.TipoTab == vmSumarioAlta.TabsSumarioAlta.PosAlta)
                {
                    panelN1.ToolTip = "Medicamentos Pós Alta";
                }
                if (NivelI.TipoTab == vmSumarioAlta.TabsSumarioAlta.Transferencia)
                {
                    Binding binding = new Binding("HabilitaAbaTransferencia");
                    binding.Source = (this.DataContext as vmSumarioAlta);
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Converter = new HMV.Core.Framework.WPF.Converters.BoolToVisibilityConverter();
                    BindingOperations.SetBinding(panelN1, DocumentPanel.VisibilityProperty, binding);
                }
                else if (NivelI.TipoTab != vmSumarioAlta.TabsSumarioAlta.Concluir)
                {
                    Binding bindingAlta = new Binding("MostraAbas");
                    bindingAlta.Source = (this.DataContext as vmSumarioAlta);
                    bindingAlta.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bindingAlta.Converter = new HMV.Core.Framework.WPF.Converters.BoolToVisibilityConverter();
                    BindingOperations.SetBinding(panelN1, DocumentPanel.VisibilityProperty, bindingAlta);
                }

                if ((this.DataContext as vmSumarioAlta).SumarioAlta.Atendimento.TipoDeAtendimento != Core.Domain.Enum.TipoAtendimento.Internacao)
                {
                    if (NivelI.TipoTab != vmSumarioAlta.TabsSumarioAlta.CausaExterna || NivelI.TipoTab != vmSumarioAlta.TabsSumarioAlta.Farmacos || NivelI.TipoTab != vmSumarioAlta.TabsSumarioAlta.Exames)
                    {
                        panelN1.FloatOnDoubleClick = false;
                        panelN1.AllowFloat = false;
                        panelN1.AllowDrag = false;
                        panelN1.AllowHide = false;
                        panelN1.AllowClose = false;

                        if (NivelI.TipoTab == vmSumarioAlta.TabsSumarioAlta.AltaMedica) 
                            (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                        else if (NivelI.TipoTab == vmSumarioAlta.TabsSumarioAlta.Concluir)
                            if ((this.DataContext as vmSumarioAlta).JaTemAlta)
                                (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                    }
                }
                else
                {
                    panelN1.FloatOnDoubleClick = false;
                    panelN1.AllowFloat = false;
                    panelN1.AllowDrag = false;
                    panelN1.AllowHide = false;
                    panelN1.AllowClose = false;

                    if (NivelI.TipoTab == vmSumarioAlta.TabsSumarioAlta.AltaMedica)
                        (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                    else if (NivelI.TipoTab == vmSumarioAlta.TabsSumarioAlta.Concluir)
                        if ((this.DataContext as vmSumarioAlta).JaTemAlta)
                            (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                    
                }

                controles.Add(NivelI.TipoTab, panelN1);

            }
            this.ControlaSave = true;
        }

        private void documentGroupN1_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {

            if (e.Item.IsNotNull() && e.Item.Tag.IsNotNull())
            {
                var tab = (vmSumarioAlta.TabsSumarioAlta)e.Item.Tag;
                 if (controles.ContainsKey(tab))
                {
                    (controles[tab].Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                    controles.Remove(tab);
                }
            }
            
            if (this.ControlaSave)
            {
                this._ajustabotoes(false);
                if ((vmSumarioAlta.TabsSumarioAlta)e.Item.Tag == vmSumarioAlta.TabsSumarioAlta.PosAlta)
                {
                    var lista = (this.DataContext as vmSumarioAlta).VerificaSeDeveAbrirATelaParaSelecionarOsMedicamentosDaUltimaPrescricao();
                    if (lista.Count > 0)
                    {
                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            if (DXMessageBox.Show("Deseja buscar medicamentos da última prescrição? ", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                winPosAltaUltimaPrescricao win = new winPosAltaUltimaPrescricao(lista, (this.DataContext as vmSumarioAlta).SumarioAlta);
                                win.ShowDialog(base.OwnerBase);
                            }
                        }));
                    }
                }
                else if ((vmSumarioAlta.TabsSumarioAlta)e.Item.Tag == vmSumarioAlta.TabsSumarioAlta.Concluir)
                {
                    (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                    this._ajustabotoes(true);
                }
                else if ((vmSumarioAlta.TabsSumarioAlta)documentGroupN1.SelectedItem.Tag == vmSumarioAlta.TabsSumarioAlta.AltaMedica)
                    this._ajustabotoes(true);

                this.Save();

            }

            if (e.Item != null)
            {
                if (e.OldItem != null)
                {
                    this.Save();

                    if ((vmSumarioAlta.TabsSumarioAlta)e.Item.Tag == vmSumarioAlta.TabsSumarioAlta.Concluir)
                    {
                        (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                        this._ajustabotoes(true);
                    }
                    else if ((vmSumarioAlta.TabsSumarioAlta)documentGroupN1.SelectedItem.Tag == vmSumarioAlta.TabsSumarioAlta.AltaMedica)
                        this._ajustabotoes(true);
                }
            }
        }

        public void Save()
        {
            (this.DataContext as vmSumarioAlta).SaveSumarioAltaCommand.Execute(null);
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAlta).VerificaSePodeImprimir)
            {
                ucResumoAltaMedica uc = new ucResumoAltaMedica();
                uc.SetData((this.DataContext as vmSumarioAlta));

                (this.DataContext as vmSumarioAlta).setaDadosDeImpressao();

                uc.Print();
            }
            else
            {
                dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAlta.TabsSumarioAlta)x.Tag == vmSumarioAlta.TabsSumarioAlta.AltaMedica).Single());
            }
        }

        private void ContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ControlaSave)
            {
                if ((sender as ContentControl).DataContext != null)
                    dockLayoutManagerN1.DockController.Activate(documentGroupN1.Items.Where(x => (vmSumarioAlta.TabsSumarioAlta)x.Tag == (vmSumarioAlta.TabsSumarioAlta)(sender as ContentControl).DataContext).SingleOrDefault());
            }
        }

        private void ChamaEvolucoes(Atendimento pAtendimentoRN)
        {
            String[] lDados = "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.RelatorioEvolucoes".Split(';');

            string pasta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly MyAsm = Assembly.LoadFrom(Path.Combine(pasta, lDados[1]));

            System.Type lType = MyAsm.GetType(lDados[2]);
            object lRet;

            if (lType != null)
            {
                ConexaoDTO conn = new ConexaoDTO();
                conn.Banco = App.Banco;
                conn.IdUsuario = App.Usuario.cd_usuario;
                
                List<Atendimento> list = new List<Atendimento>() { (this.DataContext as vmSumarioAlta).Atendimento };
                if (pAtendimentoRN.IsNotNull())
                    list.Add(pAtendimentoRN);
                conn.Dados = list;

                lRet = Activator.CreateInstance(lType, BindingFlags.ExactBinding, null, null, null);
                string lpasso = string.Empty;

                try
                {
                    lpasso = "Passo1";
                    IUserControl obj = (IUserControl)lRet;
                    lpasso = "Passo2";
                    obj.SetData(conn);
                    lpasso = "Passo3";
                }
                catch (Exception e)
                {
                    DXMessageBox.Show("Não foi possível abrir o componente das Evolucoes" + Environment.NewLine + lpasso + "  " + e.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
            }
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext as vmSumarioAlta).vmListaProblemaSumario.ValidaCIDs())
            {
                dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAlta.TabsSumarioAlta)x.Tag == vmSumarioAlta.TabsSumarioAlta.AltaMedica).Single());
                return;
            }

            if ((this.DataContext as vmSumarioAlta).vmAltaMedica.ValidaAlta())
            {
                (this.DataContext as vmSumarioAlta).vmListaProblemaSumario.Save(); // Salva os CIDs

                winConfirmaAlta win = new winConfirmaAlta((this.DataContext as vmSumarioAlta).vmAltaMedica);
                bool? resp = win.ShowDialog(base.OwnerBase);
                if (resp.HasValue)
                    if (resp.Value)
                    {
                        UIHelper.SetBusyState();
                        ucResumoAltaMedica uc = new ucResumoAltaMedica();
                        uc.SetData((this.DataContext as vmSumarioAlta));

                        //Chamaimpressao das evolucoes
                        IRepositorioDeParametrosClinicas repp = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                        Parametro _habilita = repp.OndePEPHabilitaEvolucao().Single();
                        if (_habilita.Valor == "S")
                        {
                            if ((this.DataContext as vmSumarioAlta).Atendimento.AtendimentoPai.IsNull())
                            {
                                var atend = (this.DataContext as vmSumarioAlta).Atendimento.Paciente.Atendimentos.Where(x => x.ID != (this.DataContext as vmSumarioAlta).Atendimento.ID
                                    && x.DataAltaMedica.IsNotNull() && x.AtendimentoPai.IsNotNull()
                                    && x.DataAltaMedica.Value <= (this.DataContext as vmSumarioAlta).Atendimento.DataHoraAtendimento.AddHours(6)).ToList();
                                if (atend.HasItems())
                                {
                                    var aten = atend.FirstOrDefault();
                                    ChamaEvolucoes(aten);
                                }
                                else
                                    ChamaEvolucoes(null);
                            }
                            else
                                ChamaEvolucoes(null);
                        }

                        (this.DataContext as vmSumarioAlta).setaDadosDeImpressao();

                        uc.report.Imprime(2, false);
                        this._ajustabotoes(true);
                        UIHelper.SetBusyState();
                       // DXMessageBox.Show("Alta realizada com sucesso!", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Information);
                        (panelN1.Control as IUserControl).SetData((this.DataContext as vmSumarioAlta));
                    }
            }
            //TFS: 3801
            //else
            //{
            if ((this.DataContext as vmSumarioAlta).vmAltaMedica.AbrePerguntaPrescricao)
            {
                var win = new winMensagemPrescricao();
                win.ShowDialog(base.OwnerBase);
                if (win.DialogResult == true)
                {
                    this.ChamaPrescricao();
                }
                dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAlta.TabsSumarioAlta)x.Tag == vmSumarioAlta.TabsSumarioAlta.AltaMedica).Single());
            }
            //}            
        }
    }
}
