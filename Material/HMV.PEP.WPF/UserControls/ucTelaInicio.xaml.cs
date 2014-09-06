using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using System.Linq;
using HMV.PEP.WPF.Cadastros;
using HMV.Core.Domain.Enum;
using System;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.PEP.WPF.Windows;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Constant;
using StructureMap;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucTelaInicio.xaml
    /// </summary>
    public partial class ucTelaInicio : UserControlBase, IUserControl
    {
        public ucTelaInicio()
        {
            InitializeComponent();
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
            btnAlterarAtivo.IsEnabled = false;
            btnInativar.IsEnabled = false;
            btnExcluir.IsEnabled = false;
            btnExcluirInativos.IsEnabled = false;
        }

        public bool CancelClose
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        vmAlergias vmalergias = null;
        vmMedicamentosEmUsoProntuario vmmedicamentos = null;
        Atendimento ate = null;
        Paciente pac = null;
        public void SetData(object pData)
        {
            this.DataContext = pData;

            if (this.DataContext.GetType() == typeof(Paciente) || (this.DataContext.GetType().BaseType == typeof(Paciente)))
            {
                pac = (this.DataContext as Paciente);
            }
            else if (this.DataContext.GetType() == typeof(Atendimento) || (this.DataContext.GetType().BaseType == typeof(Atendimento)))
            {
                ate = (this.DataContext as Atendimento);
                pac = (this.DataContext as Atendimento).Paciente;
            }

            this.ProcedimentosRealizados.SetData(new vmProcedimentosRealizados(pac, true, true));
            this.gdListaProblemasAtivos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Ativo).ToList();
            this.gdListaProblemasInativos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();

            vmalergias = new vmAlergias(pac, App.Usuario, new GetSettings().IsCorpoClinico, ate);
            this.gdAlergias.ItemsSource = vmalergias.ListaAlergias;
            vmmedicamentos = new vmMedicamentosEmUsoProntuario(pac, App.Usuario);
            this.gdMedicamentos.ItemsSource = vmmedicamentos.ListaMedicamentosEmUso;
        }

        private void btnIncluir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winCadListaProblema win = new winCadListaProblema(pac, ate);
            if (win.ShowDialog(base.OwnerBase) == true)
            {
                this.gdListaProblemasAtivos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Ativo).ToList();
                this.gdListaProblemasInativos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();
                gdListaProblemasAtivos.RefreshData();
                gdListaProblemasInativos.RefreshData();
            }
        }

        private void btnInativar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((ProblemasPaciente)gdListaProblemasAtivos.GetFocusedRow() != null)
            {
                if ((gdListaProblemasAtivos.GetFocusedRow() as ProblemasPaciente).Status != StatusAlergiaProblema.Excluído)
                {
                    ProblemasPaciente pro = (ProblemasPaciente)gdListaProblemasAtivos.GetFocusedRow();
                    string complemento = pro.Descricao;
                    DateTime? fim = pro.DataFim;
                    winInativarProblema win = new winInativarProblema(pro);
                    if (win.ShowDialog(base.OwnerBase).Equals(true))
                    {
                        this.gdListaProblemasAtivos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Ativo).ToList();
                        this.gdListaProblemasInativos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();
                        gdListaProblemasAtivos.RefreshData();
                        gdListaProblemasInativos.RefreshData();
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

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if ((ProblemasPaciente)gdListaProblemasAtivos.GetFocusedRow() != null)
            {
                if (DXMessageBox.Show("Deseja realmente excluir o CID selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ProblemasPaciente pro = (ProblemasPaciente)gdListaProblemasAtivos.GetFocusedRow();
                    Paciente pac = pro.Paciente;
                    pro.Status = StatusAlergiaProblema.Excluído;
                    pac.AlteraProblemasPaciente(pro);
                    HMV.PEP.Interfaces.IPacienteService srv = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IPacienteService>();
                    srv.Salvar(pac);
                    this.gdListaProblemasAtivos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Ativo).ToList();
                    gdListaProblemasAtivos.RefreshData();
                }
            }
        }

        private void btnExcluirInativos_Click(object sender, RoutedEventArgs e)
        {
            if ((ProblemasPaciente)gdListaProblemasInativos.GetFocusedRow() != null)
            {
                if (DXMessageBox.Show("Deseja realmente excluir o CID selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ProblemasPaciente pro = (ProblemasPaciente)gdListaProblemasInativos.GetFocusedRow();
                    Paciente pac = pro.Paciente;
                    pro.Status = StatusAlergiaProblema.Excluído;
                    pac.AlteraProblemasPaciente(pro);
                    HMV.PEP.Interfaces.IPacienteService srv = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IPacienteService>();
                    srv.Salvar(pac);
                    this.gdListaProblemasInativos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();
                    gdListaProblemasInativos.RefreshData();
                }
            }
        }

        private void btnAlterarAtivo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((ProblemasPaciente)gdListaProblemasAtivos.GetFocusedRow() != null)
            {
                if ((gdListaProblemasAtivos.GetFocusedRow() as ProblemasPaciente).Status != StatusAlergiaProblema.Excluído)
                {
                    ProblemasPaciente pro = (ProblemasPaciente)gdListaProblemasAtivos.GetFocusedRow();
                    string complemento = pro.Descricao;
                    DateTime? fim = pro.DataFim;

                    winCadListaProblema win = new winCadListaProblema(pro, ate);
                    if (win.ShowDialog(base.OwnerBase).Equals(true))
                    {
                        this.gdListaProblemasAtivos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Ativo).ToList();
                        this.gdListaProblemasInativos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();
                        gdListaProblemasAtivos.RefreshData();
                        gdListaProblemasInativos.RefreshData();
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

        private void btnIncluirAlergia_Click(object sender, RoutedEventArgs e)
        {
            vmalergias.NovoRegistro();

            Windows.Alergia.winCadAlergia win = new Windows.Alergia.winCadAlergia(vmalergias);
            win.ShowDialog(base.OwnerBase);

            vmalergias.AtualizaListaAlergias(pac);
            this.gdAlergias.ItemsSource = vmalergias.ListaAlergias;
            this.gdAlergias.RefreshData();
        }

        private void btnAlterarAlergia_Click(object sender, RoutedEventArgs e)
        {
            if ((wrpAlergia)gdAlergias.GetFocusedRow() == null)
                return;
            if (vmalergias.SemAlergiasConhecidas)
            {
                DXMessageBox.Show("Não é possível alterar '" + Constantes.coSemAlergiasConhecidas + "'", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            vmalergias.AlergiaSelecionada = gdAlergias.GetFocusedRow() as wrpAlergia;
            Windows.Alergia.winCadAlergia win = new Windows.Alergia.winCadAlergia(vmalergias);
            win.ShowDialog(base.OwnerBase);

            vmalergias.AtualizaListaAlergias(pac);
            this.gdAlergias.ItemsSource = vmalergias.ListaAlergias;
            this.gdAlergias.RefreshData();
        }

        private void btnIncluirMedicamento_Click(object sender, RoutedEventArgs e)
        {
            winCadMedicamentosEmUso win = new winCadMedicamentosEmUso(vmmedicamentos, null);
            win.ShowDialog(base.OwnerBase);
            vmmedicamentos.carrega();
            this.gdMedicamentos.ItemsSource = vmmedicamentos.ListaMedicamentosEmUso;
            gdMedicamentos.RefreshData();
        }

        private void btnAlterarMedicamento_Click(object sender, RoutedEventArgs e)
        {
            if (gdMedicamentos.GetFocusedRow() != null)
            {
                if (vmmedicamentos.verificaSemMedicamentosEmUso)
                {
                    DXMessageBox.Show("Não é possível alterar 'Sem Uso de Medicamentos'", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                winCadMedicamentosEmUso win = new winCadMedicamentosEmUso(vmmedicamentos, gdMedicamentos.GetFocusedRow() as wrpMedicamentosEmUsoProntuario);
                if (win.ShowDialog(base.OwnerBase) == true)
                {
                    vmmedicamentos.carrega();
                    this.gdMedicamentos.ItemsSource = vmmedicamentos.ListaMedicamentosEmUso;
                    gdMedicamentos.RefreshData();
                }
                else
                {
                    this.gdMedicamentos.ItemsSource = vmmedicamentos.ListaMedicamentosEmUso;
                    gdMedicamentos.RefreshData();
                }
            }
            else
            {
                DXMessageBox.Show("Selecione um item.", "Aviso:", MessageBoxButton.OK);
            }
        }

        public void atualiza()
        {
            if (this.DataContext.IsNotNull())
            {
                this.gdListaProblemasAtivos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Ativo).ToList();
                this.gdListaProblemasInativos.ItemsSource = pac.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();
                gdListaProblemasAtivos.RefreshData();
                gdListaProblemasInativos.RefreshData();
                vmalergias.AtualizaListaAlergias(pac);
                this.gdAlergias.ItemsSource = vmalergias.ListaAlergias;
                this.gdAlergias.RefreshData();
                vmmedicamentos.carrega();
                this.gdMedicamentos.ItemsSource = vmmedicamentos.ListaMedicamentosEmUso;
                gdMedicamentos.RefreshData();
            }
        }
    }
}
