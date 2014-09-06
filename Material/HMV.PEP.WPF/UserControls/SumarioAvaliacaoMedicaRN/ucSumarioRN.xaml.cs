using System;
using System.Windows.Data;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN;
using System.Linq;
using System.Windows;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaRN
{
    /// <summary>
    /// Interaction logic for ucSumarioRN.xaml
    /// </summary>
    public partial class ucSumarioRN : UserControlBase, IUserControl
    {
        DocumentGroup documentGroupN1 = new DocumentGroup();
        DocumentPanel panel;
        LayoutGroup lg = new LayoutGroup();

        public ucSumarioRN()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            base.SetData(new vmSumarioAvaliacaoMedicaRN(pData as Atendimento, App.Usuario, new GetSettings().IsCorpoClinico));
            (this.DataContext as vmSumarioAvaliacaoMedicaRN).PodeAbrir();
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
            documentGroupN1.DestroyOnClosingChildren = true;

            foreach (var tab in (this.DataContext as vmSumarioAvaliacaoMedicaRN).Tabs)
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
            base.ExecutaCommandSalvar(null);
            this.dockLayoutManagerN1.Margin = new Thickness(0, 0, 0, 0);
            this.grdBotoes.Visibility = System.Windows.Visibility.Collapsed;

            if (e.Item.IsNotNull())
            {
                (this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada = ((e.Item as DocumentPanel).ToolTip as vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN?);

                if (((e.Item as DocumentPanel).Control as UserControlBase).DataContext.IsNull() && (e.Item as DocumentPanel).Tag.IsNotNull())
                {
                    BindingOperations.SetBinding((e.Item as DocumentPanel).Control as UserControlBase, UserControlBase.DataContextProperty, (e.Item as DocumentPanel).Tag as Binding);
                    if ((this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada == vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir)
                    {
                        this.dockLayoutManagerN1.Margin = new Thickness(0, 0, 0, 35);
                        this.grdBotoes.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                    if ((this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada == vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir)
                    //Sempre seta novamente quando é relatório para atualizar.
                    {
                        this.dockLayoutManagerN1.Margin = new Thickness(0, 0, 0, 35); //Margin="0,0,0,35"
                        ((e.Item as DocumentPanel).Control as ucResumoRN).SetData((this.DataContext as vmSumarioAvaliacaoMedicaRN));
                        this.grdBotoes.Visibility = System.Windows.Visibility.Visible;
                    }

                if ((this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada != vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN.FinalizarImprimir)
                {
                    var item = documentGroupN1.Items.Where(x => (x as DocumentPanel).Control.GetType() == typeof(ucResumoRN)).FirstOrDefault();
                    if (item.IsNotNull())
                        ((item as DocumentPanel).Control as ucResumoRN).Clear();
                }
            }
        }

        private void HMVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((panel as DocumentPanel).IsNotNull())
                if (((panel as DocumentPanel).Control as ucResumoRN).IsNotNull())
                    if (((panel as DocumentPanel).Control as ucResumoRN).rpt.IsNotNull())
                        ((panel as DocumentPanel).Control as ucResumoRN).rpt.Print();
        }

        public void Save()
        {
            base.ExecutaCommandSalvar(null);
        }

        private void btnFinaliza_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAvaliacaoMedicaRN).Imprimir())
            {
                this.grdBotoes.Visibility = System.Windows.Visibility.Collapsed;
                documentGroupN1.Visibility = System.Windows.Visibility.Collapsed;
                documentGroupN1.Clear();
                InsereTabs();
                documentGroupN1.Visibility = System.Windows.Visibility.Visible;
                this.grdBotoes.Visibility = System.Windows.Visibility.Visible;
                if ((panel as DocumentPanel).Control is ucResumoRN)
                    ((panel as DocumentPanel).Control as ucResumoRN).rpt.Print();
            }
            else
            {
                if (documentGroupN1.Items.Count(x => (vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN)x.ToolTip == (this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada) > 0)
                    dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN)x.ToolTip == (this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada).Single());


                //if ((this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada == vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN.SumarioObstetrico)
                //{
                //    ((dockLayoutManagerN1.ActiveDockItem as DocumentPanel).Control as ucObstetrico).SetaAba();
                //}
                //if ((this.DataContext as vmSumarioAvaliacaoMedicaRN).TipoTabSelecionada == vmSumarioAvaliacaoMedicaRN.TabsSumarioAvaliacaoMedicaRN.ExameClinico)
                //{
                //    ((dockLayoutManagerN1.ActiveDockItem as DocumentPanel).Control as ucExameFisico).SetaAba();
                //}
            }
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private void UserControlBase_Unloaded(object sender, RoutedEventArgs e)
        {
            var item = documentGroupN1.Items.Where(x => (x as DocumentPanel).Control.GetType() == typeof(ucResumoRN)).FirstOrDefault();
            if (item.IsNotNull())
                ((item as DocumentPanel).Control as ucResumoRN).Clear();
        }
    }
}
