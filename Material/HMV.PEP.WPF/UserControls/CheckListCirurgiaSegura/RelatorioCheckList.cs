using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.WPF.Report.CheckListCirurgia;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;
using System.Windows.Forms;
using HMV.Core.Framework.Expression;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura
{
    public class RelatorioCheckList
    {
        private wrpCheckListCirurgia _checklist;
        public RelatorioCheckList(wrpCheckListCirurgia pCheckList)
        {
            this._checklist = pCheckList;
        }

        public rptCheckListCirurgia Relatorio()
        {
            rptCheckListCirurgia rpt = new rptCheckListCirurgia();

            #region _checklist CIRURGIA SEGURA
            rpt.p10NomeDoPaciente.Text = _checklist.Paciente.Nome;
            rpt.p10ProcedimentoProposto.Text = _checklist.Cirurgia.ds_cirurgia;
            rpt.p10Cirurgiao.Text = _checklist.Prestador.NomeExibicao;
                //.NomeExibicaoPrestador;
            rpt.p10AvisoCirurgia.Text = _checklist.AvisoCirurgia.cd_aviso_cirurgia.ToString();
            rpt.p10Registro.Text = _checklist.Paciente.ID.ToString();
            rpt.p10Atendimento.Text = _checklist.Atendimento.ID.ToString();
            rpt.p10Data.Text = _checklist.AvisoCirurgia.dt_aviso_cirurgia.ToString("dd/MM/yyyy");

            rpt.p3NomeDoPaciente.Text = _checklist.Paciente.Nome;
            rpt.p2ProcedimentoProposto.Text = _checklist.Cirurgia.ds_cirurgia;
            rpt.p2Cirurgiao.Text = _checklist.Prestador.NomeExibicao;
                //.NomeExibicaoPrestador;
            rpt.p2AvisoCirurgia.Text = _checklist.AvisoCirurgia.cd_aviso_cirurgia.ToString();
            rpt.p2Registro.Text = _checklist.Paciente.ID.ToString();
            rpt.p2Atendimento.Text = _checklist.Atendimento.ID.ToString();
            rpt.p2Data.Text = _checklist.AvisoCirurgia.dt_aviso_cirurgia.ToString("dd/MM/yyyy");
            #endregion

            #region ANTES DA ENTRADA DO PACIENTE EM SALA CIRURGICA


            if (_checklist.AntesEntradaPaciente.ConfirmacaoPeloProntuario != SimNao.Sim)
                rpt.p1PacienteOuResponsavel.Text = _checklist.AntesEntradaPaciente.Responsavel.IsNotEmptyOrWhiteSpace() ? _checklist.AntesEntradaPaciente.Responsavel : _checklist.Paciente.Nome;

            rpt.p1Nome.CheckState = _checklist.AntesEntradaPaciente.Nome.IsNotNull() ? _checklist.AntesEntradaPaciente.Nome.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1Procedimento.CheckState = _checklist.AntesEntradaPaciente.Procedimento.IsNotNull() ? _checklist.AntesEntradaPaciente.Procedimento.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1LocalDoProcedimento.CheckState = _checklist.AntesEntradaPaciente.LocalProcedimento.IsNotNull() ? _checklist.AntesEntradaPaciente.LocalProcedimento.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1ConfirmacaoPeloProntuario.CheckState = _checklist.AntesEntradaPaciente.ConfirmacaoPeloProntuario.IsNotNull() ? _checklist.AntesEntradaPaciente.ConfirmacaoPeloProntuario.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1ObservacoesProcedimento.Text = _checklist.AntesEntradaPaciente.Observacao;

            //
            //    ALERGIAS
            //          
            string vTexto = string.Empty;
            foreach (wrpAlergia Alergia in this._checklist.Alergias.Where(x => x.Status == StatusAlergiaProblema.Ativo).ToList() /*_vmalergiasevento.AlergiaCollection*/)
            {
                //if (Alergia.Selecionado)
                vTexto += Alergia.Agente.Combine(", ");
            }

            if (!vTexto.IsEmpty())
                rpt.p1Alergias.Text = vTexto.Remove(vTexto.Length - 2);

            rpt.p1MID.CheckState = _checklist.AntesEntradaPaciente.Pulseira.IsNotNull() ? _checklist.AntesEntradaPaciente.Pulseira.Equals(Pulseira.MID).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p1MIE.CheckState = _checklist.AntesEntradaPaciente.Pulseira.IsNotNull() ? _checklist.AntesEntradaPaciente.Pulseira.Equals(Pulseira.MIE).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p1MSD.CheckState = _checklist.AntesEntradaPaciente.Pulseira.IsNotNull() ? _checklist.AntesEntradaPaciente.Pulseira.Equals(Pulseira.MSD).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p1MSE.CheckState = _checklist.AntesEntradaPaciente.Pulseira.IsNotNull() ? _checklist.AntesEntradaPaciente.Pulseira.Equals(Pulseira.MSE).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p1SitioCirurgicoMarcadoNA.CheckState = _checklist.AntesEntradaPaciente.SitioNA.IsNotNull() ? _checklist.AntesEntradaPaciente.SitioNA.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p1Local.Text = _checklist.AntesEntradaPaciente.Local;

            rpt.p1LocalDireito.CheckState = _checklist.AntesEntradaPaciente.Direito.IsNotNull() ? _checklist.AntesEntradaPaciente.Direito.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1LocalEsquerdo.CheckState = _checklist.AntesEntradaPaciente.Esquerdo.IsNotNull() ? _checklist.AntesEntradaPaciente.Esquerdo.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1LocalNA.CheckState = _checklist.AntesEntradaPaciente.NA.IsNotNull() ? _checklist.AntesEntradaPaciente.NA.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1LocalNivelObservacao.Text = _checklist.AntesEntradaPaciente.Nivel;

            rpt.p1ConsentimentoAnestesiaCompleto.CheckState = _checklist.AntesEntradaPaciente.ConsentimentoAnestesia.IsNotNull() ? _checklist.AntesEntradaPaciente.ConsentimentoAnestesia.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1ConsentimentoAnestesiaCompletoObs.Text = _checklist.AntesEntradaPaciente.ConsentimentoAnestesiaObservacao;

            rpt.p1AvaliacaoPreAnestesica.CheckState = _checklist.AntesEntradaPaciente.AvaliacaoPreAnestesica.IsNotNull() ? _checklist.AntesEntradaPaciente.AvaliacaoPreAnestesica.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1AvaliacaoPreAnestesicaObs.Text = _checklist.AntesEntradaPaciente.AvaliacaoPreAnestesicaObservacao;

            rpt.p1ConsentimentoCirurgicoCompleto.CheckState = _checklist.AntesEntradaPaciente.ConsentimentoCirurgico.IsNotNull() ? _checklist.AntesEntradaPaciente.ConsentimentoCirurgico.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1ConsentimentoCirurgicoCompletoObs.Text = _checklist.AntesEntradaPaciente.ConsentimentoCirurgicoObservacao;

            rpt.p1SumarioDeAvaliacaoMedica.CheckState = _checklist.AntesEntradaPaciente.SumarioAvaliacaoMedica.IsNotNull() ? _checklist.AntesEntradaPaciente.SumarioAvaliacaoMedica.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p1SumarioDeAvaliacaoMedicaObs.Text = _checklist.AntesEntradaPaciente.SumarioAvaliacaoMedicaObservacao;


            rpt.p1Data.Text = _checklist.AntesEntradaPaciente.Data.ToString("dd/MM/yyyy");
            rpt.p1Hora.Text = _checklist.AntesEntradaPaciente.Data.ToString("HH:mm");

            #endregion

            #region ANTES DA INDUÇÃO ANESTÉSICA
            rpt.p2NomeDoPaciente.CheckState = _checklist.AntesInducaoAnestesica.Nome.IsNotNull() ? _checklist.AntesInducaoAnestesica.Nome.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p2Procedimento.CheckState = _checklist.AntesInducaoAnestesica.Procedimento.IsNotNull() ? _checklist.AntesInducaoAnestesica.Procedimento.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p2LadoAbordado.CheckState = _checklist.AntesInducaoAnestesica.LadoAbordado.IsNotNull() ? _checklist.AntesInducaoAnestesica.LadoAbordado.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p2DificuldadesViaAereaSim.CheckState = _checklist.AntesInducaoAnestesica.ViaAerea.IsNotNull() ? _checklist.AntesInducaoAnestesica.ViaAerea.Equals(SimNaoNA.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2DificuldadesViaAereaNao.CheckState = _checklist.AntesInducaoAnestesica.ViaAerea.IsNotNull() ? _checklist.AntesInducaoAnestesica.ViaAerea.Equals(SimNaoNA.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2DificuldadesViaAereaNA.CheckState = _checklist.AntesInducaoAnestesica.ViaAerea.IsNotNull() ? _checklist.AntesInducaoAnestesica.ViaAerea.Equals(SimNaoNA.NA).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2DificuldadesViaAereaSimObs.Text = _checklist.AntesInducaoAnestesica.EquipamentosDisponiveis;

            rpt.p2RiscoPerdaSanguineaSim.CheckState = _checklist.AntesInducaoAnestesica.RiscoPerdaSangue.IsNotNull() ? _checklist.AntesInducaoAnestesica.RiscoPerdaSangue.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2RiscoPerdaSanguineaNao.CheckState = _checklist.AntesInducaoAnestesica.RiscoPerdaSangue.IsNotNull() ? _checklist.AntesInducaoAnestesica.RiscoPerdaSangue.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            //rpt.p2RiscoPerdaSanguineaNA.CheckState = _checklist.AntesInducaoAnestesica.RiscoPerdaSangue.IsNotNull() ? _checklist.AntesInducaoAnestesica.RiscoPerdaSangue.Equals(SimNaoNA.NA).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p2ExamesImagensSim.CheckState = _checklist.AntesInducaoAnestesica.ExamesImagens.IsNotNull() ? _checklist.AntesInducaoAnestesica.ExamesImagens.Equals(SimNaoNA.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2ExamesImagensNao.CheckState = _checklist.AntesInducaoAnestesica.ExamesImagens.IsNotNull() ? _checklist.AntesInducaoAnestesica.ExamesImagens.Equals(SimNaoNA.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2ExamesImagensNA.CheckState = _checklist.AntesInducaoAnestesica.ExamesImagens.IsNotNull() ? _checklist.AntesInducaoAnestesica.ExamesImagens.Equals(SimNaoNA.NA).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2EquipamentosEspecificosSim.CheckState = _checklist.AntesInducaoAnestesica.OPME.IsNotNull() ? _checklist.AntesInducaoAnestesica.OPME.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p2EquipamentosEspecificosNao.CheckState = _checklist.AntesInducaoAnestesica.OPME.IsNotNull() ? _checklist.AntesInducaoAnestesica.OPME.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            #endregion

            #region TIME OUT
            rpt.p3ReconfirmacaoSitioNA.CheckState = _checklist.TimeOut.SitioCirurgico.IsNotNull() ? _checklist.TimeOut.SitioCirurgico.Equals(SimNA.NA).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p3ReconfirmacaoSitioSim.CheckState = _checklist.TimeOut.SitioCirurgico.IsNotNull() ? _checklist.TimeOut.SitioCirurgico.Equals(SimNA.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            // não está mais sendo usado. 
            //rpt.p3ReconfirmacaoSitioNao.CheckState = _checklist.TimeOut.SitioCirurgico.IsNotNull() ? _checklist.TimeOut.SitioCirurgico.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p3AntimicrobianoSim.CheckState = _checklist.TimeOut.Antimicrobiano.IsNotNull() ? _checklist.TimeOut.Antimicrobiano.Equals(SimNA.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p3AntimicrobianoNA.CheckState = _checklist.TimeOut.Antimicrobiano.IsNotNull() ? _checklist.TimeOut.Antimicrobiano.Equals(SimNA.NA).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p3AntibioticoDose.Text = _checklist.TimeOut.AntimicrobianoObservacao;

            rpt.p3ConfirmarCirculanteTodosEquip.CheckState = _checklist.TimeOut.EquipamentosConformePlanejamento.IsNotNull() ? _checklist.TimeOut.EquipamentosConformePlanejamento.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p3ConfirmarInstrumentadorEsterilizacao.CheckState = _checklist.TimeOut.EsterilizacaoInstrumental.IsNotNull() ? _checklist.TimeOut.EsterilizacaoInstrumental.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p3ConfirmarInstrumentadorIndicadores.CheckState = _checklist.TimeOut.IndicadoresEsterilidade.IsNotNull() ? _checklist.TimeOut.IndicadoresEsterilidade.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p3ConfirmarInstrumentadorOPME.CheckState = _checklist.TimeOut.OPME.IsNotNull() ? _checklist.TimeOut.OPME.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p3ConfirmarInstrumentadorNA.CheckState = _checklist.TimeOut.InstrumentadorNA.IsNotNull() ? _checklist.TimeOut.InstrumentadorNA.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p3ConfirmarCirurgiaoEtapasCriticas.CheckState = _checklist.TimeOut.CirurgiaoEtapa.ConvertSimNaoToCheckstate();
            rpt.p3ConfirmarCirurgiaoDuvidasEquipe.CheckState = _checklist.TimeOut.CirurgiaoEquipe.ConvertSimNaoToCheckstate();
            rpt.p3ConfirmarCirurgiaoObs.Text = _checklist.TimeOut.CirurgiaoObservacao;
            #endregion

            #region CHECK OUT
            rpt.p4PrescricaoRealizada.CheckState = _checklist.CheckOut.Prescricao.IsNotNull() ? _checklist.CheckOut.Prescricao.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4DescricaoCirurgica.CheckState = _checklist.CheckOut.DescricaoCirurgica.IsNotNull() ? _checklist.CheckOut.DescricaoCirurgica.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4Justificativas.CheckState = _checklist.CheckOut.Justificativa.IsNotNull() ? _checklist.CheckOut.Justificativa.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ContagemDeInstrumentais.CheckState = _checklist.CheckOut.InstrumentosContagem.IsNotNull() ? _checklist.CheckOut.InstrumentosContagem.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ContagemDeInstrumentaisObs.Text = _checklist.CheckOut.InstrumentosObservacao;
            rpt.p4SumarioAlta.CheckState = _checklist.CheckOut.SumarioAltaAmbulatorial.IsNotNull() ? _checklist.CheckOut.SumarioAltaAmbulatorial.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p4DebitosLancados.CheckState = _checklist.CheckOut.Debitos.IsNotNull() ? _checklist.CheckOut.Debitos.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4EquipamentosFuncionaram.CheckState = _checklist.CheckOut.Equipamento.IsNotNull() ? _checklist.CheckOut.Equipamento.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p4Equipamento.Text = _checklist.CheckOut.EquipamentoDescricao;

            rpt.p4Solucionado.CheckState = _checklist.CheckOut.Solucionado.IsNotNull() ? _checklist.CheckOut.Solucionado.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4Trocado.CheckState = _checklist.CheckOut.Trocado.IsNotNull() ? _checklist.CheckOut.Trocado.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4Identificacao.CheckState = _checklist.CheckOut.Patalogico.IsNotNull() && _checklist.CheckOut.Patalogico.Value == SimNA.Sim ? _checklist.CheckOut.Patalogico.ConvertSimNAToCheckstate() : CheckState.Unchecked;
            rpt.p4IdentificacaoNA.CheckState = rpt.p4Identificacao.CheckState == CheckState.Unchecked ? CheckState.Checked : CheckState.Unchecked;
            rpt.p4NumeroPecas.Text = _checklist.CheckOut.Pecas;

            rpt.p4Oximetria.CheckState = _checklist.CheckOut.Oximetria.IsNotNull() ? _checklist.CheckOut.Oximetria.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            //rpt.P4AnestesiaLocal.CheckState = _checklist.CheckOut.AnestesiaLocal.IsNotNull() ? _checklist.CheckOut.AnestesiaLocal.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4OximetriaObs.Text = _checklist.CheckOut.EquipamentoObservacao;

            rpt.p4ExamesLAB.CheckState = _checklist.CheckOut.LAB.IsNotNull() ? _checklist.CheckOut.LAB.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesLABObs.Text = _checklist.CheckOut.LABObservacao;
            rpt.p4ExamesECG.CheckState = _checklist.CheckOut.ECG.IsNotNull() ? _checklist.CheckOut.ECG.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesECGObs.Text = _checklist.CheckOut.ECGObservacao;
            rpt.p4ExamesECO.CheckState = _checklist.CheckOut.ECO.IsNotNull() ? _checklist.CheckOut.ECO.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesECOObs.Text = _checklist.CheckOut.ECOObservacao;
            rpt.p4ExamesRX.CheckState = _checklist.CheckOut.RX.IsNotNull() ? _checklist.CheckOut.RX.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesRXObs.Text = _checklist.CheckOut.RXObservacao;
            rpt.p4ExamesRNM.CheckState = _checklist.CheckOut.RNM.IsNotNull() ? _checklist.CheckOut.RNM.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesRNMObs.Text = _checklist.CheckOut.RNMObservacao;
            rpt.p4ExamesCDCirurgia.CheckState = _checklist.CheckOut.CDCirurgia.IsNotNull() ? _checklist.CheckOut.CDCirurgia.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesCDCirurgiaObs.Text = _checklist.CheckOut.CDCirurgiaObservacao;
            rpt.p4ExamesTC.CheckState = _checklist.CheckOut.TC.IsNotNull() ? _checklist.CheckOut.TC.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesTCObs.Text = _checklist.CheckOut.TCObservacao;
            rpt.p4ExamesOutros.CheckState = _checklist.CheckOut.Outros.IsNotNull() ? _checklist.CheckOut.Outros.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p4ExamesOutrosObs.Text = _checklist.CheckOut.OutrosObservacao;
            rpt.p4ExamesObs.Text = _checklist.CheckOut.Observacoes;

            #endregion

            #region ANOTACOES TRANS OPERATORIO

            rpt.p5Data.Text = _checklist.TransOperatorio.Data.ToString("dd/MM/yyyy");
            rpt.p5HoraDeEntradaEmSala.Text = _checklist.TransOperatorio.Data.ToString("HH:mm");
            rpt.p5Sala.Text = _checklist.TransOperatorio.Sala;

            rpt.p5NivelDeConscienciaLucido.CheckState = _checklist.TransOperatorio.NivelDeConsciencia.Lucido.IsNotNull() ? _checklist.TransOperatorio.NivelDeConsciencia.Lucido.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5NivelDeConscienciaSonolento.CheckState = _checklist.TransOperatorio.NivelDeConsciencia.Sonolento.IsNotNull() ? _checklist.TransOperatorio.NivelDeConsciencia.Sonolento.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5NivelDeConscienciaEntubado.CheckState = _checklist.TransOperatorio.NivelDeConsciencia.Entubado.IsNotNull() ? _checklist.TransOperatorio.NivelDeConsciencia.Entubado.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5NivelDeConscienciaOutros.CheckState = _checklist.TransOperatorio.NivelDeConsciencia.Outros.IsNotNull() ? _checklist.TransOperatorio.NivelDeConsciencia.Outros.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5NivelDeConscienciaOutrosObs.Text = _checklist.TransOperatorio.NivelDeConsciencia.OutrosObservacao;

            rpt.p5EntradaEmSalaCirurgicaMaca.CheckState = _checklist.TransOperatorio.EntradaSalaCirurgica.Maca.IsNotNull() ? _checklist.TransOperatorio.EntradaSalaCirurgica.Maca.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5EntradaEmSalaCirurgicaCama.CheckState = _checklist.TransOperatorio.EntradaSalaCirurgica.Cama.IsNotNull() ? _checklist.TransOperatorio.EntradaSalaCirurgica.Cama.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5EntradaEmSalaCirurgicaDeambulando.CheckState = _checklist.TransOperatorio.EntradaSalaCirurgica.Deambulando.IsNotNull() ? _checklist.TransOperatorio.EntradaSalaCirurgica.Deambulando.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5EntradaEmSalaCirurgicaOutros.CheckState = _checklist.TransOperatorio.EntradaSalaCirurgica.Outros.IsNotNull() ? _checklist.TransOperatorio.EntradaSalaCirurgica.Outros.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5EntradaEmSalaCirurgicaOutrosObs.Text = _checklist.TransOperatorio.EntradaSalaCirurgica.OutrosObservacao;

            rpt.p5AcessoVenosoPerifericoNA.CheckState = _checklist.TransOperatorio.AcessoVenosoPeriferico.NA.IsNotNull() ? _checklist.TransOperatorio.AcessoVenosoPeriferico.NA.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoVenosoPerifericoMSD.CheckState = _checklist.TransOperatorio.AcessoVenosoPeriferico.MSD.IsNotNull() ? _checklist.TransOperatorio.AcessoVenosoPeriferico.MSD.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoVenosoPerifericoMSE.CheckState = _checklist.TransOperatorio.AcessoVenosoPeriferico.MSE.IsNotNull() ? _checklist.TransOperatorio.AcessoVenosoPeriferico.MSE.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoVenosoPerifericoMI.CheckState = _checklist.TransOperatorio.AcessoVenosoPeriferico.MI.IsNotNull() ? _checklist.TransOperatorio.AcessoVenosoPeriferico.MI.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoVenosoPerifericoInsyte.Text = _checklist.TransOperatorio.AcessoVenosoPeriferico.Insyte;

            rpt.p5AcessoCentralNA.CheckState = _checklist.TransOperatorio.AcessoCentral.NA.IsNotNull() ? _checklist.TransOperatorio.AcessoCentral.NA.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoCentralMonolumen.CheckState = _checklist.TransOperatorio.AcessoCentral.Monolumen.IsNotNull() ? _checklist.TransOperatorio.AcessoCentral.Monolumen.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoCentralDuplolumen.CheckState = _checklist.TransOperatorio.AcessoCentral.Duplolumen.IsNotNull() ? _checklist.TransOperatorio.AcessoCentral.Duplolumen.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoCentralPIC.CheckState = _checklist.TransOperatorio.AcessoCentral.PIC.IsNotNull() ? _checklist.TransOperatorio.AcessoCentral.PIC.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AcessoCentralLocal.Text = _checklist.TransOperatorio.AcessoCentral.Local;

            rpt.p5PAMNao.CheckState = _checklist.TransOperatorio.PAM.Pam.IsNotNull() ? _checklist.TransOperatorio.PAM.Pam.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5PAMSim.CheckState = _checklist.TransOperatorio.PAM.Pam.IsNotNull() ? _checklist.TransOperatorio.PAM.Pam.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5PAMLocal.Text = _checklist.TransOperatorio.PAM.Local;

            rpt.p5AnestesiaGeral.CheckState = _checklist.TransOperatorio.Anestesia.Geral.IsNotNull() ? _checklist.TransOperatorio.Anestesia.Geral.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaTupoEndotraqueal.CheckState = _checklist.TransOperatorio.Anestesia.TuboEndotraqueal.IsNotNull() ? _checklist.TransOperatorio.Anestesia.TuboEndotraqueal.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaTuboEndotraquealObs.Text = _checklist.TransOperatorio.Anestesia.TuboEndotraquealNumero;
            rpt.p5AnestesiaMascaraLaringea.CheckState = _checklist.TransOperatorio.Anestesia.MascaraLaringea.IsNotNull() ? _checklist.TransOperatorio.Anestesia.MascaraLaringea.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaMascaraLaringeaObs.Text = _checklist.TransOperatorio.Anestesia.MascaraLaringeaNumero;
            rpt.p5AnestesiaBloqPeridural.CheckState = _checklist.TransOperatorio.Anestesia.BloqueioPeridural.IsNotNull() ? _checklist.TransOperatorio.Anestesia.BloqueioPeridural.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaRaqui.CheckState = _checklist.TransOperatorio.Anestesia.Raqui.IsNotNull() ? _checklist.TransOperatorio.Anestesia.Raqui.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaSedacao.CheckState = _checklist.TransOperatorio.Anestesia.Sedacao.IsNotNull() ? _checklist.TransOperatorio.Anestesia.Sedacao.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaLocal.CheckState = _checklist.TransOperatorio.Anestesia.Local.IsNotNull() ? _checklist.TransOperatorio.Anestesia.Local.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaOutros.CheckState = _checklist.TransOperatorio.Anestesia.Outros.IsNotNull() ? _checklist.TransOperatorio.Anestesia.Outros.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AnestesiaOutrosObs.Text = _checklist.TransOperatorio.Anestesia.OutrosObservacao;
            rpt.p5AnestesiaSondagem.CheckState = _checklist.TransOperatorio.Anestesia.Sondagem.IsNotNull() ? _checklist.TransOperatorio.Anestesia.Sondagem.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p5PosicaoCirurgicaTipo.Text = _checklist.TransOperatorio.PosicaoCirurgicaTipo;

            rpt.p5CoxinsDeProtecaoNao.CheckState = _checklist.TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.IsNotNull() ? _checklist.TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5CoxinsDeProtecaoSim.CheckState = _checklist.TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.IsNotNull() ? _checklist.TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5CoxinsDeProtecaoLocal.Text = _checklist.TransOperatorio.CoxinsDeProtecao.Local;

            rpt.p5TricotomiaNao.CheckState = _checklist.TransOperatorio.Tricotomia.IsNotNull() ? _checklist.TransOperatorio.Tricotomia.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5TricotomiaSim.CheckState = _checklist.TransOperatorio.Tricotomia.IsNotNull() ? _checklist.TransOperatorio.Tricotomia.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p5DegermacaoNao.CheckState = _checklist.TransOperatorio.Degermacao.IsNotNull() ? _checklist.TransOperatorio.Degermacao.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5DegermacaoSim.CheckState = _checklist.TransOperatorio.Degermacao.IsNotNull() ? _checklist.TransOperatorio.Degermacao.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5DegermacaoCloro.CheckState = _checklist.TransOperatorio.Clorohexidine.IsNotNull() ? _checklist.TransOperatorio.Clorohexidine.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5DegermacaoIodo.CheckState = _checklist.TransOperatorio.Iododegermante.IsNotNull() ? _checklist.TransOperatorio.Iododegermante.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p5AssepsiaIodoTopico.CheckState = _checklist.TransOperatorio.Assepsia.IodoTopico.IsNotNull() ? _checklist.TransOperatorio.Assepsia.IodoTopico.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AssepsiaIodoAlcoolico.CheckState = _checklist.TransOperatorio.Assepsia.IodoAlcoolico.IsNotNull() ? _checklist.TransOperatorio.Assepsia.IodoAlcoolico.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AssepsiaSoroFisiologico.CheckState = _checklist.TransOperatorio.Assepsia.SoroFisiologico.IsNotNull() ? _checklist.TransOperatorio.Assepsia.SoroFisiologico.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AssepsiaClorohexidrineDegermante.CheckState = _checklist.TransOperatorio.Assepsia.CloroDegermante.IsNotNull() ? _checklist.TransOperatorio.Assepsia.CloroDegermante.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AssepsiaClorohexidrineAlcoolico.CheckState = _checklist.TransOperatorio.Assepsia.CloroAlcoolico.IsNotNull() ? _checklist.TransOperatorio.Assepsia.CloroAlcoolico.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AssepsiaAlcool.CheckState = _checklist.TransOperatorio.Assepsia.Alcool70.IsNotNull() ? _checklist.TransOperatorio.Assepsia.Alcool70.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5AssepsiaClorohexidrineTopico.CheckState = _checklist.TransOperatorio.Assepsia.CloroTopico.IsNotNull() ? _checklist.TransOperatorio.Assepsia.CloroTopico.ConvertSimNaoToCheckstate() : CheckState.Unchecked;

            rpt.p5SondagemVesicalDeDemoraNao.CheckState = _checklist.Sondagem.VesicalDemora.IsNotNull() ? _checklist.Sondagem.VesicalDemora.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5SondagemVesicalDeDemoraSim.CheckState = _checklist.Sondagem.VesicalDemora.IsNotNull() ? _checklist.Sondagem.VesicalDemora.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5SondagemVesicalDeDemoraFoleyNro.Text = _checklist.Sondagem.FoleyNumero;
            rpt.p5SondagemVesicalDeDemoraVolumeSala.Text = _checklist.Sondagem.VolumeSala;
            rpt.p5SondagemVesicalDeDemoraVolumeBalonete.CheckState = _checklist.Sondagem.Balonete.IsNotNull() ? _checklist.Sondagem.Balonete.ConvertSimNaoToCheckstate() : CheckState.Unchecked;
            rpt.p5SondagemVesicalDeDemoraVolumeBaloneteObs.Text = _checklist.Sondagem.VolumeBalonete;
            rpt.p5SondagemVesicalDeDemoraAspectoDiurese.Text = _checklist.Sondagem.AspectoDiurese;

            rpt.p5SondagemVesicalDeDemoraEnfa.Text = "Enfª: ";
            if (_checklist.Sondagem.Usuario != null)
                rpt.p5SondagemVesicalDeDemoraEnfa.Text += _checklist.Sondagem.Usuario.Nome;

            if (_checklist.Sondagem.VesicalDemoraCirurgiao == SimNao.Sim)
                rpt.p5SondagemVesicalDeDemoraEnfa.Text = "PROCEDIMENTO REALIZADO PELO CIRURGIÃO";

            rpt.p5SondagemVesicalDeAlivioNao.CheckState = _checklist.Sondagem.VesicalAlivio.IsNotNull() ? _checklist.Sondagem.VesicalAlivio.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5SondagemVesicalDeAlivioSim.CheckState = _checklist.Sondagem.VesicalAlivio.IsNotNull() ? _checklist.Sondagem.VesicalAlivio.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5SondagemVesicalDeAlivioVolume.Text = _checklist.Sondagem.VesicalAlivioVolume;

            rpt.p5PlacaDeEletrocauterioNao.CheckState = _checklist.TransOperatorio.PlacaEletrocauterio.IsNotNull() ? _checklist.TransOperatorio.PlacaEletrocauterio.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5PlacaDeEletrocauterioSim.CheckState = _checklist.TransOperatorio.PlacaEletrocauterio.IsNotNull() ? _checklist.TransOperatorio.PlacaEletrocauterio.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p5ColchaoTermicoNao.CheckState = _checklist.TransOperatorio.ColchaoTermico.IsNotNull() ? _checklist.TransOperatorio.ColchaoTermico.Colchao.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5ColchaoTermicoSim.CheckState = _checklist.TransOperatorio.ColchaoTermico.IsNotNull() ? _checklist.TransOperatorio.ColchaoTermico.Colchao.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5ColchaoTermicoTemperatura.Text = _checklist.TransOperatorio.ColchaoTermico.IsNotNull() ? _checklist.TransOperatorio.ColchaoTermico.Temperatura : string.Empty;

            rpt.p5MantaTermicaNao.CheckState = _checklist.TransOperatorio.MantaTermica.IsNotNull() ? _checklist.TransOperatorio.MantaTermica.Manta.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5MantaTermicaSim.CheckState = _checklist.TransOperatorio.MantaTermica.IsNotNull() ? _checklist.TransOperatorio.MantaTermica.Manta.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5MantaTermicaTemperatura.Text = _checklist.TransOperatorio.MantaTermica.Temperatura;

            rpt.p5BotasDeRetornoVenosoNao.CheckState = _checklist.TransOperatorio.BotaRetornoVenoso.IsNotNull() ? _checklist.TransOperatorio.BotaRetornoVenoso.Bota.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5BotasDeRetornoVenosoSim.CheckState = _checklist.TransOperatorio.BotaRetornoVenoso.IsNotNull() ? _checklist.TransOperatorio.BotaRetornoVenoso.Bota.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5BotasDeRetornoVenosoTipo.Text = _checklist.TransOperatorio.BotaRetornoVenoso.Tipo;

            rpt.p5MeiasElasticasNao.CheckState = _checklist.TransOperatorio.MeiasElasticas.IsNotNull() ? _checklist.TransOperatorio.MeiasElasticas.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5MeiasElasticasSim.CheckState = _checklist.TransOperatorio.MeiasElasticas.IsNotNull() ? _checklist.TransOperatorio.MeiasElasticas.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;

            rpt.p5DrenoNao.CheckState = _checklist.TransOperatorio.Drenos.IsNotNull() ? _checklist.TransOperatorio.Drenos.Dreno.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5DrenoSim.CheckState = _checklist.TransOperatorio.Drenos.IsNotNull() ? _checklist.TransOperatorio.Drenos.Dreno.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5DrenoTipo.Text = _checklist.TransOperatorio.Drenos.Tipo;

            rpt.p5TransfusaoSanguineaNao.CheckState = _checklist.TransOperatorio.TransfusaoSanguinea.IsNotNull() ? _checklist.TransOperatorio.TransfusaoSanguinea.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5TransfusaoSanguineaSim.CheckState = _checklist.TransOperatorio.TransfusaoSanguinea.IsNotNull() ? _checklist.TransOperatorio.TransfusaoSanguinea.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5TransfusaoSanguineaObs.Text = _checklist.TransOperatorio.TransfusaoSanguineaObservacao;

            rpt.p5GarrotePneumaticoNao.CheckState = _checklist.TransOperatorio.GarrotePneumatico.IsNotNull() ? _checklist.TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.Equals(SimNao.Nao).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5GarrotePneumaticoSim.CheckState = _checklist.TransOperatorio.GarrotePneumatico.IsNotNull() ? _checklist.TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.Equals(SimNao.Sim).ConvertBooleanToCheckstate() : CheckState.Unchecked;
            rpt.p5GarrotePneumaticoTempo.Text = _checklist.TransOperatorio.GarrotePneumatico.Tempo;

            rpt.p5DestinoDoPaciente.Text = _checklist.TransOperatorio.DestinoPaciente;

            #endregion

            rpt.pMembrosRegistro.Text = string.Empty;
            rpt.p3MembrosNome.Text = string.Empty;

            if (_checklist.TimeOut.Cirurgiao.IsNotNull() && !string.IsNullOrWhiteSpace(_checklist.TimeOut.Cirurgiao.Nome))
            {
                rpt.pMembrosRegistro.Text = _checklist.TimeOut.Cirurgiao.Registro.Combine(Environment.NewLine);
                rpt.p3MembrosNome.Text = _checklist.TimeOut.Cirurgiao.Nome.Length > 25 ? _checklist.TimeOut.Cirurgiao.Nome.Substring(0, 25).Combine(Environment.NewLine) : _checklist.TimeOut.Cirurgiao.Nome.Combine(Environment.NewLine);
            }

            if (_checklist.TimeOut.Auxiliar1.IsNotNull() && !string.IsNullOrWhiteSpace(_checklist.TimeOut.Auxiliar1.Nome))
            {
                rpt.pMembrosRegistro.Text += _checklist.TimeOut.Auxiliar1.Registro.Combine(Environment.NewLine);
                rpt.p3MembrosNome.Text += _checklist.TimeOut.Auxiliar1.Nome.Length > 25 ? _checklist.TimeOut.Auxiliar1.Nome.Substring(0, 25).Combine(Environment.NewLine) : _checklist.TimeOut.Auxiliar1.Nome.Combine(Environment.NewLine);
            }

            if (_checklist.TimeOut.Auxiliar2.IsNotNull() && !string.IsNullOrWhiteSpace(_checklist.TimeOut.Auxiliar2.Nome))
            {
                rpt.pMembrosRegistro.Text += _checklist.TimeOut.Auxiliar2.Registro.Combine(Environment.NewLine);
                rpt.p3MembrosNome.Text += _checklist.TimeOut.Auxiliar2.Nome.Length > 25 ? _checklist.TimeOut.Auxiliar2.Nome.Substring(0, 25).Combine(Environment.NewLine) : _checklist.TimeOut.Auxiliar2.Nome.Combine(Environment.NewLine);
            }

            if (_checklist.TimeOut.Instrumentador.IsNotNull() && !string.IsNullOrWhiteSpace(_checklist.TimeOut.Instrumentador.Nome))
            {
                rpt.pMembrosRegistro.Text += _checklist.TimeOut.Instrumentador.Registro.Combine(Environment.NewLine);
                rpt.p3MembrosNome.Text += _checklist.TimeOut.Instrumentador.Nome.Length > 25 ? _checklist.TimeOut.Instrumentador.Nome.Substring(0, 25).Combine(Environment.NewLine) : _checklist.TimeOut.Instrumentador.Nome.Combine(Environment.NewLine);
            }

            if (_checklist.TimeOut.Circulante.IsNotNull() && !string.IsNullOrWhiteSpace(_checklist.TimeOut.Circulante.Nome))
            {
                rpt.pMembrosRegistro.Text += _checklist.TimeOut.Circulante.Registro.Combine(Environment.NewLine);
                rpt.p3MembrosNome.Text += _checklist.TimeOut.Circulante.Nome.Length > 25 ? _checklist.TimeOut.Circulante.Nome.Substring(0, 25).Combine(Environment.NewLine) : _checklist.TimeOut.Circulante.Nome.Combine(Environment.NewLine);
            }
            if (_checklist.TimeOut.Anestesista.IsNotNull() && !string.IsNullOrWhiteSpace(_checklist.TimeOut.Anestesista.Nome))
            {
                rpt.pMembrosRegistro.Text += _checklist.TimeOut.Anestesista.Registro.Combine(Environment.NewLine);
                rpt.p3MembrosNome.Text += _checklist.TimeOut.Anestesista.Nome.Length > 25 ? _checklist.TimeOut.Anestesista.Nome.Substring(0, 25).Combine(Environment.NewLine) : _checklist.TimeOut.Anestesista.Nome.Combine(Environment.NewLine);
            }

            #region Assinaturas
            rpt.xrAssinaturaAntesEntrada.Text = _checklist.AntesEntradaPaciente.Usuario.AssinaturaNaLinhaSemColchetes;
            rpt.xrAssinaturaAntesInducao.Text = _checklist.AntesInducaoAnestesica.Usuario.AssinaturaNaLinhaSemColchetes;
            rpt.xrAssinaturaCheckOut.Text = _checklist.CheckOut.Usuario.AssinaturaNaLinhaSemColchetes;
            rpt.xrAssinaturaTimeOut.Text = _checklist.TimeOut.Usuario.AssinaturaNaLinhaSemColchetes;
            #endregion

            rpt.xrAssinatura.Text = _checklist.UsuarioEncerramento.IsNotNull() ? _checklist.UsuarioEncerramento.Assinatura : this._checklist.Usuario.Assinatura;

            byte[] signature = _checklist.TransOperatorio.CorpoHumanoImg;
            if (signature.IsNotNull())
            {
                MyInkCanvas ink = XamlReader.Load(new XmlTextReader(new StringReader(signature.GetString().Replace("<x:Null />", "")))) as MyInkCanvas;

                Transform transform = ink.LayoutTransform;
                ink.LayoutTransform = null;

                System.Windows.Size size = new System.Windows.Size(ink.Width, ink.Height);
                ink.Measure(size);
                ink.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap =
                  new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32);
                renderBitmap.Render(ink);

                System.Drawing.Image imagem;

                using (MemoryStream outStream = new MemoryStream())
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(outStream);
                    imagem = System.Drawing.Image.FromStream(outStream);
                }

                rpt.p4ImagemCorpo.Image = imagem;
            }

            return rpt;
        }
    }
}
