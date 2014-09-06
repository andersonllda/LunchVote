using System.Windows.Data;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.PEP.WPF.Windows.CentroObstetrico.BoletimDeEmergencia;
using System.Linq;
using HMV.PEP.WPF.Report.CentroObstetrico;
using HMV.PEP.WPF.Report;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucBoletimCO.xaml
    /// </summary>
    public partial class ucBoletimCO : UserControlBase, IUserControl
    {
        private DocumentGroup documentGroupN1;

        public ucBoletimCO()
        {
            InitializeComponent();
        }

        //public override void SetData(ViewModelBase pVMObject)
        //{
        //}

        private void InsereTabs(vmBoletimEmergenciaCO vm)
        {
            documentGroupN1 = null; 
            DocumentPanel panel;

            LayoutGroup lg = new LayoutGroup();
            documentGroupN1 = new DocumentGroup();
            documentGroupN1.MDIStyle = MDIStyle.Tabbed;
            documentGroupN1.ShowCaptionImage = true;
            documentGroupN1.FloatOnDoubleClick = false;
            documentGroupN1.AllowContextMenu = false; documentGroupN1.AllowClose = false;
            documentGroupN1.AllowDock = false; documentGroupN1.AllowDrag = false;
            documentGroupN1.AllowMove = false; documentGroupN1.ShowControlBox = false;
            documentGroupN1.AllowRename = false; documentGroupN1.ShowDropDownButton = false;
            documentGroupN1.ShowRestoreButton = false; documentGroupN1.AllowHide = false;
            documentGroupN1.ClosePageButtonShowMode = ClosePageButtonShowMode.NoWhere;
            documentGroupN1.AllowRestore = false; documentGroupN1.ShowCloseButton = false;

            foreach (var tab in (this.DataContext as vmBoletimEmergenciaCO).Tabs)
            {
                panel = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, tab.Componente);
                panel.Caption = tab.Descricao;
                //guarda o binding para setar APENAS quando entrar a primeira vez na tab...
                panel.Tag = tab.Binding;
                //guarda o tipo da tab no tooltip!
                panel.ToolTip = tab.TipoTab;
                panel.FloatOnDoubleClick = false;
                panel.AllowFloat = false;
                panel.AllowDrag = false;
                panel.AllowHide = false;
                panel.AllowClose = false;
                lg.Add(documentGroupN1);
                dockLayoutManagerN1.LayoutRoot = lg;
            }
            //documentGroupN1.SelectedItemChanged += new SelectedItemChangedEventHandler(documentGroupN1_SelectedItemChanged);        
            //this.documentGroupN1_SelectedItemChanged(documentGroupN1, new SelectedItemChangedEventArgs(documentGroupN1.SelectedItem, null));
            if ( vm.Boletim.DataAlta.HasValue )
                documentGroupN1.SelectedTabIndex = (int)HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.Relatorio;
            else if ( vm.Usuarios.DomainObject.Prestador.Conselho.isMedico() )
                documentGroupN1.SelectedTabIndex = (int)HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.MotivoConsulta;
            else 
                documentGroupN1.SelectedTabIndex = (int)HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.Classificacao;
            
            dockLayoutManagerN1_DockItemActivated(documentGroupN1, new DockItemActivatedEventArgs(documentGroupN1.SelectedItem, null));
   
        }

        private void documentGroupN1_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {

            vmBoletimEmergenciaCO vm = (this.DataContext as vmBoletimEmergenciaCO);
            HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO? tab = ((e.Item as DocumentPanel).ToolTip as vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO?);

            bool isValid = true;
            if (tab != vm.TipoTabSelecionada && vm.DictionaryCO.HasItems() && vm.DictionaryCO.Count(x => x.Key == vm.TipoTabSelecionada.Value) > 0 )
            {
                isValid = vm.DictionaryCO[vm.TipoTabSelecionada.Value].IsValid;
                if (!isValid)
                {
                    documentGroupN1.SelectedTabIndex = (int)vm.TipoTabSelecionada;
                }
            }

            if (isValid)
            {
                base.ExecutaCommandSalvar(null);
                (this.DataContext as vmBoletimEmergenciaCO).TipoTabSelecionada = ((e.Item as DocumentPanel).ToolTip as vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO?);

                if (((e.Item as DocumentPanel).Control as UserControlBase).DataContext.IsNull())
                {
                    BindingOperations.SetBinding((e.Item as DocumentPanel).Control as UserControlBase, UserControlBase.DataContextProperty, (e.Item as DocumentPanel).Tag as Binding);

                    if ((e.Item as DocumentPanel).Control.GetType() == typeof(ucAvaliacaoClinica))
                        ((e.Item as DocumentPanel).Control as UserControlBase).SetData(null);

                    if ((e.Item as DocumentPanel).Control.GetType() == typeof(ucSinaisVitais))
                        ((e.Item as DocumentPanel).Control as UserControlBase).SetData(null);
                }

                // Atualiza o relatório 
                if ((e.Item as DocumentPanel).Control.GetType() == typeof(ucRelatorio))
                    ((e.Item as DocumentPanel).Control as UserControlBase).SetData(null);
            }
        }

        private void btnImprimir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btnImprimir.IsEnabled = false;
            vmRelatorioBoletimEmergenciaCO vm = new vmRelatorioBoletimEmergenciaCO((this.DataContext as vmBoletimEmergenciaCO).Boletim);
            rptBoletimEmergenciaCO rpt = new rptBoletimEmergenciaCO(vm);
            winRelatorio rel = new winRelatorio(rpt, false, "", (this.DataContext as vmBoletimEmergenciaCO).Boletim.DataAlta == null);
            rpt.Print();
            DXMessageBox.Show("Documento encaminhado para impressora!");
            btnImprimir.IsEnabled = true;
        }

        private void HMVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            base.ExecutaCommandSalvar(null);
            vmBoletimEmergenciaCO vm = (this.DataContext as vmBoletimEmergenciaCO);

            if (vm.validaAlta())
            {
                vmAlta alta = new vmAlta(vm.Boletim, vm.Usuarios);
                winAlta win = new winAlta(alta);
                win.ShowDialog(this.OwnerBase);

                if (vm.Boletim.AltaCO.IsNotNull())
                {
                    vm.Tabs = null;
                    InsereTabs(vm);
                    documentGroupN1.SelectedTabIndex = (int)HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.Relatorio;
                    dockLayoutManagerN1_DockItemActivated(documentGroupN1, new DockItemActivatedEventArgs(documentGroupN1.SelectedItem, null));
                }
            }

            vm.RefreshViewModel();
        }

        private void dockLayoutManagerN1_DockItemActivated(object sender, DockItemActivatedEventArgs e)
        {
            if (e.Item.IsNull())
                return;

            vmBoletimEmergenciaCO vm = (this.DataContext as vmBoletimEmergenciaCO);
            HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO? tab = ((e.Item as DocumentPanel).ToolTip as vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO?);

            bool isValid = true;
            if (tab != vm.TipoTabSelecionada && vm.DictionaryCO.HasItems() && vm.DictionaryCO.Count(x => x.Key == vm.TipoTabSelecionada.Value) > 0)
            {
                isValid = vm.DictionaryCO[vm.TipoTabSelecionada.Value].IsValid;
                if (!isValid)
                {
                    documentGroupN1.SelectedTabIndex = (int)vm.TipoTabSelecionada;
                }
            }

            if (isValid)
            {
                base.ExecutaCommandSalvar(null);
                (this.DataContext as vmBoletimEmergenciaCO).TipoTabSelecionada = ((e.Item as DocumentPanel).ToolTip as vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO?);

                if (((e.Item as DocumentPanel).Control as UserControlBase).DataContext.IsNull())
                {
                    BindingOperations.SetBinding((e.Item as DocumentPanel).Control as UserControlBase, UserControlBase.DataContextProperty, (e.Item as DocumentPanel).Tag as Binding);

                    if ((e.Item as DocumentPanel).Control.GetType() == typeof(ucAvaliacaoClinica))
                        ((e.Item as DocumentPanel).Control as UserControlBase).SetData(null);

                    //if ((e.Item as DocumentPanel).Control.GetType() == typeof(ucSinaisVitais))
                    //    ((e.Item as DocumentPanel).Control as UserControlBase).SetData(null);
                }

                // Atualiza o relatório 
                if ((e.Item as DocumentPanel).Control.GetType() == typeof(ucRelatorio))
                    ((e.Item as DocumentPanel).Control as UserControlBase).SetData(null);
            }
        }

        private void btnNovo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vmBoletimEmergenciaCO vm = (this.DataContext as vmBoletimEmergenciaCO);
            vm.Commands.ExecuteCommand(Core.Framework.Commands.enumCommand.CommandIncluir, null);
            vm.Tabs = null;
            
            InsereTabs(vm);
            documentGroupN1.SelectedTabIndex = (int)HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia.vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.Classificacao;
            dockLayoutManagerN1_DockItemActivated(documentGroupN1, new DockItemActivatedEventArgs(documentGroupN1.SelectedItem, null));
        }

        public void SetData(object pData)
        {
            vmBoletimEmergenciaCO vm = new vmBoletimEmergenciaCO(pData as Atendimento, (pData as Atendimento).Paciente, App.Usuario);
            base.SetData(vm);            
            InsereTabs(vm);
            SetEvents();
        }

        public bool CancelClose
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        private void btnVisualizar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vmRelatorioBoletimEmergenciaCO vm = new vmRelatorioBoletimEmergenciaCO((this.DataContext as vmBoletimEmergenciaCO).Boletim);
            rptBoletimEmergenciaCO rpt = new rptBoletimEmergenciaCO(vm);
            winRelatorio rel = new winRelatorio(rpt, false, "Boletim de Emergência", !(this.DataContext as vmBoletimEmergenciaCO).boolImprimir);
            rel.ShowDialog(base.OwnerBase);             
        }

        public void Salva()
        {
            (this.DataContext as vmBoletimEmergenciaCO).Salva();
        }
    }
}
