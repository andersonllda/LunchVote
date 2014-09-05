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
using HMV.Core.Domain.Repository;
using HMV.PEP.WPF.Report;
using HMV.PEP.Consult;
using StructureMap;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.PEP.DTO;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winDescricaoCirurgica.xaml
    /// </summary>
    public partial class winDescricaoCirurgica : WindowBase
    {
        public winDescricaoCirurgica(SumarioDeAtendimentosDTO pSumarioDeAtendimentosDTO)
        {
            InitializeComponent();
            this.DataContext = new vmSumarioAtendimento(pSumarioDeAtendimentosDTO);
            if ((this.DataContext as vmSumarioAtendimento).DescricoesCirurgicas.Count() == 0)
            {
                System.Windows.Forms.MessageBox.Show("Não há Relatórios", "Atenção");
                this.Visibility = System.Windows.Visibility.Hidden;
            }
            else if ((this.DataContext as vmSumarioAtendimento).DescricoesCirurgicas.Count() == 1)
            {
                (this.DataContext as vmSumarioAtendimento).DescricaoCirurgicaSelecionada = (this.DataContext as vmSumarioAtendimento).DescricoesCirurgicas.FirstOrDefault();
                Relatorio();
                this.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public winDescricaoCirurgica(vmProcedimentosRealizados pVm)
        {
            InitializeComponent();
            this.DataContext = pVm;
            if (pVm.DescricoesCirurgicas.Count() == 0)
            {
                System.Windows.Forms.MessageBox.Show("Não há Relatórios", "Atenção");
                this.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (pVm.DescricoesCirurgicas.Count() == 1)
            {
                pVm.DescricaoCirurgicaSelecionada = pVm.DescricoesCirurgicas.FirstOrDefault();
                Relatorio();
                this.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                Relatorio();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            Relatorio();
        }

        private void Relatorio()
        {
            IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
            rptDescricaoCirurgica rtp = new rptDescricaoCirurgica();

            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("rep", rep);
            IDescricaoCirurgicaConsult consult = ObjectFactory.GetInstance<IDescricaoCirurgicaConsult>(args);
            List<DescricaoCirurgicaDTO> ret = new List<DescricaoCirurgicaDTO>();
            if (this.DataContext is vmSumarioAtendimento)
                ret = consult.DescricaoCirurgica((DataContext as vmSumarioAtendimento).DescricaoCirurgicaSelecionada.Id);
            else
                ret = consult.DescricaoCirurgica((DataContext as vmProcedimentosRealizados).DescricaoCirurgicaSelecionada.Id);

            rtp.DataSource = ret;
            rtp.labelPaciente.Text = ret[0].IDPaciente.ToString() + " - " + ret[0].NomePaciente;
            rtp.labelConvenio.Text = ret[0].IDConvenio.ToString() + " - " + ret[0].Convenio;

            rtp.lbCEC.Visible = false;
            rtp.lblCEC.Visible = false;
            if (ret[0].CEC.Equals(Core.Domain.Enum.SimNao.Sim))
            {
                rtp.lbCEC.Visible = true;
                rtp.lblCEC.Visible = true;
            }

            rtp.lbRobo.Visible = false;
            rtp.lblRobo.Visible = false;
            if (ret[0].Robotica.Equals(Core.Domain.Enum.SimNao.Sim))
            {
                rtp.lbRobo.Visible = true;
                rtp.lblRobo.Visible = true;
            }

            rtp.lbCidFinal.Visible = false;
            if (ret[0].CID != null)
                rtp.lbCidFinal.Visible = true;

            rtp.drRiscoCirurgico.Visible = false;
            if (ret[0].RiscoCirurgico != null)
                rtp.drRiscoCirurgico.Visible = true;

            rtp.drOrtoseProtese.Visible = false;
            if (ret[0].OrteseProtese != null)
                rtp.drOrtoseProtese.Visible = true;

            rtp.drIntercorrencias.Visible = false;
            if (ret[0].Intercorrencias != null)
                rtp.drIntercorrencias.Visible = true;

            rtp.drDescricaoCirurgica.Visible = false;
            if (ret[0].DescricaoCirurgica != null)
            {
                if (this.DataContext is vmSumarioAtendimento)
                    rtp.subDescricoes.ReportSource.DataSource = (DataContext as vmSumarioAtendimento).RelDescricoes;
                else
                    rtp.subDescricoes.ReportSource.DataSource = (DataContext as vmProcedimentosRealizados).RelDescricoes;
                rtp.drDescricaoCirurgica.Visible = true;
            }

            rtp.xrEquipeMedica.Visible = false;
            if (ret[0].EquipeMedica.Count > 0)
            {
                rtp.xrEquipeMedica.ReportSource.DataSource = ret[0].EquipeMedica.DistinctBy(x => x.ID);
                rtp.xrEquipeMedica.Visible = true;
            }

            rtp.xrAchados.Visible = false;
            if (ret[0].AchadosCirurgicos.Count > 0)
            {
                rtp.xrAchados.ReportSource.DataSource = ret[0].AchadosCirurgicos;
                rtp.xrAchados.Visible = true;
            }

            rtp.xrProcedimentos.Visible = false;
            if (ret[0].Procedimentos.Count > 0)
            {
                rtp.xrProcedimentos.ReportSource.DataSource = ret[0].Procedimentos;
                rtp.xrProcedimentos.Visible = true;
            }


            rtp.labelDR.Text = (string.Format(@"{0} CRM: {1}",
                   ret[0].EquipeMedica[0].Prestador + Environment.NewLine,
                     ret[0].EquipeMedica[0].Registro + Environment.NewLine));


            winRelatorio win = new winRelatorio(rtp, true, "Relatório de Descrição Cirúrgica", false);
            win.ShowDialog(this.Owner);
        }
    }
}
