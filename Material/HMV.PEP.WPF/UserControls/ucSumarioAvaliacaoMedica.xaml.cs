using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Interfaces;
using HMV.PEP.WPF.Cadastros.SumarioAvaliacaoM;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoM;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoM;
using HMV.ProcessosEnfermagem.ViewModel;
using NHibernate.Validator.Engine;
using StructureMap;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.PEP.Consult;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucSumarioAvaliacaoMedica.xaml
    /// </summary>
    public partial class ucSumarioAvaliacaoMedica : UserControlBase, IUserControl
    {
        private DocumentPanel panelN2;
        private ucResumoAvaliacaoMedica ucRAM = new ucResumoAvaliacaoMedica();
        public event EventHandler ExecuteMethod;
        private bool _tabatualdiagnosticohipotese = false;

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty);
        }

        public bool CancelClose { get; set; }

        public ucSumarioAvaliacaoMedica()
        {
            InitializeComponent();
        }

        private class TabDinamica
        {
            public int ID { get; set; }
            public string Descricao { get; set; }
            public string Componente { get; set; }
            public List<TabDinamica> Filhos { get; set; }
        }

        private vmAlergiasEvento _vmalergiasevento;
        private vmMedicamentosEmUsoEvento _vmmedicamentosemusoevento;
        private void CriarNiveis()
        {
            SumarioAvaliacaoMedicaTipo sumariotipo = (this.DataContext as Atendimento).SumarioAvaliacaoMedica.Tipo;
            if ((this.DataContext as Atendimento).Paciente.TipoDoPaciente == Core.Domain.Enum.TipoPaciente.Pediatrico)
                this.txtTitulo.Text += " Pediátrico";

            if (sumariotipo.NivelI == null)
                return;

            List<SumarioAvaliacaoMedicaNivelI> NiveisI = sumariotipo.NivelI.Where(x=>x.Status==Core.Domain.Enum.Status.Ativo).ToList();

            List<TabDinamica> lstNivelI = new List<TabDinamica>();
            DocumentPanel panelN1;

            foreach (var NivelI in NiveisI.Where(x=>x.Status == Core.Domain.Enum.Status.Ativo).OrderBy(x => x.Ordem))
            {
                List<TabDinamica> lstNivelII = new List<TabDinamica>();

                foreach (var NivelII in NivelI.NivelII.Where(x=>x.Status==Core.Domain.Enum.Status.Ativo).ToList().OrderBy(x => x.Ordem))
                {
                    lstNivelII.Add(new TabDinamica { ID = NivelII.ID, Componente = NivelII.Componente, Descricao = NivelII.Descricao });
                }
                lstNivelI.Add(new TabDinamica { ID = NivelI.ID, Descricao = NivelI.Descricao, Filhos = lstNivelII });
            }

            AdmissaoAssistencial adm = (this.DataContext as Atendimento).AdmissaoAssistencial.FirstOrDefault(a => a.DataConclusao.IsNotNull());

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

                foreach (var NivelII in NivelI.Filhos)
                {
                    try
                    {
                        panelN2 = dockLayoutManagerN2.DockController.AddDocumentPanel(documentGroupN2, new Uri(@NivelII.Componente, UriKind.Relative));
                        panelN2.Caption = NivelII.Descricao;
                        panelN2.FloatOnDoubleClick = false;
                        panelN2.AllowFloat = false;
                        panelN2.AllowDrag = false;
                        panelN2.AllowHide = false;
                        panelN2.AllowClose = false;
                        //panelN1 = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, new Uri("UserControls\\SumarioAvaliacaoM\\ucResumoAvaliacaoMedica.xaml", UriKind.Relative));
                        if (NivelII.Descricao == "Alergias")
                        {
                            if (this._vmalergiasevento.IsNull())
                            {
                                this._vmalergiasevento = new vmAlergiasEvento(false, new wrpPaciente((this.DataContext as Atendimento).Paciente),
                                    new wrpUsuarios(App.Usuario), new GetSettings().IsCorpoClinico, Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica, this._AlergiaCollection, (this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID,
                                    new wrpAtendimento((this.DataContext as Atendimento)));
                                if (this._novo)
                                    this._vmalergiasevento.AlergiaCollection.Each(x => { x.Selecionado = true; });
                            }
                            (panelN2.Control as IUserControl).SetData(this._vmalergiasevento);
                        }
                        // atv 3965 else if (NivelII.Descricao == "Medicamentos em Uso")
                        else if (NivelII.Descricao == "Medicamentos Habituais")
                        {
                            if (this._vmmedicamentosemusoevento.IsNull())
                            {
                                this._vmmedicamentosemusoevento = new vmMedicamentosEmUsoEvento(false, new wrpPaciente((this.DataContext as Atendimento).Paciente),
                                    new wrpUsuarios(App.Usuario), Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica, _MedicamentosCollection, (this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID, pMostraUltAdministracao: true,
                                    pMostraDataInicio: false, pMostraVia: true, pMostraFrequencia: true, pMostraComboVia: false, pMostraComboFrequencia: false, pAtendimento: new wrpAtendimento((this.DataContext as Atendimento)));
                                if (this._novo)
                                    this._vmmedicamentosemusoevento.MarcarTodosDoAtendimentoAdmissao((this.DataContext as Atendimento));
                            }
                            (panelN2.Control as IUserControl).SetData(this._vmmedicamentosemusoevento);
                        }
                        else if (!NivelII.Componente.Contains("ucResumoAvaliacaoMedica"))
                            (panelN2.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
                    }
                    catch (Exception e)
                    {
                        DXMessageBox.Show(e.Message, "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                documentGroupN2.Caption = NivelI.Descricao;
                documentGroupN2.SelectedItemChanged += new SelectedItemChangedEventHandler(documentGroup_SelectedItemChanged);
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

            documentGroupN1.SelectedItemChanged += new SelectedItemChangedEventHandler(documentGroup_SelectedItemChanged);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.OnExecuteMethod();
        }

        DocumentPanel document;
        DocumentPanel documentfilho;
        bool _salvaalergia = false;
        bool _salvamedicamento = false;
        void documentGroup_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            bool rel = false;
            this.btnImprimir.Visibility = Visibility.Collapsed;
            if (e.Item != null)
                if (e.Item.GetType() == typeof(DocumentPanel))
                {
                    document = e.Item as DocumentPanel;
                    if (document != null)
                        if (document.Control != null)
                            if (document.Control.GetType() == typeof(DockLayoutManager))
                            {
                                if ((document.Control as DockLayoutManager).LayoutRoot.HasSingleItem)
                                {
                                    DocumentGroup documentGroupFilho = (DocumentGroup)(document.Control as DockLayoutManager).LayoutRoot.Items.FirstOrDefault();
                                    documentfilho = (DocumentPanel)documentGroupFilho.SelectedItem;
                                    if (documentfilho.Control.GetType() == typeof(ucResumoAvaliacaoMedica))
                                    {
                                        this.btnImprimir.Visibility = System.Windows.Visibility.Visible;
                                        if ((this.DataContext as Atendimento).SumarioAvaliacaoMedica != null)
                                            rel = true;
                                    }
                                }
                            }
                            else
                                documentfilho = null;
                }




            this.Save();

            if (document.IsNotNull() && document.Control.IsNotNull() && document.Control.GetType() == typeof(ucDiagnosticoHipoteses))
            {
                _tabatualdiagnosticohipotese = true;
            }



            if (rel)
                (panelN2.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);

            if (documentfilho.IsNotNull())
                if (documentfilho.Control.GetType() == typeof(ucAlergiasAvaliacaoMedica))
                {
                    _salvaalergia = true;
                    _salvamedicamento = false;
                }
                else if (documentfilho.Control.GetType() == typeof(ucMedicamentosEmUsoAvaliacaoMedica))
                {
                    _salvamedicamento = true;
                    _salvaalergia = false;
                }
                else if (documentfilho.Control.GetType() == typeof(ucDiagnosticoHipoteses))
                {
                    _salvaalergia = false;
                    _salvamedicamento = false;
                    _tabatualdiagnosticohipotese = true;
                }
                else
                {
                    _salvaalergia = false;
                    _salvamedicamento = false;
                }
            else
                if (document.IsNotNull() && document.Control.IsNotNull() && document.Control.GetType() == typeof(ucAlergiasAvaliacaoMedica))
                {
                    _salvaalergia = true;
                    _salvamedicamento = false;
                }
                else if (document.IsNotNull() && document.Control.IsNotNull() && document.Control.GetType() == typeof(ucMedicamentosEmUsoAvaliacaoMedica))
                {
                    _salvaalergia = false;
                    _salvamedicamento = true;
                }
                else
                {
                    _salvaalergia = false;
                    _salvamedicamento = false;
                }
        }

        public void Save()
        {
            try
            {
                if (_tabatualdiagnosticohipotese)
                {
                    ICidService srvCID = ObjectFactory.GetInstance<ICidService>();
                    srvCID.verificaSeOCIDJaEstaNaListaDeProblemas((this.DataContext as Atendimento).SumarioAvaliacaoMedica.Atendimento.Cid, (this.DataContext as Atendimento), App.Usuario);
                    _tabatualdiagnosticohipotese = false;
                }

                ISumarioAvaliacaoMedicaService srv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
                srv.Save((this.DataContext as Atendimento).SumarioAvaliacaoMedica);

                if (_salvaalergia || _salvamedicamento)
                {
                    if (_salvaalergia)
                    {
                        //Salva AlergiasEvento
                        if (this._vmalergiasevento.IsNotNull())
                            this._vmalergiasevento.SalvarNovo();
                    }
                    if (_salvamedicamento)
                    {
                        //Salva MedicamentosEmUsoEvento        
                        if (this._vmmedicamentosemusoevento.IsNotNull())
                            this._vmmedicamentosemusoevento.SalvarNovo();
                    }
                }
            }
            catch (BusinessValidatorException ex)
            {
                this.CancelClose = true;
                string ret = string.Empty;
                ex.GetErros().ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection;
        wrpAlergiaEventoCollection _AlergiaCollection;
        DocumentPanel panelN1;
        private bool _novo;

        public void SetData(object pData)
        {
            this.btnImportar.Visibility = Visibility.Visible;
            this.btnEditarSumario.Visibility = Visibility.Collapsed;

            (pData as Atendimento).AddSumarioAvaliacaoMedica(App.Usuario, Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"]));
            this.DataContext = (pData as Atendimento);

            if ((this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID == 0)
            {
                this._novo = true;
                ISumarioAvaliacaoMedicaService srv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
                srv.Save((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
            }

            IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
            repp.OndeChaveIgual((this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID);
            repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica);
            var ret = repp.List();
            if (ret.IsNotNull())
                _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);

            IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
            repa.OndeChaveIgual((this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID);
            repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica);
            var reta = repa.List();
            if (reta.IsNotNull())
                _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

            Parametro _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).OndePodeEditarPEP().Single();

            if ((!App.Usuario.Prestador.IsNull()) && (!App.Usuario.Prestador.Conselho.IsNull()) && (_param.Valor.Contains(App.Usuario.Prestador.Conselho.ds_conselho.ToUpper())))
            {
                if ((pData as Atendimento).SumarioAvaliacaoMedica.PodeEditarSumario(App.Usuario))
                {
                    CriarNiveis();
                    btnImprimir.IsEnabled = true;
                }
                else
                {
                    panelN1 = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, new Uri("UserControls\\SumarioAvaliacaoM\\ucResumoAvaliacaoMedica.xaml", UriKind.Relative));
                    panelN1.Caption = "Concluir";
                    panelN1.Name = "panelVisualizar";
                    btnImprimir.IsEnabled = false;
                    panelN1.FloatOnDoubleClick = false;
                    panelN1.AllowFloat = false;
                    panelN1.AllowDrag = false;
                    panelN1.AllowHide = false;
                    panelN1.AllowClose = false;
                    (panelN1.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
                    this.btnImprimir.Visibility = Visibility.Visible;
                    this.btnImportar.Visibility = Visibility.Collapsed;
                    if ((pData as Atendimento).SumarioAvaliacaoMedica.DataEncerramento.HasValue)
                    {
                        this.btnNotaAdicional.Visibility = Visibility.Visible;
                        this.btnImprimirSomente.Visibility = Visibility.Visible;
                    }

                    if ((pData as Atendimento).SumarioAvaliacaoMedica.DataEncerramento == null && !(pData as Atendimento).SumarioAvaliacaoMedica.PodeEditarSumario(App.Usuario))
                        this.btnEditarSumario.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                panelN1 = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, new Uri("UserControls\\SumarioAvaliacaoM\\ucResumoAvaliacaoMedica.xaml", UriKind.Relative));
                //panelN1.Caption = "Visualizar";
                panelN1.Name = "panelVisualizar";
                btnImprimir.IsEnabled = false;
                panelN1.FloatOnDoubleClick = false;
                panelN1.AllowFloat = false;
                panelN1.AllowDrag = false;
                panelN1.AllowHide = false;
                panelN1.AllowClose = false;
                (panelN1.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
                this.btnImprimir.Visibility = Visibility.Visible;
                this.btnImportar.Visibility = Visibility.Collapsed;
                if ((pData as Atendimento).SumarioAvaliacaoMedica.DataEncerramento.HasValue)
                {
                    this.btnNotaAdicional.Visibility = Visibility.Visible;
                    this.btnImprimirSomente.Visibility = Visibility.Visible;
                }

                if ((pData as Atendimento).SumarioAvaliacaoMedica.PodeEditarSumario(App.Usuario))
                    this.btnEditarSumario.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((this.DataContext as Atendimento).SumarioAvaliacaoMedica.PodeEditarSumario(App.Usuario))
                {
                    if (DXMessageBox.Show("Deseja realmente finalizar e imprimir o Sumário de Avaliação Médica?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                        repp.OndeChaveIgual((this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID);
                        repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica);
                        if (!repp.List().HasItems())
                            throw new BusinessValidatorException(new InvalidValue("Marque um dos Medicamentos Habituais para Finalizar o Sumário.", this.GetType(), "Sumário de Avaliação Médica", null, this, new Object[] { HMV.Core.Framework.Exception.Tag.Error }));

                        IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                        repa.OndeChaveIgual((this.DataContext as Atendimento).SumarioAvaliacaoMedica.ID);
                        repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica);
                        if (!repa.List().HasItems())
                            throw new BusinessValidatorException(new InvalidValue("Marque uma das Alergias para Finalizar o Sumário.", this.GetType(), "Sumário de Avaliação Médica", null, this, new Object[] { HMV.Core.Framework.Exception.Tag.Error }));


                        ISumarioAvaliacaoMedicaService srv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
                        srv.Finalizar((this.DataContext as Atendimento).SumarioAvaliacaoMedica, App.Usuario);

                        documentGroupN1.Clear();

                        DocumentPanel panelN1;
                        panelN1 = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, new Uri("UserControls\\SumarioAvaliacaoM\\ucResumoAvaliacaoMedica.xaml", UriKind.Relative));
                        //panelN1.Caption = "Visualizar";
                        this.btnImportar.Visibility = System.Windows.Visibility.Collapsed;
                        panelN1.FloatOnDoubleClick = false;
                        panelN1.AllowFloat = false;
                        panelN1.AllowDrag = false;
                        panelN1.AllowHide = false;
                        panelN1.AllowClose = false;

                        (panelN1.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
                        (panelN1.Control as ucResumoAvaliacaoMedica).Imprimir();
                    }
                }
            }
            catch (BusinessValidatorException ex)
            {
                string ret = string.Empty;
                ex.GetErros().ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {
            var qr = (this.DataContext as Atendimento).Paciente.Atendimentos;
            if (qr.Count() > 0)
            {
                var T = (qr.Where(y => y.SumarioAvaliacaoMedica != null).ToList());
                if (T.Where(x => x.SumarioAvaliacaoMedica.Atendimento.ID != (this.DataContext as Atendimento).SumarioAvaliacaoMedica.Atendimento.ID && x.SumarioAvaliacaoMedica.Tipo == (this.DataContext as Atendimento).SumarioAvaliacaoMedica.Tipo).Count() > 0)
                {
                    this.Save();
                    WinImportaDados win = new WinImportaDados(this.DataContext as Atendimento);
                    if (win.ShowDialog(base.OwnerBase).Value)
                    {
                        this.RecriarNiveis();
                        DXMessageBox.Show("Dados importado com sucesso!", "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                    DXMessageBox.Show("Não há dados para Importar", "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RecriarNiveis()
        {
            while (documentGroupN1.Items.Count() != 0)
            {
                documentGroupN1.Remove(documentGroupN1[documentGroupN1.Items.Count() - 1]);
            }
            this.Save();
            this.CriarNiveis();
        }

        public void RefreshMedicamentosEmUsoDoPaciente()
        {
            if (document != null)
            {
                if (document.Control.GetType().Equals(typeof(ucMedicamentosEmUsoPaciente)))
                    (document.Control as ucMedicamentosEmUsoPaciente).RefreshMedicamentosEmUsoDoPaciente();
                else if (documentfilho != null && documentfilho.Control.GetType().Equals(typeof(ucMedicamentosEmUsoPaciente)))
                    (documentfilho.Control as ucMedicamentosEmUsoPaciente).RefreshMedicamentosEmUsoDoPaciente();

                if (document.Control.GetType().Equals(typeof(ucResumoAvaliacaoMedica)))
                    (document.Control as ucResumoAvaliacaoMedica).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
                else if (documentfilho != null && documentfilho.Control.GetType().Equals(typeof(ucResumoAvaliacaoMedica)))
                    (documentfilho.Control as ucResumoAvaliacaoMedica).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
            }
        }

        private void btnNotaAdicional_Click(object sender, RoutedEventArgs e)
        {
            winNotasAdicionais win = new winNotasAdicionais((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
            win.ShowDialog(base.OwnerBase);
            (panelN1.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
        }

        private void btnImprimirSomente_Click(object sender, RoutedEventArgs e)
        {
            (panelN1.Control as IUserControl).SetData((this.DataContext as Atendimento).SumarioAvaliacaoMedica);
            (panelN1.Control as ucResumoAvaliacaoMedica).Imprimir();
        }

        private void btnEditarSumario_Click(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedica sum = (this.DataContext as Atendimento).SumarioAvaliacaoMedica;

            if (DXMessageBox.Show("Este sumário de avaliação médica foi iniciado pelo Profissional: "
                + sum.Usuario.nm_usuario
                + ". Deseja editar este sumário ? ", "Atenção", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                sum.Usuario = App.Usuario;
                this.Save();
                documentGroupN1.Clear();

                this.SetData(this.DataContext as Atendimento);
            }
        }
    }
}
