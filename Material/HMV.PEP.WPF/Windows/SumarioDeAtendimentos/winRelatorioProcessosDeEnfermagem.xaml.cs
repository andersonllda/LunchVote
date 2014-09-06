using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.NavBar;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.DocumentosEletronicos;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.PEP.ViewModel.PEP.BoletimEmergencia;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.PEP.ViewModel.PEP.SumarioDeAtendimentos;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.BoletimEmergencia;
using HMV.PEP.WPF.Report.CentroObstetrico;
using HMV.PEP.WPF.Windows.BoletimDeEmergencia;
using DevExpress.XtraReports.UI;
using HMV.ProcessosEnfermagem.ViewModel;
using HMV.ProcessosEnfermagem.Relatorio.AdmissaoAssistencial;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.ProcessosEnfermagem.ViewModel.CentroObstetrico;
using HMV.ProcessosEnfermagem.WPF.Views.CentroObstetrico.AdmissaoAssistencial.Relatorio;
using HMV.Core.Wrappers.ObjectWrappers.PEP.AdmissaoAssistencialCTI;
using HMV.ProcessosEnfermagem.ViewModel.CTINEO.AdmissaoAssistencial;
using HMV.ProcessosEnfermagem.ViewModel.AvaliacaoDeRisco;
using HMV.ProcessosEnfermagem.WPF.Views.ProcessosEnfermagem.AvaliacaoRisco.Relatorio;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura;
using HMV.ProcessosEnfermagem.WPF.Views.ProcessosEnfermagem.PlanoEducacional;
using HMV.Core.Wrappers.ObjectWrappers.PEP.ProcessosEnfermagem.AdmissaoAssistencialDeEndoscopia;
using HMV.ProcessosEnfermagem.ViewModel.AdmissaoAssistencialDeEndoscopia;
using HMV.ProcessosEnfermagem.WPF.Views.ProcessosEnfermagem.AdmissaoAssistencialEndoscopia.Relatorios;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;
using HMV.PEP.WPF.UserControls.CheckListUDI;
using HMV.ProcessosEnfermagem.WPF.Views.ProcessosEnfermagem.AdmissaoAssistencialUrodinamica.Relatorios;
using HMV.ProcessosEnfermagem.ViewModel.AdmissaoAssistencialDeUrodinamica;

namespace HMV.PEP.WPF.Windows.SumarioDeAtendimentos
{
    /// <summary>
    /// Interaction logic for winRelatorioProcessosDeEnfermagem.xaml
    /// </summary>
    public partial class winRelatorioProcessosDeEnfermagem : WindowBase
    {
        XtraReport report = new XtraReport();
        private TreeViewItem _TreeViewItem = new TreeViewItem();
        public winRelatorioProcessosDeEnfermagem(wrpAtendimento pAtendimento, Usuarios pUsuarios)
        {
            InitializeComponent();

            this.DataContext = new vmRelatorioProcessosDeEnfermagem(pAtendimento);

            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).PodeAbrir)
                CriaMenu();            
        }

        private bool SetaRelatorio(object pItem)
        {
            while (gridPreview.Children.Count > 0)
                this.gridPreview.Children.RemoveAt(0);

            if (pItem.GetType() == typeof(wrpAdmissaoAssistencial))
            {
                vmAdmissaoAssistencialRelatorio vm = new vmAdmissaoAssistencialRelatorio(pItem as wrpAdmissaoAssistencial);
                rptAdmissaoAssitencial rpt = new rptAdmissaoAssitencial(vm, false);
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(wrpAdmissaoAssistencialCO))
            {
                rptAdmissaoAssistencial rpt = new rptAdmissaoAssistencial(new vmRelatorioAdmissaoCentroObstetrico(pItem as wrpAdmissaoAssistencialCO));
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(wrpAdmissaoAssistencialCTINEO))
            {
                HMV.ProcessosEnfermagem.WPF.Views.CTINEO.AdmissaoAssistencial.Relatorio.rptAdmissaoAssistencial rpt =
                    new HMV.ProcessosEnfermagem.WPF.Views.CTINEO.AdmissaoAssistencial.Relatorio.rptAdmissaoAssistencial(new vmRelatorioAdmissaoAssistencialCTINEO(pItem as wrpAdmissaoAssistencialCTINEO));
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(wrpAdmissaoAssistencialEndoscopia))
            {             
                rptAdmissaoAssistencialEndoscopia rpt = new rptAdmissaoAssistencialEndoscopia(new vmRelatorioAdmissaoEndoscopia(pItem as wrpAdmissaoAssistencialEndoscopia), false);               
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(wrpAdmissaoAssistencialUrodinamica))
            {                                
                rptAdmissaoAssistencialUrodinamica rpt = new rptAdmissaoAssistencialUrodinamica(new vmRelatorioAdmissaoUrodinamica(pItem as wrpAdmissaoAssistencialUrodinamica), false);
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;                
            }

            if (pItem.GetType() == typeof(wrpAvaliacaoRisco))
            {
                vmAvaliacaoRiscoRelatorio vmRel = new vmAvaliacaoRiscoRelatorio((this.DataContext as vmRelatorioProcessosDeEnfermagem).Atendimento.Paciente
                                                  , (this.DataContext as vmRelatorioProcessosDeEnfermagem).AvaliacoesDeRiscos);
                rptAvaliacaoRisco rpt = new rptAvaliacaoRisco(vmRel);
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(HMV.PEP.ViewModel.PEP.CheckListDeCirurgia.CheckListDTO))
            {
                var rpt = new RelatorioCheckList((pItem as HMV.PEP.ViewModel.PEP.CheckListDeCirurgia.CheckListDTO).CheckList).Relatorio();
                var uc = new ucReportBase(rpt, true, false, false);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(HMV.PEP.ViewModel.PEP.CheckListDeUDI.CheckListDTO))
            {
                var rpt = new vmRelatorioChecklistUDI((pItem as HMV.PEP.ViewModel.PEP.CheckListDeUDI.CheckListDTO).CheckList).Relatorio();
                var uc = new ucReportBase(rpt, true, false, false);
                this.gridPreview.Children.Add(uc);
                return true;
            }

            if (pItem.GetType() == typeof(wrpPerguntasPaciente))
            {
                vmPlanoEducacional vmRel = new vmPlanoEducacional((this.DataContext as vmRelatorioProcessosDeEnfermagem).Atendimento.DomainObject
                    , (this.DataContext as vmRelatorioProcessosDeEnfermagem).Atendimento.Paciente.DomainObject, App.Usuario);
                vmRel.Completo = true;
                vmRel.PerguntaPacienteSelecionada = (this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaPerguntasPaciente.FirstOrDefault();
                var rpt = new RelatorioPlanoEducacional(vmRel).Relatorio();
                var uc = new ucReportBase(rpt, true, false, true);
                this.gridPreview.Children.Add(uc);
                return true;
            }
            
            return false;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CriaMenu()
        {
            //Monta Menu das Admissões Assistenciais
            grpAdmissaoAssistecial.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencial.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencial)
                {
                    stackAdmissaoAssistencial.Children.Add(CriaSubMenu(item, item.DataInclusao.ToString("dd/MM/yyyy HH:mm")));
                }
                grpAdmissaoAssistecial.IsVisible = true;
            }

            //Monta Menu das Admissões Assistenciais CO
            grpAdmissaoAssistecialCO.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialCO.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialCO)
                {
                    stackAdmissaoAssistencialCO.Children.Add(CriaSubMenu(item, item.Data.ToString("dd/MM/yyyy HH:mm")));
                }
                grpAdmissaoAssistecialCO.IsVisible = true;
            }

            //Monta Menu das Admissões Assistenciais CTINEO
            grpAdmissaoAssistecialCTINEO.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialCTINEO.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialCTINEO)
                {
                    stackAdmissaoAssistencialCTINEO.Children.Add(CriaSubMenu(item, item.Data.ToString("dd/MM/yyyy HH:mm")));
                }
                grpAdmissaoAssistecialCTINEO.IsVisible = true;
            }

            //Monta Menu das Admissões Assistenciais Endoscopia
            grpAdmissaoAssistecialEndoscopia.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialEndoscopia.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialEndoscopia)
                {
                    stackAdmissaoAssistencialEndoscopia.Children.Add(CriaSubMenu(item, item.DataInclusao.ToString("dd/MM/yyyy HH:mm")));
                }
                grpAdmissaoAssistecialEndoscopia.IsVisible = true;
            }

            //Monta Menu das Admissões Assistenciais Urodinamica
            grpAdmissaoAssistecialUrodinamica.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialUrodinamica.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaDeAdmissaoAssistencialUrodinamica)
                {
                    stackAdmissaoAssistencialUrodinamica.Children.Add(CriaSubMenu(item, item.DataInclusao.ToString("dd/MM/yyyy HH:mm")));
                }
                grpAdmissaoAssistecialUrodinamica.IsVisible = true;
            }

            //Monta Menu das Avaliacoes de Risco
            grpAvaliacaoRisco.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).AvaliacoesDeRiscos.HasItems())
            {
                stackAvaliacaoRisco.Children.Add(CriaSubMenu((this.DataContext as vmRelatorioProcessosDeEnfermagem).AvaliacoesDeRiscos.FirstOrDefault(), "Resumo"));
                grpAvaliacaoRisco.IsVisible = true;
            }

            //Monta Menu dos CheckList
            grpCheckList.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).CheckListCollection.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).CheckListCollection)
                {
                    stackCheckList.Children.Add(CriaSubMenu(item, item.DataAviso.ToString("dd/MM/yyyy HH:mm")));
                }                

                grpCheckList.IsVisible = true;
            }

            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).CheckListUDICollection.HasItems())
            {
                foreach (var item in (this.DataContext as vmRelatorioProcessosDeEnfermagem).CheckListUDICollection)
                {
                    stackCheckList.Children.Add(CriaSubMenu(item, item.DataAviso.ToString("dd/MM/yyyy HH:mm")));
                }

                grpCheckList.IsVisible = true;
            }

            //Monta Menu do Plano Educacional
            grpPlanoEducacional.IsVisible = false;
            if ((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaPerguntasPaciente.HasItems())
            {
                stackPlanoEducacional.Children.Add(CriaSubMenu((this.DataContext as vmRelatorioProcessosDeEnfermagem).ListaPerguntasPaciente.FirstOrDefault(), "Completo"));
                grpPlanoEducacional.IsVisible = true;
            }
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

        void treeview_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _TreeViewItem.IsSelected = false;
            if (SetaRelatorio((sender as TreeViewItem).Tag))
                (sender as TreeViewItem).IsSelected = true;
            _TreeViewItem = (sender as TreeViewItem);
        }

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            report.Imprime();
        }

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext as vmRelatorioProcessosDeEnfermagem).PodeAbrir)            
            {
                DXMessageBox.Show("Não existem documentos para este atendimento!", "Sumário de Atendimentos", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        //private static ImageSource Imagem(bool MV)
        //{
        //    HMV.Core.Framework.WPF.Converters.BoolToImageMVMonitorMonitorConverter con = new Core.Framework.WPF.Converters.BoolToImageMVMonitorMonitorConverter();
        //    ImageSource myImage;
        //    BitmapImage myBitmapImage = new BitmapImage();
        //    myBitmapImage.BeginInit();

        //    if (MV)
        //        myBitmapImage.UriSource = new Uri(con.Convert(true, null, null, null).ToString());
        //    else
        //        myBitmapImage.UriSource = new Uri(con.Convert(false, null, null, null).ToString());

        //    myBitmapImage.DecodePixelWidth = 200;
        //    myBitmapImage.EndInit();
        //    myImage = myBitmapImage;
        //    return myImage;
        //}
    }
}
