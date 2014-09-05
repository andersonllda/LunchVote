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
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Base;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoM;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for winNotasAdicionais.xaml
    /// </summary>
    public partial class winNotasAdicionais : WindowBase
    {
        public winNotasAdicionais(SumarioAvaliacaoMedica pSumarioAvaliacaoMedica)
        {
            InitializeComponent();
            this.DataContext = new vmNotasAdicionais(pSumarioAvaliacaoMedica);
            (this.DataContext as vmNotasAdicionais).ActionCommandFechar += this.Close;
            this.CriarNiveis();
        }

        private DocumentPanel panelN2;

        private class TabDinamica
        {
            public int ID { get; set; }
            public string Descricao { get; set; }
            public string Componente { get; set; }
            public string NotaAdicional { get; set; }
            public List<TabDinamica> Filhos { get; set; }
        }

        private void CriarNiveis()
        {
            SumarioAvaliacaoMedicaTipo sumariotipo = (this.DataContext as vmNotasAdicionais).SumarioAvaliacaoMedica.DomainObject.Tipo;

            if (sumariotipo.NivelI == null)
                return;

            List<SumarioAvaliacaoMedicaNivelI> NiveisI = sumariotipo.NivelI.ToList();

            List<TabDinamica> lstNivelI = new List<TabDinamica>();
            DocumentPanel panelN1;

            foreach (var NivelI in NiveisI.OrderBy(x => x.Ordem))
            {
                List<TabDinamica> lstNivelII = new List<TabDinamica>();

                foreach (var NivelII in NivelI.NivelII.ToList().OrderBy(x => x.Ordem))
                {
                    lstNivelII.Add(new TabDinamica { ID = NivelII.ID, Componente = NivelII.Componente, Descricao = NivelII.Descricao, NotaAdicional = NivelII.NotaAdicional });
                }
                lstNivelI.Add(new TabDinamica { ID = NivelI.ID, Descricao = NivelI.Descricao, Filhos = lstNivelII });
            }

            foreach (var NivelI in lstNivelI)
            {
                DockLayoutManager dockLayoutManagerN2 = new DockLayoutManager();
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

                if (NivelI.Filhos.Count(x => !x.NotaAdicional.IsEmpty()) == 0)
                    continue;

                foreach (var NivelII in NivelI.Filhos)
                {
                    try
                    {
                        if (NivelII.NotaAdicional.IsEmpty())
                            continue;
                        panelN2 = dockLayoutManagerN2.DockController.AddDocumentPanel(documentGroupN2, new Uri(@"UserControls\SumarioAvaliacaoM\ucNotasAdicionais.xaml", UriKind.Relative));
                        panelN2.Caption = NivelII.Descricao;
                        panelN2.FloatOnDoubleClick = false;
                        panelN2.AllowFloat = false;
                        panelN2.AllowDrag = false;
                        panelN2.AllowHide = false;
                        panelN2.AllowClose = false;                        
                        (panelN2.Control as IUserControl).SetData(new object[] { this.DataContext, NivelII.NotaAdicional });
                    }
                    catch (Exception e)
                    {
                        DXMessageBox.Show(e.Message, "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                lg.Add(documentGroupN2);
                dockLayoutManagerN2.LayoutRoot = lg;

                panelN1 = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1);
                panelN1.Caption = NivelI.Descricao;
                panelN1.FloatOnDoubleClick = false;
                panelN1.AllowFloat = false;
                panelN1.AllowDrag = false;
                panelN1.AllowHide = false;
                panelN1.AllowClose = false;
                panelN1.Content = dockLayoutManagerN2;
            }
        }
    }
}
