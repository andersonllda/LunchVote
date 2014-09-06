using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Expression;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.WPF.Report.CheckListUDI;
using System.Windows.Forms;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Constant;

namespace HMV.PEP.WPF.UserControls.CheckListUDI
{
    public class vmRelatorioChecklistUDI
    {
        private wrpCheckListUDI _checklist;

        public vmRelatorioChecklistUDI(wrpCheckListUDI pChecklist)
        {
            _checklist = pChecklist;
        }

        public rptCheckListUDI Relatorio()
        {
            rptCheckListUDI rpt = new rptCheckListUDI();

            #region Dados do Rodapé

            if (_checklist.Paciente.IsNotNull())
            {
                rpt.pNomePaciente.Text = _checklist.Paciente.Nome;
                rpt.pRegistro.Text = _checklist.Paciente.ID.ToString();
            }
            rpt.pProcedimentoProposto.Text = _checklist.Cirurgia.ds_cirurgia;
            rpt.pCirurgiao.Text = _checklist.Prestador.IsNotNull() ? _checklist.Prestador.NomeExibicao : string.Empty;
            rpt.pAvisoCirurgia.Text = _checklist.AvisoCirurgia.IsNotNull() ? _checklist.AvisoCirurgia.cd_aviso_cirurgia.ToString() : string.Empty;
            rpt.pData.Text = _checklist.Data.ToShortDateString();


            rpt.lbAssinatura1.Text = _checklist.Usuario.AssinaturaPadrao(true,false);
            rpt.lbDatahora1.Text = DateTime.Now.ToString();
            rpt.lbAssinatura2.Text = _checklist.Usuario.AssinaturaPadrao(true, false);
            rpt.lbDatahora2.Text = DateTime.Now.ToString();
            rpt.lbAssinatura3.Text = _checklist.Usuario.AssinaturaPadrao(true, false);
            rpt.lbDatahora3.Text = DateTime.Now.ToString();
            rpt.lbAssinatura4.Text = _checklist.Usuario.AssinaturaPadrao(true, false);
            rpt.lbDatahora4.Text = DateTime.Now.ToString();
            #endregion

            #region Antes da Entrada do Paciente na Sala de Procedimento

            if (_checklist.AntesEntradaUDI.Responsavel.IsEmpty() && _checklist.AntesEntradaUDI.Prontuario.IsEmpty())
            {
                rpt.lbConfirmadoPor.Text = "Paciente confirmou:";
                rpt.pConfirmadoPor.Text = _checklist.Paciente.Nome;
            }
            else if (_checklist.AntesEntradaUDI.Responsavel.IsNotEmpty())
            {
                rpt.lbConfirmadoPor.Text = "Responsável confirmou:";
                rpt.pConfirmadoPor.Text = _checklist.AntesEntradaUDI.Responsavel;
            }
            else if (_checklist.AntesEntradaUDI.Prontuario.IsNotEmpty())
            {
                rpt.lbConfirmadoPor.Text = "Confirmação pelo prontuário:";
                rpt.pConfirmadoPor.Text = _checklist.AntesEntradaUDI.Prontuario;
            }

            rpt.chNomeCompleto.CheckState = _checklist.AntesEntradaUDI.Paciente == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chProcedimento.CheckState = _checklist.AntesEntradaUDI.Paciente == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;

            //rpt.chNAProcedimento.CheckState = _checklist.AntesEntradaUDI.ProcedimentoNA == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;
            
            rpt.pRegiao.Text = _checklist.AntesEntradaUDI.Regiao;

            rpt.chDireito.CheckState = _checklist.AntesEntradaUDI.Lado == Posicao.Direito ? CheckState.Checked : CheckState.Unchecked;
            rpt.chEsquerdo.CheckState = _checklist.AntesEntradaUDI.Lado == Posicao.Esquerdo ? CheckState.Checked : CheckState.Unchecked;
            rpt.chAmbos.CheckState = _checklist.AntesEntradaUDI.Lado == Posicao.Ambos ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNALado.CheckState = _checklist.AntesEntradaUDI.Lado == Posicao.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chNPO.CheckState = _checklist.AntesEntradaUDI.NPO == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNPONA.CheckState = _checklist.AntesEntradaUDI.NPO == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chExamesAnteriores.CheckState = _checklist.AntesEntradaUDI.ExamesAnteriores == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNAExamesAnteriores.CheckState = _checklist.AntesEntradaUDI.ExamesAnteriores == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chPedidoExame.CheckState = _checklist.AntesEntradaUDI.PedidoExame == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;

            rpt.chSemAlergias.Visible = false;
            wrpAlergiaEventoCollection _AlergiaCollection = null;
            IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
            repa.OndeChaveIgual(_checklist.ID);
            repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.CheckListUDI);
            var reta = repa.List();
            if (reta.IsNotNull())
                _AlergiaCollection = new wrpAlergiaEventoCollection(reta);
            foreach (var item in _AlergiaCollection)
            {
                if (item.Alergia.Status == StatusAlergiaProblema.Ativo)
                {
                    if (item.Alergia.Agente == Constantes.coSemAlergiasConhecidas)
                    {
                        rpt.chSemAlergias.CheckState = CheckState.Checked;
                        rpt.chSemAlergias.Visible = true;
                        break;
                    }
                    else
                        rpt.pAlergias.Text += item.Alergia.Agente.Combine(", ");
                }
            }
            rpt.pAlergias.Text = rpt.pAlergias.Text.Trim().TrimEnd(',');

            rpt.chMIEPulseira.CheckState = _checklist.AntesEntradaUDI.Pulseira == Pulseira.MIE ? CheckState.Checked : CheckState.Unchecked;
            rpt.chMSEPulseira.CheckState = _checklist.AntesEntradaUDI.Pulseira == Pulseira.MSE ? CheckState.Checked : CheckState.Unchecked;
            rpt.chMSDPulseira.CheckState = _checklist.AntesEntradaUDI.Pulseira == Pulseira.MSD ? CheckState.Checked : CheckState.Unchecked;
            rpt.chMIDPulseira.CheckState = _checklist.AntesEntradaUDI.Pulseira == Pulseira.MID ? CheckState.Checked : CheckState.Unchecked;

            rpt.chNAPuncaoVenosa.CheckState = _checklist.AntesEntradaUDI.PuncaoVenosa == PuncaoVenosa.NA ? CheckState.Checked : CheckState.Unchecked;
            rpt.chMSDPuncaoVenosa.CheckState = _checklist.AntesEntradaUDI.PuncaoVenosa == PuncaoVenosa.MSD ? CheckState.Checked : CheckState.Unchecked;
            rpt.chMSEPuncaoVenosa.CheckState = _checklist.AntesEntradaUDI.PuncaoVenosa == PuncaoVenosa.MSE ? CheckState.Checked : CheckState.Unchecked;

            rpt.chConsentimentoAnestesia.CheckState = _checklist.AntesEntradaUDI.ConsentimentoAnestesia == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chConsentimentoAnestesiaNA.CheckState = _checklist.AntesEntradaUDI.ConsentimentoAnestesia == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chAvaliacaoPreAnestesica.CheckState = _checklist.AntesEntradaUDI.AvaliacaoPreAnestesica == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chAvaliacaoPreAnestesicaNA.CheckState = _checklist.AntesEntradaUDI.AvaliacaoPreAnestesica == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chConsentimentoProcedimento.CheckState = _checklist.AntesEntradaUDI.ConsentimentoProcedimento == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chConsentimentoProcedimentoNA.CheckState = _checklist.AntesEntradaUDI.ConsentimentoProcedimento == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chPesquisaPacAlergicos.CheckState = _checklist.AntesEntradaUDI.PesquisaAlergico == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chPesquisaPacAlergicosNA.CheckState = _checklist.AntesEntradaUDI.PesquisaAlergico == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chConsentimentoContIodado.CheckState = _checklist.AntesEntradaUDI.ConsentimentoContraste == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chConsentimentoContIodadoNA.CheckState = _checklist.AntesEntradaUDI.ConsentimentoContraste == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;            
            #endregion

            #region TimeOut
            rpt.chNomeCompletoEquipe.CheckState = _checklist.TimeOutUDI.NomeCompleto == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chProcedimentoEquipe.CheckState = _checklist.TimeOutUDI.Procedimento == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chLadoAbordado.CheckState = _checklist.TimeOutUDI.Lado == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNAEquipe.CheckState = _checklist.TimeOutUDI.NA == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;

            rpt.chSimAnestesia.CheckState = _checklist.TimeOutUDI.ProcedimentoAnestesia == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNaoAnestesia.CheckState = _checklist.TimeOutUDI.ProcedimentoAnestesia == SimNao.Nao ? CheckState.Checked : CheckState.Unchecked;

            rpt.chCarro.CheckState = _checklist.TimeOutUDI.Carro == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNACarro.CheckState = _checklist.TimeOutUDI.Carro == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chMonitorCardiaco.CheckState = _checklist.TimeOutUDI.Monitor == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNAMonitor.CheckState = _checklist.TimeOutUDI.Monitor == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.chOximetro.CheckState = _checklist.TimeOutUDI.Oximetro == SimNA.Sim ? CheckState.Checked : CheckState.Unchecked;
            rpt.chNAOximetro.CheckState = _checklist.TimeOutUDI.Oximetro == SimNA.NA ? CheckState.Checked : CheckState.Unchecked;

            rpt.ObservacaoTimeOUT.Text = _checklist.TimeOutUDI.Observacoes;
            #endregion

            #region CheckOut
            rpt.chNaoAnalise.CheckState = _checklist.CheckOutUDI.Material == SimNao.Nao ? CheckState.Checked : CheckState.Unchecked;
            rpt.chSimAnalise.CheckState = _checklist.CheckOutUDI.Material == SimNao.Sim ? CheckState.Checked : CheckState.Unchecked;

            foreach (var item in _checklist.CheckOutUDI.CheckOutMaterialUDI)
            {
                rpt.pLaboratorios.Text += item.Descricao + Environment.NewLine;
                rpt.pLaboratoriosPecas.Text += item.NumeroPecas + Environment.NewLine;
            }
  
            rpt.ObservacaoCheckOUT.Text = _checklist.CheckOutUDI.Observacao;
            #endregion

            return rpt;
        }
    }

    public class ChecklistUDIRelatorio
    {
        // Entrada
        public virtual string Paciente { get; set; }
        public virtual string Prontuario { get; set; }
        public virtual string Responsavel { get; set; }
        public virtual string isNomeCompleto { get; set; }
        public virtual string isProcedimento { get; set; }
        public virtual string Procedimento { get; set; }
        public virtual string Regiao { get; set; }
        public virtual string Lado { get; set; }
        public virtual string ExamesAnteriores { get; set; }
        public virtual string PedidoExame { get; set; }
        public virtual string Pulseira { get; set; }
        public virtual string PuncaoVenosa { get; set; }
        public virtual string ConsentimentoAnestesia { get; set; }
        public virtual string AvaliacaoAnestesica { get; set; }
        public virtual string ConsentimentoProcedimento { get; set; }
        public virtual string PesquisaPaciente { get; set; }
        public virtual string ConsentimentoContraste { get; set; }
        public virtual List<Alergia> alergias { get; set; }

        // Time OUT
        //public virtual string isNomeCompleto { get; set; }
        //public virtual string isProcedimento { get; set; }
        public virtual string isLadoAbordado { get; set; }
        public virtual string isProcedimentoAnestesia { get; set; }
        public virtual string isCarro { get; set; }
        public virtual string isMonitor { get; set; }
        public virtual string isOximetro { get; set; }
        public virtual string Observacao { get; set; }

        // Check OUT
        public virtual string isMaterial { get; set; }
        public virtual string ObservacaoCheckOut { get; set; }
        public virtual List<Laboratorio> laboratorios { get; set; }
    }

    public class Alergia
    {
        public virtual string Agente { get; set; }
        public virtual string Tipo { get; set; }
        public virtual string DataInicio { get; set; }
        public virtual string Status { get; set; }
        public virtual string Profissional { get; set; }
        public virtual string Comentários { get; set; }
    }

    public class Laboratorio
    {
        public virtual string Nome { get; set; }
        public virtual string NumeroPecas { get; set; }
    }
}
