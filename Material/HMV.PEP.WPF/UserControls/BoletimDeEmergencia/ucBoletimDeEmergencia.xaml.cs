using System;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Printing;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.PEP.ViewModel.PEP.BoletimEmergencia;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.BoletimEmergencia;
using HMV.PEP.WPF.Windows;
using StructureMap;

namespace HMV.PEP.WPF.UserControls.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucBoletimDeEmergencia.xaml
    /// </summary>
    public partial class ucBoletimDeEmergencia : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        private rtpBoletimEmergencia rtp = null;

        public ucBoletimDeEmergencia()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            rtp = new rtpBoletimEmergencia();

            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.Refresh(pData as Atendimento);

            this.DataContext = new vmBoletimEmergencia(pData as Atendimento, App.Usuario, App.IsPAME);

            if (!(this.DataContext as vmBoletimEmergencia).IsEventHandlerRegistered("EventRelatorio"))
                (this.DataContext as vmBoletimEmergencia).EventRelatorio += new EventHandler(ucBoletimDeEmergencia_EventRelatorio);

            if (App.IsPAME)
                if (!App.Usuario.Prestador.IsNurse)
                    if (!(this.DataContext as vmBoletimEmergencia).TemPAME && !(this.DataContext as vmBoletimEmergencia).BoletimFechado)
                    {
                        winCadPAME win = new winCadPAME(this.DataContext as vmBoletimEmergencia);
                        win.ShowDialog(this.OwnerBase);
                    }

            if ((this.DataContext as vmBoletimEmergencia).BoletimFechado)
                ucBoletimDeEmergencia_EventRelatorio(this, null);
            else
                InsereTabs();
        }

        void ucBoletimDeEmergencia_EventRelatorio(object sender, EventArgs e)
        {
            this.gReport.Visibility = System.Windows.Visibility.Visible;
            rtp.DataSource = new vmRelatorioBoletimEmergencia((this.DataContext as vmBoletimEmergencia).BoletimEmergencia, App.IsPAME).Relatorio();
            var uc = new ucReportBase(rtp, true, false, true);
            this.gReport.Children.Add(uc);
        }

        private void InsereTabs()
        {
            DocumentPanel panel;

            LayoutGroup lg = new LayoutGroup();
            DocumentGroup documentGroupN2 = new DocumentGroup();
            documentGroupN2.MDIStyle = MDIStyle.Tabbed;
            documentGroupN2.ShowCaptionImage = true;
            documentGroupN2.FloatOnDoubleClick = false;
            documentGroupN2.AllowContextMenu = false; documentGroupN2.AllowClose = false;
            documentGroupN2.AllowDock = false; documentGroupN2.AllowDrag = false;
            documentGroupN2.AllowMove = false; documentGroupN2.ShowControlBox = false;
            documentGroupN2.AllowRename = false; documentGroupN2.ShowDropDownButton = false;
            documentGroupN2.ShowRestoreButton = false; documentGroupN2.AllowHide = false;
            documentGroupN2.ClosePageButtonShowMode = ClosePageButtonShowMode.NoWhere;
            documentGroupN2.AllowRestore = false; documentGroupN2.ShowCloseButton = false;

            panel = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN2, new Uri(@"UserControls\BoletimDeEmergencia\ucItensDeRegistro.xaml", UriKind.Relative));

            panel.Caption = "Itens de Registro";
            panel.Name = "ItensRegistro";
            panel.FloatOnDoubleClick = false;
            panel.AllowFloat = false;
            panel.AllowDrag = false;
            panel.AllowHide = false;
            panel.AllowClose = false;

            Binding binding = new Binding("vmItensDeRegistro");
            binding.Source = (this.DataContext as vmBoletimEmergencia);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding((panel.Control as ucItensDeRegistro), ucItensDeRegistro.DataContextProperty, binding);

            (panel.Control as ucItensDeRegistro).SetFocusTexto();

            lg.Add(documentGroupN2);
            dockLayoutManagerN1.LayoutRoot = lg;

            panel = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN2, new Uri(@"UserControls\BoletimDeEmergencia\ucCIDsDiagnostico.xaml", UriKind.Relative));

            panel.Caption = "CID's / Diagnóstico";
            panel.Name = "CID";
            panel.FloatOnDoubleClick = false;
            panel.AllowFloat = false;
            panel.AllowDrag = false;
            panel.AllowHide = false;
            panel.AllowClose = false;

            binding = new Binding("vmCidDiagnostico");
            binding.Source = (this.DataContext as vmBoletimEmergencia);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding((panel.Control as ucCIDsDiagnostico), ucCIDsDiagnostico.DataContextProperty, binding);

            lg.Add(documentGroupN2);

            dockLayoutManagerN1.LayoutRoot = lg;
        }

        private void btnSalvar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as vmBoletimEmergencia).SalvaBoletim();
            if (dockLayoutManagerN1.DockController.ActiveItem != null && dockLayoutManagerN1.DockController.ActiveItem.Parent != null
                && dockLayoutManagerN1.DockController.ActiveItem.Parent.SelectedTabIndex == 1)
                dockLayoutManagerN1.DockController.ActiveItem.Parent.SelectedTabIndex = 0;
        }

        public void Salva(bool pMostraMsg)
        {
            (this.DataContext as vmBoletimEmergencia).SalvaBoletim(pMostraMsg);
        }

        private void btnNovo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as vmBoletimEmergencia).NovoBoletim();
            this.gReport.Children.Clear();
            this.gReport.Visibility = System.Windows.Visibility.Collapsed;
            InsereTabs();
        }

        private void btnVisualizar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            rtp.DataSource = new vmRelatorioBoletimEmergencia((this.DataContext as vmBoletimEmergencia).BoletimEmergencia, App.IsPAME).Relatorio();
            winRelatorio win = new winRelatorio(rtp, false, "Boletim de Emergência", true);
            win.ShowDialog(base.OwnerBase);
        }

        private void dockLayoutManagerN1_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            DocumentPanel panel = dockLayoutManagerN1.GetItem(e.NewValue.ToString()) as DocumentPanel;
            dockLayoutManagerN1.LayoutController.Activate(panel);
        }

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            rtp.Imprime();
        }

        private void btnPame_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IsEnabled = false;
            winCadPAME win = new winCadPAME(this.DataContext as vmBoletimEmergencia);
            win.ShowDialog(this.OwnerBase);
            this.IsEnabled = true;
        }
    }
}
