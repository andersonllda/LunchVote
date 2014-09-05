using System;
using System.Reflection;
using System.Windows;
using System.IO;
using HMV.Core.Framework.WPF;
using HMV.Core.DTO;
using HMV.Core.Interfaces;
using HMV.PEP.WPF.UserControls;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using System.Linq;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    /// <summary>
    /// Interaction logic for ucSumarioAvalMedica.xaml
    /// </summary>
    public partial class ucSumarioAvalMedica : UserControlBase, IUserControl
    {
        DocumentGroup documentGroupN1 = new DocumentGroup();
        DocumentPanel panel;
        LayoutGroup lg = new LayoutGroup();
        public event EventHandler ExecuteMethod;
       

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty);
        }

        public ucSumarioAvalMedica()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            base.SetData(new vmSumarioAvaliacaoMedicaCO(pData as Atendimento, App.Usuario, new GetSettings().IsCorpoClinico));
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

            foreach (var tab in (this.DataContext as vmSumarioAvaliacaoMedicaCO).Tabs)
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

            if (e.Item.IsNotNull())
            {
                (this.DataContext as vmSumarioAvaliacaoMedicaCO).TipoTabSelecionada = ((e.Item as DocumentPanel).ToolTip as vmSumarioAvaliacaoMedicaCO.TabsSumarioAvaliacaoMedicaCO?);

                if (((e.Item as DocumentPanel).Control as UserControlBase).DataContext.IsNull() && (e.Item as DocumentPanel).Tag.IsNotNull())
                    BindingOperations.SetBinding((e.Item as DocumentPanel).Control as UserControlBase, UserControlBase.DataContextProperty, (e.Item as DocumentPanel).Tag as Binding);

                if ((this.DataContext as vmSumarioAvaliacaoMedicaCO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCO.TabsSumarioAvaliacaoMedicaCO.FinalizarImprimir) //Sempre seta novamente quando é relatório para atualizar.
                {
                    this.dockLayoutManagerN1.Margin = new Thickness(0, 0, 0, 35); //Margin="0,0,0,35"
                    ((e.Item as DocumentPanel).Control as ucResumo).SetData((this.DataContext as vmSumarioAvaliacaoMedicaCO));
                    //((e.Item as DocumentPanel).Control as ucResumo).rpt = null;
                }

                if ((this.DataContext as vmSumarioAvaliacaoMedicaCO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCO.TabsSumarioAvaliacaoMedicaCO.DiagnosticosHipotesesDiagnosticas)
                    (this.DataContext as vmSumarioAvaliacaoMedicaCO).SalvaDiagnosticoHipotese = true;
                else
                    (this.DataContext as vmSumarioAvaliacaoMedicaCO).SalvaDiagnosticoHipotese = false;

                   
                    
            }
        }

        private void HMVButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((panel as DocumentPanel).Control as ucResumo).rpt.Print();
        }

        private void btnFinaliza_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAvaliacaoMedicaCO).Imprimir())
            {                
                documentGroupN1.Visibility = System.Windows.Visibility.Collapsed;
                documentGroupN1.Items.Clear();
                InsereTabs();
                documentGroupN1.Visibility = System.Windows.Visibility.Visible;
                ((panel as DocumentPanel).Control as ucResumo).rpt.Print();
            }
            else
            {
                dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAvaliacaoMedicaCO.TabsSumarioAvaliacaoMedicaCO)x.ToolTip == (this.DataContext as vmSumarioAvaliacaoMedicaCO).TipoTabSelecionada).Single());
                if ((this.DataContext as vmSumarioAvaliacaoMedicaCO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCO.TabsSumarioAvaliacaoMedicaCO.Anamnese)
                {
                    ((dockLayoutManagerN1.ActiveDockItem as DocumentPanel).Control as ucAnamnese).SetaAba();
                }
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

        private void UserControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!(this.DataContext as vmSumarioAvaliacaoMedicaCO).PodeAbrir)
            //{
            //    DXMessageBox.Show("O Sumário não existe!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            //    this.OnExecuteMethod();
            //}
        }

    }
}
