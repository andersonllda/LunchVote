using System.Collections.Generic;
using System.Linq;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using StructureMap;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.ProcessosEnfermagem.ViewModel;
using HMV.Core.Wrappers.ObjectWrappers;
using System;
using HMV.Core.Domain.Constant;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmRelatorioSumarioAvaliacaoMedicaCO
    {
        public vmRelatorioSumarioAvaliacaoMedicaCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            this._WrpSumarioAvaliacaoMedicaCO = pWrpSumarioAvaliacaoMedicaCO;
        }

        #region Relatorio

        private List<SumarioAvaliacaoMedicaRelatorioCO> LisRelatorio = new List<SumarioAvaliacaoMedicaRelatorioCO>();
        private wrpSumarioAvaliacaoMedicaCO _WrpSumarioAvaliacaoMedicaCO;

        public List<SumarioAvaliacaoMedicaRelatorioCO> Relatorio()
        {
            SumarioAvaliacaoMedicaRelatorioCO sumario = new SumarioAvaliacaoMedicaRelatorioCO();
            sumario.MostraCodigoBarras = false;
            sumario.MostraIDPaciente = false;

            sumario.Nome = this._WrpSumarioAvaliacaoMedicaCO.Paciente.Nome;
            sumario.Prontuario = this._WrpSumarioAvaliacaoMedicaCO.Paciente.ID.ToString();
            sumario.Atendimento = this._WrpSumarioAvaliacaoMedicaCO.Atendimento.ID.ToString();
            sumario.DataAtendimento = this._WrpSumarioAvaliacaoMedicaCO.Atendimento.DataAtendimento.ToShortDateString();
            sumario.Sexo = this._WrpSumarioAvaliacaoMedicaCO.Paciente.Sexo.ToString();
            sumario.Cor = this._WrpSumarioAvaliacaoMedicaCO.Paciente.Cor.ToString();

            if (this._WrpSumarioAvaliacaoMedicaCO.Paciente.Idade.IsNotNull())
                sumario.Idade = this._WrpSumarioAvaliacaoMedicaCO.Paciente.Idade.GetDate(this._WrpSumarioAvaliacaoMedicaCO.Atendimento.HoraAtendimento);

            if (this._WrpSumarioAvaliacaoMedicaCO.Atendimento.IsNotNull())
            {
                if (this._WrpSumarioAvaliacaoMedicaCO.Paciente.IsNotNull())
                {
                    sumario.NomePaciente = this._WrpSumarioAvaliacaoMedicaCO.Paciente.Nome;
                    sumario.IDPaciente = this._WrpSumarioAvaliacaoMedicaCO.Paciente.ID;
                    sumario.MostraIDPaciente = true;
                }

                sumario.NomeResumo = this._WrpSumarioAvaliacaoMedicaCO.Atendimento.Leito.IsNotNull() ? this._WrpSumarioAvaliacaoMedicaCO.Atendimento.Leito.Descricao : string.Empty;
                sumario.CodigoBarras = this._WrpSumarioAvaliacaoMedicaCO.Atendimento.ID.ToString();
                sumario.MostraCodigoBarras = true;

                if (this._WrpSumarioAvaliacaoMedicaCO.Atendimento.Prestador.IsNotNull())
                {
                    sumario.NomePrestador = this._WrpSumarioAvaliacaoMedicaCO.Atendimento.Prestador.Nome;
                    sumario.Registro = this._WrpSumarioAvaliacaoMedicaCO.Atendimento.Prestador.Registro;
                }
            }

            // Assinatura
            List<AssinaturaCO> lista = new List<AssinaturaCO>();
            AssinaturaCO assinatura = new AssinaturaCO();

            if (this._WrpSumarioAvaliacaoMedicaCO.DataEncerramento.IsNotNull())
            {
                assinatura.Assinatura = this._WrpSumarioAvaliacaoMedicaCO.Usuario.AssinaturaNaLinhaSemColchetes;
                assinatura.DataEncerramento = _WrpSumarioAvaliacaoMedicaCO.DataEncerramento.Value.ToShortDateString() + " " + _WrpSumarioAvaliacaoMedicaCO.DataEncerramento.Value.ToShortTimeString();
            }
            else
                assinatura.Assinatura = this._WrpSumarioAvaliacaoMedicaCO.Usuario.AssinaturaNaLinhaSemColchetes;

            lista.Add(assinatura);

            sumario.listasrAssinatura = lista;
            //

            #region Anamenese

            _listaAnamneseSource = new List<AnamneseSumariCO>();
            AnamneseSumariCO anamense = new AnamneseSumariCO();

            //motivo internacao
            anamense.MotivoInternacaoDescricao = _WrpSumarioAvaliacaoMedicaCO.MotivoInternacao;
            anamense.Primeiro = _WrpSumarioAvaliacaoMedicaCO.MotivoInternacao.IsNotEmptyOrWhiteSpace();

            //historia atual
            anamense.HistoriaAtualDescricao = _WrpSumarioAvaliacaoMedicaCO.HistoriaAtual;
            anamense.Segundo = _WrpSumarioAvaliacaoMedicaCO.HistoriaAtual.IsNotEmptyOrWhiteSpace();

            //gestacao atual
            anamense._listaGestacaoAtualSumarioCO = GestacaoAtual(this._WrpSumarioAvaliacaoMedicaCO);

            //alergias
            anamense.listaAlergiasCO = Alergias(this._WrpSumarioAvaliacaoMedicaCO);

            //gestacoes anteriores
            anamense._listaGestacoesAnterioreSumarioCO = GestacoesAnteriore(this._WrpSumarioAvaliacaoMedicaCO);

            //historia pregressa
            anamense._listasrHistoriaPregressaSumarioCO = HistoriaPregressa(this._WrpSumarioAvaliacaoMedicaCO);

            //pscico social
            anamense.listaPerfilPsicoSocial = PscicoSocial(this._WrpSumarioAvaliacaoMedicaCO);

            //medicamentos em uso
            anamense._listaMedicamentosEmUsoCO = MedicamentosEmUsoItem(this._WrpSumarioAvaliacaoMedicaCO);

            _listaAnamneseSource.Add(anamense);

            #endregion Anamenese

            #region Exames Realizados

            _listaExamesRealizadosSource = new List<ExamesRealizadosSumarioCO>();
            ExamesRealizadosSumarioCO exameRealizado = new ExamesRealizadosSumarioCO();

            //exames realizados
            exameRealizado = ExamesRealizados(_WrpSumarioAvaliacaoMedicaCO);

            _listaExamesRealizadosSource.Add(exameRealizado);

            #endregion

            #region Exames Físicos

            _listaExamesFisicosSource = new List<ExamesFisicosCO>();
            ExamesFisicosCO exameFisico = new ExamesFisicosCO();

            //exames físicos
            exameFisico = ExameFisico(_WrpSumarioAvaliacaoMedicaCO);

            _listaExamesFisicosSource.Add(exameFisico);

            #endregion

            #region Diagnósticos

            _listaDiagnosticosCOSource = new List<DiagnosticosCO>();
            DiagnosticosCO diagnostico = new DiagnosticosCO();

            //diagnósticos
            diagnostico = Diagnosticos(_WrpSumarioAvaliacaoMedicaCO);

            _listaDiagnosticosCOSource.Add(diagnostico);

            #endregion

            #region Plano Diagnóstico e Terapéutico

            _listaPlanoDiagnosticoETerapeuticoCOSource = new List<PlanoDiagnosticoETerapeuticoCO>();
            PlanoDiagnosticoETerapeuticoCO plano = new PlanoDiagnosticoETerapeuticoCO();

            plano = PlanoDiagnosticoETerapeutico(_WrpSumarioAvaliacaoMedicaCO);

            _listaPlanoDiagnosticoETerapeuticoCOSource.Add(plano);

            #endregion

            LisRelatorio.Clear();
            LisRelatorio.Add(sumario);

            return LisRelatorio;
        }

        #endregion Relatorio

        #region Propriedades Publico

        public List<AnamneseSumariCO> _listaAnamneseSource { get; set; }
        public List<ExamesRealizadosSumarioCO> _listaExamesRealizadosSource { get; set; }
        public List<ExamesFisicosCO> _listaExamesFisicosSource { get; set; }
        public List<DiagnosticosCO> _listaDiagnosticosCOSource { get; set; }
        public List<PlanoDiagnosticoETerapeuticoCO> _listaPlanoDiagnosticoETerapeuticoCOSource { get; set; }
        public List<AssinaturaCO> listaAssinaturaCOSource { get; set; }

        #endregion Propriedades Publico

        #region Metodos

        private List<GestacaoAtualSumarioCO> GestacaoAtual(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<GestacaoAtualSumarioCO> lista = new List<GestacaoAtualSumarioCO>();
            GestacaoAtualSumarioCO gestacaoAtual = new GestacaoAtualSumarioCO();
            gestacaoAtual.MostraGestacaoAtual = false;
            gestacaoAtual.MostraProcedencia = false;
            gestacaoAtual.MostraGesta = false;
            gestacaoAtual.MostraDatasEIdadeGestacionalEco = false;
            gestacaoAtual.MostraIdadeGestacional = false;

            if (pWrpSumarioAvaliacaoMedicaCO.Procedencia.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCO.ProcedenciaOutros.ConvertNullToStringEmpty().IsNotEmpty())
                    gestacaoAtual.Procedencia = "Procedência: " + pWrpSumarioAvaliacaoMedicaCO.ProcedenciaOutros;
                else
                    gestacaoAtual.Procedencia = "Procedência: " + pWrpSumarioAvaliacaoMedicaCO.Procedencia.Descricao;

                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraProcedencia = true;
            }

            gestacaoAtual.Gesta = pWrpSumarioAvaliacaoMedicaCO.Gesta.IsNotNull() ? "Gesta: " + pWrpSumarioAvaliacaoMedicaCO.Gesta + "    " : string.Empty;
            gestacaoAtual.Gesta += pWrpSumarioAvaliacaoMedicaCO.Para.IsNotNull() ? " Para: " + pWrpSumarioAvaliacaoMedicaCO.Para + "    " : string.Empty;
            gestacaoAtual.Gesta += pWrpSumarioAvaliacaoMedicaCO.Cesarea.IsNotNull() ? " Cesárea: " + pWrpSumarioAvaliacaoMedicaCO.Cesarea + "    " : string.Empty;
            gestacaoAtual.Gesta += pWrpSumarioAvaliacaoMedicaCO.Aborto.IsNotNull() ? " Aborto: " + pWrpSumarioAvaliacaoMedicaCO.Aborto + "    " : string.Empty;
            gestacaoAtual.Gesta += pWrpSumarioAvaliacaoMedicaCO.Ectopica.IsNotNull() ? " Ectópica: " + pWrpSumarioAvaliacaoMedicaCO.Ectopica + "    " : string.Empty;
            gestacaoAtual.Gesta += pWrpSumarioAvaliacaoMedicaCO.GravidezMultipla.IsNotNull() ? " Gravidez Múltipla: " + pWrpSumarioAvaliacaoMedicaCO.GravidezMultipla : string.Empty;

            if (!gestacaoAtual.Gesta.IsEmptyOrWhiteSpace())
            {
                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraGesta = true;
            }

            if (pWrpSumarioAvaliacaoMedicaCO.UltimaMenstruacao.IsNotNull())
            {
                gestacaoAtual.DataEIdadeGestacionalEco = "Última Menstruação: " + pWrpSumarioAvaliacaoMedicaCO.UltimaMenstruacao.Value.ToShortDateString();
                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraDatasEIdadeGestacionalEco = true;
            }

            if (pWrpSumarioAvaliacaoMedicaCO.EcografiaData.IsNotNull())
            {
                if (gestacaoAtual.DataEIdadeGestacionalEco.IsNotEmpty())
                    gestacaoAtual.DataEIdadeGestacionalEco += "     Data 1ª Ecografia: " + pWrpSumarioAvaliacaoMedicaCO.EcografiaData.Value.ToShortDateString();
                else
                    gestacaoAtual.DataEIdadeGestacionalEco = "Data 1ª Ecografia: " + pWrpSumarioAvaliacaoMedicaCO.EcografiaData.Value.ToShortDateString();

                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraDatasEIdadeGestacionalEco = true;
            }
            //

            // Idade Gestacional na 1° ECO
            string idadeGestacionalEco = string.Empty;
            if (pWrpSumarioAvaliacaoMedicaCO.IdadeEcoSemana.IsNotNull())
                idadeGestacionalEco = "Idade Gestacional na 1° ECO: " + pWrpSumarioAvaliacaoMedicaCO.IdadeEcoSemana + " semana(s)";

            if (pWrpSumarioAvaliacaoMedicaCO.IdadeEcoDias.IsNotNull())
            {
                if (idadeGestacionalEco.IsEmpty())
                    idadeGestacionalEco = "Idade Gestacional na 1° ECO: " + pWrpSumarioAvaliacaoMedicaCO.IdadeEcoDias + " dia(s)";
                else
                    idadeGestacionalEco += " e " + pWrpSumarioAvaliacaoMedicaCO.IdadeEcoDias + " dia(s)";
            }

            if (idadeGestacionalEco.IsNotEmpty())
            {
                if (gestacaoAtual.DataEIdadeGestacionalEco.IsEmpty())
                    gestacaoAtual.DataEIdadeGestacionalEco = idadeGestacionalEco;
                else
                    gestacaoAtual.DataEIdadeGestacionalEco += "     " + idadeGestacionalEco;
            }

            if (gestacaoAtual.DataEIdadeGestacionalEco.IsNotEmpty())
            {
                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraDatasEIdadeGestacionalEco = true;
            }
            //

            // Idade Gestacional
            if (pWrpSumarioAvaliacaoMedicaCO.IdadeDesconhecida == Core.Domain.Enum.SimNao.Sim)
            {
                if (gestacaoAtual.IdadeGestacional.IsNotEmpty())
                    gestacaoAtual.IdadeGestacional += "     Idade gestacional: Desconhecida";
                else
                    gestacaoAtual.IdadeGestacional = "Idade gestacional: Desconhecida";

                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraIdadeGestacional = true;
            }

            string idadeGestacional = string.Empty;
            if (pWrpSumarioAvaliacaoMedicaCO.IdadeSemana.IsNotNull())
                idadeGestacional = "Idade Gestacional: " + pWrpSumarioAvaliacaoMedicaCO.IdadeSemana + " semana(s)";

            if (pWrpSumarioAvaliacaoMedicaCO.IdadeDias.IsNotNull())
            {
                if (idadeGestacional.IsEmpty())
                    idadeGestacional = "Idade Gestacional: " + pWrpSumarioAvaliacaoMedicaCO.IdadeDias + " dia(s)";
                else
                    idadeGestacional += " e " + pWrpSumarioAvaliacaoMedicaCO.IdadeDias + " dia(s)";
            }

            if (idadeGestacional.IsNotEmpty())
            {
                if (gestacaoAtual.IdadeGestacional.IsEmpty())
                    gestacaoAtual.IdadeGestacional = idadeGestacional;
                else
                    gestacaoAtual.IdadeGestacional += "     " + idadeGestacional;
            }

            if (gestacaoAtual.IdadeGestacional.IsNotEmpty())
            {
                gestacaoAtual.MostraGestacaoAtual = true;
                gestacaoAtual.MostraIdadeGestacional = true;
            }
            //

            gestacaoAtual._listaPatologiasSumarioCo = PatologiasSumarioCo(pWrpSumarioAvaliacaoMedicaCO);
            if (gestacaoAtual._listaPatologiasSumarioCo.FirstOrDefault().MostraPatologias)
                gestacaoAtual.MostraGestacaoAtual = true;

            gestacaoAtual.listaGestacaoAtualOutrosObservacoesCO = GestacaoAtualOutrosObservacoesCO(pWrpSumarioAvaliacaoMedicaCO);
            if (gestacaoAtual.listaGestacaoAtualOutrosObservacoesCO.FirstOrDefault().MostraOutrosObservacoes)
                gestacaoAtual.MostraGestacaoAtual = true;

            lista.Add(gestacaoAtual);

            return lista;
        }

        private List<GestacaoAtualOutrosObservacoesCO> GestacaoAtualOutrosObservacoesCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<GestacaoAtualOutrosObservacoesCO> lista = new List<GestacaoAtualOutrosObservacoesCO>();
            GestacaoAtualOutrosObservacoesCO gestacaoAtual = new GestacaoAtualOutrosObservacoesCO();
            gestacaoAtual.MostraOutrosObservacoes = false;

            if (pWrpSumarioAvaliacaoMedicaCO.PatologiaObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                gestacaoAtual.TituloOutrosObservacoes = "Outros/Observações:";
                gestacaoAtual.ValorOutrosObservacoes = pWrpSumarioAvaliacaoMedicaCO.PatologiaObservacao;
                gestacaoAtual.MostraOutrosObservacoes = true;
            }

            lista.Add(gestacaoAtual);

            return lista;
        }

        private List<PatologiasSumarioCo> PatologiasSumarioCo(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<PatologiasSumarioCo> lista = new List<PatologiasSumarioCo>();
            PatologiasSumarioCo patologia = new PatologiasSumarioCo();
            patologia.MostraPatologias = false;
            patologia.MostraNenhuma = false;

            patologia._listaPatologiasSumarioCoItem = PatologiasSumarioCoItem(pWrpSumarioAvaliacaoMedicaCO);
            if (patologia._listaPatologiasSumarioCoItem.FirstOrDefault().MostraPatologiasItem)
                patologia.MostraPatologias = true;

            if (pWrpSumarioAvaliacaoMedicaCO.IsPatologia == SimNao.Sim)
            {
                patologia.Nenhuma = "Nenhuma";
                patologia.MostraPatologias = true;
                patologia.MostraNenhuma = true;
            }

            lista.Add(patologia);

            return lista;
        }

        private List<PatologiasSumarioCoItem> PatologiasSumarioCoItem(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<PatologiasSumarioCoItem> lista = new List<PatologiasSumarioCoItem>();
            PatologiasSumarioCoItem item;

            foreach (var itemE in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOItens)
            {
                if (itemE.ItemCO.Patologia == Core.Domain.Enum.SimNao.Sim)
                {
                    item = new PatologiasSumarioCoItem();
                    item.Descricao = itemE.ItemCO.Descricao;
                    item.Observacao = itemE.Observacao;
                    item.MostraPatologiasItem = true;
                    lista.Add(item);
                }
            }

            if (lista.Count == 0)
            {
                item = new PatologiasSumarioCoItem();
                item.MostraPatologiasItem = false;
                lista.Add(item);
            }

            return lista;
        }

        private List<AlergiasCO> Alergias(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<AlergiasCO> lista = new List<AlergiasCO>();
            AlergiasCO alergia = new AlergiasCO();
            alergia.MostraAlergias = false;
            alergia.MostraNenhumaAlergiaConhecida = false;

            alergia.listaAlergiasItemCO = AlergiasItemCO(pWrpSumarioAvaliacaoMedicaCO);
            if (alergia.listaAlergiasItemCO.FirstOrDefault().MostraAlergiasItem)
            {
                if (alergia.listaAlergiasItemCO.FirstOrDefault().Agente == Constantes.coSemAlergiasConhecidas)
                {
                    alergia.listaAlergiasItemCO.FirstOrDefault().MostraAlergiasItem = false;
                    alergia.NenhumaAlergiaConhecida = Constantes.coSemAlergiasConhecidas;
                    alergia.MostraNenhumaAlergiaConhecida = true;
                }

                alergia.MostraAlergias = true;
            }

            lista.Add(alergia);

            return lista;
        }

        private List<AlergiasItemCO> AlergiasItemCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            IRepositorioDeEventoSumarioAvaliacaoMedicaCO repe = ObjectFactory.GetInstance<IRepositorioDeEventoSumarioAvaliacaoMedicaCO>();
            wrpEventoSumarioAvaliacaoMedicaCO _evento = new wrpEventoSumarioAvaliacaoMedicaCO(repe.Single());

            List<AlergiasItemCO> lista = new List<AlergiasItemCO>();
            AlergiasItemCO alergia;

            string valida = Constantes.coSemAlergiasConhecidas;

            foreach (var item in pWrpSumarioAvaliacaoMedicaCO.Paciente.Alergias)
            {
                if (item.IsNotNull())
                {
                    if (_evento.AlergiaEventos.Count(x => x.Chave == pWrpSumarioAvaliacaoMedicaCO.Id && x.Alergia.ID == item.ID && x.Alergia.Status == StatusAlergiaProblema.Ativo) > 0)
                        lista.Add(new AlergiasItemCO
                        {
                            Agente = item.Agente,
                            Comentario = item.Agente == valida ? string.Empty : item.Comentario,
                            Profissional = item.Agente == valida ? string.Empty : item.Profissional.IsNull() ? string.Empty : item.Profissional.nome,
                            Status = item.Agente == valida ? string.Empty : item.Status.ToString(),
                            Tipo = item.Agente == valida ? string.Empty : item.AlergiaTipo.IsNull() ? string.Empty : item.AlergiaTipo.Descricao,
                            MostraAlergiasItem = true
                        });
                }
            }

            if (lista.Count == 0)
            {
                alergia = new AlergiasItemCO();
                alergia.MostraAlergiasItem = false;
                lista.Add(alergia);
            }

            return lista;
        }

        private List<GestacoesAnterioreSumarioCO> GestacoesAnteriore(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<GestacoesAnterioreSumarioCO> lista = new List<GestacoesAnterioreSumarioCO>();
            GestacoesAnterioreSumarioCO pat = new GestacoesAnterioreSumarioCO();
            pat.MostraGestacoesAnteriores = false;

            if (pWrpSumarioAvaliacaoMedicaCO.IsGestacaoAnterior == Core.Domain.Enum.SimNao.Sim)
                pat.IntercorrenciaOuPrimeiraGestacao = "<< Sem Intercorrências >>";
            else if (pWrpSumarioAvaliacaoMedicaCO.IsPrimeiraGestacao == Core.Domain.Enum.SimNao.Sim)
                pat.IntercorrenciaOuPrimeiraGestacao = "<< Primeira Gestação >>";

            if (pat.IntercorrenciaOuPrimeiraGestacao.IsNotEmpty())
            {
                pat.MostraGestacoesAnteriores = true;
                pat.MostraIntercorrenciaOuPrimeiraGestacao = true;
            }

            pat._listaGestacoesAnterioreSumarioCOItem = GestacoesAnterioreItem(pWrpSumarioAvaliacaoMedicaCO);
            if (pat._listaGestacoesAnterioreSumarioCOItem.FirstOrDefault().MostraGestacoesAnterioreItem)
                pat.MostraGestacoesAnteriores = true;

            if (pWrpSumarioAvaliacaoMedicaCO.GestacaoAnteriorObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                pat.ObservacaoGeral = "Observação: " + pWrpSumarioAvaliacaoMedicaCO.GestacaoAnteriorObservacao;
                pat.MostraGestacoesAnteriores = true;
                pat.MostraObservacoes = true;
            }

            lista.Add(pat);

            return lista;
        }

        private List<GestacoesAnterioreSumarioCOItem> GestacoesAnterioreItem(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<GestacoesAnterioreSumarioCOItem> lista = new List<GestacoesAnterioreSumarioCOItem>();
            GestacoesAnterioreSumarioCOItem item;

            foreach (var itemE in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOItens)
            {
                if (itemE.ItemCO.GestacaoAnterior == Core.Domain.Enum.SimNao.Sim)
                {
                    item = new GestacoesAnterioreSumarioCOItem();
                    item.Descricao = itemE.ItemCO.Descricao;
                    item.Observacao = itemE.Observacao;
                    item.MostraGestacoesAnterioreItem = true;
                    lista.Add(item);
                }
            }

            if (lista.Count == 0)
            {
                item = new GestacoesAnterioreSumarioCOItem();
                item.MostraGestacoesAnterioreItem = false;
                lista.Add(item);
            }

            return lista;
        }

        private List<HistoriaPregressaSumarioCO> HistoriaPregressa(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<HistoriaPregressaSumarioCO> lista = new List<HistoriaPregressaSumarioCO>();
            HistoriaPregressaSumarioCO Hit = new HistoriaPregressaSumarioCO();

            Hit.TituloNega = pWrpSumarioAvaliacaoMedicaCO.NegaPatologiaPrevia == Core.Domain.Enum.SimNao.Sim ? "<< Nega Patologias Prévias >>" : "";
            Hit._listasrHistoriaPregressaItemSumarioCO = HistoriaPregressaItem(pWrpSumarioAvaliacaoMedicaCO);

            if (Hit.TituloNega.IsNotEmptyOrWhiteSpace())
            {
                Hit.Terceiro = true;
                Hit._listasrHistoriaPregressaItemSumarioCO.Clear();
            }

            Hit.ObservacaoGeral = pWrpSumarioAvaliacaoMedicaCO.PatologiaPreviaObservacao.IsNotEmptyOrWhiteSpace() ? "Observação: " + pWrpSumarioAvaliacaoMedicaCO.PatologiaPreviaObservacao : "";
            Hit.Primeiro = pWrpSumarioAvaliacaoMedicaCO.PatologiaPreviaObservacao.IsNotEmptyOrWhiteSpace();

            if (pWrpSumarioAvaliacaoMedicaCO.NegaPatologiaPrevia == Core.Domain.Enum.SimNao.Sim || Hit._listasrHistoriaPregressaItemSumarioCO.Count > 0 || pWrpSumarioAvaliacaoMedicaCO.PatologiaPreviaObservacao.IsNotEmptyOrWhiteSpace())
                lista.Add(Hit);

            return lista;
        }

        private List<HistoriaPregressaSumarioCOItem> HistoriaPregressaItem(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<HistoriaPregressaSumarioCOItem> lista = new List<HistoriaPregressaSumarioCOItem>();
            HistoriaPregressaSumarioCOItem item;

            foreach (var itemE in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOItens)
            {
                if (itemE.ItemCO.DoencasPrevias == Core.Domain.Enum.SimNao.Sim)
                {
                    item = new HistoriaPregressaSumarioCOItem();
                    item.Descricao = itemE.ItemCO.Descricao;
                    item.Observacao = itemE.Observacao;
                    lista.Add(item);
                }
            }
            return lista;
        }

        private List<PerfilPsicoSocialCO> PscicoSocial(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<PerfilPsicoSocialCO> lista = new List<PerfilPsicoSocialCO>();
            PerfilPsicoSocialCO perfil = new PerfilPsicoSocialCO();
            perfil.MostraPerfilPsicoSocial = false;

            perfil.listaOutrosObservacoesCO = PerfilPsicoSocialOutrosObservacoesCO(pWrpSumarioAvaliacaoMedicaCO);
            if (perfil.listaOutrosObservacoesCO.FirstOrDefault().MostraOutrosObservacoes)
                perfil.MostraPerfilPsicoSocial = true;

            perfil._listaPscicoSocialSumarioCOItem = PscicoSocialItem(pWrpSumarioAvaliacaoMedicaCO);
            if (perfil._listaPscicoSocialSumarioCOItem.FirstOrDefault().MostraPerfilPsicoSocialItem)
                perfil.MostraPerfilPsicoSocial = true;

            lista.Add(perfil);

            return lista;
        }

        private List<PerfilPsicoSocialOutrosObservacoesCO> PerfilPsicoSocialOutrosObservacoesCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<PerfilPsicoSocialOutrosObservacoesCO> lista = new List<PerfilPsicoSocialOutrosObservacoesCO>();
            PerfilPsicoSocialOutrosObservacoesCO perfil = new PerfilPsicoSocialOutrosObservacoesCO();
            perfil.MostraOutrosObservacoes = false;

            if (pWrpSumarioAvaliacaoMedicaCO.PerfilObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                perfil.TituloOutrosObservacoes = "Outros/Observações:";
                perfil.ValorOutrosObservacoes = pWrpSumarioAvaliacaoMedicaCO.PerfilObservacao;
                perfil.MostraOutrosObservacoes = true;
            }

            lista.Add(perfil);

            return lista;
        }

        private List<PscicoSocialSumarioCOItem> PscicoSocialItem(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<PscicoSocialSumarioCOItem> lista = new List<PscicoSocialSumarioCOItem>();
            PscicoSocialSumarioCOItem perfil;

            foreach (var itemE in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOItens)
            {
                if (itemE.ItemCO.Habitos == Core.Domain.Enum.SimNao.Sim)
                {
                    perfil = new PscicoSocialSumarioCOItem();
                    perfil.Descricao = itemE.ItemCO.Descricao;
                    perfil.Observacao = itemE.Observacao;
                    perfil.MostraPerfilPsicoSocialItem = true;
                    lista.Add(perfil);
                }
            }

            if (lista.Count == 0)
            {
                perfil = new PscicoSocialSumarioCOItem();
                perfil.MostraPerfilPsicoSocialItem = false;
                lista.Add(perfil);
            }

            return lista;
        }

        private List<MedicamentosEmUsoCO> MedicamentosEmUsoItem(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<MedicamentosEmUsoCO> lista = new List<MedicamentosEmUsoCO>();
            MedicamentosEmUsoCO medicamento = new MedicamentosEmUsoCO();
            medicamento.MostraMedicamentosEmUso = false;
            medicamento.MostraSemUsoDeMedicamentos = false;

            //IRepositorioDeEventoSumarioAvaliacaoMedicaCO repe = ObjectFactory.GetInstance<IRepositorioDeEventoSumarioAvaliacaoMedicaCO>();
            //wrpEventoSumarioAvaliacaoMedicaCO _evento = new wrpEventoSumarioAvaliacaoMedicaCO(repe.Single());

            wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;

            IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
            repp.OndeChaveIgual(pWrpSumarioAvaliacaoMedicaCO.Id);
            repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedicaCO);
            var ret = repp.List();
            if (ret.IsNotNull())
                _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);            

            vmMedicamentosEmUsoEvento vmEvento = new vmMedicamentosEmUsoEvento(false, pWrpSumarioAvaliacaoMedicaCO.Paciente, pWrpSumarioAvaliacaoMedicaCO.Usuario,
                TipoEvento.SumarioAvaliacaoMedicaCO, _MedicamentosCollection, pWrpSumarioAvaliacaoMedicaCO.Id, pMostraUltAdministracao: true, pMostraDataInicio: false,
                pMostraVia: false, pMostraFrequencia: false, pMostraComboVia: true, pMostraComboFrequencia: true/*, pAtendimento: pWrpSumarioAvaliacaoMedicaCO.Atendimento*/);

            medicamento._listaMedicamentosEmUsoCOItem = MedicamentosEmUsoItem(vmEvento);
            if (medicamento._listaMedicamentosEmUsoCOItem.FirstOrDefault().Medicamento == "<< Sem uso de medicamentos >>")
            {
                medicamento.Medicamento = "<< Sem uso de medicamentos >>";
                medicamento.MostraMedicamentosEmUso = true;
                medicamento.MostraSemUsoDeMedicamentos = true;
            }

            if (medicamento._listaMedicamentosEmUsoCOItem.FirstOrDefault().MostraMedicamentosEmUsoItem)
                medicamento.MostraMedicamentosEmUso = true;

            lista.Add(medicamento);
            return lista;
        }

        private List<MedicamentosEmUsoItemCO> MedicamentosEmUsoItem(vmMedicamentosEmUsoEvento pvmMedicamentosEmUsoEvento)
        {
            List<MedicamentosEmUsoItemCO> lista = new List<MedicamentosEmUsoItemCO>();
            MedicamentosEmUsoItemCO medicamento;

            foreach (var item in pvmMedicamentosEmUsoEvento.MedicamentosCollection)
            {
                if (item.Selecionado)
                {
                    medicamento = new MedicamentosEmUsoItemCO();
                    medicamento.Medicamento = item.Medicamento;
                    medicamento.Dose = item.Dose;
                    medicamento.Via = item.Via;
                    medicamento.Frequencia = item.Frequencia;
                    medicamento.Status = item.Status.GetEnumCustomDisplay();
                    medicamento.MostraMedicamentosEmUsoItem = true;
                    lista.Add(medicamento);
                }
            }

            if (lista.Count > 0)
            {
                if (lista.FirstOrDefault().Medicamento == "<< Sem uso de medicamentos >>")
                    lista.FirstOrDefault().MostraMedicamentosEmUsoItem = false;
            }

            if (lista.Count == 0)
            {
                medicamento = new MedicamentosEmUsoItemCO();
                medicamento.MostraMedicamentosEmUsoItem = false;
                lista.Add(medicamento);
            }

            return lista;
        }

        private ExamesRealizadosSumarioCO ExamesRealizados(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            ExamesRealizadosSumarioCO exame = new ExamesRealizadosSumarioCO();
            exame.MostraExamesRealizados = false;

            exame._listaTipagemSanguineaETesteRapidoCO = TipagemSanguineaETesteRapidoCO(pWrpSumarioAvaliacaoMedicaCO);
            if (exame._listaTipagemSanguineaETesteRapidoCO.FirstOrDefault().MostraTipagemSanguineaETesteRapido)
                exame.MostraExamesRealizados = true;

            exame._listaExamesrealizadosOutrosCO = ExamesrealizadosOutrosCO(pWrpSumarioAvaliacaoMedicaCO.ExamesRealizadosObservacao);
            if (exame._listaExamesrealizadosOutrosCO.FirstOrDefault().MostraOutros)
                exame.MostraExamesRealizados = true;

            exame._listaExamesLaboratoriaisCO = ExamesLaboratoriais(_WrpSumarioAvaliacaoMedicaCO);
            if (exame._listaExamesLaboratoriaisCO.FirstOrDefault().MostraExamesLaboratoriais)
                exame.MostraExamesRealizados = true;

            return exame;
        }

        private List<ExamesrealizadosOutrosCO> ExamesrealizadosOutrosCO(string pOutros)
        {
            List<ExamesrealizadosOutrosCO> lista = new List<ExamesrealizadosOutrosCO>();
            ExamesrealizadosOutrosCO exame = new ExamesrealizadosOutrosCO();
            exame.MostraOutros = false;

            if (pOutros.ConvertNullToStringEmpty().IsNotEmpty())
            {
                exame.TituloOutros = "Outros/Observações:";
                exame.DescricaoOutros = pOutros;
                exame.MostraOutros = true;
            }

            lista.Add(exame);

            return lista;
        }

        private List<TipagemSanguineaETesteRapidoCO> TipagemSanguineaETesteRapidoCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<TipagemSanguineaETesteRapidoCO> lista = new List<TipagemSanguineaETesteRapidoCO>();
            TipagemSanguineaETesteRapidoCO tipagemEtesteRapido = new TipagemSanguineaETesteRapidoCO();
            tipagemEtesteRapido.MostraTipagemSanguineaETesteRapido = false;

            if (pWrpSumarioAvaliacaoMedicaCO.TipagemPaciente.IsNotNull())
            {
                tipagemEtesteRapido.TipagemSanguineaETesteRapido = "Tipagem Sanguínea: " + pWrpSumarioAvaliacaoMedicaCO.TipagemPaciente.Value.GetEnumDescription();

                if (pWrpSumarioAvaliacaoMedicaCO.RHPaciente.IsNotNull())
                {
                    tipagemEtesteRapido.TipagemSanguineaETesteRapido += pWrpSumarioAvaliacaoMedicaCO.RHPaciente.Value.GetEnumDescription();
                    tipagemEtesteRapido.MostraTipagemSanguineaETesteRapido = true;
                }
            }

            if (pWrpSumarioAvaliacaoMedicaCO.Coombs.IsNotNull())
            {
                if (tipagemEtesteRapido.TipagemSanguineaETesteRapido.IsNotEmpty())
                    tipagemEtesteRapido.TipagemSanguineaETesteRapido += "     " + "Coombs Indireto: " + pWrpSumarioAvaliacaoMedicaCO.Coombs.Value.GetEnumCustomDisplay();
                else
                    tipagemEtesteRapido.TipagemSanguineaETesteRapido += "Coombs Indireto: " + pWrpSumarioAvaliacaoMedicaCO.Coombs.Value.GetEnumCustomDisplay();

                tipagemEtesteRapido.MostraTipagemSanguineaETesteRapido = true;
            }

            if (pWrpSumarioAvaliacaoMedicaCO.HIV.IsNotNull())
            {
                if (tipagemEtesteRapido.TipagemSanguineaETesteRapido.IsNotEmpty())
                    tipagemEtesteRapido.TipagemSanguineaETesteRapido += "     Teste rápido HIV: " + pWrpSumarioAvaliacaoMedicaCO.HIV.Value.GetEnumCustomDisplay();
                else
                    tipagemEtesteRapido.TipagemSanguineaETesteRapido += "Teste rápido HIV: " + pWrpSumarioAvaliacaoMedicaCO.HIV.Value.GetEnumCustomDisplay();

                tipagemEtesteRapido.MostraTipagemSanguineaETesteRapido = true;
            }

            lista.Add(tipagemEtesteRapido);

            return lista;
        }

        private List<ExamesLaboratoriaisCO> ExamesLaboratoriais(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<ExamesLaboratoriaisCO> lista = new List<ExamesLaboratoriaisCO>();
            ExamesLaboratoriaisCO exame;

            foreach (var itemExame in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOItens)
            {
                if ((itemExame.ItemCO.Sorologia == SimNao.Sim || itemExame.ItemCO.Exames == SimNao.Sim) && (itemExame.Resultado.IsNotNull()
                    || itemExame.IsTrimestre1 == SimNao.Sim || itemExame.IsTrimestre2 == SimNao.Sim || itemExame.IsTrimestre3 == SimNao.Sim))
                {
                    exame = new ExamesLaboratoriaisCO();
                    exame.SorologiaExames = itemExame.ItemCO.Descricao;

                    if (itemExame.Resultado == ResultadoItemCO.Negativo)
                        exame.NegNaoRea = "þ";

                    if (itemExame.Resultado == ResultadoItemCO.Positivo)
                        exame.PosRea = "þ";

                    if (itemExame.Resultado == ResultadoItemCO.NaoDisponivel)
                        exame.NaoDisponivel = "þ";

                    if (itemExame.IsTrimestre1 == SimNao.Sim)
                        exame.Trimestre1 = "þ";

                    if (itemExame.IsTrimestre2 == SimNao.Sim)
                        exame.Trimestre2 = "þ";

                    if (itemExame.IsTrimestre3 == SimNao.Sim)
                        exame.Trimestre3 = "þ";

                    exame.MostraExamesLaboratoriais = true;
                    lista.Add(exame);
                }
            }

            if (lista.Count == 0)
            {
                exame = new ExamesLaboratoriaisCO();
                exame.MostraExamesLaboratoriais = false;
                lista.Add(exame);
            }

            return lista.OrderBy(x => x.SorologiaExames).ToList();
        }

        private ExamesFisicosCO ExameFisico(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            ExamesFisicosCO exameFisico = new ExamesFisicosCO();
            exameFisico.MostraSrExamesFisicos = false;
            exameFisico.MostraDinamicaToqueVaginalESangramento = false;
            exameFisico.MostraMembranasDataHoraELiquidoAmniotico = false;

            if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.IsNotNull())
            {
                exameFisico.listaSinaisVitaisEAntropometricos = SinaisVitaisEAntropometricosCO(pWrpSumarioAvaliacaoMedicaCO);
                if (exameFisico.listaSinaisVitaisEAntropometricos.FirstOrDefault().MostraSinaisVitaisEAntropometricos)
                    exameFisico.MostraSrExamesFisicos = true;

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Dinamica.IsNotNull())
                {
                    exameFisico.DinamicaToqueVaginalESangramento = "Dinâmica: " + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Dinamica.Value + " contrações/10 minutos";
                    exameFisico.MostraSrExamesFisicos = true;
                    exameFisico.MostraDinamicaToqueVaginalESangramento = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.ToqueVaginal.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    if (exameFisico.DinamicaToqueVaginalESangramento.IsNotEmpty())
                        exameFisico.DinamicaToqueVaginalESangramento += "     Toque Vaginal: " + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.ToqueVaginal;
                    else
                        exameFisico.DinamicaToqueVaginalESangramento = "Toque Vaginal: " + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.ToqueVaginal;

                    exameFisico.MostraSrExamesFisicos = true;
                    exameFisico.MostraDinamicaToqueVaginalESangramento = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.IsSangramento.IsNotNull())
                {
                    if (exameFisico.DinamicaToqueVaginalESangramento.IsNotEmpty())
                        exameFisico.DinamicaToqueVaginalESangramento += "     Sangramento: " + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.IsSangramento.Value.GetEnumCustomDisplay();
                    else
                        exameFisico.DinamicaToqueVaginalESangramento = "Sangramento: " + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.IsSangramento.Value.GetEnumCustomDisplay();

                    exameFisico.MostraSrExamesFisicos = true;
                    exameFisico.MostraDinamicaToqueVaginalESangramento = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Membrana.IsNotNull())
                {
                    exameFisico.MembranasDataHoraELiquidoAmniotico = "Membranas Amnióticas: " + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Membrana.Value;
                    exameFisico.MostraMembranasDataHoraELiquidoAmniotico = true;
                    exameFisico.MostraSrExamesFisicos = true;

                    if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.DataMembrana.IsNotNull())
                    {
                        DateTime data = pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.DataMembrana.Value;
                        exameFisico.MembranasDataHoraELiquidoAmniotico += "     Data: " + data.ToShortDateString() + "     Hora: " + data.ToShortTimeString();
                    }
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmniotico.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmniotico == LiquidoAmniotico.Outros)
                    {
                        if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmnioticoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                        {
                            if (exameFisico.MembranasDataHoraELiquidoAmniotico.IsNotEmpty())
                                exameFisico.MembranasDataHoraELiquidoAmniotico += "     Líquido Amniótico: ";
                            else
                                exameFisico.MembranasDataHoraELiquidoAmniotico = "Líquido Amniótico: ";

                            exameFisico.MembranasDataHoraELiquidoAmniotico += pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmnioticoObservacao;
                        }
                    }
                    else
                    {
                        if (exameFisico.MembranasDataHoraELiquidoAmniotico.IsNotEmpty())
                            exameFisico.MembranasDataHoraELiquidoAmniotico += "     Líquido Amniótico: ";
                        else
                            exameFisico.MembranasDataHoraELiquidoAmniotico = "Líquido Amniótico: ";

                        exameFisico.MembranasDataHoraELiquidoAmniotico += pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.LiquidoAmniotico.Value.GetEnumCustomDisplay();
                    }

                    exameFisico.MostraMembranasDataHoraELiquidoAmniotico = true;
                    exameFisico.MostraSrExamesFisicos = true;
                }

                exameFisico._listaExamesFisicosObservacoesCO = ExamesFisicosObservacoesCO(pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Observacao);
                if (exameFisico._listaExamesFisicosObservacoesCO.FirstOrDefault().MostraObservacoes)
                    exameFisico.MostraSrExamesFisicos = true;
            }

            return exameFisico;
        }

        private List<SinaisVitaisEAntropometricosCO> SinaisVitaisEAntropometricosCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<SinaisVitaisEAntropometricosCO> lista = new List<SinaisVitaisEAntropometricosCO>();
            SinaisVitaisEAntropometricosCO sinais = new SinaisVitaisEAntropometricosCO();
            sinais.MostraSinaisVitaisEAntropometricos = false;

            if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.PAAlta.IsNotNull())
                {
                    sinais.Sinais = "PA: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.PAAlta.Value.ToString();
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.PABaixa.IsNotNull())
                {
                    if (sinais.Sinais.IsEmpty())
                    {
                        sinais.Sinais = "PA: ";
                    }
                    sinais.Sinais = sinais.Sinais + "/" + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.PABaixa.Value.ToString() + " mmhg";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.TAX.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    Tax: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.TAX.Value + " °C";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.FC.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    FC: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.FC.Value + " bpm";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.FR.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    FR: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.FR.Value + " mrpm";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.DOR.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    Dor: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.DOR.Value.GetEnumCustomDisplay();
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.BCF.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    BCF: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.BCF.Value + " bpm";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Peso.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    Peso: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Peso.Value + " Kg";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Altura.IsNotNull())
                {
                    sinais.Sinais = sinais.Sinais + "    Altura: ";
                    sinais.Sinais = sinais.Sinais + pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOExameFisico.Altura.Value + " cm";
                    sinais.MostraSinaisVitaisEAntropometricos = true;
                }
            }

            lista.Add(sinais);

            return lista;
        }

        private List<ExamesFisicosObservacoesCO> ExamesFisicosObservacoesCO(string pObservacoes)
        {
            List<ExamesFisicosObservacoesCO> lista = new List<ExamesFisicosObservacoesCO>();
            ExamesFisicosObservacoesCO exameFisico = new ExamesFisicosObservacoesCO();
            exameFisico.MostraObservacoes = false;

            if (pObservacoes.ConvertNullToStringEmpty().IsNotEmpty())
            {
                exameFisico.TituloObservacoes = "Observações:";
                exameFisico.DescricaoObservacao = pObservacoes;
                exameFisico.MostraObservacoes = true;
            }

            lista.Add(exameFisico);

            return lista;
        }

        private DiagnosticosCO Diagnosticos(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            DiagnosticosCO diagnosticos = new DiagnosticosCO();
            diagnosticos.MostraSrDiagnostico = false;

            diagnosticos._listaDiagnosticosHipotesesDiagnosticasCIDCO = DiagnosticosHipotesesDiagnosticasCIDCO(pWrpSumarioAvaliacaoMedicaCO.Atendimento);
            if (diagnosticos._listaDiagnosticosHipotesesDiagnosticasCIDCO.FirstOrDefault().MostraCID)
                diagnosticos.MostraSrDiagnostico = true;

            diagnosticos._listaOutrosCidsCO = OutrosCids(_WrpSumarioAvaliacaoMedicaCO);
            if (diagnosticos._listaOutrosCidsCO.FirstOrDefault().MostraOutrosCids)
                diagnosticos.MostraSrDiagnostico = true;

            diagnosticos._listaHipotesesCO = Hipoteses(_WrpSumarioAvaliacaoMedicaCO);
            if (diagnosticos._listaHipotesesCO.FirstOrDefault().MostraHipoteses)
                diagnosticos.MostraSrDiagnostico = true;

            return diagnosticos;
        }

        private List<DiagnosticosHipotesesDiagnosticasCIDCO> DiagnosticosHipotesesDiagnosticasCIDCO(wrpAtendimento pAtendimento)
        {
            List<DiagnosticosHipotesesDiagnosticasCIDCO> lista = new List<DiagnosticosHipotesesDiagnosticasCIDCO>();
            DiagnosticosHipotesesDiagnosticasCIDCO diagnostico = new DiagnosticosHipotesesDiagnosticasCIDCO();
            diagnostico.MostraCID = false;

            if (pAtendimento.Cid.IsNotNull())
            {
                diagnostico.DescricaoCid = "CID: " + pAtendimento.Cid.Id + " " + pAtendimento.Cid.Descricao;
                diagnostico.MostraCID = true;
            }

            lista.Add(diagnostico);

            return lista;
        }

        private List<OutrosCids> OutrosCids(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            OutrosCids cid;
            List<OutrosCids> lista = new List<OutrosCids>();

            foreach (var itemD in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCODiagnostico)
            {
                cid = new OutrosCids();
                cid.Cid = itemD.CID10.Id;
                cid.DescricaoCid = itemD.CID10.Descricao;
                cid.Complemento = itemD.Complemento;
                cid.MostraOutrosCids = true;
                lista.Add(cid);
            }

            if (lista.Count == 0)
            {
                cid = new OutrosCids();
                cid.MostraOutrosCids = false;
                lista.Add(cid);
            }

            return lista;
        }

        private List<HipotesesCO> Hipoteses(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            HipotesesCO hipotese;
            List<HipotesesCO> lista = new List<HipotesesCO>();

            foreach (var itemH in pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOHipotese)
            {
                hipotese = new HipotesesCO();
                hipotese.Hipoteses = itemH.Hipotese;
                hipotese.MostraHipoteses = true;
                lista.Add(hipotese);
            }

            if (lista.Count == 0)
            {
                hipotese = new HipotesesCO();
                hipotese.MostraHipoteses = false;
                lista.Add(hipotese);
            }

            return lista;
        }

        private PlanoDiagnosticoETerapeuticoCO PlanoDiagnosticoETerapeutico(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            PlanoDiagnosticoETerapeuticoCO plano = new PlanoDiagnosticoETerapeuticoCO();
            plano.MostraPlanoDiagnosticoETerapeutico = false;

            plano._listaCondutaCO = PlanoDiagnosticoETerapeuticoCondutaCO(pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano);
            if (plano._listaCondutaCO.FirstOrDefault().MostraConduta)
                plano.MostraPlanoDiagnosticoETerapeutico = true;

            plano._listaCesariaCO = PlanoDiagnosticoETerapeuticoCesariaCO(pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano);
            if (plano._listaCesariaCO.FirstOrDefault().MostraCesaria)
                plano.MostraPlanoDiagnosticoETerapeutico = true;

            plano._listaObservacoesCO = PlanoDiagnosticoETerapeuticoObservacoesCO(pWrpSumarioAvaliacaoMedicaCO);
            if (plano._listaObservacoesCO.FirstOrDefault().MostraObservacoes)
                plano.MostraPlanoDiagnosticoETerapeutico = true;

            return plano;
        }

        private List<PlanoDiagnosticoETerapeuticoCondutaCO> PlanoDiagnosticoETerapeuticoCondutaCO(wrpSumarioAvaliacaoMedicaCOPlano pWrpSumarioAvaliacaoMedicaCOPlano)
        {
            List<PlanoDiagnosticoETerapeuticoCondutaCO> lista = new List<PlanoDiagnosticoETerapeuticoCondutaCO>();
            PlanoDiagnosticoETerapeuticoCondutaCO conduta = new PlanoDiagnosticoETerapeuticoCondutaCO();
            conduta.MostraConduta = false;

            if (pWrpSumarioAvaliacaoMedicaCOPlano.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.AcompanhamentoTrabalhoParto)
                    {
                        conduta.Conduta = pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto.Value.GetEnumCustomDisplay();
                        conduta.MostraConduta = true;
                    }
                }
            }

            lista.Add(conduta);

            return lista;
        }

        private List<PlanoDiagnosticoETerapeuticoCesariaCO> PlanoDiagnosticoETerapeuticoCesariaCO(wrpSumarioAvaliacaoMedicaCOPlano pWrpSumarioAvaliacaoMedicaCOPlano)
        {
            List<PlanoDiagnosticoETerapeuticoCesariaCO> lista = new List<PlanoDiagnosticoETerapeuticoCesariaCO>();
            PlanoDiagnosticoETerapeuticoCesariaCO cesaria = new PlanoDiagnosticoETerapeuticoCesariaCO();
            cesaria.MostraCesaria = false;

            if (pWrpSumarioAvaliacaoMedicaCOPlano.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto.IsNotNull())
                {
                    cesaria.Cesaria = pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto.Value.GetEnumCustomDisplay();

                    if (pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaUrgencia || pWrpSumarioAvaliacaoMedicaCOPlano.TipoParto == TipoPartoCO.CesareaEletiva)
                    {
                        if (pWrpSumarioAvaliacaoMedicaCOPlano.Justificativa.ConvertNullToStringEmpty().IsNotEmpty())
                            cesaria.Cesaria += ": " + pWrpSumarioAvaliacaoMedicaCOPlano.Justificativa;

                        cesaria.MostraCesaria = true;
                    }
                }
            }

            lista.Add(cesaria);

            return lista;
        }

        private List<PlanoDiagnosticoETerapeuticoObservacoesCO> PlanoDiagnosticoETerapeuticoObservacoesCO(wrpSumarioAvaliacaoMedicaCO pWrpSumarioAvaliacaoMedicaCO)
        {
            List<PlanoDiagnosticoETerapeuticoObservacoesCO> lista = new List<PlanoDiagnosticoETerapeuticoObservacoesCO>();
            PlanoDiagnosticoETerapeuticoObservacoesCO observacoes = new PlanoDiagnosticoETerapeuticoObservacoesCO();
            observacoes.MostraObservacoes = false;

            if (pWrpSumarioAvaliacaoMedicaCO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano.Observacao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        observacoes.TituloOutrosObservacoes = "Outros/Observações:";
                        observacoes.ValorOutrosObservacoes = pWrpSumarioAvaliacaoMedicaCO.SumarioAvaliacaoMedicaCOPlano.Observacao;
                        observacoes.MostraObservacoes = true;
                    }
                }
            }

            lista.Add(observacoes);

            return lista;
        }

        #endregion Metodos
    }

    public class SumarioAvaliacaoMedicaRelatorioCO
    {
        public string Nome { get; set; }
        public string Prontuario { get; set; }
        public string Atendimento { get; set; }
        public string DataAtendimento { get; set; }
        public string Sexo { get; set; }
        public string Cor { get; set; }
        public string Idade { get; set; }

        public string NomePaciente { get; set; }
        public string NomeResumo { get; set; }
        public int IDPaciente { get; set; }
        public string NomePrestador { get; set; }
        public string Registro { get; set; }
        public string CodigoBarras { get; set; }

        public bool MostraCodigoBarras { get; set; }
        public bool MostraIDPaciente { get; set; }

        public List<AssinaturaCO> listasrAssinatura { get; set; }
    }

    public class AssinaturaCO
    {
        public string Assinatura { get; set; }
        public string DataEncerramento { get; set; }
    }

    public class AnamneseSumariCO
    {
        public List<GestacaoAtualSumarioCO> _listaGestacaoAtualSumarioCO { get; set; }
        public List<AlergiasCO> listaAlergiasCO { get; set; }
        public List<GestacoesAnterioreSumarioCO> _listaGestacoesAnterioreSumarioCO { get; set; }
        public List<HistoriaPregressaSumarioCO> _listasrHistoriaPregressaSumarioCO { get; set; }
        public List<PerfilPsicoSocialCO> listaPerfilPsicoSocial { get; set; }
        public List<MedicamentosEmUsoCO> _listaMedicamentosEmUsoCO { get; set; }

        public string MotivoInternacaoDescricao { get; set; }
        public string HistoriaAtualDescricao { get; set; }

        public bool Primeiro { get; set; }
        public bool Segundo { get; set; }
    }

    public class GestacaoAtualSumarioCO
    {
        public string Procedencia { get; set; }
        public string Gesta { get; set; }
        public string DataEIdadeGestacionalEco { get; set; }
        public string IdadeGestacional { get; set; }

        public bool MostraGestacaoAtual { get; set; }
        public bool MostraProcedencia { get; set; }
        public bool MostraGesta { get; set; }
        public bool MostraDatasEIdadeGestacionalEco { get; set; }
        public bool MostraIdadeGestacional { get; set; }

        public List<PatologiasSumarioCo> _listaPatologiasSumarioCo { get; set; }
        public List<GestacaoAtualOutrosObservacoesCO> listaGestacaoAtualOutrosObservacoesCO { get; set; }
    }

    public class GestacaoAtualOutrosObservacoesCO
    {
        public string TituloOutrosObservacoes { get; set; }
        public string ValorOutrosObservacoes { get; set; }

        public bool MostraOutrosObservacoes { get; set; }
    }

    public class PatologiasSumarioCo
    {
        public string Nenhuma { get; set; }

        public bool MostraPatologias { get; set; }
        public bool MostraNenhuma { get; set; }

        public List<PatologiasSumarioCoItem> _listaPatologiasSumarioCoItem { get; set; }
    }

    public class PatologiasSumarioCoItem
    {
        public string Descricao { get; set; }
        public string Observacao { get; set; }

        public bool MostraPatologiasItem { get; set; }
    }

    public class AlergiasCO
    {
        public string NenhumaAlergiaConhecida { get; set; }

        public bool MostraAlergias { get; set; }
        public bool MostraNenhumaAlergiaConhecida { get; set; }

        public List<AlergiasItemCO> listaAlergiasItemCO { get; set; }
    }

    public class AlergiasItemCO
    {
        public string Agente { get; set; }
        public string Tipo { get; set; }
        public string DataInicio { get; set; }
        public string Status { get; set; }
        public string Profissional { get; set; }
        public string Comentario { get; set; }

        public bool MostraAlergiasItem { get; set; }
    }

    public class GestacoesAnterioreSumarioCO
    {
        public string IntercorrenciaOuPrimeiraGestacao { get; set; }
        public string ObservacaoGeral { get; set; }

        public bool MostraGestacoesAnteriores { get; set; }
        public bool MostraIntercorrenciaOuPrimeiraGestacao { get; set; }
        public bool MostraObservacoes { get; set; }

        public List<GestacoesAnterioreSumarioCOItem> _listaGestacoesAnterioreSumarioCOItem { get; set; }
    }

    public class GestacoesAnterioreSumarioCOItem
    {
        public string Descricao { get; set; }
        public string Observacao { get; set; }

        public bool MostraGestacoesAnterioreItem { get; set; }
    }

    public class HistoriaPregressaSumarioCO
    {
        public List<HistoriaPregressaSumarioCOItem> _listasrHistoriaPregressaItemSumarioCO { get; set; }
        public string TituloNega { get; set; }
        public string ChNega { get; set; }
        public string ObservacaoGeral { get; set; }

        public bool Primeiro { get; set; }
        public bool Terceiro { get; set; }
    }

    public class HistoriaPregressaSumarioCOItem
    {
        public string Descricao { get; set; }
        public string Observacao { get; set; }
    }

    public class PerfilPsicoSocialCO
    {
        public bool MostraPerfilPsicoSocial { get; set; }

        public List<PscicoSocialSumarioCOItem> _listaPscicoSocialSumarioCOItem { get; set; }
        public List<PerfilPsicoSocialOutrosObservacoesCO> listaOutrosObservacoesCO { get; set; }
    }

    public class PerfilPsicoSocialOutrosObservacoesCO
    {
        public string TituloOutrosObservacoes { get; set; }
        public string ValorOutrosObservacoes { get; set; }

        public bool MostraOutrosObservacoes { get; set; }
    }

    public class PscicoSocialSumarioCOItem
    {
        public string Descricao { get; set; }
        public string Observacao { get; set; }

        public bool MostraPerfilPsicoSocialItem { get; set; }
    }

    public class MedicamentosEmUsoCO
    {
        public string Medicamento { get; set; }

        public bool MostraMedicamentosEmUso { get; set; }
        public bool MostraSemUsoDeMedicamentos { get; set; }

        public List<MedicamentosEmUsoItemCO> _listaMedicamentosEmUsoCOItem { get; set; }
    }

    public class MedicamentosEmUsoItemCO
    {
        public string Medicamento { get; set; }
        public string Dose { get; set; }
        public string Frequencia { get; set; }
        public string Via { get; set; }
        public string Status { get; set; }

        public bool MostraMedicamentosEmUsoItem { get; set; }
    }

    public class ExamesRealizadosSumarioCO
    {
        public bool MostraExamesRealizados { get; set; }

        public List<ExamesLaboratoriaisCO> _listaExamesLaboratoriaisCO { get; set; }
        public List<TipagemSanguineaETesteRapidoCO> _listaTipagemSanguineaETesteRapidoCO { get; set; }
        public List<ExamesrealizadosOutrosCO> _listaExamesrealizadosOutrosCO { get; set; }
    }

    public class TipagemSanguineaETesteRapidoCO
    {
        public string TipagemSanguineaETesteRapido { get; set; }

        public bool MostraTipagemSanguineaETesteRapido { get; set; }
    }

    public class ExamesrealizadosOutrosCO
    {
        public string TituloOutros { get; set; }
        public string DescricaoOutros { get; set; }

        public bool MostraOutros { get; set; }
    }

    public class ExamesLaboratoriaisCO
    {
        public string SorologiaExames { get; set; }
        public string NegNaoRea { get; set; }
        public string PosRea { get; set; }
        public string NaoDisponivel { get; set; }
        public string Trimestre1 { get; set; }
        public string Trimestre2 { get; set; }
        public string Trimestre3 { get; set; }

        public bool MostraExamesLaboratoriais { get; set; }
    }

    public class ExamesFisicosCO
    {
        public string DinamicaToqueVaginalESangramento { get; set; }
        public string MembranasDataHoraELiquidoAmniotico { get; set; }

        public string TituloLiquidoAmniotico { get; set; }
        public string ChLiquidoAmniotico { get; set; }
        public string DsLiquidoAmniotico { get; set; }
        public string LiquidoAmnioticoOutros { get; set; }

        public bool MostraSrExamesFisicos { get; set; }
        public bool MostraDinamicaToqueVaginalESangramento { get; set; }
        public bool MostraMembranasDataHoraELiquidoAmniotico { get; set; }

        public List<ExamesFisicosObservacoesCO> _listaExamesFisicosObservacoesCO { get; set; }
        public List<SinaisVitaisEAntropometricosCO> listaSinaisVitaisEAntropometricos { get; set; }
    }

    public class SinaisVitaisEAntropometricosCO
    {
        public string Sinais { get; set; }
        public string DinamicaToqueVaginalESangramento { get; set; }
        public string MembranasDataHoraELiquidoAmniotico { get; set; }

        public bool MostraSinaisVitaisEAntropometricos { get; set; }
    }

    public class ExamesFisicosObservacoesCO
    {
        public string TituloObservacoes { get; set; }
        public string DescricaoObservacao { get; set; }

        public bool MostraObservacoes { get; set; }
    }

    public class DiagnosticosCO
    {
        public bool MostraSrDiagnostico { get; set; }

        public List<OutrosCids> _listaOutrosCidsCO { get; set; }
        public List<HipotesesCO> _listaHipotesesCO { get; set; }
        public List<DiagnosticosHipotesesDiagnosticasCIDCO> _listaDiagnosticosHipotesesDiagnosticasCIDCO { get; set; }
    }

    public class DiagnosticosHipotesesDiagnosticasCIDCO
    {
        public string DescricaoCid { get; set; }

        public bool MostraCID { get; set; }
    }

    public class OutrosCids
    {
        public string Cid { get; set; }
        public string DescricaoCid { get; set; }
        public string Complemento { get; set; }

        public bool MostraOutrosCids { get; set; }
    }

    public class HipotesesCO
    {
        public string Hipoteses { get; set; }

        public bool MostraHipoteses { get; set; }
    }

    public class PlanoDiagnosticoETerapeuticoCO
    {
        public string TituloJustificativaCesaria { get; set; }
        public string ValorJustificativaCesaria { get; set; }

        public string TituloOutrosObservacoes { get; set; }
        public string ValorOutrosObservacoes { get; set; }

        public bool MostraPlanoDiagnosticoETerapeutico { get; set; }

        public List<PlanoDiagnosticoETerapeuticoCondutaCO> _listaCondutaCO { get; set; }
        public List<PlanoDiagnosticoETerapeuticoCesariaCO> _listaCesariaCO { get; set; }
        public List<PlanoDiagnosticoETerapeuticoObservacoesCO> _listaObservacoesCO { get; set; }
    }

    public class PlanoDiagnosticoETerapeuticoCondutaCO
    {
        public string Conduta { get; set; }

        public bool MostraConduta { get; set; }
    }

    public class PlanoDiagnosticoETerapeuticoCesariaCO
    {
        public string Cesaria { get; set; }

        public bool MostraCesaria { get; set; }
    }

    public class PlanoDiagnosticoETerapeuticoObservacoesCO
    {
        public string TituloOutrosObservacoes { get; set; }
        public string ValorOutrosObservacoes { get; set; }

        public bool MostraObservacoes { get; set; }
    }
}
