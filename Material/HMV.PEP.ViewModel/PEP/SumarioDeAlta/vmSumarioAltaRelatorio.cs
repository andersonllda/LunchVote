using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Wrappers;
using HMV.Core.Domain.Model;
using System.Collections;
using System.Windows.Data;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Framework.Extensions;
using System.Configuration;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Enum.CentroObstetrico;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmSumarioAltaRelatorio
    {
        private wrpSumarioAlta _sumarioalta { get; set; }
        private wrpUsuarios _usuarios { get; set; }
        private wrpDadosNascimento _dadosNascimento { get; set; }
        private bool mostraTransferencia { get; set; }

        public List<SumarioAltaRelatorio> ListaSumarioAlta { get; set; }

        public vmSumarioAltaRelatorio(wrpSumarioAlta pSumario, wrpDadosNascimento pDadosNascimento, Usuarios pUsuario, bool pMostraTransferencia)
        {
            _sumarioalta = pSumario;
            _dadosNascimento = pDadosNascimento;
            _usuarios = new wrpUsuarios(pUsuario);

            ListaSumarioAlta = new List<SumarioAltaRelatorio>();
            SumarioAltaRelatorio sumario = new SumarioAltaRelatorio() { MostraSumarioAlta = false };

             if (_sumarioalta.IsNotNull())
            {
                if (_sumarioalta.Atendimento.IsNotNull())
                {
                    sumario.AtendimentoAmbulatorial = (_sumarioalta.Atendimento.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Ambulatorio);

                    mostraTransferencia = pMostraTransferencia;

                    if (IsRN)
                    {
                        sumario.Titulo = "Sumário de Alta do Recém-Nascido";
                        sumario.TituloRodape = "Sumário de Alta do Recém-Nascido";
                    }
                    else if (_sumarioalta.Atendimento.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Ambulatorio)
                    {
                        sumario.Titulo = "Sumário de Alta Ambulatorial";
                        sumario.TituloRodape = "Sumário de Alta Ambulatorial";
                    }
                    else
                    {
                        sumario.Titulo = "Sumário de Alta";
                        sumario.TituloRodape = "Sumário de Alta";
                    }

                    if (_sumarioalta.Atendimento.Paciente.IsNotNull())
                    {
                        sumario.NomePacienteCodigoBarras = _sumarioalta.Atendimento.Paciente.Nome;
                        sumario.LeitoCodigoBarras = _sumarioalta.Atendimento.Leito.IsNotNull() ? _sumarioalta.Atendimento.Leito.Descricao : string.Empty;
                        sumario.ProntuarioCodigoBarras = _sumarioalta.Atendimento.Paciente.ID != 0 ? _sumarioalta.Atendimento.Paciente.ID.ToString() : string.Empty;
                        sumario.PrestadorCodigoBarras = _sumarioalta.Atendimento.Prestador.Nome;
                        sumario.RegistroCodigoBarras = _sumarioalta.Atendimento.Prestador.Registro;
                        sumario.AtendimentoCodigoBarras = _sumarioalta.Atendimento.ID.ToString();

                        sumario.Nome = _sumarioalta.Atendimento.Paciente.Nome;

                        if (!IsRN)                       
                        {
                            sumario.Idade = _sumarioalta.Idade.GetDate(_sumarioalta.Atendimento.HoraAtendimento);
                        }

                        sumario.DataNascimento = _sumarioalta.Atendimento.Paciente.DataNascimento.IsNotNull() ? _sumarioalta.Atendimento.Paciente.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty;

                        if (_sumarioalta.Atendimento.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Ambulatorio)
                            sumario.DataInternacao = _sumarioalta.Atendimento.DataAtendimento.IsNotNull() ? _sumarioalta.Atendimento.DataAtendimento.ToString("dd/MM/yyyy") + "   Hora: " + _sumarioalta.Atendimento.HoraAtendimento.ToString("HH:mm") : string.Empty;
                        else
                            sumario.DataInternacao = _sumarioalta.Atendimento.DataAtendimento.IsNotNull() ? _sumarioalta.Atendimento.DataAtendimento.ToString("dd/MM/yyyy") : string.Empty;

                        sumario.Sexo = _sumarioalta.Atendimento.Paciente.Sexo.IsNotNull() ? _sumarioalta.Atendimento.Paciente.Sexo.ToString() : string.Empty;
                        sumario.DataAlta = _sumarioalta.DataAlta.IsNotNull() ? _sumarioalta.DataAlta.Value.ToString("dd/MM/yyyy") : string.Empty;
                        sumario.Registro = _sumarioalta.Atendimento.Paciente.ID.ToString();
                        sumario.HoraAlta = _sumarioalta.HoraAlta.IsNotNull() ? _sumarioalta.HoraAlta.Value.ToString("HH:mm") : string.Empty;
                        sumario.Atendimento = _sumarioalta.Atendimento.ID.ToString();

                        if (_sumarioalta.Atendimento.DataAtendimento == null || _sumarioalta.DataAlta == null)
                            sumario.Permanencia = string.Empty;
                        else
                        {
                            TimeSpan diff = _sumarioalta.DataAlta.Value.Subtract(_sumarioalta.Atendimento.DataAtendimento);
                            sumario.Permanencia = diff.Days > 1 ? diff.Days.ToString() + " Dias" : diff.Days.ToString() + " Dia";
                        }
                    }

                    // Médico Assistente
                    sumario.ListaMedicoAssistente = _MedicoAssistente();

                    // Condições Motivo Internação
                    sumario.ListaCondicaoMotivoInternacao = _CondicaoMotivoInternacao();

                    // Condições de Alta
                    sumario.ListaCondicaoAlta = _CondicaoAlta();

                    // Diagnósticos
                    sumario.ListaDiagnosticos = _Diagnostico();

                    // Causas Externas
                    sumario.ListaCausaExternas = _CausaExterna();

                    // Dados do Nascimento
                    sumario.ListaDadosDoNascimento = _DadosDoNascimento();

                    // Procedimentos
                    sumario.ListaProcedimento = _Procedimentos();

                    // Farmacos
                    sumario.ListaFarmacos = _Farmacos();

                    // Farmacos Observação
                    sumario.ListaFarmacosObservacoes = _FarmacosObservacao();

                    // Evolução
                    sumario.ListaEvolucao = _Evolucao();

                    // Medicamentos Pós Alta e Plano Pós Alta
                    if (IsObito)
                    {
                        sumario.ListaMedicamentosPosAlta = new List<MedicamentosPosAltaSA>() { new MedicamentosPosAltaSA() { MostraMedicamentosPosAlta = false } };
                        sumario.ListaPlanoPosAlta = new List<PlanoPosAltaSA>() { new PlanoPosAltaSA() { MostraPlanoPosAlta = false } };
                    }
                    else //if (IsInternado)
                    {
                        sumario.ListaMedicamentosPosAlta = new List<MedicamentosPosAltaSA>() { new MedicamentosPosAltaSA() { MostraMedicamentosPosAlta = false } };
                        sumario.ListaPlanoPosAlta = _PlanoPosAlta();
                    }
                    //else
                    //{
                    //sumario.ListaMedicamentosPosAlta = _MedicamentosPosAlta();
                    //sumario.ListaPlanoPosAlta = new List<PlanoPosAltaSA>() { new PlanoPosAltaSA() { MostraPlanoPosAlta = false } };
                    //}

                    // Exames
                    sumario.ListaExame = _Exame();

                    // Recomendações
                    sumario.ListaRecomendacoes = _Recomendacoes();

                    // Consulta Médica
                    sumario.ListaConsultaMedica = _ConsultaMedica();

                    // Consulta Urgência
                    sumario.ListaConsultaUrgencia = _ConsultaUrgencia();

                    // Transferência
                    sumario.ListaTransferencia = _Transferencia();

                    // Assinatura
                    sumario.Assinatura = _Assinatura();

                    ListaSumarioAlta.Add(sumario);
                }
            }
        }

        #region Métodos Privados

        private List<MedicoAssistenteSA> _MedicoAssistente()
        {
            if (_sumarioalta.Prestadores.Count() == 0)
                return new List<MedicoAssistenteSA>() { new MedicoAssistenteSA() { MostraMedicoAssistente = false } };

            List<MedicoAssistenteSA> qry = (from T in _sumarioalta.Prestadores
                                            select new MedicoAssistenteSA()
                                              {
                                                  Titulo = _sumarioalta.Atendimento.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Internacao ? "Médico Assistente" : "Médico/Cirurgião",
                                                  Nome = (T.Nome).ToUpper(),
                                                  CRM = T.Registro,
                                                  MostraMedicoAssistente = true
                                              }).ToList();

            if (qry.Count == 0)
                return new List<MedicoAssistenteSA>() { new MedicoAssistenteSA() { MostraMedicoAssistente = false } };

            return qry;
        }

        private List<CondicaoMotivoInternacaoSA> _CondicaoMotivoInternacao()
        {
            CondicaoMotivoInternacaoSA motivo = new CondicaoMotivoInternacaoSA() { MostraCondicaoMotivoInternacao = false };

            IRepositorioDeSumarioAlta repsum = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
            var ret = repsum.BuscaMotivoInternacaoSumarios(_sumarioalta.Atendimento.ID);

            if (ret.IsNotEmptyOrWhiteSpace())
            {
                motivo.Descricao = _CondicaoInternacao;
                motivo.MostraCondicaoMotivoInternacao = true;
            }

            return new List<CondicaoMotivoInternacaoSA>() { motivo };
        }

        private List<CondicaoAltaSA> _CondicaoAlta()
        {
            CondicaoAltaSA condicao = new CondicaoAltaSA() { MostraCondicaoAlta = false };

            if (_sumarioalta.Atendimento.MotivoAlta != null)
                condicao.Descricao = _sumarioalta.Atendimento.MotivoAlta.Descricao;
            else if (!MotivoAltaRelatorio.DescMotivoAltaRelatorio.IsEmpty())
                condicao.Descricao = MotivoAltaRelatorio.DescMotivoAltaRelatorio;
            else if (_sumarioalta.MotivoAltaDiaSeguinte != null)
                condicao.Descricao = _sumarioalta.MotivoAltaDiaSeguinte.Descricao;
            else
                condicao.Descricao = string.Empty;

            if (condicao.Descricao.ConvertNullToStringEmpty().IsNotEmpty())
                condicao.MostraCondicaoAlta = true;

            return new List<CondicaoAltaSA>() { condicao };
        }

        private List<DiagnosticosSA> _Diagnostico()
        {
            List<DiagnosticosSA> diagnosticos = new List<DiagnosticosSA>();
            DiagnosticosSA diagnostico = new DiagnosticosSA() { MostraDiagnosticos = false };

            if (_sumarioalta.CIDPrincipal.IsNotNull() && !_sumarioalta.Atendimento.DataAltaMedica.HasValue)
                diagnostico.ListaCidPrincipal = new List<CIDPrincipalSA>() { new CIDPrincipalSA() { Descricao = _sumarioalta.CIDPrincipal.CidMV.Id + " - " + _sumarioalta.CIDPrincipal.CidMV.Descricao } };
            else
                diagnostico.ListaCidPrincipal = new List<CIDPrincipalSA>() { new CIDPrincipalSA() { Descricao = _sumarioalta.Atendimento.Cid.IsNull() ? string.Empty : _sumarioalta.Atendimento.Cid.CidMV.Id + " - " + _sumarioalta.Atendimento.Cid.CidMV.Descricao } };

            // Outros CIDs
            diagnostico.ListaOutrosCIDs = _OutrosCids();
            if (diagnostico.ListaOutrosCIDs.FirstOrDefault().MostraOutrosCids) { diagnostico.MostraDiagnosticos = true; }

            diagnosticos.Add(diagnostico);

            return diagnosticos;
        }

        private List<OutrosCIDs> _OutrosCids()
        {
            var ret = (from T in _sumarioalta.Atendimento.Paciente.ProblemasPaciente.Where(x => x.CID.IsNotNull() 
                        && (_sumarioalta.CIDPrincipal.IsNotNull() && x.CID.IsNotNull() && x.CID.Id != _sumarioalta.CIDPrincipal.Id) 
                        && x.Status.Equals(StatusAlergiaProblema.Ativo) && x.Atendimento.IsNotNull() 
                        && x.Atendimento.ID.Equals(_sumarioalta.Atendimento.ID))
                        select new OutrosCIDs { Descricao = T.CID.Id + " - " + T.CID.Descricao, MostraOutrosCids = true }).ToList();
            if (ret.HasItems())
                return ret;
            else
                return new List<OutrosCIDs>() { new OutrosCIDs() { MostraOutrosCids = false } };
        }

        private List<CausaExternasSA> _CausaExterna()
        {
            //IList<CausaExternas> qry = null;
            //        if (_sumarioalta.CausaExterna.Count() == 0)
            //        {
            //            if (_sumarioalta.SemCausaExterna == Core.Domain.Enum.SimNao.Sim)
            //            {
            //                qry = new List<CausaExternas>();
            //                qry.Add(new CausaExternas() { Descricao = "Não se Aplica", Observacao = "" });
            //            }
            //        }
            //        else
            //        {
            //            qry = (from T in _sumarioalta.CausaExterna
            //                   select new CausaExternas
            //                  {
            //                      Observacao = T.Observacao,
            //                      Descricao = T.Cid.Descricao
            //                  }).ToList();
            //        }
            //        return qry;

            if (IsInternado && !IsRN)
            {
                if (_sumarioalta.CausaExterna.Count() == 0)
                {
                    if (_sumarioalta.SemCausaExterna == SimNao.Sim)
                        return new List<CausaExternasSA>() { new CausaExternasSA() { Descricao = "Não se Aplica", Observacao = string.Empty, MostraCausaExternas = true } };
                    else
                        return new List<CausaExternasSA>() { new CausaExternasSA() { MostraCausaExternas = false } };
                }
                else
                    return (from T in _sumarioalta.CausaExterna select new CausaExternasSA { Descricao = T.Cid.Descricao, Observacao = T.Observacao, MostraCausaExternas = true }).ToList();
            }
            else
                return new List<CausaExternasSA>() { new CausaExternasSA() { MostraCausaExternas = false } };
        }

        private List<DadosDoNascimentoSA> _DadosDoNascimento()
        {
            if (IsRN)
                if (_MontaDadosDoNascimento().MostraDadosNascimento)
                    return new List<DadosDoNascimentoSA>() { _MontaDadosDoNascimento() };

            return new List<DadosDoNascimentoSA>() { new DadosDoNascimentoSA() { MostraDadosNascimento = false } };
        }

        private DadosDoNascimentoSA _MontaDadosDoNascimento()
        {
            DadosDoNascimentoSA dados = new DadosDoNascimentoSA() { MostraDadosNascimento = false };
            if (_dadosNascimento.IsNotNull())
            {
                dados = new DadosDoNascimentoSA();
                dados.MostraDadosNascimento = false;

                if (_dadosNascimento.DomainObject.DataNascimento.IsNotNull())
                {
                    //dados.TituloData = "Data do Nascimento";
                    dados.Data = _dadosNascimento.DomainObject.DataNascimento.Value.ToShortDateString();
                    //dados.TituloHora = "Hora";
                    dados.Hora = _dadosNascimento.DomainObject.DataNascimento.Value.ToShortTimeString();
                    dados.MostraDadosNascimento = true;
                }

                //dados.TipagemSanguinea = _dadosNascimento.TipagemPaciente.IsNotNull() ? _dadosNascimento.TipagemPaciente.Value.GetEnumCustomDisplay() : string.Empty;
                //dados.TipagemSanguinea += _dadosNascimento.RHPaciente.IsNotNull() ? _dadosNascimento.RHPaciente.Value.GetEnumDescription() : string.Empty;

                if (_dadosNascimento.TipagemPaciente.IsNotNull())
                {
                    dados.TipagemSanguinea = _dadosNascimento.TipagemPaciente.Value.GetEnumCustomDisplay();
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.RHPaciente.IsNotNull())
                {
                    dados.TipagemSanguinea += _dadosNascimento.RHPaciente.Value.GetEnumDescription();
                    dados.MostraDadosNascimento = true;
                }

                //if (_dadosNascimento.TipoParto.IsNotNull())
                //    dados.TipoDeParto = _dadosNascimento.MotivoCesarianaUrgencia.ConvertNullToStringEmpty().IsNotEmpty() ? (String.Format("{0}    Motivo: {1}", _dadosNascimento.TipoParto.Value.GetEnumCustomDisplay(), _dadosNascimento.MotivoCesarianaUrgencia)) : _dadosNascimento.TipoParto.Value.GetEnumCustomDisplay();

                if (_dadosNascimento.TipoParto.IsNotNull())
                {
                    dados.TipoDeParto = _dadosNascimento.TipoParto.Value.GetEnumCustomDisplay();
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.MotivoCesarianaUrgencia.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    dados.TipoDeParto += "    Motivo: " + _dadosNascimento.MotivoCesarianaUrgencia;
                    dados.MostraDadosNascimento = true;
                }

                //if (dados.TipoDeParto.IsNotEmpty())
                //{
                //    dados.TituloTipoDeParto = "Tipo de Parto";
                //    dados.MostraDadosNascimento = true;
                //}

                if (_dadosNascimento.Membrana.IsNotNull())
                {
                    //dados.TituloMembranasAmnioticas = "Membranas Amnióticas";
                    dados.MembranasAmnioticas = _dadosNascimento.Membrana.Value.GetEnumCustomDisplay();
                    dados.MostraDadosNascimento = true;
                }

                dados.TituloDataMembranasAmnioticas = string.Empty;
                dados.TituloHoraMembranasAmnioticas = string.Empty;
                if (_dadosNascimento.DataMembrana.IsNotNull())
                {
                    dados.TituloDataMembranasAmnioticas = "Data";
                    dados.DataMembranasAmnioticas = _dadosNascimento.DataMembrana.Value.ToShortDateString();
                    dados.TituloHoraMembranasAmnioticas = "Hora";
                    dados.HoraMembranasAmnioticas = _dadosNascimento.DataMembrana.Value.ToShortTimeString();
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.LiquidoAmniotico.IsNotNull())
                {
                    //dados.TituloLiquidoAmniotico = "Líquido Amniótico";
                    dados.LiquidoAmniotico = _dadosNascimento.LiquidoAmnioticoObservacao.ConvertNullToStringEmpty().IsNotEmpty() ? _dadosNascimento.LiquidoAmnioticoObservacao : _dadosNascimento.LiquidoAmniotico.Value.GetEnumCustomDisplay();
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.IsIdadeDesconhecido.IsNotNull())
                    if (_dadosNascimento.IsIdadeDesconhecido.Value == SimNao.Sim)
                    {
                        //dados.TituloIdadeGestacional = "Idade Gestacional";
                        dados.IdadeGestacional = "Desconhecida";
                    }

                //dados.TituloIdadeGestacional = "Idade Gestacional";
                if (_dadosNascimento.IdadeSemanas.IsNotNull())
                    dados.IdadeGestacional = String.Format("{0} semanas", _dadosNascimento.IdadeSemanas);

                if (_dadosNascimento.IdadeDias.IsNotNull())
                    if (dados.IdadeGestacional.ConvertNullToStringEmpty().IsNotEmpty())
                        dados.IdadeGestacional += String.Format(" e {0} dias", _dadosNascimento.IdadeDias);
                    else
                        dados.IdadeGestacional = _dadosNascimento.IdadeDias + " dias";

                //if (dados.IdadeGestacional.IsNotEmpty())
                //{
                //    dados.TituloIdadeGestacional = "Idade Gestacional";
                //    dados.MostraDadosNascimento = true;
                //}

                if (_dadosNascimento.Classificacao.IsNotNull())
                {
                    //dados.TituloClassificacao = "Classificação";
                    dados.Classificacao = _dadosNascimento.Classificacao.Value.GetEnumCustomDisplay();
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.Peso.IsNotNull())
                {
                    //dados.TituloPeso = "Peso de Nascimento";
                    dados.Peso = _dadosNascimento.Peso.Value + " g";
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.Comprimento.IsNotNull())
                {
                    //dados.TituloComprimento = "Comprimento";
                    dados.Comprimento = _dadosNascimento.Comprimento + " cm";
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.PerimentroCefalico.IsNotNull())
                {
                    //dados.TituloPerimetroCefalico = "Perímetro Cefálico";
                    dados.PerimetroCefalico = _dadosNascimento.PerimentroCefalico.Value + " cm";
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.PerimetroToracico.IsNotNull())
                {
                    //dados.TituloPerimetroToracico = "Perímetro Torácico";
                    dados.PerimetroToracico = _dadosNascimento.PerimetroToracico.Value + " cm";
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.ApgarPrimeiro.IsNotNull())
                {
                    //dados.TituloApgar = "APGAR";
                    dados.Apgar = "1º " + _dadosNascimento.ApgarPrimeiro.Value;
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.ApgarQuinto.IsNotNull())
                {
                    //dados.TituloApgar = "APGAR";
                    if (dados.Apgar.ConvertNullToStringEmpty().IsNotEmpty())
                        dados.Apgar += String.Format("    5º {0}", _dadosNascimento.ApgarQuinto.Value);
                    else
                        dados.Apgar = "5º " + _dadosNascimento.ApgarQuinto.Value;
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.ApgarDessimo.IsNotNull())
                {
                    //dados.TituloApgar = "APGAR";
                    if (dados.Apgar.ConvertNullToStringEmpty().IsNotEmpty())
                        dados.Apgar += String.Format("    10º {0}", _dadosNascimento.ApgarDessimo.Value);
                    else
                        dados.Apgar = "5º " + _dadosNascimento.ApgarDessimo.Value;
                    dados.MostraDadosNascimento = true;
                }

                if (_dadosNascimento.TesteOlhinho.IsNotNull())
                {
                    if (_dadosNascimento.TesteOlhinho.Value == SimNao.Sim)
                        dados.TesteOlhinho = "Normal";
                }

                if (_dadosNascimento.TesteOlhinhoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    if (dados.TesteOlhinho.IsNotEmpty())
                        dados.TesteOlhinho += ": " + _dadosNascimento.TesteOlhinhoObservacao;
                    else
                        dados.TesteOlhinho = _dadosNascimento.TesteOlhinhoObservacao;
                }

                //if (dados.TesteOlhinho.IsNotEmpty())
                //{
                //    dados.TituloTesteOlhinho = "Teste do Olhinho";
                //    dados.MostraDadosNascimento = true;
                //}
            }

            return dados;
        }

        private List<ProcedimentoSA> _Procedimentos()
        {
            if (_sumarioalta.ProcedimentosAlta.Count() == 0)
            {
                if (_sumarioalta.SemProcedimentoInvasivo == SimNao.Sim)
                    return new List<ProcedimentoSA>() { new ProcedimentoSA() { Descricao = "Não Foram realizados procedimentos invasivos durante a internação", MostraProcedimentos = true } };
            }
            else
                return (from T in _sumarioalta.ProcedimentosAlta orderby T.Cirurgia.ds_cirurgia select new ProcedimentoSA() { Descricao = (T.Cirurgia.ds_cirurgia).ToUpper() + "     " + T.Data.ToShortDateString(), MostraProcedimentos = true }).ToList();

            return new List<ProcedimentoSA>() { new ProcedimentoSA() { MostraProcedimentos = false } };
        }

        private List<FarmacosSA> _Farmacos()
        {
            if (_sumarioalta.Farmacos.Count() == 0)
            {
                if (_sumarioalta.SemFarmaco == Core.Domain.Enum.SimNao.Sim)
                    return new List<FarmacosSA>() { new FarmacosSA() { Descricao = "Paciente não recebeu medicamentos durante internação", MostraFarmacos = true } };
                else
                    return new List<FarmacosSA>() { new FarmacosSA() { MostraFarmacos = false } };
            }
            else
                return (from T in _sumarioalta.Farmacos select new FarmacosSA() { Descricao = T.Produto.Descricao != null ? T.Produto.Descricao : "Não há itens", MostraFarmacos = true }).ToList();
        }

        private List<FarmacosObservacaoSA> _FarmacosObservacao()
        {
            if (_sumarioalta.FarmacoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                return new List<FarmacosObservacaoSA>() { new FarmacosObservacaoSA() { Descricao = _sumarioalta.FarmacoObservacao, MostraObservacao = true } };
            else
                return new List<FarmacosObservacaoSA>() { new FarmacosObservacaoSA() { MostraObservacao = false } };
        }

        private List<EvolucaoSA> _Evolucao()
        {
            if (_sumarioalta.SemEvolucao == SimNao.Sim && _sumarioalta.Atendimento.TipoDeAtendimento != Core.Domain.Enum.TipoAtendimento.Internacao)
                return new List<EvolucaoSA>() { new EvolucaoSA() { Descricao = "Paciente evoluiu sem intercorrências significativas", MostraEvolucao = true } };
            else if (_sumarioalta.Evolucao.ConvertNullToStringEmpty().IsNotEmpty())
                return new List<EvolucaoSA>() { new EvolucaoSA() { Descricao = _sumarioalta.Evolucao, MostraEvolucao = true } };
            else
                return new List<EvolucaoSA>() { new EvolucaoSA() { MostraEvolucao = false } };
        }

        private List<PlanoPosAltaSA> _PlanoPosAlta()
        {
            PlanoPosAltaSA plano = new PlanoPosAltaSA();
            plano.MostraPlanoPosAlta = false;
            plano.MostraNaoSeAplica = false;

            if (_sumarioalta.PlanoPosAlta.Count() == 0)
            {
                if (_sumarioalta.SemMedPosAlta == SimNao.Sim)
                {
                    plano.Descricao = "Não há prescrição de medicamentos no pós-alta";
                    plano.MostraPlanoPosAlta = true;
                    plano.MostraNaoSeAplica = true;
                    plano.ListaPlanoPosAltaItem = new List<PlanoPosAltaItemSA>() { new PlanoPosAltaItemSA() { MostraPlanoPosAltaItem = false } };
                }
                else
                {
                    plano.MostraPlanoPosAlta = false;
                    plano.MostraNaoSeAplica = false;
                    plano.ListaPlanoPosAltaItem = new List<PlanoPosAltaItemSA>() { new PlanoPosAltaItemSA() { MostraPlanoPosAltaItem = false } };
                }
            }
            else
            {
                plano.MostraPlanoPosAlta = true;
                plano.MostraNaoSeAplica = false;
                plano.ListaPlanoPosAltaItem = (from T in _sumarioalta.PlanoPosAlta
                                               select new PlanoPosAltaItemSA
                                              {
                                                  Nome = T.Produto != null ? T.Produto.Descricao : (T.TipoPrescricaoMedica != null ? T.TipoPrescricaoMedica.Descricao : T.NomeComercial),
                                                  Dose = T.Dose,
                                                  Via = T.Via,
                                                  Frequencia = T.Frequencia,
                                                  Tempo = T.Tempo,
                                                  MostraPlanoPosAltaItem = true
                                              }).ToList();
            }

            return new List<PlanoPosAltaSA>() { plano };
        }

        private List<MedicamentosPosAltaSA> _MedicamentosPosAlta()
        {
            List<MedicamentosPosAltaSA> qry = null;
            if (_sumarioalta.PlanoPosAlta.Count() == 0)
            {
                if (_sumarioalta.SemMedPosAlta == Core.Domain.Enum.SimNao.Sim)
                {
                    qry = new List<MedicamentosPosAltaSA>();
                    qry.Add(new MedicamentosPosAltaSA() { Nome = "Não há prescrição de medicamentos no pós-alta" });
                }
            }
            else
            {
                qry = (from T in _sumarioalta.PlanoPosAlta
                       select new MedicamentosPosAltaSA
                       {
                           Nome = T.Produto != null ? T.Produto.Descricao : (T.TipoPrescricaoMedica != null ? T.TipoPrescricaoMedica.Descricao : T.NomeComercial),
                           Dose = T.Dose,
                           Via = T.Via,
                           Frequencia = T.Frequencia,
                           Tempo = T.Tempo
                       }).ToList();
            }
            return qry;
        }

        private List<ExameSA> _Exame()
        {
            ExameSA exame = new ExameSA() { MostraExame = false };

            if ((_sumarioalta.SumarioExames.Count() == 0) && (this._sumarioalta.ExameObservacao.IsEmptyOrWhiteSpace()))
            {
                exame.ListaExameObservacao = new List<ExameObservacaoSA>() { new ExameObservacaoSA() { MostraExameObservacao = false } };
                exame.ListaAnalisesClinicas = new List<AnalisesClinicasSA>() { new AnalisesClinicasSA() { MostraAnalisesClinicas = false } };
                exame.ListaExameItem = new List<ExameItemSA>() { new ExameItemSA() { MostraExameItem = false } };

                if (_sumarioalta.SemParticularidadeExames == SimNao.Sim)
                {
                    exame.Descricao = "Exames sem particularidades";
                    exame.MostraExame = true;
                    exame.MostraExameDescricao = true;
                }
                else if (_sumarioalta.SemExamesRealizados == SimNao.Sim)
                {
                    exame.Descricao = "Não foram realizados exames durante a internação";
                    exame.MostraExame = true;
                    exame.MostraExameDescricao = true;
                }
                else
                    return new List<ExameSA>() { exame };
            }
            else if ((_sumarioalta.SumarioExames.Count() == 0) && (!this._sumarioalta.ExameObservacao.IsEmptyOrWhiteSpace()))
            {
                exame.ListaExameObservacao = new List<ExameObservacaoSA>() { new ExameObservacaoSA() { MostraExameObservacao = false } };
                exame.ListaAnalisesClinicas = new List<AnalisesClinicasSA>() { new AnalisesClinicasSA() { MostraAnalisesClinicas = false } };
                exame.ListaExameItem = new List<ExameItemSA>() { new ExameItemSA() { MostraExameItem = false } };

                if (_sumarioalta.SemParticularidadeExames == SimNao.Sim)
                {
                    exame.Descricao = "Exames sem particularidades" + Environment.NewLine + _sumarioalta.ExameObservacao;
                    exame.MostraExame = true;
                    exame.MostraExameDescricao = true;
                }
                else if (_sumarioalta.SemExamesRealizados == SimNao.Sim)
                {
                    exame.Descricao = "Não foram realizados exames durante a internação" + Environment.NewLine + _sumarioalta.ExameObservacao;
                    exame.MostraExame = true;
                    exame.MostraExameDescricao = true;
                }
                else if (_sumarioalta.ExameObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    exame.Descricao = _sumarioalta.ExameObservacao;
                    exame.MostraExame = true;
                    exame.MostraExameDescricao = true;
                }

                return new List<ExameSA>() { exame };
            }
            else
            {
                exame.ListaExameItem = (from T in _sumarioalta.SumarioExames
                                        select new ExameItemSA()
                                        {
                                            Resultado = T.Observacao == null ? string.Empty : T.Observacao,
                                            Procedimento = T.Procedimento.Descricao,
                                            MostraExameItem = true
                                        }).ToList();

                if (exame.ListaExameItem.FirstOrDefault().MostraExameItem)
                    exame.MostraExame = true;

                if (_sumarioalta.ExameObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    exame.ListaExameObservacao = new List<ExameObservacaoSA>() { new ExameObservacaoSA() { Observacao = _sumarioalta.ExameObservacao, MostraExameObservacao = true } };
                    exame.MostraExame = true;
                }
                else
                    exame.ListaExameObservacao = new List<ExameObservacaoSA>() { new ExameObservacaoSA() { MostraExameObservacao = false } };

                if (_sumarioalta.AnaliseClinica.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    exame.ListaAnalisesClinicas = new List<AnalisesClinicasSA>() { new AnalisesClinicasSA() { Descricao = _sumarioalta.AnaliseClinica, MostraAnalisesClinicas = true } };
                    exame.MostraExame = true;
                }
                else
                    exame.ListaAnalisesClinicas = new List<AnalisesClinicasSA>() { new AnalisesClinicasSA() { MostraAnalisesClinicas = false } };
            }

            return new List<ExameSA>() { exame };
        }

        private List<RecomendacoesSA> _Recomendacoes()
        {
            RecomendacoesSA recomendacao = new RecomendacoesSA() { MostraRecomendacoes = false, MostraSemRecomendacoes = false };

            if (_sumarioalta.recomendacoes.Count() == 0)
            {
                if (_sumarioalta.SemRecomendacao == SimNao.Sim)
                {
                    recomendacao.SemRecomendacoes = "Não há recomendação especifica";
                    recomendacao.MostraRecomendacoes = true;
                    recomendacao.MostraSemRecomendacoes = true;
                    recomendacao.ListaRecomendacoesItem = new List<RecomendacoesItemSA>() { new RecomendacoesItemSA() { MostraRecomendacoesItem = false } };
                }
                else
                {
                    recomendacao.ListaRecomendacoesItem = new List<RecomendacoesItemSA>() { new RecomendacoesItemSA() { MostraRecomendacoesItem = false } };
                }
            }
            else
            {
                recomendacao.ListaRecomendacoesItem = _RecomendacoesItem();
                if (recomendacao.ListaRecomendacoesItem.HasItems())
                    if (recomendacao.ListaRecomendacoesItem.FirstOrDefault().MostraRecomendacoesItem)
                        recomendacao.MostraRecomendacoes = true;
            }

            return new List<RecomendacoesSA>() { recomendacao };
        }

        private List<RecomendacoesItemSA> _RecomendacoesItem()
        {
            List<RecomendacoesItemSA> qry = new List<RecomendacoesItemSA>();
            qry = (from T in _sumarioalta.recomendacoes where T.Descricao.ConvertNullToStringEmpty().IsNotEmpty() orderby T.Recomendacao.Descricao select new RecomendacoesItemSA { Descricao = T.Descricao, Recomendacao = T.Recomendacao.Descricao }).ToList();

            if (qry.Count > 0)
                qry.FirstOrDefault().MostraRecomendacoesItem = true;

            return qry;
        }

        private List<ExamesDiagnosticosSA> _ExamesDiagnosticos()
        {
            List<ExamesDiagnosticosSA> qry = null;

            if ((_sumarioalta.SumarioExames.Count() == 0) && (this._sumarioalta.ExameObservacao.IsEmptyOrWhiteSpace()))
            {
                qry = new List<ExamesDiagnosticosSA>();
                if (_sumarioalta.SemParticularidadeExames == Core.Domain.Enum.SimNao.Sim)
                {
                    qry.Add(new ExamesDiagnosticosSA() { Observacao = "Exames sem particularidades" });
                }
                else if (_sumarioalta.SemExamesRealizados == Core.Domain.Enum.SimNao.Sim)
                {
                    qry.Add(new ExamesDiagnosticosSA() { Observacao = "Não foram realizados exames durante a internação" });
                }
            }
            else if ((_sumarioalta.SumarioExames.Count() == 0) && (!this._sumarioalta.ExameObservacao.IsEmptyOrWhiteSpace()))
            {
                qry = new List<ExamesDiagnosticosSA>(/*indexExame*/);
                if (_sumarioalta.SemParticularidadeExames == Core.Domain.Enum.SimNao.Sim)
                {
                    qry.Add(new ExamesDiagnosticosSA() { Observacao = "Exames sem particularidades" + Environment.NewLine + this._sumarioalta.ExameObservacao });
                }
                else if (_sumarioalta.SemExamesRealizados == Core.Domain.Enum.SimNao.Sim)
                {
                    qry.Add(new ExamesDiagnosticosSA() { Observacao = "Não foram realizados exames durante a internação" + Environment.NewLine + this._sumarioalta.ExameObservacao });
                }
                else
                    qry.Add(new ExamesDiagnosticosSA() { Observacao = this._sumarioalta.ExameObservacao, AnalisesClinicas = _sumarioalta.AnaliseClinica.IsNull() ? string.Empty : _sumarioalta.AnaliseClinica }); // Procedimento = "Não foram informados exames.", 
            }
            else
                qry = (from T in _sumarioalta.SumarioExames
                       select new ExamesDiagnosticosSA()
                       {
                           Resultado = T.Observacao == null ? string.Empty : T.Observacao,
                           Procedimento = T.Procedimento.Descricao,
                           Observacao = _sumarioalta.ExameObservacao == null ? string.Empty : _sumarioalta.ExameObservacao,
                           AnalisesClinicas = _sumarioalta.AnaliseClinica.IsNull() ? string.Empty : _sumarioalta.AnaliseClinica
                       }).ToList();

            if (qry.Count == 0 && _sumarioalta.AnaliseClinica.IsNotEmptyOrWhiteSpace())
                qry.Add(new ExamesDiagnosticosSA() { Observacao = string.Empty, AnalisesClinicas = _sumarioalta.AnaliseClinica.IsNull() ? string.Empty : _sumarioalta.AnaliseClinica });


            return qry;
        }

        private List<ConsultaMedicaSA> _ConsultaMedica()
        {
            if (_sumarioalta.RevisaoMedicaEm.IsNotNull())
                return new List<ConsultaMedicaSA>() { new ConsultaMedicaSA() { Descricao = _sumarioalta.RevisaoMedicaEm, MostraConsultaMedica = true } };
            else
                return new List<ConsultaMedicaSA>() { new ConsultaMedicaSA() { MostraConsultaMedica = false } };
        }

        private List<ConsultaUrgenciaSA> _ConsultaUrgencia()
        {
            if (_sumarioalta.EmCasoDeUrgencia.ConvertNullToStringEmpty().IsNotEmpty())
                return new List<ConsultaUrgenciaSA>() { new ConsultaUrgenciaSA() { Descricao = _sumarioalta.EmCasoDeUrgencia, MostraConsultaUrgencia = true } };
            else
                return new List<ConsultaUrgenciaSA>() { new ConsultaUrgenciaSA() { Descricao = _sumarioalta.EmCasoDeUrgencia, MostraConsultaUrgencia = false } };
        }

        private List<TransferenciaSA> _Transferencia()
        {
            if (!mostraTransferencia)
            {
                return new List<TransferenciaSA>() { new TransferenciaSA() { MostraTranferencia = false } };
            }
            else
            {
                TransferenciaSA transferencia = new TransferenciaSA() { MostraTranferencia = true };

                ////report.lbTitulo.Text = "Sumário de Transferência";
                ////report.lbTituloRodape.Text = "Sumário de Transferência";
                ////report.DetailTransferencia.Visible = true;
                //transferencia.Destino = "TRANSFERIDO";
                //transferencia.MomentoTrans = _sumarioalta.Transferencia.ExameFisicoSumario;
                //transferencia.HospitalDestino = _sumarioalta.Transferencia.NomeHospitalDestino;
                //transferencia.PA = _sumarioalta.Transferencia.PressaoArterial.Alta.ToString() + " / " + _sumarioalta.Transferencia.PressaoArterial.Baixa.ToString();
                //transferencia.FC = _sumarioalta.Transferencia.FrequenciaCardiaca.IsNotNull() ? _sumarioalta.Transferencia.FrequenciaCardiaca.Value.ToString() : string.Empty;
                //transferencia.FR = _sumarioalta.Transferencia.FrequenciaRespiratoria.IsNotNull() ? _sumarioalta.Transferencia.FrequenciaRespiratoria.Value.ToString() : string.Empty;
                //transferencia.ReservaLeito = _sumarioalta.Transferencia.UsuarioLeitoReserva.IsNotNull() ? _sumarioalta.Transferencia.UsuarioLeitoReserva : string.Empty;
                //transferencia.ContatoInstitu = _sumarioalta.Transferencia.ContatoInstituicao.IsNotNull() ? _sumarioalta.Transferencia.ContatoInstituicao : string.Empty;
                //transferencia.AcompMedico = _sumarioalta.Transferencia.AcompanhaMedico.ToString();
                //transferencia.Transporte = _sumarioalta.Transferencia.IsNotNull() ? _sumarioalta.Transferencia.MeioDetransporte.Descricao : string.Empty;
                //transferencia.Oxigenio = _sumarioalta.Transferencia.Oxigenio.ToString();
                //transferencia.Motorizacao = _sumarioalta.Transferencia.Motorizacao.ToString();
                //transferencia.Ventilacao = _sumarioalta.Transferencia.Ventilacao.ToString();            
                //transferencia.Municipio = _sumarioalta.Transferencia.Municipio.IsNotNull() ? _sumarioalta.Transferencia.Municipio.Descricao : !string.IsNullOrWhiteSpace(_sumarioalta.DomainObject.Transferencia.MunicipioPEPAntigo) ? _sumarioalta.DomainObject.Transferencia.MunicipioPEPAntigo : string.Empty;

                // Destino
                transferencia.ListaDestino = new List<DestinoSA>() { new DestinoSA() { Descricao = "TRANSFERIDO", MostraDestino = true } };
                //transferencia.MostraTranferencia = true;

                // Exame Físico Sumário no Momento Transferência
                if (_sumarioalta.Transferencia.ExameFisicoSumario.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    transferencia.ListaMomentoTransferencia = new List<MomentoTransferenciaSA>() { new MomentoTransferenciaSA() { Descricao = _sumarioalta.Transferencia.ExameFisicoSumario, MostraMomentoTransferencia = true } };
                    //transferencia.MostraTranferencia = true;
                }
                else
                    transferencia.ListaMomentoTransferencia = new List<MomentoTransferenciaSA>() { new MomentoTransferenciaSA() { MostraMomentoTransferencia = false } };

                // Hospital de Destino
                if (_sumarioalta.Transferencia.NomeHospitalDestino.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    transferencia.ListaHospitalDestino = new List<HospitalDestinoSA>() { new HospitalDestinoSA() { Descricao = _sumarioalta.Transferencia.NomeHospitalDestino, MostraHospitalDestino = true } };
                    //transferencia.MostraTranferencia = true;
                }
                else
                    transferencia.ListaHospitalDestino = new List<HospitalDestinoSA>() { new HospitalDestinoSA() { MostraHospitalDestino = false } };

                // Pressão Arterial e Frequências
                PressaoArterialEFrequenciasSA obj = new PressaoArterialEFrequenciasSA();
                string pa = string.Empty;
                if (_sumarioalta.Transferencia.PressaoArterial.Alta.IsNotNull())
                {
                    pa = _sumarioalta.Transferencia.PressaoArterial.Alta.ToString();
                    obj.MostraPressaoArterialEFrequencias = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.PressaoArterial.Baixa.IsNotNull())
                {
                    pa += " / " + _sumarioalta.Transferencia.PressaoArterial.Baixa.ToString();
                    obj.MostraPressaoArterialEFrequencias = true;
                    //transferencia.MostraTranferencia = true;
                }
                obj.PressaoArterial = pa;

                if (_sumarioalta.Transferencia.FrequenciaCardiaca.IsNotNull())
                {
                    obj.FrequenciaCardiaca = _sumarioalta.Transferencia.FrequenciaCardiaca.ToString();
                    obj.MostraPressaoArterialEFrequencias = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.FrequenciaRespiratoria.IsNotNull())
                {
                    obj.FrequenciaRespiratoria = _sumarioalta.Transferencia.FrequenciaRespiratoria.ToString();
                    obj.MostraPressaoArterialEFrequencias = true;
                    //transferencia.MostraTranferencia = true;
                }
                transferencia.ListaPressaoArterialEFrequencias = new List<PressaoArterialEFrequenciasSA>() { obj };

                // Reserva Leito
                ReservaLeitoSA reserva = new ReservaLeitoSA() { MostraReservaLeito = false };
                if (_sumarioalta.Transferencia.UsuarioLeitoReserva.IsNotNull())
                {
                    reserva.QuemReservou = _sumarioalta.Transferencia.UsuarioLeitoReserva;
                    reserva.MostraReservaLeito = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.ContatoInstituicao.IsNotNull())
                {
                    reserva.ContatoOutraInstituicao = _sumarioalta.Transferencia.ContatoInstituicao;
                    reserva.MostraReservaLeito = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.Municipio.IsNotNull())
                {
                    reserva.MunicipioDestino = _sumarioalta.Transferencia.Municipio.Descricao;
                    reserva.MostraReservaLeito = true;
                    //transferencia.MostraTranferencia = true;
                }
                transferencia.ListaReservaLeito = new List<ReservaLeitoSA>() { reserva };

                // Dados
                DadosSA dados = new DadosSA() { MostraDados = false };
                if (_sumarioalta.Transferencia.IsNotNull())
                {
                    dados.Transporte = _sumarioalta.Transferencia.MeioDetransporte.Descricao;
                    dados.MostraDados = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.Oxigenio.IsNotNull())
                {
                    dados.Oxigenio = _sumarioalta.Transferencia.Oxigenio.ToString();
                    dados.MostraDados = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.Motorizacao.IsNotNull())
                {
                    dados.Motorizacao = _sumarioalta.Transferencia.Motorizacao.ToString();
                    dados.MostraDados = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.Ventilacao.IsNotNull())
                {
                    dados.Ventilacao = _sumarioalta.Transferencia.Ventilacao.ToString();
                    dados.MostraDados = true;
                    //transferencia.MostraTranferencia = true;
                }
                if (_sumarioalta.Transferencia.AcompanhaMedico.IsNotNull())
                {
                    dados.AcompanhamentoMedico = _sumarioalta.Transferencia.AcompanhaMedico.ToString();
                    dados.MostraDados = true;
                    //transferencia.MostraTranferencia = true;
                }
                transferencia.ListaDados = new List<DadosSA>() { dados };

                return new List<TransferenciaSA>() { transferencia };
            }
        }

        private string _Assinatura()
        {
            if (_sumarioalta == null)
                return string.Empty;

            wrpPrestador _medico;
            string nome;
            if (this._sumarioalta.DomainObject.UsuarioImpressao.IsNotNull())
            {
                _medico = this._sumarioalta.UsuarioImpressao.Prestador;
                nome = this._sumarioalta.UsuarioImpressao.NomeExibicao;                
            }
            else
            {
                if (this._usuarios.Prestador.DomainObject.IsNurse && this._sumarioalta.UsuarioInclusao.IsNotNull())
                {
                    _medico = this._sumarioalta.UsuarioInclusao.Prestador;
                    nome = this._sumarioalta.UsuarioInclusao.Prestador.NomeExibicao;
                }
                else
                {
                    _medico = this._usuarios.Prestador;
                    nome = this._usuarios.NomeExibicao;                    
                    if (string.IsNullOrWhiteSpace(nome))
                        nome = this._usuarios.DomainObject.Prestador.NomeExibicao;
                }
            }

            return (string.Format(@"{0} {1} {2}",
               nome + Environment.NewLine,
               _medico.Conselho + " " + _medico.Registro + Environment.NewLine, _sumarioalta.DataAlta));
        }

        #endregion

        #region ----- Dados do Paciente -----    

        public string TipoAtendimento
        {
            get { return _sumarioalta.Atendimento.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Internacao ? "Sumário de Alta" : "Sumário de Alta Ambulatorial"; }
        }    

        public String DataAlta
        {
            get { return _sumarioalta.DataAlta == null ? string.Empty : _sumarioalta.DataAlta.Value.ToString("dd/MM/yyyy"); }
        }

        public string _CondicaoInternacao
        {
            get
            {
                IRepositorioDeSumarioAlta repsum = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
                var ret = repsum.BuscaMotivoInternacaoSumarios(_sumarioalta.Atendimento.ID);
                if (ret.IsNotEmptyOrWhiteSpace())
                {
                    //temCondicaoInternacao = true;
                    return ret;
                }
                return null;
            }
        }

        //public String CondicaoAlta
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.MotivoAlta != null ? _sumarioalta.Atendimento.MotivoAlta.Descricao :
        //            !MotivoAltaRelatorio.DescMotivoAltaRelatorio.IsEmpty() ? MotivoAltaRelatorio.DescMotivoAltaRelatorio :
        //            _sumarioalta.MotivoAltaDiaSeguinte != null ? _sumarioalta.MotivoAltaDiaSeguinte.Descricao :
        //            string.Empty;
        //    }
        //}

        //public String HoraAlta
        //{
        //    get
        //    {
        //        if (_sumarioalta.HoraAlta == null)
        //            return "";
        //        return _sumarioalta.HoraAlta.Value.ToString("HH:mm");
        //    }
        //}

        //public String Evolucao
        //{
        //    get
        //    {
        //        return (_sumarioalta.SemEvolucao == SimNao.Sim && _sumarioalta.Atendimento.TipoDeAtendimento != Core.Domain.Enum.TipoAtendimento.Internacao)
        //            ? "Paciente evoluiu sem intercorrências significativas" : _sumarioalta.Evolucao;
        //    }
        //}

        //public String Consulta
        //{
        //    get
        //    {
        //        if (_sumarioalta.RevisaoMedicaEm == null)
        //            return "";
        //        return _sumarioalta.RevisaoMedicaEm.ToString();
        //    }
        //}

        //public String CasoUrgencia
        //{
        //    get
        //    {
        //        return _sumarioalta.EmCasoDeUrgencia;
        //    }
        //}

        //public String CidPrincipal
        //{
        //    get
        //    {
        //        if (_sumarioalta.CIDPrincipal != null && !_sumarioalta.Atendimento.DataAltaMedica.HasValue)
        //            return _sumarioalta.CIDPrincipal.CidMV.Id.ToString() + " - " + _sumarioalta.CIDPrincipal.CidMV.Descricao;
        //        else
        //            return _sumarioalta.Atendimento.Cid == null ? "" : _sumarioalta.Atendimento.Cid.CidMV.Id.ToString() + " - " + _sumarioalta.Atendimento.Cid.CidMV.Descricao;
        //    }
        //}

        //public IList<ExamesDiagnosticos> Exames
        //{
        //    get
        //    {
        //        IList<ExamesDiagnosticos> qry = null;

        //        if ((_sumarioalta.SumarioExames.Count() == 0) && (this._sumarioalta.ExameObservacao.IsEmptyOrWhiteSpace()))
        //        {
        //            qry = new List<ExamesDiagnosticos>();
        //            if (_sumarioalta.SemParticularidadeExames == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry.Add(new ExamesDiagnosticos() { Observacao = "Exames sem particularidades" });
        //            }
        //            else if (_sumarioalta.SemExamesRealizados == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry.Add(new ExamesDiagnosticos() { Observacao = "Não foram realizados exames durante a internação" });
        //            }
        //        }
        //        else if ((_sumarioalta.SumarioExames.Count() == 0) && (!this._sumarioalta.ExameObservacao.IsEmptyOrWhiteSpace()))
        //        {
        //            qry = new List<ExamesDiagnosticos>(indexExame);
        //            if (_sumarioalta.SemParticularidadeExames == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry.Add(new ExamesDiagnosticos() { Observacao = "Exames sem particularidades" + Environment.NewLine + this._sumarioalta.ExameObservacao });
        //            }
        //            else if (_sumarioalta.SemExamesRealizados == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry.Add(new ExamesDiagnosticos() { Observacao = "Não foram realizados exames durante a internação" + Environment.NewLine + this._sumarioalta.ExameObservacao });
        //            }
        //            else
        //                qry.Add(new ExamesDiagnosticos() { Observacao = this._sumarioalta.ExameObservacao, AnalisesClinicas = _sumarioalta.AnaliseClinica.IsNull() ? string.Empty : _sumarioalta.AnaliseClinica }); // Procedimento = "Não foram informados exames.", 
        //        }
        //        else
        //            qry = (from T in _sumarioalta.SumarioExames
        //                   select new ExamesDiagnosticos()
        //                   {
        //                       Resultado = T.Observacao == null ? string.Empty : T.Observacao,
        //                       Procedimento = T.Procedimento.Descricao,
        //                       Observacao = _sumarioalta.ExameObservacao == null ? string.Empty : _sumarioalta.ExameObservacao,
        //                       AnalisesClinicas = _sumarioalta.AnaliseClinica.IsNull() ? string.Empty : _sumarioalta.AnaliseClinica
        //                   }).ToList();

        //        if (qry.Count == 0 && _sumarioalta.AnaliseClinica.IsNotEmptyOrWhiteSpace())
        //            qry.Add(new ExamesDiagnosticos() { Observacao = string.Empty, AnalisesClinicas = _sumarioalta.AnaliseClinica.IsNull() ? string.Empty : _sumarioalta.AnaliseClinica });


        //        return qry;
        //    }
        //}

        //public string ExameObservacao
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        //public IList<Procedimento> Procedimentos
        //{
        //    get
        //    {
        //        IList<Procedimento> qry = null;
        //        if (_sumarioalta.ProcedimentosAlta.Count() == 0)
        //        {
        //            if (_sumarioalta.SemProcedimentoInvasivo == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry = new List<Procedimento>();
        //                qry.Add(new Procedimento() { ProcedimentoNome = "Não se Aplica" });
        //            }
        //        }
        //        else
        //            qry = (from T in _sumarioalta.ProcedimentosAlta
        //                   orderby T.Cirurgia.ds_cirurgia
        //                   select new Procedimento()
        //                           {
        //                               ProcedimentoNome = (T.Cirurgia.ds_cirurgia).ToUpper(),
        //                               ProcedimentoData = T.Data
        //                           }).ToList();
        //        return qry;
        //    }
        //}

        //public IList<Farmacos> FarmacosInternacao
        //{
        //    get
        //    {
        //        IList<Farmacos> qry = null;
        //        if (_sumarioalta.Farmacos.Count() == 0)
        //        {
        //            if (_sumarioalta.SemFarmaco == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry = new List<Farmacos>();
        //                qry.Add(new Farmacos() { Descricao = "Não se Aplica" });
        //            }
        //        }
        //        else
        //            qry = (from T in _sumarioalta.Farmacos
        //                   select new Farmacos()
        //                       {
        //                           Descricao = T.Produto.Descricao != null ? T.Produto.Descricao : "Não há itens"
        //                       }).ToList();
        //        return qry;
        //    }
        //}

        //public String ObservacaoFarmacos
        //{
        //    get
        //    {
        //        return _sumarioalta.FarmacoObservacao == null ? null : _sumarioalta.FarmacoObservacao.ToString();
        //    }
        //}

        //public bool transferencia
        //{
        //    get
        //    {
        //        if (_sumarioalta.Atendimento.MotivoAlta != null)
        //            if (_sumarioalta.Atendimento.MotivoAlta.Id == 7)
        //                return true;
        //        return false;
        //    }
        //}

        //public IList<MedicoAssistenteNome> MedicoAssistenteCRMCRO
        //{
        //    get
        //    {
        //        if (_sumarioalta.Prestadores.Count() == 0)
        //            return null;
        //        IList<MedicoAssistenteNome> qry = (from T in _sumarioalta.Prestadores
        //                                           select new MedicoAssistenteNome(IsInternado)
        //                                           {
        //                                               Nome = (T.Nome).ToUpper(),
        //                                               CRM = T.Registro
        //                                           }).ToList();
        //        return qry;
        //    }
        //}

        //public IList<MedicamentosPos> MedicamentoPosAlta
        //{
        //    get
        //    {
        //        IList<MedicamentosPos> qry = null;
        //        if (_sumarioalta.PlanoPosAlta.Count() == 0)
        //        {
        //            if (_sumarioalta.SemMedPosAlta == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry = new List<MedicamentosPos>();
        //                qry.Add(new MedicamentosPos() { Nome = "Não se Aplica" });
        //            }
        //        }
        //        else
        //        {
        //            qry = (from T in _sumarioalta.PlanoPosAlta
        //                   select new MedicamentosPos
        //                        {
        //                            Nome = T.Produto != null ? T.Produto.Descricao : (T.TipoPrescricaoMedica != null ? T.TipoPrescricaoMedica.Descricao : T.NomeComercial),
        //                            Dose = T.Dose,
        //                            Via = T.Via,
        //                            Frequencia = T.Frequencia,
        //                            Tempo = T.Tempo
        //                        }).ToList();
        //        }
        //        return qry;

        //    }
        //}

        //public IList<Recomendacoes> Recomendacao
        //{
        //    get
        //    {
        //        IList<Recomendacoes> qry = null;
        //        if (_sumarioalta.recomendacoes.Count() == 0)
        //        {
        //            if (_sumarioalta.SemRecomendacao == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry = new List<Recomendacoes>();
        //                qry.Add(new Recomendacoes() { Descricao = "Sem Recomendações" });
        //            }
        //        }

        //        else
        //        {
        //            qry = (from T in _sumarioalta.recomendacoes
        //                   orderby T.Recomendacao.Descricao
        //                   select new Recomendacoes
        //                          {
        //                              Descricao = T.Descricao,
        //                              Recomendacao = T.Recomendacao.Descricao
        //                          }).ToList();
        //        }

        //        return qry;
        //    }
        //}

        //public IList<CausaExternas> CausaExterna
        //{
        //    get
        //    {
        //        IList<CausaExternas> qry = null;
        //        if (_sumarioalta.CausaExterna.Count() == 0)
        //        {
        //            if (_sumarioalta.SemCausaExterna == Core.Domain.Enum.SimNao.Sim)
        //            {
        //                qry = new List<CausaExternas>();
        //                qry.Add(new CausaExternas() { Descricao = "Não se Aplica", Observacao = "" });
        //            }
        //        }
        //        else
        //        {
        //            qry = (from T in _sumarioalta.CausaExterna
        //                   select new CausaExternas
        //                  {
        //                      Observacao = T.Observacao,
        //                      Descricao = T.Cid.Descricao
        //                  }).ToList();
        //        }
        //        return qry;
        //    }
        //}

        //public List<CID> Cids
        //{
        //    get
        //    {
        //        if (_sumarioalta.DomainObject.Cids.Where(x => x != null && x.CidMV != null).Count() == 0)
        //            return null;
        //        List<CID> qry = (from T in _sumarioalta.DomainObject.Cids.Where(x => x != null && x.CidMV != null)
        //                         select new CID
        //                                   {
        //                                       CidDiagnostico = T.CidMV.Id + " - " + T.CidMV.Descricao
        //                                   }).ToList();

        //        return qry;
        //    }
        //}        

        public bool IsObito
        {
            get { return _sumarioalta.MotivoAlta != null && _sumarioalta.MotivoAlta.Tipo == TipoMotivoAlta.Óbito; }
        }

        public bool IsRN
        {
            get 
            {
                if (this._sumarioalta.Atendimento.AtendimentoPai.IsNotNull())
                    return  true;
                else
                {
                    var repA = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>();
                    var ret = repA.OndeCodigoPacienteIgual(this._sumarioalta.Atendimento.Paciente.ID).List();
                    if (ret.IsNotNull())
                    {
                        var atendmentosanteriores = ret.Where(x => x.DataAlta.IsNotNull() && x.DataAlta.Value.AddDays(1) >= this._sumarioalta.Atendimento.DataAtendimento).ToList();
                        if (atendmentosanteriores.HasItems())
                            if (atendmentosanteriores.Count(x => x.AtendimentoPai.IsNotNull()) > 0)
                                return true;
                    }
                }
                return false;//_sumarioalta.Atendimento.AtendimentoPai.IsNotNull(); 
            }
        }

        #region Transferencia
        //public String Destino
        //{
        //    get { return "TRANSFERIDO"; }
        //}

        //public String HospitalDestinor
        //{
        //    get { return _sumarioalta.Transferencia.NomeHospitalDestino; }
        //}

        //public String Transporte
        //{
        //    get
        //    {
        //        if (_sumarioalta.Transferencia != null)
        //            return _sumarioalta.Transferencia.MeioDetransporte.Descricao;
        //        return string.Empty;
        //    }
        //}

        //public String Oxgenio
        //{
        //    get
        //    {
        //        return _sumarioalta.Transferencia.Oxigenio.ToString();
        //    }
        //}

        //public String Ventilacao
        //{
        //    get { return _sumarioalta.Transferencia.Ventilacao.ToString(); }
        //}

        //public String Monitorizacao
        //{
        //    get { return _sumarioalta.Transferencia.Motorizacao.ToString(); }
        //}

        //public String AcompanhamentoMedico
        //{
        //    get { return _sumarioalta.Transferencia.AcompanhaMedico.ToString(); }
        //}

        //public String MomentoTransferencia
        //{
        //    get { return _sumarioalta.Transferencia.ExameFisicoSumario; }
        //}

        //public String PA
        //{
        //    get { return _sumarioalta.Transferencia.PressaoArterial.Alta.ToString() + " / " + _sumarioalta.Transferencia.PressaoArterial.Baixa.ToString(); }
        //}

        //public String FC
        //{
        //    get
        //    {
        //        if (_sumarioalta.Transferencia.FrequenciaCardiaca != null)
        //            return _sumarioalta.Transferencia.FrequenciaCardiaca.Value.ToString();
        //        return string.Empty;
        //    }
        //}

        //public String FR
        //{
        //    get
        //    {
        //        if (_sumarioalta.Transferencia.FrequenciaRespiratoria != null)
        //            return _sumarioalta.Transferencia.FrequenciaRespiratoria.Value.ToString();
        //        return string.Empty;
        //    }
        //}

        //public String QuemReservolLeito
        //{
        //    get
        //    {
        //        if (_sumarioalta.Transferencia.UsuarioLeitoReserva != null)
        //            return _sumarioalta.Transferencia.UsuarioLeitoReserva;
        //        return string.Empty;
        //    }
        //}

        //public String ContatoInstituicao
        //{
        //    get
        //    {
        //        if (_sumarioalta.Transferencia.ContatoInstituicao != null)
        //            return _sumarioalta.Transferencia.ContatoInstituicao;
        //        return string.Empty;
        //    }
        //}

        //public String Municipio
        //{
        //    get
        //    {
        //        return _sumarioalta.Transferencia.Municipio != null ? _sumarioalta.Transferencia.Municipio.Descricao :
        //            !string.IsNullOrWhiteSpace(_sumarioalta.DomainObject.Transferencia.MunicipioPEPAntigo) ? _sumarioalta.DomainObject.Transferencia.MunicipioPEPAntigo :
        //            string.Empty;
        //    }
        //}

        public bool IsInternado
        {
            get { return _sumarioalta.Atendimento.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Internacao; }
        }

        //public string EmergenciaRodapeSumarioDeAlta
        //{
        //    get
        //    {
        //        IRepositorioDeParametrosInternet rep = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
        //        ParametroInternet par = rep.OndeDadosEmergenciaRodapeSumarioDeAlta().Single();
        //        return (par == null ? string.Empty : par.valor);
        //    }
        //}

        //public string EnderecoRodapeSumarioDeAlta
        //{
        //    get
        //    {
        //        IRepositorioDeParametrosInternet rep = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
        //        ParametroInternet par = rep.OndeDadosEnderecoRodapeSumarioDeAlta().Single();
        //        return (par == null ? string.Empty : par.valor);
        //    }
        //}

        //public string RodapeAltaMedica
        //{
        //    get
        //    {
        //        return EmergenciaRodapeSumarioDeAlta + " / " + EnderecoRodapeSumarioDeAlta;
        //    }
        //}

        //public bool VisibleMotivoInternacao;
        //public bool VisibleCondicaoAlta;
        //public bool VisibleDiagnosticos;
        //public bool VisibleCausaExterna;
        //public bool VisibleProcedimentos;
        //public bool VisibleFarmaco;
        //public bool VisibleEvolucao;
        //public bool VisibleExame;
        //public bool VisibleMedPosAlta;
        //public bool VisiblePlanoAlta;
        //public bool VisiblePlanoAlta2;
        //public bool VisibleDadosNascimento;

        #endregion
        //private int indexMotivoInternacao;
        //private int indexCondicaoAlta;
        //private int indexDiagnostico;
        //private int indexCausaExterna;
        //private int indexDadosNascimento;
        //private int indexProcedimentos;
        //private int indexFarmaco;
        //private int indexEvolucao;
        //private int indexMedicamento;
        //private int indexExame;
        //private int indexPlanoAlta;
        //private int indexPlanoAlta2;
        //private int indexTransf;

        //public void SetaIndex()
        //{
        //int _index = 2;

        //if (VisibleMotivoInternacao)
        //    indexMotivoInternacao = _index += 1;

        //if (VisibleCondicaoAlta)
        //    indexCondicaoAlta = _index += 1;

        //if (VisibleDiagnosticos)
        //    indexDiagnostico = _index += 1;

        //if (VisibleCausaExterna)
        //    indexCausaExterna = _index += 1;

        //if (VisibleDadosNascimento)
        //    indexDadosNascimento = _index += 1;

        //if (VisibleProcedimentos)
        //    indexProcedimentos = _index += 1;

        //if (VisibleFarmaco)
        //    indexFarmaco = _index += 1;

        //if (VisibleEvolucao)
        //    indexEvolucao = _index += 1;

        //if (VisibleMedPosAlta)
        //    indexMedicamento = _index += 1;

        //if (VisibleExame)
        //    indexExame = _index += 1;

        //if (VisiblePlanoAlta)
        //    indexPlanoAlta = _index += 1;

        //if (VisiblePlanoAlta2)
        //    indexPlanoAlta2 = _index += 1;

        //indexTransf = _index += 1;
        //}

        //public string TituloCondicaoMotivoInternacao { get { return indexMotivoInternacao + ". Motivo de Internação"; } }
        //public string TituloCondicaoAlta { get { return indexCondicaoAlta + ". Condições de Alta"; } }
        //public string TituloDiagnosticos { get { return indexDiagnostico + ". Diagnósticos"; } }
        //public string TituloCausaExterna { get { return indexCausaExterna + ". Causa Externa"; } }
        //public string TituloDadosNascimento { get { return indexDadosNascimento + ". Dados do Nascimento"; } }
        //public string TituloProcedimentos { get { return indexProcedimentos + ". Procedimentos"; } }
        //public string TituloFarmacos { get { return indexFarmaco + ". Principais Fármacos na Internação"; } }
        //public string TituloEvolucao { get { return indexEvolucao + ". Evolução"; } }
        //public string TituloMedicamentosPosAlta { get { return indexMedicamento + ". Medicamentos Pós-Alta"; } }
        //public string TituloExamesDiagnosticos { get { return indexExame + ". Resultados principais de exames diagnósticos"; } 
        //public string TituloPlanoPosAlta { get { return indexPlanoAlta + ". Plano Pós-Alta"; } }
        //public string TituloPlanoPosAlta2 { get { return indexPlanoAlta2 + ". Plano Pós-Alta"; } }
        //public string TituloTransferencia { get { return indexTransf + ". Transferência para outro Hospital"; } }

        #endregion

        #region Etiqueta

        //public string NomePaciente
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.Paciente.IsNotNull() ? _sumarioalta.Atendimento.Paciente.Nome : string.Empty;
        //    }
        //}

        //public string NomeResumo
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.IsNotNull() ? _sumarioalta.Atendimento.Leito.IsNotNull() ? _sumarioalta.Atendimento.Leito.Descricao : string.Empty : string.Empty;
        //    }
        //}

        //public string IDPaciente
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.Paciente.IsNotNull() ? _sumarioalta.Atendimento.Paciente.ID != 0 ? _sumarioalta.Atendimento.Paciente.ID.ToString() : string.Empty : string.Empty;
        //    }
        //}

        //public string NomePrestador
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.IsNotNull() ? _sumarioalta.Atendimento.Prestador.IsNotNull() ? _sumarioalta.Atendimento.Prestador.Nome : string.Empty : string.Empty;
        //    }
        //}

        //public string Registro
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.IsNotNull() ? _sumarioalta.Atendimento.Prestador.IsNotNull() ? _sumarioalta.Atendimento.Prestador.Registro : string.Empty : string.Empty;
        //    }
        //}

        //public string CodigoBarras
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.IsNotNull() ? _sumarioalta.Atendimento.ID.ToString() : string.Empty;
        //    }
        //}

        //public bool MostraCodigoBarras
        //{
        //    get
        //    {
        //        return _sumarioalta.Atendimento.IsNotNull();
        //    }
        //}

        #endregion

        #region --- Procedimentos ---

        #endregion

        public class SumarioAltaRelatorio
        {
            public string Nome { get; set; }
            public string Idade { get; set; }
            public string DataNascimento { get; set; }
            public string DataInternacao { get; set; }
            public string Sexo { get; set; }
            public string DataAlta { get; set; }
            public string Registro { get; set; }
            public string HoraAlta { get; set; }
            public string Atendimento { get; set; }
            public string Permanencia { get; set; }
            public bool AtendimentoAmbulatorial { get; set; }

            public string Titulo { get; set; }
            public string TituloRodape { get; set; }
            public string Assinatura { get; set; }
            public string NomePacienteCodigoBarras { get; set; }
            public string LeitoCodigoBarras { get; set; }
            public string ProntuarioCodigoBarras { get; set; }
            public string PrestadorCodigoBarras { get; set; }
            public string RegistroCodigoBarras { get; set; }
            public string AtendimentoCodigoBarras { get; set; }
            public bool MostraSumarioAlta { get; set; }

            public List<MedicoAssistenteSA> ListaMedicoAssistente { get; set; }
            public List<CondicaoAltaSA> ListaCondicaoAlta { get; set; }
            public List<CondicaoMotivoInternacaoSA> ListaCondicaoMotivoInternacao { get; set; }
            public List<DiagnosticosSA> ListaDiagnosticos { get; set; }
            public List<CausaExternasSA> ListaCausaExternas { get; set; }
            public List<DadosDoNascimentoSA> ListaDadosDoNascimento { get; set; }
            public List<ProcedimentoSA> ListaProcedimento { get; set; }
            public List<FarmacosSA> ListaFarmacos { get; set; }
            public List<FarmacosObservacaoSA> ListaFarmacosObservacoes { get; set; }
            public List<EvolucaoSA> ListaEvolucao { get; set; }
            public List<MedicamentosPosAltaSA> ListaMedicamentosPosAlta { get; set; }
            public List<ExameSA> ListaExame { get; set; }
            public List<PlanoPosAltaSA> ListaPlanoPosAlta { get; set; }
            public List<RecomendacoesSA> ListaRecomendacoes { get; set; }
            public List<ExamesDiagnosticosSA> ListaExamesDiagnosticos { get; set; }


            public List<ConsultaMedicaSA> ListaConsultaMedica { get; set; }
            public List<ConsultaUrgenciaSA> ListaConsultaUrgencia { get; set; }
            public List<TransferenciaSA> ListaTransferencia { get; set; }
        }

        public class MedicoAssistenteSA
        {
            public string Titulo { get; set; }
            public string CRM { get; set; }
            public string Nome { get; set; }
            public bool MostraMedicoAssistente { get; set; }
        }

        public class CondicaoMotivoInternacaoSA
        {
            public string Descricao { get; set; }
            public bool MostraCondicaoMotivoInternacao { get; set; }
        }

        public class CondicaoAltaSA
        {
            public string Descricao { get; set; }
            public bool MostraCondicaoAlta { get; set; }
        }

        public class DiagnosticosSA
        {
            public List<CIDPrincipalSA> ListaCidPrincipal { get; set; }
            public List<OutrosCIDs> ListaOutrosCIDs { get; set; }
            public bool MostraDiagnosticos { get; set; }
        }

        public class CIDPrincipalSA
        {
            public string Descricao { get; set; }
            //public bool MostraCidPrincipal { get; set; }
        }

        public class OutrosCIDs
        {
            public string Descricao { get; set; }
            public bool MostraOutrosCids { get; set; }
        }

        public class ExamesDiagnosticosSA
        {
            //int _index;
            //public ExamesDiagnosticos(int pindex)
            //{
            //    _index = pindex;
            //}
            public String Observacao { get; set; }
            public String Procedimento { get; set; }
            public String Resultado { get; set; }
            public String AnalisesClinicas { get; set; }
            //public string TituloExamesDiagnosticos { get { return _index + ". Resultados principais de exames diagnósticos"; } }
        }

        public class CausaExternasSA
        {
            public string Descricao { get; set; }
            public string Observacao { get; set; }
            public bool MostraCausaExternas { get; set; }
        }

        public class FarmacosSA
        {
            public string Descricao { get; set; }
            public bool MostraFarmacos { get; set; }
        }

        public class FarmacosObservacaoSA
        {
            public string Descricao { get; set; }
            public bool MostraObservacao { get; set; }
        }

        public class MedicamentosPosAltaSA
        {
            public String Nome { get; set; }
            public String Dose { get; set; }
            public String Via { get; set; }
            public String Frequencia { get; set; }
            public String Tempo { get; set; }
            public bool MostraMedicamentosPosAlta { get; set; }
        }

        public class ExameSA
        {
            //public string Observacao { get; set; }
            //public string Resultado { get; set; }
            //public string Procedimento { get; set; }
            //public string AnalisesClinicas { get; set; }
            public string Descricao { get; set; }
            public bool MostraExame { get; set; }
            public bool MostraExameDescricao { get; set; }
            public List<ExameItemSA> ListaExameItem { get; set; }
            public List<ExameObservacaoSA> ListaExameObservacao { get; set; }
            public List<AnalisesClinicasSA> ListaAnalisesClinicas { get; set; }
        }

        public class ExameItemSA
        {
            public string Procedimento { get; set; }
            public string Resultado { get; set; }
            //public string AnalisesClinicas { get; set; }
            //public string Observacao { get; set; }
            public bool MostraExameItem { get; set; }
        }

        public class ExameObservacaoSA
        {
            public string Observacao { get; set; }
            public bool MostraExameObservacao { get; set; }
        }

        public class AnalisesClinicasSA
        {
            public string Descricao { get; set; }
            public bool MostraAnalisesClinicas { get; set; }
        }

        //public class ExamesDiagnosticosSA
        //{
        //    public string Descricao { get; set; }
        //    public bool MostraDiagnosticos { get; set; }
        //}

        public class PlanoPosAltaSA
        {
            public string Descricao { get; set; }
            public List<PlanoPosAltaItemSA> ListaPlanoPosAltaItem { get; set; }
            public bool MostraNaoSeAplica { get; set; }
            public bool MostraPlanoPosAlta { get; set; }
        }

        public class PlanoPosAltaItemSA
        {
            public String Nome { get; set; }
            public String Dose { get; set; }
            public String Via { get; set; }
            public String Frequencia { get; set; }
            public String Tempo { get; set; }
            public bool MostraPlanoPosAltaItem { get; set; }
        }

        public class ProcedimentoSA
        {
            //int _index;
            //public Procedimento(int pindex)
            //{
            //    _index = pindex;
            //}
            public string Descricao { get; set; }
            //public DateTime ProcedimentoData { get; set; }
            public bool MostraProcedimentos { get; set; }
            //public string TituloProcedimentos { get { return _index + ". Procedimentos"; } }
        }

        //public class MedicoAssistenteNome
        //{
        //    bool IsInternado;
        //    public MedicoAssistenteNome(bool pIsInternado)
        //    {
        //        IsInternado = pIsInternado;
        //    }
        //    public String CRM { get; set; }
        //    public String Nome { get; set; }
        //    public string TituloMedico { get { return IsInternado ? "2. Médico Assistente" : "2. Médico/Cirurgião"; } }
        //}

        public class RecomendacoesSA
        {
            public string SemRecomendacoes { get; set; }
            public bool MostraRecomendacoes { get; set; }
            public bool MostraSemRecomendacoes { get; set; }
            public List<RecomendacoesItemSA> ListaRecomendacoesItem { get; set; }
        }

        public class RecomendacoesItemSA
        {
            public string Descricao { get; set; }
            public string Recomendacao { get; set; }
            public bool MostraRecomendacoesItem { get; set; }
        }

        public class CID
        {
            public String CidDiagnostico { get; set; }
        }

        public class EvolucaoSA
        {
            public string Descricao { get; set; }
            public bool MostraEvolucao { get; set; }
        }

        public class ConsultaMedicaSA
        {
            public string Descricao { get; set; }
            public bool MostraConsultaMedica { get; set; }
        }

        public class ConsultaUrgenciaSA
        {
            public string Descricao { get; set; }
            public bool MostraConsultaUrgencia { get; set; }
        }

        public class TransferenciaSA
        {
            //public string Descricao { get; set; }
            ////report.lbTitulo.Text = "Sumário de Transferência";
            ////report.lbTituloRodape.Text = "Sumário de Transferência";
            //public string AcompMedico { get; set; }
            //public string ContatoInstitu { get; set; }
            //public string Destino { get; set; }
            //public string FC { get; set; }
            //public string FR { get; set; }
            //public string HospitalDestino { get; set; }
            //public string MomentoTrans { get; set; }
            //public string Motorizacao { get; set; }
            //public string Municipio { get; set; }
            //public string Oxigenio { get; set; }
            //public string PA { get; set; }
            //public string ReservaLeito { get; set; }
            //public string Transporte { get; set; }
            //public string Ventilacao { get; set; }

            public bool MostraTranferencia { get; set; }
            public List<DestinoSA> ListaDestino { get; set; }
            public List<MomentoTransferenciaSA> ListaMomentoTransferencia { get; set; }
            public List<HospitalDestinoSA> ListaHospitalDestino { get; set; }
            public List<PressaoArterialEFrequenciasSA> ListaPressaoArterialEFrequencias { get; set; }
            public List<ReservaLeitoSA> ListaReservaLeito { get; set; }
            public List<DadosSA> ListaDados { get; set; }
        }

        public class DestinoSA
        {
            public string Descricao { get; set; }
            public bool MostraDestino { get; set; }
        }

        public class MomentoTransferenciaSA
        {
            public string Descricao { get; set; }
            public bool MostraMomentoTransferencia { get; set; }
        }

        public class HospitalDestinoSA
        {
            public string Descricao { get; set; }
            public bool MostraHospitalDestino { get; set; }
        }

        public class PressaoArterialEFrequenciasSA
        {
            public string PressaoArterial { get; set; }
            public string FrequenciaCardiaca { get; set; }
            public string FrequenciaRespiratoria { get; set; }
            public bool MostraPressaoArterialEFrequencias { get; set; }
        }

        public class ReservaLeitoSA
        {
            public string QuemReservou { get; set; }
            public string ContatoOutraInstituicao { get; set; }
            public string MunicipioDestino { get; set; }
            public bool MostraReservaLeito { get; set; }
        }

        public class DadosSA
        {
            public string Transporte { get; set; }
            public string Oxigenio { get; set; }
            public string Motorizacao { get; set; }
            public string Ventilacao { get; set; }
            public string AcompanhamentoMedico { get; set; }
            public bool MostraDados { get; set; }
        }

        public class DadosDoNascimentoSA
        {
            public string Data { get; set; }
            public string Hora { get; set; }
            public string TipagemSanguinea { get; set; }
            public string TipoDeParto { get; set; }
            public string MembranasAmnioticas { get; set; }
            public string DataMembranasAmnioticas { get; set; }
            public string HoraMembranasAmnioticas { get; set; }
            public string LiquidoAmniotico { get; set; }
            public string IdadeGestacional { get; set; }
            public string Classificacao { get; set; }
            public string Peso { get; set; }
            public string Comprimento { get; set; }
            public string PerimetroCefalico { get; set; }
            public string PerimetroToracico { get; set; }
            public string Apgar { get; set; }
            public string TesteOlhinho { get; set; }

            //public string TituloData { get; set; }
            //public string TituloHora { get; set; }
            //public string TituloTipagemSanguinea { get; set; }
            //public string TituloTipoDeParto { get; set; }
            //public string TituloMembranasAmnioticas { get; set; }
            public string TituloDataMembranasAmnioticas { get; set; }
            public string TituloHoraMembranasAmnioticas { get; set; }
            //public string TituloLiquidoAmniotico { get; set; }
            //public string TituloIdadeGestacional { get; set; }
            //public string TituloClassificacao { get; set; }
            //public string TituloPeso { get; set; }
            //public string TituloPerimetroToracico { get; set; }
            //public string TituloComprimento { get; set; }
            //public string TituloPerimetroCefalico { get; set; }
            //public string TituloApgar { get; set; }
            //public string TituloTesteOlhinho { get; set; }

            public bool MostraDadosNascimento { get; set; }
        }

        private string _contaEspacos(string valor)
        {
            string espaco = string.Empty;

            for (int i = 0; i < valor.Length; i++)
            {
                espaco += " ";
            }

            return espaco;
        }
    }

    public static class MotivoAltaRelatorio
    {
        public static string DescMotivoAltaRelatorio;
    }
}