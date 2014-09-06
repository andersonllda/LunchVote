using System.Windows.Data;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Framework.Extensions;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Framework.ViewModelBaseClasses;
using System.Linq;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaEndoscopia
{
    /// <summary>
    /// Interaction logic for ucSumarioAvaliacaoMedicaEndoscopia.xaml
    /// </summary>
    public partial class ucSumarioAvaliacaoMedicaEndoscopia : UserControlBase
    {
        DocumentPanel panel;
        LayoutGroup lg = new LayoutGroup();
        DocumentGroup documentGroupN1 = new DocumentGroup();
        public ucSumarioAvaliacaoMedicaEndoscopia()
        {
            InitializeComponent();
        }

        public override void SetData(ViewModelBase pVMObject)
        {
            base.SetData(pVMObject);
            InsereTabs();
        }

        private void InsereTabs()
        {
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

            foreach (var tab in (this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).Tabs)
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
            documentGroupN1.SelectedItemChanged += new SelectedItemChangedEventHandler(documentGroupN1_SelectedItemChanged);
            this.documentGroupN1_SelectedItemChanged(documentGroupN1, new SelectedItemChangedEventArgs(documentGroupN1.SelectedItem, null));
        }

        private void documentGroupN1_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            //if (Finalizando)
            {
                base.ExecutaCommandSalvar(null);

                if (e.Item.IsNotNull())
                {
                    (this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).TipoTabSelecionada = ((e.Item as DocumentPanel).ToolTip as vmSumarioAvaliacaoMedicaEndoscopia.TabsSumarioAvaliacaoMedicaEndoscopia?);

                    if (((e.Item as DocumentPanel).Control as UserControlBase).DataContext.IsNull() && (e.Item as DocumentPanel).Tag.IsNotNull())
                        BindingOperations.SetBinding((e.Item as DocumentPanel).Control as UserControlBase, UserControlBase.DataContextProperty, (e.Item as DocumentPanel).Tag as Binding);

                    if ((this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).TipoTabSelecionada == vmSumarioAvaliacaoMedicaEndoscopia.TabsSumarioAvaliacaoMedicaEndoscopia.Visualizar) //Sempre seta novamente quando é relatório para atualizar.
                        ((e.Item as DocumentPanel).Control as ucVisualizar).SetData((this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia));
                }
            }
        }

        private void HMVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((panel as DocumentPanel).Control as ucVisualizar).rpt.Print();
        }

        private void btnFinaliza_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).Imprimir())
            {
                documentGroupN1.Visibility = System.Windows.Visibility.Collapsed;
                documentGroupN1.Items.Clear();
                InsereTabs();
                documentGroupN1.Visibility = System.Windows.Visibility.Visible;
                ((panel as DocumentPanel).Control as ucVisualizar).rpt.Print();
            }
            else
                dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAvaliacaoMedicaEndoscopia.TabsSumarioAvaliacaoMedicaEndoscopia)x.ToolTip == (this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).TipoTabSelecionada).Single());
        }
    }
}
