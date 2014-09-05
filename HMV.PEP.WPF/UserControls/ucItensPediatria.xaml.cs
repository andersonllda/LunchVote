using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using HMV.PEP.ViewModel.UrgenciaP;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.Pim2;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucItensPediatria.xaml
    /// </summary>
    public partial class ucItensPediatria : UserControlBase, IUserControl
    {
        XtraReport report = new XtraReport();
        TreeViewItem treeviewitem = new TreeViewItem();
        private TreeViewItem _TreeViewItem = new TreeViewItem();

        public ucItensPediatria()
        {
            InitializeComponent();
        }

        public bool CancelClose
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        
        public void SetData(object pData)
        {
            if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {
                this.DataContext = new vmItensPediatria(pData as Paciente);
            }
            else if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                this.DataContext = new vmItensPediatria((pData as Atendimento).Paciente);
            }

            CriaMenu();
        }
        
        private void CriaMenu()
        {
            grpPediatria.IsVisible = false;
            bool exibeprimeirorelatorio = true;
            if ((this.DataContext as vmItensPediatria).ExisteMotivoOuPIM2)
            {
                NavBarGroup grpMotivoPim = new NavBarGroup();
                grpMotivoPim.Header = "Motivo / PIM2";
                grpMotivoPim.ImageSource = Imagem(true);
                grpMotivoPim.DisplaySource = DisplaySource.Content;
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                grpMotivoPim.Content = stackPanel;
                foreach (var item in (this.DataContext as vmItensPediatria).AtendimentoCollection)
                {
                    stackPanel.Children.Add(CriaSubMenu(item, item.HoraAtendimento.ToString("dd/MM/yyyy HH:mm")));
                    if (exibeprimeirorelatorio)
                    {
                        RelatorioMotivoPim(item.DomainObject);
                        exibeprimeirorelatorio = false;
                    }
                }
                grpPediatria.NavBar.Groups.Add(grpMotivoPim);
            }

            if ((this.DataContext as vmItensPediatria).ExisteUrgenciasPediatricas)
            {
                NavBarGroup grpUrgenciaPed = new NavBarGroup();
                grpUrgenciaPed.Header = "Urgência Pediátrica";
                grpUrgenciaPed.ImageSource = Imagem(true);
                grpUrgenciaPed.DisplaySource = DisplaySource.Content;
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                grpUrgenciaPed.Content = stackPanel;
                foreach (var item in (this.DataContext as vmItensPediatria).UrgenciaPediatricaAtendimentoCollection)
                {
                    stackPanel.Children.Add(CriaSubMenu(item, item.HoraInclusao.ToString("dd/MM/yyyy HH:mm")));
                    if (exibeprimeirorelatorio)
                    {
                        RelatorioUrgenciaPediatrica(item);
                        exibeprimeirorelatorio = false;
                    }
                }
                grpPediatria.NavBar.Groups.Add(grpUrgenciaPed);
            }
        }

        private static ImageSource Imagem(bool Sim)
        {
            HMV.Core.Framework.WPF.Converters.BoolToImageMVMonitorMonitorConverter con = new Core.Framework.WPF.Converters.BoolToImageMVMonitorMonitorConverter();
            ImageSource myImage;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();

            if (Sim)
                myBitmapImage.UriSource = new Uri("pack://application:,,,/HMV.Core.Framework.WPF;component/Images/Crianca.png");
            else
                myBitmapImage.UriSource = new Uri(con.Convert(false, null, null, null).ToString());
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            myImage = myBitmapImage;
            return myImage;
        }

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            report.Imprime();
        }

        private void RelatorioMotivoPim(Atendimento pAtendimento)
        {
            rptPim2 rpt = new rptPim2();
            rpt.sCabecalho.ReportSource.DataSource = new vmMotivoInternacaoPim2(pAtendimento, App.Usuario, true).vmPin2.RelCabecalho;
            rpt.sListaValores.ReportSource.DataSource = new vmMotivoInternacaoPim2(pAtendimento, App.Usuario, true).vmPin2.RelListaValores;
            rpt.sMotivoInternacao.ReportSource.DataSource = new vmMotivoInternacaoPim2(pAtendimento, App.Usuario, true).vmMotivoDeInternacao.RelMotivoInternacao;

            report = rpt;
            XtraReportPreviewModel model = new XtraReportPreviewModel(report);
            relPediatrico.Model = model;
            relPediatrico.Model.Zoom = 90;
            report.CreateDocument(false);
        }

        private void RelatorioUrgenciaPediatrica(wrpUrgenciaPediatricaAtendimento wrpUP)
        {
            vmUrgenciaPediatrica vmUP = new vmUrgenciaPediatrica(wrpUP.DomainObject.Atendimento);
            vmUP.UrgenciaPediatricaAtendimentoSelecionado = wrpUP;

            rptUrgenciaPediatrica rpt = new rptUrgenciaPediatrica();
            rpt.srUrgenciaItens.ReportSource.DataSource = vmUP.GetUrgenciaItensRel();
            rpt.lbPacienteNome.Text = "[" + vmUP.Atendimento.Paciente.ID + "]" + vmUP.Atendimento.Paciente.Nome;
            rpt.lbPacienteDataNasc.Text = vmUP.Atendimento.Paciente.DataNascimento.HasValue ? vmUP.Atendimento.Paciente.DataNascimento.Value.ToShortDateString() : string.Empty;
            rpt.lbPacienteIdade.Text = vmUP.UrgenciaPediatricaAtendimentoSelecionado.IdadeInclusao;
            rpt.lbPacienteDataFicha.Text = vmUP.UrgenciaPediatricaAtendimentoSelecionado.DataInclusao.ToShortDateString();
            rpt.lbPacientePeso.Text = vmUP.UrgenciaPediatricaAtendimentoSelecionado.Peso.ToString() + " kg";
            rpt.lbPacienteSC.Text = vmUP.UrgenciaPediatricaAtendimentoSelecionado.SC.ToString() + " m²";
            rpt.lblTuboTraqueal.Text = vmUP.GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.Tubotraqueal);
            rpt.lblLaminaLaringoscopio.Text = vmUP.GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.LaminaLaringo);
            rpt.lblInsercao.Text = vmUP.GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.Insercao);
            rpt.lblAMBU.Text = vmUP.GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.AMBU);
            rpt.lblSondaSucccao.Text = vmUP.GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.SondaSuccao);
            rpt.lblFluxoO2.Text = vmUP.GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.FluxoO2);
            
            report = rpt;
            XtraReportPreviewModel model = new XtraReportPreviewModel(report);
            relPediatrico.Model = model;
            relPediatrico.Model.Zoom = 90;
            report.CreateDocument(false);
        }

        private TreeViewItem CriaSubMenu(object pNome, string pHeader)
        {
            treeviewitem = new TreeViewItem();
            treeviewitem.Tag = pNome;
            treeviewitem.Header = pHeader;            
            treeviewitem.MouseLeftButtonUp += new MouseButtonEventHandler(treeview_PreviewMouseLeftButtonUp);
            if (_TreeViewItem.IsSelected.Equals(false))
            {
                _TreeViewItem = treeviewitem;
                _TreeViewItem.IsSelected = true;
            }
            return treeviewitem;
        }

        void treeview_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _TreeViewItem.IsSelected = false;
            if ((sender as TreeViewItem).Tag.GetType() == typeof(wrpAtendimento))
            {
                RelatorioMotivoPim(((wrpAtendimento)(sender as TreeViewItem).Tag).DomainObject);
                (sender as TreeViewItem).IsSelected = true;    
            }
            else if ((sender as TreeViewItem).Tag.GetType() == typeof(wrpUrgenciaPediatricaAtendimento))
            {
                RelatorioUrgenciaPediatrica(((wrpUrgenciaPediatricaAtendimento)(sender as TreeViewItem).Tag));
                (sender as TreeViewItem).IsSelected = true;
            }
            _TreeViewItem = (sender as TreeViewItem);
        }
    }
}
