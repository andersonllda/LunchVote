using System;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Docking;
using HMV.Core.Framework.WPF;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Framework.Extensions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;

namespace HMV.PEP.WPF.Windows.CheckListUDI
{
    /// <summary>
    /// Interaction logic for winCadCheckList.xaml
    /// </summary>
    public partial class winCadCheckList : WindowBase
    {
        private DocumentGroup documentGroupN2 = new DocumentGroup();
        private LayoutGroup lg = new LayoutGroup();
        private Telas TelaSelecionada = new Telas();
        private List<Telas> TabsCheckList;
        private bool JaAbriu = false;
        private vmCheckListUDI _vmchecklist;
        private DocumentPanel panel;

        public winCadCheckList(int Ordem, vmCheckListUDI pvm)
        {
            InitializeComponent();
            Inicializa();
            this._vmchecklist = pvm;
            this.TabsCheckList = TabsCollection();
            this.Title = "CHECKLIST UDI ..:: Aviso: {" + this._vmchecklist.CheckListdto.AvisoCirurgia.cd_aviso_cirurgia.ToString() + "} ::..";
            this.TelaSelecionada = TabsCheckList.Single(x => x.Ordem == Ordem);
            this.CarregaUserControl(TelaSelecionada);
            this.JaAbriu = true;
        }

        private List<Telas> TabsCollection()
        {
            List<Telas> list = new List<Telas>();
            list.Add(new Telas()
            {
                Nome = "ucAntesDaEntrada",
                Url = @"UserControls\CheckListUDI\ucAntesDaEntrada.xaml",
                Caption = "Antes da Entrada do Paciente na Sala de Procedimento",
                vm = this._vmchecklist.vmAntesDaEntrada,
                Ordem = 1
            });           

            list.Add(new Telas()
            {
                Nome = "ucTimeOut",
                Caption = "TIME OUT - Antes do Início do Procedimento ",
                Url = @"UserControls\CheckListUDI\ucTimeOut.xaml",
                vm = this._vmchecklist.vmTimeOut,
                Ordem = 2
            });

            list.Add(new Telas()
            {
                Nome = "ucCheckOut",
                Caption = "CHECK OUT - Antes da Saída do Paciente",
                Url = @"UserControls\CheckListUDI\ucCheckOut.xaml",
                vm = this._vmchecklist.vmCheckOut,
                Ordem = 3
            });            

            return list;
        }

        private void CarregaUserControl(Telas pTelasSelecionada)
        {
            panel = new DocumentPanel();
            if (this.lg.Items.HasItems())
                this.lg.Items.RemoveAt(0);
            if (this.documentGroupN2.Items.HasItems())
                this.documentGroupN2.Items.RemoveAt(0);

            panel = this.dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN2, new Uri(pTelasSelecionada.Url, UriKind.Relative));
            panel.Name = pTelasSelecionada.Nome;
            panel.Caption = pTelasSelecionada.Caption;
            panel.FloatOnDoubleClick = false;
            panel.AllowFloat = false;
            panel.AllowDrag = false;
            panel.AllowHide = false;
            panel.AllowClose = false;
            panel.AllowMaximize = false;
            panel.AllowMinimize = false;
            (panel.Control as IUserControl).SetData(this._vmchecklist);
            this.lg.Add(documentGroupN2);
            this.dockLayoutManagerN1.LayoutRoot = lg;

            if (this.JaAbriu)
                this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
            this.Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
            this.InvalidateMeasure();
            this.UpdateLayout();

            this.dockLayoutManagerN1.DockController.Activate(panel, true);

            this.habilitabotoes(0);
        }

        private void Inicializa()
        {
            this.documentGroupN2.MDIStyle = MDIStyle.MDI;
            this.documentGroupN2.ShowCaptionImage = true;
            this.documentGroupN2.Expanded = true;
            this.documentGroupN2.FloatOnDoubleClick = false;
            this.documentGroupN2.AllowContextMenu = false; this.documentGroupN2.AllowClose = false;
            this.documentGroupN2.AllowDock = false; this.documentGroupN2.AllowDrag = false;
            this.documentGroupN2.AllowMove = false; this.documentGroupN2.ShowControlBox = false;
            this.documentGroupN2.AllowRename = false; this.documentGroupN2.ShowDropDownButton = false;
            this.documentGroupN2.ShowRestoreButton = false; this.documentGroupN2.AllowHide = false;
            this.documentGroupN2.ClosePageButtonShowMode = ClosePageButtonShowMode.NoWhere;
            this.documentGroupN2.AllowRestore = false; this.documentGroupN2.ShowCloseButton = false;
        }

        private void btnSair_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ms = DXMessageBox.Show("Deseja salvar este 'CheckList'?:", "Salvar:", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (ms.Equals(MessageBoxResult.Cancel))
                return;

            else if (ms == MessageBoxResult.Yes)
                this._vmchecklist.SalvarFechar(this.TelaSelecionada.vm);

            this.Close();
            this._vmchecklist.Refresh();
        }

        private void Proximo_Click(object sender, RoutedEventArgs e)
        {
            if ((this.TelaSelecionada.vm as ViewModelBase).IsValid)
            {
                this._vmchecklist.Salvar();
                if (this.btnProximo.ButtonText == "Salvar e Finalizar")
                {
                    this._vmchecklist.Salvar();
                    this.Close();
                    this._vmchecklist.Refresh();
                }
                else
                {
                    this.TelaSelecionada = this.TabsCheckList.Single(x => x.Ordem.Equals(TelaSelecionada.Ordem + 1));
                    this.CarregaUserControl(this.TelaSelecionada);                    
                }
            }
        }

        public void habilitabotoes(int Tab)
        {
            if (this.TelaSelecionada.Ordem.Equals(3))
            {
                this.btnProximo.ButtonText = "Salvar e Finalizar";
                this.btnProximo.ToolTip = this.btnProximo.ButtonText;
                this.btnProximo.ButtonImage = new BitmapImage(new Uri(@"/HMV.Core.Framework.WPF;component/Images/Save.png", UriKind.Relative));
            }
            else
            {
                this.btnProximo.ButtonText = "Salvar e Avançar";
                this.btnProximo.ToolTip = this.btnProximo.ButtonText;
                this.btnProximo.ButtonImage = new BitmapImage(new Uri(@"/HMV.Core.Framework.WPF;component/Images/go.png", UriKind.Relative));
            }
        }
    }

    public class Telas
    {
        public string Nome { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public int Ordem { get; set; }
        public ViewModelBase vm { get; set; }
    }
}
