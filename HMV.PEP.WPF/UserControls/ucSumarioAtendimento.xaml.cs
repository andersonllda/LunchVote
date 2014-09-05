using System;
using System.Linq;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaCTINEO;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.WPF.Report.SumarioAvaliacaoMedicaCTINEO;
using HMV.PEP.WPF.Report.SumarioAvaliacaoMedicaRN;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoM;
using HMV.PEP.WPF.UserControls.SumarioDeAlta;
using HMV.PEP.WPF.Windows;
using HMV.PEP.WPF.Windows.BoletimDeEmergencia;
using StructureMap;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.PEP.WPF.Report.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.PEP.WPF.Windows.SumarioDeAtendimentos;
using DevExpress.Xpf.Core;
using System.Windows;
using HMV.PEP.Interfaces;
using HMV.PEP.WPF.ResumoProntuario;
using System.Reflection;
using System.IO;
using HMV.Core.DTO;
using System.Collections.Generic;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucSumarioAtendimento.xaml
    /// </summary>
    public partial class ucSumarioAtendimento : UserControlBase, IUserControl
    {

        public TipoAtendimentoSumario fTipoAtendimentoSumario = TipoAtendimentoSumario.Todos;

        public ucSumarioAtendimento() { }

        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                _Atendimento = (pData as Atendimento);
                this.DataContext = new vmSumarioAtendimento((pData as Atendimento).Paciente, fTipoAtendimentoSumario, false, false, App.Usuario, _Atendimento);
            }
            else if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {
                this.DataContext = new vmSumarioAtendimento((pData as Paciente), fTipoAtendimentoSumario, false, false, App.Usuario, null);
            }
            ucProcedimentosRealizados1.SetData((this.DataContext as vmSumarioAtendimento).vmProcedimentosRealizados);
            InitializeComponent();         
        }      

        public bool CancelClose { get; set; }

        private Atendimento _Atendimento { get; set; }
        private void btnDescricaoCirurgica_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winDescricaoCirurgica win = new winDescricaoCirurgica((this.DataContext as vmSumarioAtendimento).SumarioAtendimentoSelecionado);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnAvaliacaoMedica_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // O botao de sumario, habilita somente se existe algum sumario encerrado, regra no banco fnc_siga_prontuario.
            // Entao so e aberto os sumario finalizados. 

            winRelatorio win = null;

            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento _atendimento = rep.OndeCodigoAtendimentoIgual((this.DataContext as vmSumarioAtendimento).SumarioAtendimentoSelecionado.IdAtendimento).Single();

            IRepositorioDeSumarioAvaliacaoMedicaCTINEO repCTINEO = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
            var sumarioCTINEO = repCTINEO.OndeCodigoAtendimentoIgual(_atendimento).List().FirstOrDefault(x=>x.DataEncerramento.HasValue);

            IRepositorioDeSumarioDeAvaliacaoMedicaRN repRN = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
            var sumarioRN = repRN.OndeCodigoAtendimentoIgual(_atendimento).List().FirstOrDefault(x => x.DataEncerramento.HasValue);

            IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia repEND = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia>();
            var sumarioendoscopia = repEND.OndeIdAtendimentoIgual(_atendimento.ID).Single();
            
            // Se existir os dois sumario, o usuario deve escolher qual ele quer abrir
            if (sumarioCTINEO.IsNotNull() && sumarioRN.IsNotNull())
            {
                // Verifica qual sumario deve abrir 
                winSelecionaSumarioDeAvaliacao winSel = new winSelecionaSumarioDeAvaliacao();
                if (winSel.ShowDialog(base.OwnerBase) == true)
                {
                    if (winSel.SumarioCTINEO)
                        sumarioRN = null;
                    if (winSel.SumarioRN)
                        sumarioCTINEO = null;
                }
                else               
                   return;                                   
            }

            // Abri sumario do RN
            if (sumarioRN.IsNotNull())
            {
                var vm = new vmSumarioAvaliacaoMedicaRN(_atendimento, App.Usuario, new GetSettings().IsCorpoClinico);
                var rpt = new rptSumarioAvaliacaoMedicaRN(new vmRelatorioSumarioAvaliacaoMedicaRN(vm));
                win = new winRelatorio(rpt, vm.SumarioAvaliacaoMedicaRN.DataEncerramento.HasValue, "Relatório de Avaliação Médica", !vm.SumarioAvaliacaoMedicaRN.DataEncerramento.HasValue);
            }

            // Abri Sumario da CTINEO
            if (sumarioCTINEO.IsNotNull())
            {
                var rpt = new rptSumarioAvaliacaoMedicaCTINEO(new vmRelatorioSumarioAvaliacaoMedicaCTINEO(new Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO.wrpSumarioAvaliacaoMedicaCTINEO(sumarioCTINEO)));
                win = new winRelatorio(rpt, sumarioCTINEO.DataEncerramento.HasValue, "Relatório de Avaliação Médica", !sumarioCTINEO.DataEncerramento.HasValue);
            }
            
            // Abri sumario do CO
            if (win.IsNull())
            {
                IRepositorioDeSumarioAvaliacaoMedicaCO repe = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCO>();
                var ret = repe.OndeCodigoAtendimentoIgual(_atendimento).List().FirstOrDefault(x=>x.DataEncerramento.HasValue);
                if (ret.IsNotNull())
                {
                    wrpSumarioAvaliacaoMedicaCO sumCO = new wrpSumarioAvaliacaoMedicaCO(ret);
                    rptSumarioAvaliacaoMedicaCO rpt = new rptSumarioAvaliacaoMedicaCO(new vmRelatorioSumarioAvaliacaoMedicaCO(sumCO));
                    win = new winRelatorio(rpt, sumCO.DataEncerramento == null ? false : true, "Relatório de Avaliação Médica", sumCO.DataEncerramento == null ? true : false);
                }
            }

            // Abri sumario de endoscopia
            if (sumarioendoscopia.IsNotNull())
            {
                var rpt = new rptSumarioAvaliacaoMedicaEndoscopia(new vmRelatorioSumarioAvaliacaoMedicaEndoscopia(new vmSumarioAvaliacaoMedicaEndoscopia(_atendimento, _atendimento.Paciente, App.Usuario, new GetSettings().IsCorpoClinico)));
                win = new winRelatorio(rpt, sumarioendoscopia.DataEncerramento.HasValue, "Relatório de Avaliação Médica", !sumarioendoscopia.DataEncerramento.HasValue);
            }

            // Abri sumario Adulto 
            if (win.IsNull())
            {
                ucResumoAvaliacaoMedica uc = new ucResumoAvaliacaoMedica();
                uc.SetData(_atendimento.SumarioAvaliacaoMedica);
                //quando o sumario de avaliacao medica ainda nao foi finalizado, mostra a marca dagua "Documento Incompleto" e esconde a barra de botoes superior.
                win = new winRelatorio(uc.ucRelSumarioAvaliacaoMedica1.report, _atendimento.SumarioAvaliacaoMedica.DataEncerramento == null ? false : true, "Relatório de Avaliação Médica", _atendimento.SumarioAvaliacaoMedica.DataEncerramento == null ? true : false);
            }
          
            win.ShowDialog(base.OwnerBase);
        }

        private void btnAltaMedica_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            vmSumarioAlta vm = new vmSumarioAlta((this.DataContext as vmSumarioAtendimento).Atendimento, App.Usuario, false);
            ucResumoAltaMedica uc = new ucResumoAltaMedica();
            uc.SetData(vm);
            //winRelatorio win = new winRelatorio(uc.report, (this.DataContext as vmSumarioAtendimento).Atendimento.SumarioAlta.DataAlta == null ? false : true, "Relatório de Alta Médica", (this.DataContext as vmSumarioAtendimento).Atendimento.SumarioAlta.DataAlta == null ? true : false);

            winRelatorio win = new winRelatorio(uc.report, vm.JaTemAlta, "Relatório de Alta Médica", !vm.JaTemAlta);

            win.ShowDialog(base.OwnerBase);
        }

        private void btnEmergencia_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winRelatorioBoletimEmergencia win = new winRelatorioBoletimEmergencia(new wrpAtendimento((this.DataContext as vmSumarioAtendimento).Atendimento), App.Usuario);
            win.ShowDialog(base.OwnerBase);
        }

        private void btnProntuario_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ObjectFactory.GetInstance<IParametroPEPService>().HabilitaNovoProntuarioAmbulatorialNoSumarioDeAtendimento())
            {
                winResumoDoProntuarioNovo win = new winResumoDoProntuarioNovo(new vmResumoDoProntuario((this.DataContext as vmSumarioAtendimento).Paciente, (this.DataContext as vmSumarioAtendimento).SumarioAtendimentoSelecionado.Seq_Atendimento));
                win.ShowDialog(base.OwnerBase);
            }
            else
            {
                winResumoDoProntuario win = new winResumoDoProntuario(new vmResumoDoProntuario((this.DataContext as vmSumarioAtendimento).Paciente, (this.DataContext as vmSumarioAtendimento).SumarioAtendimentoSelecionado.Seq_Atendimento));
                win.ShowDialog(base.OwnerBase);
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((TableView)gdSumarioAtendimentos.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdSumarioAtendimentos.View).BestFitColumns()));
            //((TableView)gdProcedimentosRealizados.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdProcedimentosRealizados.View).BestFitColumns()));
        }

        private void btnGED_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAtendimento).Atendimento != null)
            {
                winGED win = new winGED((this.DataContext as vmSumarioAtendimento).Atendimento);
                win.ShowDialog(base.OwnerBase);
            }
        }

        private void btnLegenda_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var win = new winSumarioAtendimentoLegenda();
            win.ShowDialog(base.OwnerBase);
        }

        private void btnPreAnestesica_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSumarioAtendimentoSelecionaPreAnestesica win = new winSumarioAtendimentoSelecionaPreAnestesica((this.DataContext as vmSumarioAtendimento));
            if ((this.DataContext as vmSumarioAtendimento).SumarioAvaliacaoPreAnestesicaCollection.Count == 1)
            {
                win.Relatorio();
            }
            else
            {
                win.ShowDialog(base.OwnerBase);
            }
            win = null;            
        }

        private void btnProcEnfermagem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAtendimento).Atendimento.IsNotNull())
            {
                winRelatorioProcessosDeEnfermagem win = new winRelatorioProcessosDeEnfermagem(new wrpAtendimento((this.DataContext as vmSumarioAtendimento).Atendimento), App.Usuario);
                win.ShowDialog(base.OwnerBase);
            }
            else
            {
                DXMessageBox.Show("Não existem documentos para este atendimento!", "Sumário de Atendimentos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEvolucoes_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmSumarioAtendimento).Atendimento.IsNotNull())
            {
                ChamaEvolucoes();
            }
        }

        private void ChamaEvolucoes()
        {

            String[] lDados = "DLL;HMV.ProcessosEnfermagem.WPF.dll;HMV.ProcessosEnfermagem.WPF.RelatorioEvolucoes".Split(';');

            string pasta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assembly MyAsm = Assembly.LoadFrom(Path.Combine(pasta, lDados[1]));

            System.Type lType = MyAsm.GetType(lDados[2]);
            object lRet;

            if (lType != null)
            {
                ConexaoDTO conn = new ConexaoDTO();
                conn.Banco = App.Banco;
                conn.IdUsuario = App.Usuario.cd_usuario;
                conn.Dados = new List<Atendimento>() { (this.DataContext as vmSumarioAtendimento).Atendimento };
                conn.Parametros = new object[] { true, true };
                lRet = Activator.CreateInstance(lType, BindingFlags.ExactBinding, null, null, null);

                string lpasso = string.Empty;
                try
                {
                    lpasso = "Passo1";
                    IUserControl obj = (IUserControl)lRet;
                    lpasso = "Passo2";
                    obj.SetData(conn);
                    lpasso = "Passo3";
                }
                catch (Exception e)
                {
                    DXMessageBox.Show("Não foi possível abrir o componente das Evolucoes" + Environment.NewLine + lpasso + "  " + e.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw;
                }
            }
            
        }
    }
}
