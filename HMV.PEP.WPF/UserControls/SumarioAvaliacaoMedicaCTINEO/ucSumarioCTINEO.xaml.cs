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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Docking.Base;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaCTINEO;
using HMV.PEP.WPF.Report.SumarioAvaliacaoMedicaCTINEO;
using HMV.ProcessoEnfermagem.WPF.Base;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaCTINEO
{
    /// <summary>
    /// Interaction logic for ucSumarioCTINEO.xaml
    /// </summary>
    public partial class ucSumarioCTINEO : UserControlBase, IUserControl
    {
        DocumentGroup documentGroupN1 = new DocumentGroup();
        DocumentPanel panel;
        LayoutGroup lg = new LayoutGroup();


        public ucSumarioCTINEO()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            base.SetData(new vmSumarioAvaliacaoMedicaCTINEO(pData as Atendimento, App.Usuario, new GetSettings().IsCorpoClinico));           
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

            foreach (var tab in (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).Tabs)
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

            if (e.Item.IsNotNull())
            {
                (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).TipoTabSelecionada = ((e.Item as DocumentPanel).ToolTip as vmSumarioAvaliacaoMedicaCTINEO.TabsSumarioAvaliacaoMedicaCTINEO?);

                if (((e.Item as DocumentPanel).Control as UserControlBase).DataContext.IsNull() && (e.Item as DocumentPanel).Tag.IsNotNull())
                    BindingOperations.SetBinding((e.Item as DocumentPanel).Control as UserControlBase, UserControlBase.DataContextProperty, (e.Item as DocumentPanel).Tag as Binding);

                if ((this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCTINEO.TabsSumarioAvaliacaoMedicaCTINEO.FinalizarImprimir) //Sempre seta novamente quando é relatório para atualizar.
                    ((e.Item as DocumentPanel).Control as ucResumo).SetData((this.DataContext as vmSumarioAvaliacaoMedicaCTINEO));

                if ((this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCTINEO.TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico)
                    ((e.Item as DocumentPanel).Control as ucExameFisico).Refresh();

                if ((this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCTINEO.TabsSumarioAvaliacaoMedicaCTINEO.DiagnosticosHipotesesDiagnosticas)
                    (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).SalvaDiagnosticoHipotese = true;
                else
                    (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).SalvaDiagnosticoHipotese = false;
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

        private void btnFinaliza_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).Imprimir())
            {
                documentGroupN1.Visibility = System.Windows.Visibility.Collapsed;
                documentGroupN1.Clear();
                InsereTabs();
                documentGroupN1.Visibility = System.Windows.Visibility.Visible;
                ((panel as DocumentPanel).Control as ucResumo).rpt.Print();
            }
            else
            {
                dockLayoutManagerN1.Activate(documentGroupN1.Items.Where(x => (vmSumarioAvaliacaoMedicaCTINEO.TabsSumarioAvaliacaoMedicaCTINEO)x.ToolTip == (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).TipoTabSelecionada).Single());
                if ((this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).TipoTabSelecionada == vmSumarioAvaliacaoMedicaCTINEO.TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico)
                {
                    ((dockLayoutManagerN1.ActiveDockItem as DocumentPanel).Control as ucExameFisico).SetaAba();
                }
            }
        }

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            ((panel as DocumentPanel).Control as ucResumo).rpt.Print();
        }

        public void Save()
        {
            base.ExecutaCommandSalvar(null);
        }
    }
}
