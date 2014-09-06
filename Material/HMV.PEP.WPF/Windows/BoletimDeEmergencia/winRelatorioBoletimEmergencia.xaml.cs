using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Printing;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.DocumentosEletronicos;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.PEP.WPF.Report.BoletimEmergencia;
using HMV.PEP.ViewModel.PEP.BoletimEmergencia;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Report.CentroObstetrico;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.PEP.WPF.Report;

namespace HMV.PEP.WPF.Windows.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for RelatorioBoletimEmergencia.xaml
    /// </summary>
    public partial class winRelatorioBoletimEmergencia : WindowBase
    {
        DevExpress.XtraReports.UI.XtraReport report = new DevExpress.XtraReports.UI.XtraReport();
        Visibility mostraFrase = Visibility.Collapsed;

        public winRelatorioBoletimEmergencia(wrpAtendimento pAtendimento, Usuarios pUsuarios)
        {
            InitializeComponent();
            
            this._atendimento = pAtendimento.DomainObject;

            lblAviso.Visibility = mostraFrase;

            btnBoletim.Visibility = BotaoBoletim;
            this.DataContext = new vmBoletimEmergencia(pAtendimento.DomainObject, pUsuarios, App.IsPAME);
            CriaMenu();
            lblAviso.Visibility = (!mostraFrase.Equals(Visibility.Visible) && (BotaoBoletim.Equals(Visibility.Visible))) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Relatorio(wrpBoletimDeEmergencia pBoletimDeEmergencia)
        {
            if (pBoletimDeEmergencia.BoletimCO.Equals(SimNao.Sim))
            {

                vmRelatorioBoletimEmergenciaCO vm = new vmRelatorioBoletimEmergenciaCO(pBoletimDeEmergencia);
                rptBoletimEmergenciaCO rpt = new rptBoletimEmergenciaCO(vm);

                var uc = new ucReportBase(rpt, true, false, true);
                while (gBole.Children.Count > 0)
                    this.gBole.Children.RemoveAt(0);
                this.gBole.Children.Add(uc);

                //XtraReportPreviewModel model = new XtraReportPreviewModel(rpt);
                //rRelatorioBoletimEmergencia.Model = model;
                ////rpt.ClosePreview();
                //rpt.CreateDocument(false);
            }
            else
            {
                rtpBoletimEmergencia rtp = new rtpBoletimEmergencia();
                report = rtp;
                rtp.DataSource = new vmRelatorioBoletimEmergencia(pBoletimDeEmergencia, App.IsPAME).Relatorio();
                //XtraReportPreviewModel model = new XtraReportPreviewModel(rtp);

                var uc = new ucReportBase(rtp, true, false, true);
                while (gBole.Children.Count > 0)
                    this.gBole.Children.RemoveAt(0);
                this.gBole.Children.Add(uc);

                #region CheckBoxes do Relatorio
                if (!pBoletimDeEmergencia.IsNull())
                {
                    if ((!pBoletimDeEmergencia.Ventilacao.IsNull()) && (pBoletimDeEmergencia.Ventilacao.Value == SimNao.Sim))
                    {
                        rtp.chkVentilacao.CheckState = System.Windows.Forms.CheckState.Checked;
                    }
                    if ((!pBoletimDeEmergencia.Oxigenio.IsNull()) && (pBoletimDeEmergencia.Oxigenio.Value == SimNao.Sim))
                    {
                        rtp.chkOxigenio.CheckState = System.Windows.Forms.CheckState.Checked;
                    }
                    if ((!pBoletimDeEmergencia.AcompMedico.IsNull()) && (pBoletimDeEmergencia.AcompMedico.Value == SimNao.Sim))
                    {
                        rtp.chkAcompanhamento.CheckState = System.Windows.Forms.CheckState.Checked;
                    }
                    if ((!pBoletimDeEmergencia.Monitorizacao.IsNull()) && (pBoletimDeEmergencia.Monitorizacao.Value == SimNao.Sim))
                    {
                        rtp.chkMonitorizacao.CheckState = System.Windows.Forms.CheckState.Checked;
                    }
                }

                #endregion

                //rRelatorioBoletimEmergencia.Model = model;
                //rtp.CreateDocument(false);
            }
        }

        private void RelatorioMV(wrpDocumentos pDocumentos)
        {
            rptBoletimEmergenciaMV rtp = new rptBoletimEmergenciaMV();
            report = rtp;
            rtp.DataSource = new vmRelatorioBoletimEmergencia().RelatorioMV(pDocumentos);

            var uc = new ucReportBase(rtp, true, false, false);
            while (gBole.Children.Count > 0)
                this.gBole.Children.RemoveAt(0);
            this.gBole.Children.Add(uc);

            //XtraReportPreviewModel model = new XtraReportPreviewModel(rtp);
            //rtp.lblTitulo.Text = pDocumentos.TiposDocumentos.Descricao.ToTitle();
            //rRelatorioBoletimEmergencia.Model = model;
            //rtp.CreateDocument(false);
        }        

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CriaMenu()
        {
            grpAtestado.IsVisible = false;
            bool exibeprimeirorelatorio = true;
            foreach (var item in (this.DataContext as vmBoletimEmergencia).Documentos.Select(x => x.DomainObject.TiposDocumentos).Distinct().OrderBy(o => o.Descricao).ToList())
            {
                NavBarGroup group2 = new NavBarGroup();
                group2.Header = item.Descricao;
                group2.ImageSource = Imagem(true);
                group2.DisplaySource = DisplaySource.Content;
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                group2.Content = stackPanel;

                foreach (var itemd in (this.DataContext as vmBoletimEmergencia).Documentos.Where(t => t.DomainObject.TiposDocumentos.Id == item.Id).Select(x => x.DomainObject).OrderBy(o => o.TiposDocumentos.Id).ToList())
                {
                    stackPanel.Children.Add(CriaSubMenu(itemd, itemd.DataDocumento.ToString("dd/MM/yyyy HH:mm")));
                    if (exibeprimeirorelatorio) { RelatorioMV(new wrpDocumentos(itemd)); exibeprimeirorelatorio = false; }
                    mostraFrase = Visibility.Visible;
                }
                grpAtestado.NavBar.Groups.Add(group2);
            }

            if ((this.DataContext as vmBoletimEmergencia).BoletinsDeEmergencias.Where(x => x.DataAlta != null).Count() > 0)
            {
                NavBarGroup group2 = new NavBarGroup();
                group2.Header = "BOLETIM EMERGÊNCIA";
                group2.ImageSource = Imagem(false);
                group2.DisplaySource = DisplaySource.Content;
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
                group2.Content = stackPanel;
                //foreach (var itemboletim in (this.DataContext as vmBoletimEmergencia).BoletinsDeEmergencias.Where(x => x.DataAlta != null))
                foreach (var itemboletim in (this.DataContext as vmBoletimEmergencia).BoletinsDeEmergencias.Where(x => x.DataAlta != null).OrderByDescending(x => x.DataAlta))
                {
                    stackPanel.Children.Add(CriaSubMenu(itemboletim, itemboletim.DataInclusao.Value.ToString("dd/MM/yyyy HH:mm")));
                    if (exibeprimeirorelatorio) 
                    {
                        Relatorio(new wrpBoletimDeEmergencia(itemboletim.DomainObject));
                        exibeprimeirorelatorio = false;
                        mostraFrase = Visibility.Visible;
                    }
                }
                grpAtestado.NavBar.Groups.Add(group2);
            }
            //if ((this.DataContext as vmBoletimEmergencia).BoletinsDeEmergencias.Where(x => x.DataAlta == null).Count() > 0)
            //    btnEmergencia.IsEnabled = true;
        }

        TreeViewItem treeviewitem = new TreeViewItem();
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

        private static ImageSource Imagem(bool MV)
        {
            HMV.Core.Framework.WPF.Converters.BoolToImageMVMonitorMonitorConverter con = new Core.Framework.WPF.Converters.BoolToImageMVMonitorMonitorConverter();
            ImageSource myImage;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();

            if (MV)
                myBitmapImage.UriSource = new Uri(con.Convert(true, null, null, null).ToString());
            else
                myBitmapImage.UriSource = new Uri(con.Convert(false, null, null, null).ToString());
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            myImage = myBitmapImage;
            return myImage;
        }
        private TreeViewItem _TreeViewItem = new TreeViewItem();

        void treeview_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _TreeViewItem.IsSelected = false;
            if ((sender as TreeViewItem).Tag.GetType() == typeof(wrpBoletimDeEmergencia))
            {
                Relatorio((wrpBoletimDeEmergencia)(sender as TreeViewItem).Tag);
                (sender as TreeViewItem).IsSelected = true;
            }
            else
            {
                RelatorioMV(new wrpDocumentos((Documentos)(sender as TreeViewItem).Tag));
                (sender as TreeViewItem).IsSelected = true;
            }
            _TreeViewItem = (sender as TreeViewItem);
        }
        private Documentos _Documentos { get; set; }

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            report.Imprime();
        }
        
        private Atendimento _atendimento { get; set; }

        public Visibility BotaoBoletim
        {
            get
            {
                return (this._atendimento != null && this._atendimento.BoletinsDeEmergencia != null && this._atendimento.BoletinsDeEmergencia.Count(x => x.DataAlta.IsNull()) > 0).Equals(true) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void btnBoletim_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmBoletimEmergencia).BoletimEmergencia != null &&
                (this.DataContext as vmBoletimEmergencia).BoletimEmergencia.BoletimCO.Equals(SimNao.Sim))
            {
                winBoletimEmergenciaCO win = new winBoletimEmergenciaCO(this._atendimento);
                win.ShowDialog(base.OwnerBase);
                btnBoletim.Visibility = BotaoBoletim;
                CriaMenu();
                lblAviso.Visibility = (mostraFrase.Equals(Visibility.Visible) && (BotaoBoletim.Equals(Visibility.Visible))) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                winBoletimEmergencia win = new winBoletimEmergencia(this._atendimento);
                win.ShowDialog(base.OwnerBase);
                btnBoletim.Visibility = BotaoBoletim;
                CriaMenu();
                lblAviso.Visibility = (mostraFrase.Equals(Visibility.Visible) && (BotaoBoletim.Equals(Visibility.Visible))) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
