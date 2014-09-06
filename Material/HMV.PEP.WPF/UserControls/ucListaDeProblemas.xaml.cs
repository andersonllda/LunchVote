using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.PEP.WPF.Cadastros;
using HMV.PEP.WPF.Report;
using System.Windows.Input;
using HMV.Core.Framework.WPF;
using System.Linq;
using System.Collections.Generic;
using HMV.PEP.DTO;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Model.PesqClinica;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.Consult;
using StructureMap;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucListaDeProblemas.xaml
    /// </summary>
    public partial class ucListaDeProblemas : UserControlBase, IUserControl
    {
        private Atendimento _atendimento;
        private Paciente _paciente;

        public bool CancelClose { get; set; }
        public bool AlterouLista { get; set; }

        public ucListaDeProblemas()
        {
            InitializeComponent();
            this.AlterouLista = false;
            this.Filtro_Checked(null, null);



            if (App.Usuario != null)
                if (App.Usuario.Prestador == null)
                {
                    DesabilitaBotoes();
                }
                else if (App.Usuario.Prestador.Conselho == null)
                {
                    DesabilitaBotoes();
                }
                else if (!App.Usuario.Prestador.Conselho.isMedico())
                {
                    DesabilitaBotoes();
                }
        }

        private void DesabilitaBotoes()
        {
            btnIncluir.IsEnabled = false;
            //btnExcluir.IsEnabled = false;
            btnAlterar.IsEnabled = false;
            btnComentarios.IsEnabled = false;
            gdListaProblemas.IsEnabled = false;
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadListaProblema win = new winCadListaProblema((this.DataContext as Paciente), _atendimento);
            if (win.ShowDialog(base.OwnerBase) == true)
            {
                gdListaProblemas.ItemsSource = (this.DataContext as Paciente).ProblemasPaciente;
                gdListaProblemas.RefreshData();
                this.AlterouLista = true;
            }
        }

        public void atualiza()
        {
            if (this.DataContext != null)
            {
                gdListaProblemas.ItemsSource = (this.DataContext as Paciente).ProblemasPaciente;
                gdListaProblemas.RefreshData();
            }
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            if ((ProblemasPaciente)gdListaProblemas.GetFocusedRow() != null)
            {
                if ((gdListaProblemas.GetFocusedRow() as ProblemasPaciente).Status != StatusAlergiaProblema.Excluído)
                {
                    ProblemasPaciente pro = (ProblemasPaciente)gdListaProblemas.GetFocusedRow();
                    String complemento = pro.Descricao;
                    DateTime? fim = pro.DataFim;

                    winCadListaProblema win = new winCadListaProblema(pro, _atendimento);
                    if (win.ShowDialog(base.OwnerBase).Equals(true))
                    {
                        gdListaProblemas.RefreshData();
                        this.AlterouLista = true;
                    }
                    else
                    {
                        pro.Descricao = complemento;
                        pro.DataFim = fim;
                    }
                }
                else
                {
                    DXMessageBox.Show("O problema selecionado já foi excluído!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void btnComentarios_Click(object sender, RoutedEventArgs e)
        {
            if ((ProblemasPaciente)gdListaProblemas.GetFocusedRow() != null)
            {
                winComentarios win = new winComentarios((ProblemasPaciente)gdListaProblemas.GetFocusedRow());

                if (win.ShowDialog(base.OwnerBase).Equals(true))
                {
                    gdListaProblemas.RefreshData();
                    this.AlterouLista = true;
                }
            }
        }

        private void Filtro_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gdListaProblemas != null)
                {
                    if (rbAtivos.IsChecked == true)
                    {
                        gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Ativo.ToString(), BinaryOperatorType.Equal));
                    }
                    //else if (rbExcluidos.IsChecked == true)
                    //{
                    //    gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Excluído.ToString(), BinaryOperatorType.Equal));
                    //}
                    else if (rbInativos.IsChecked == true)
                    {
                        gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Inativo.ToString(), BinaryOperatorType.Equal));
                    }
                    else
                    {
                        //gdListaProblemas.FilterCriteria = null;
                        gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Excluído.ToString(), BinaryOperatorType.NotEqual));
                    }
                }
            }
            catch (Exception ex)
            {
                DXMessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType().BaseType || typeof(Atendimento) == pData.GetType())
            {
                this._atendimento = (pData as Atendimento);
                this.DataContext = (pData as Atendimento).Paciente;
            }
            else
            {
                this.DataContext = (pData as Paciente);
                _paciente = (pData as Paciente);
            }
        }

        private void viewListaProblemas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (btnAlterar.IsEnabled)
                if (grdMenu.Visibility == Visibility.Visible)
                    btnAlterar_Click(this, null);
        }

        private void gdListaProblemas_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdListaProblemas.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdListaProblemas.View).BestFitColumns()));
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            Paciente paciente = (Paciente)DataContext;
            //winRelListaProblema win = new winRelListaProblema(paciente, _atendimento);
            //win.ShowDialog();

            rptResumoPEP report = new rptResumoPEP();
            IList<RelatorioListaProblemaDTO> lista = new List<RelatorioListaProblemaDTO>();
            RelatorioListaProblemaDTO item = new RelatorioListaProblemaDTO();

            if (_atendimento.IsNotNull())
            {
                if (_atendimento.Paciente.IsNotNull())
                {
                    item.NomePaciente = _atendimento.Paciente.Nome;
                    item.IDPaciente = _atendimento.Paciente.ID;
                }

                item.NomeResumo = _atendimento.Leito.IsNotNull() ? _atendimento.Leito.Descricao : string.Empty;
                item.CodigoBarras = _atendimento.ID.ToString();

                if (_atendimento.Prestador.IsNotNull())
                {
                    item.NomePrestador = _atendimento.Prestador.Nome;
                    item.Registro = _atendimento.Prestador.Registro;
                }
            }
            else
            {
                report.BindCodigoBarras.Visible = false;
                report.BindIDPaciente.Visible = false;
            }

            item.Nome = paciente.Nome;
            item.Sexo = paciente.Sexo.ToString();
            item.Cor = paciente.Cor.HasValue ? paciente.Cor.ToString() : string.Empty;
            item.Idade = paciente.Idade.GetDate();
            item.Profissao = paciente.Profissao.IsNotNull() ? paciente.Profissao.Descricao : null;
            item.Prontuario = paciente.ID.ToString();

            item.ProblemasPaciente = paciente.ProblemasPaciente.ToList();
            var vmalergias = new vmAlergias(paciente, App.Usuario, new GetSettings().IsCorpoClinico, _atendimento);
            item.Alergias = vmalergias.ListaAlergias.ToList();
            var vmmedicamentos = new vmMedicamentosEmUsoProntuario(paciente, App.Usuario);
            item.MedicamentosEmUso = vmmedicamentos.ListaMedicamentosEmUso.ToList();
            ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();
            var procreal = (from T in consult.carregaProcedimentosRealizados(paciente)
                            orderby T.DataAtendimento descending, T.IdAtendimento descending
                            select T).ToList();
            item.ProcedimentosRealizados = procreal.ToList();
            if (paciente.PesquisaClinicaAtiva.IsNotNull())
            {
                item.listaPesquisaClinica = new List<PesquisaClinica>();
                item.listaPesquisaClinica.Add(paciente.PesquisaClinicaAtiva);
            }
            lista.Add(item);
            report.DataSource = lista;
            report.ShowPreviewDialog();
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            gdListaProblemas.RefreshData();
        }

        public void ReadOnly()
        {
            grdMenu.Visibility = Visibility.Collapsed;
        }


        #region Retirado Botao Excluir da Tela
        //private void btnExcluir_Click(object sender, RoutedEventArgs e)
        //{
        //    if ((ProblemasPaciente)gdListaProblemas.GetFocusedRow() != null)
        //    {
        //        if ((gdListaProblemas.GetFocusedRow() as ProblemasPaciente).Status != StatusAlergiaProblema.Excluído)
        //        {
        //            if (DXMessageBox.Show("Deseja realmente Excluir o CID do Diagnóstico?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //            {
        //                winJustificaExclusao win = new winJustificaExclusao((ProblemasPaciente)gdListaProblemas.GetFocusedRow());
        //                if (win.ShowDialog() == true)
        //                {
        //                    gdListaProblemas.RefreshData();
        //                    this.AlterouLista = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            DXMessageBox.Show("O problema selecionado já foi excluído!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //    }
        //    else
        //        DXMessageBox.Show("Não há item para excluir!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
        //}
        #endregion
    }
}
