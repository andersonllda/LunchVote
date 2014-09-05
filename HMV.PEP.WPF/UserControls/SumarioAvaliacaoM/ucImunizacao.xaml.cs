using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.PEP.WPF.Cadastros.SumarioAvaliacaoM;
using StructureMap;
using HMV.PEP.Interfaces;
using HMV.Core.Interfaces;
using HMV.Core.DTO;
using System.Collections.Generic;
using HMV.Core.Domain.Enum;
using HMV.PEP.WPF.Windows;
using DevExpress.Xpf.Grid;
using System;
using System.Linq;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for UserControlImunizacao.xaml
    /// </summary>
    public partial class ucImunizacao : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucImunizacao()
        {
            InitializeComponent();
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadImunizacaoDetalhe win = new winCadImunizacaoDetalhe(this.DataContext as Paciente);
            if (win.ShowDialog(base.OwnerBase).Equals(true))
            {
                this.Refresh();
                this.Save();
            }
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            if (gdImunizacao.GetFocusedRow() != null)
            {
                Imunizacao imu = (Imunizacao)gdImunizacao.GetFocusedRow();
                string observacao = imu.Observacao;
                SimNao disponivel = imu.Disponivel;
                IList<ImunizacaoDetalhe> det = new List<ImunizacaoDetalhe>();
                if (imu.ImunizacaoDetalhe != null)
                {
                    foreach (var item in imu.ImunizacaoDetalhe)
                        det.Add(item);

                    winCadImunizacaoDetalhe win = new winCadImunizacaoDetalhe(imu);
                    if (win.ShowDialog(base.OwnerBase).Equals(true))
                    {
                        this.Refresh();
                        this.Save();
                    }
                    else
                    {
                        // volta status do objeto.
                        imu.Observacao = observacao;
                        imu.Disponivel = disponivel;

                        while (imu.ImunizacaoDetalhe.Count > 0)
                            imu.ImunizacaoDetalhe.RemoveAt(0);

                        foreach (var item in det)
                            imu.ImunizacaoDetalhe.Add(item);
                    }
                }
                else
                {
                    winCadImunizacaoDetalhe win = new winCadImunizacaoDetalhe(imu);
                    if (win.ShowDialog(base.OwnerBase).Equals(true))
                    {
                        this.Refresh();
                        this.Save();
                    }
                }
            }
        }

        private void Refresh()
        {
            var x = this.DataContext as Paciente;
            this.DataContext = null;
            this.DataContext = x;
            gdImunizacao.RefreshData();
            HabilitaDesabilitaBotoes();
        }

        private void HabilitaDesabilitaBotoes()
        {
            btnAlterarVacina.IsEnabled = true;
            btnExcluirVacina.IsEnabled = true;
            btnExcluir.IsEnabled = true;
            btnAlterar.IsEnabled = true;
            if ((this.DataContext as Paciente).Imunizacoes.Count == 0 || cheNaoDisponivel.IsChecked == true)
            {
                btnExcluir.IsEnabled = false;
                btnAlterar.IsEnabled = false;
            }
            if ((this.DataContext as Paciente).OutrasVacinas.Count == 0 || cheNaoDisponivel.IsChecked == true)
            {
                btnAlterarVacina.IsEnabled = false;
                btnExcluirVacina.IsEnabled = false;
            }
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {            
            if (gdImunizacao.GetFocusedRow() != null)
            {
                if ((gdImunizacao.GetFocusedRow() as Imunizacao).ImunizacaoDetalhe == null || (gdImunizacao.GetFocusedRow() as Imunizacao).ImunizacaoDetalhe.Count == 0)
                {
                    if (DXMessageBox.Show("Deseja realmente excluir a imunização selecionada?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        (this.DataContext as Paciente).RemoveImunizacao((Imunizacao)gdImunizacao.GetFocusedRow());
                    this.Refresh();
                    this.Save();
                }
                else
                {
                    DXMessageBox.Show("Existem imunizações cadastradas, não será possível excluir!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public void SetData(object pData)
        {
            HMV.PEP.Interfaces.IPacienteService serv = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IPacienteService>();
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                (pData as Atendimento).Paciente = serv.FiltraPorID((pData as Atendimento).Paciente.ID);
                this.DataContext = (pData as Atendimento).Paciente;
            }
            else if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {
                pData = serv.FiltraPorID((pData as Paciente).ID);
                this.DataContext = (pData as Paciente);
            }
            else if (typeof(SumarioAvaliacaoMedica) == pData.GetType() || typeof(SumarioAvaliacaoMedica) == pData.GetType().BaseType)
            {
                (pData as SumarioAvaliacaoMedica).Paciente = serv.FiltraPorID((pData as SumarioAvaliacaoMedica).Paciente.ID);
                this.DataContext = (pData as SumarioAvaliacaoMedica).Paciente;
            }
            HabilitaDesabilitaBotoes();
        }

        private void cheNaoDisponivel_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                if ((bool)e.NewValue)
                {
                    (this.DataContext as Paciente).AddSemImunizacao(App.Usuario);
                }
                else
                {
                    (this.DataContext as Paciente).RemoveSemImunizacao();
                }

                this.Refresh();
            }
            catch (BusinessValidatorException ex)
            {
                e.IsCancel = true;
                e.Handled = true;
                DXMessageBox.Show(ex.GetErros()[0].Message, "Alerta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void viewImunizacao_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {           
                this.btnAlterar_Click(this, null);
        }

        public void Save()
        {
            HMV.PEP.Interfaces.IPacienteService srv = ObjectFactory.GetInstance<HMV.PEP.Interfaces.IPacienteService>();
            srv.Salvar((this.DataContext as Paciente));
            HabilitaDesabilitaBotoes();
        }

        private void btnAlterarVacina_Click(object sender, RoutedEventArgs e)
        {
            if (viewOutrasVacinas.FocusedRow == null)
                return;

            DateTime data = DateTime.Today;

            if ((viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).Data.HasValue)
                data = (viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).Data.Value;

            winCalendario formCaledario = new winCalendario(data);
            if (formCaledario.ShowDialog(base.OwnerBase) == true)
            {
                var rowhandle = viewOutrasVacinas.FocusedRowHandle;

                this.Refresh();

                viewOutrasVacinas.FocusedRow = gdOutrasVacinas.GetRow(rowhandle);                  

                data = formCaledario.Data;

                (this.DataContext as Paciente).Imunizacoes.Where(x => x.ID == (viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).ID).Single()
                    .ImunizacaoDetalhe.Where(x => x.ImunizacaoTipo.Descricao == (viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).Descricao).Single().Data = data;

                this.Refresh();
                this.Save();
            }
        }

        private void btnExcluirVacina_Click(object sender, RoutedEventArgs e)
        {
            if (viewOutrasVacinas.FocusedRow == null)
                return;

            this.Refresh();

            (this.DataContext as Paciente).Imunizacoes.Where(x => x.ID == (viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).ID).Single()
                .ImunizacaoDetalhe.Remove((this.DataContext as Paciente).Imunizacoes.Where(x => x.ID == (viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).ID).Single()
                .ImunizacaoDetalhe.Where(x => x.ImunizacaoTipo.Descricao == (viewOutrasVacinas.FocusedRow as ImunizacaoDetalheDTO).Descricao).Single());

            this.Refresh();
            this.Save();
        }        

        private void viewOutrasVacinas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnAlterarVacina_Click(sender, null);
        }
    }
}
