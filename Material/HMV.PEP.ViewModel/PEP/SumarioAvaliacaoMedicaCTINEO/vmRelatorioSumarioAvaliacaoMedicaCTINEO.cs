using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Model;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaCTINEO
{
    public class vmRelatorioSumarioAvaliacaoMedicaCTINEO
    {
        public vmRelatorioSumarioAvaliacaoMedicaCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            _wrpSumarioAvaliacaoMedicaCTINEO = pWrpSumarioAvaliacaoMedicaCTINEO;
        }

        #region Relatório

        public List<SumarioAvaliacaoMedicaCTINEO> Relatorio()
        {
            _listaRelatorioSource = new List<SumarioAvaliacaoMedicaCTINEO>();
            SumarioAvaliacaoMedicaCTINEO sumario = new SumarioAvaliacaoMedicaCTINEO();
            sumario.MostraCodigoBarras = false;
            sumario.MostraIDPaciente = false;

            sumario.Nome = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.Nome;
            sumario.Prontuario = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.ID.ToString();
            sumario.Atendimento = _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.ID.ToString();
            sumario.DataAtendimento = _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.DataAtendimento.ToShortDateString();
            sumario.Sexo = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.Sexo.ToString();
            sumario.Cor = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.Cor.ToString();
            sumario.EstadoCivil = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.EstadoCivil.ToString();

            if (_wrpSumarioAvaliacaoMedicaCTINEO.DataEncerramento.HasValue && _wrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.HasValue)
                sumario.Idade = new Age(_wrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.Value, _wrpSumarioAvaliacaoMedicaCTINEO.DataEncerramento.Value).GetDate();
            else if (_wrpSumarioAvaliacaoMedicaCTINEO.Paciente.Idade.IsNotNull())
                sumario.Idade = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.Idade.GetDate();

            if (_wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.IsNotNull())
            {
                if (_wrpSumarioAvaliacaoMedicaCTINEO.Paciente.IsNotNull())
                {
                    sumario.NomePaciente = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.Nome;
                    sumario.IDPaciente = _wrpSumarioAvaliacaoMedicaCTINEO.Paciente.ID;
                    sumario.MostraIDPaciente = true;
                }

                sumario.NomeResumo = _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Leito.IsNotNull() ? _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Leito.Descricao : string.Empty;
                sumario.CodigoBarras = _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.ID.ToString();
                sumario.MostraCodigoBarras = true;

                if (_wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Prestador.IsNotNull())
                {
                    sumario.NomePrestador = _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Prestador.Nome;
                    sumario.Registro = _wrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Prestador.Registro;
                }
            }

            // Assinatura
            AssinaturaCTINEO assinatura = new AssinaturaCTINEO();
            _listaAssinaturaSource = new List<AssinaturaCTINEO>();

            if (_wrpSumarioAvaliacaoMedicaCTINEO.DataEncerramento.IsNotNull())
            {
                assinatura.Assinatura = _wrpSumarioAvaliacaoMedicaCTINEO.Usuario.AssinaturaNaLinhaSemColchetes;
                assinatura.DataEncerramento = _wrpSumarioAvaliacaoMedicaCTINEO.DataEncerramento.Value.ToString();
            }
            else
            {
                assinatura.Assinatura = _wrpSumarioAvaliacaoMedicaCTINEO.Usuario.AssinaturaNaLinhaSemColchetes;
            }
            _listaAssinaturaSource.Add(assinatura);

            // Motivo Internação/História
            _listaMotivoInternacaoHistoriaSource = new List<MotivoInternacaoHistoriaCTINEO>();
            _listaMotivoInternacaoHistoriaSource = MotivoInternacaoHistoriaCTINEO(_wrpSumarioAvaliacaoMedicaCTINEO);

            // Dados do Nascimento
            _listaDadosDoNascimentoSource = new List<DadosDoNascimentoCTINEO>();
            _listaDadosDoNascimentoSource = DadosDoNascimentoCTINEO(_wrpSumarioAvaliacaoMedicaCTINEO);

            // Exame Físico
            _listaExameFisicoSource = new List<ExameFisicoCTINEO>();
            _listaExameFisicoSource = ExameFisicoCTINEO(_wrpSumarioAvaliacaoMedicaCTINEO);

            // Diagnósticos/Hipóteses Diagnósticas
            _listaDiagnosticosHipoteseDiagnosticasSource = new List<DiagnosticosHipoteseDiagnosticasCTINEO>();
            _listaDiagnosticosHipoteseDiagnosticasSource = DiagnosticosHipoteseDiagnosticasCTINEO(_wrpSumarioAvaliacaoMedicaCTINEO);

            // Plano Diagnóstico e Terapêutico
            _listaPlanoDiagnosticoETerapeuticoSource = new List<PlanoDiagnosticoETerapeuticoCTINEO>();
            _listaPlanoDiagnosticoETerapeuticoSource = PlanoDiagnosticoETerapeuticoCTINEO(_wrpSumarioAvaliacaoMedicaCTINEO);

            _listaRelatorioSource.Add(sumario);

            return _listaRelatorioSource;
        }

        #endregion

        #region Propriedades Públicas

        public List<SumarioAvaliacaoMedicaCTINEO> _listaRelatorioSource { get; set; }
        public List<AssinaturaCTINEO> _listaAssinaturaSource { get; set; }
        public List<MotivoInternacaoHistoriaCTINEO> _listaMotivoInternacaoHistoriaSource { get; set; }
        public List<DadosDoNascimentoCTINEO> _listaDadosDoNascimentoSource { get; set; }
        public List<ExameFisicoCTINEO> _listaExameFisicoSource { get; set; }
        public List<DiagnosticosHipoteseDiagnosticasCTINEO> _listaDiagnosticosHipoteseDiagnosticasSource { get; set; }
        public List<PlanoDiagnosticoETerapeuticoCTINEO> _listaPlanoDiagnosticoETerapeuticoSource { get; set; }

        #endregion

        #region Propriedades Privadas

        private wrpSumarioAvaliacaoMedicaCTINEO _wrpSumarioAvaliacaoMedicaCTINEO;

        #endregion

        #region Métodos

        private List<MotivoInternacaoHistoriaCTINEO> MotivoInternacaoHistoriaCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<MotivoInternacaoHistoriaCTINEO> lista = new List<MotivoInternacaoHistoriaCTINEO>();
            MotivoInternacaoHistoriaCTINEO motivo = new MotivoInternacaoHistoriaCTINEO();
            motivo.MostraMotivoInternacaoHistoria = false;
            motivo.MostraProcedencia = false;

            // Procedência
            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.Procedencia.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.Procedencia.IsOutros == SimNao.Sim)
                    {
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.ProcedenciaOutros.ConvertNullToStringEmpty().IsNotEmpty())
                            motivo.Procedencia = "Procedência: " + pWrpSumarioAvaliacaoMedicaCTINEO.ProcedenciaOutros;
                        else
                            motivo.Procedencia = "Procedência: " + pWrpSumarioAvaliacaoMedicaCTINEO.Procedencia.Descricao;
                    }
                    else
                        motivo.Procedencia = "Procedência: " + pWrpSumarioAvaliacaoMedicaCTINEO.Procedencia.Descricao;

                    if (motivo.Procedencia.IsNotEmpty())
                    {
                        motivo.MostraMotivoInternacaoHistoria = true;
                        motivo.MostraProcedencia = true;
                    }
                }
            }
            //

            // Motivo de Internação
            motivo.listaMotivoDeInternacao = MotivoDeInternacao(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (motivo.listaMotivoDeInternacao.FirstOrDefault().MostraMotivoDeInternacao)
                motivo.MostraMotivoInternacaoHistoria = true;
            //

            // História Atual
            motivo.listaHistoriaAtual = HistoriaAtual(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (motivo.listaHistoriaAtual.FirstOrDefault().MostraHistoriaAtual)
                motivo.MostraMotivoInternacaoHistoria = true;
            //

            lista.Add(motivo);

            return lista;
        }

        private List<MotivoDeInternacaoCTINEO> MotivoDeInternacao(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<MotivoDeInternacaoCTINEO> lista = new List<MotivoDeInternacaoCTINEO>();
            MotivoDeInternacaoCTINEO motivo = new MotivoDeInternacaoCTINEO();
            motivo.MostraMotivoDeInternacao = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.MotivoInternacao.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    motivo.Motivo = pWrpSumarioAvaliacaoMedicaCTINEO.MotivoInternacao;
                    motivo.MostraMotivoDeInternacao = true;
                }
            }

            lista.Add(motivo);

            return lista;
        }

        private List<HistoriaAtualCTINEO> HistoriaAtual(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<HistoriaAtualCTINEO> lista = new List<HistoriaAtualCTINEO>();
            HistoriaAtualCTINEO historia = new HistoriaAtualCTINEO();
            historia.MostraHistoriaAtual = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.HistoriaAtual.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    historia.Historia = pWrpSumarioAvaliacaoMedicaCTINEO.HistoriaAtual;
                    historia.MostraHistoriaAtual = true;
                }
            }

            lista.Add(historia);

            return lista;
        }

        private List<DadosDoNascimentoCTINEO> DadosDoNascimentoCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<DadosDoNascimentoCTINEO> lista = new List<DadosDoNascimentoCTINEO>();
            DadosDoNascimentoCTINEO dados = new DadosDoNascimentoCTINEO();
            dados.MostraDataDoNascimentoEHora = false;
            dados.MostraDadosDoNascimento = false;
            dados.MostraPesoEIdadeGestacional = false;
            dados.MostraApgarPrimeiro = false;
            dados.MostraApgarSegundo = false;
            dados.MostraApgarTerceiro = false;

            // Data do Nascimento e Hora
            if (pWrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.IsNotNull())
            {
                dados.DataDoNascimentoEHora = "Data do Nascimento: " + pWrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.Value.ToShortDateString() + "     Hora: " + pWrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.Value.ToShortTimeString();

                if (pWrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.Value >= DateTime.Now.AddDays(-5))
                    dados.DataDoNascimentoEHora += "     Idade: " + new Age(pWrpSumarioAvaliacaoMedicaCTINEO.DataNascimento.Value).GetTime() + " de Vida";

                dados.MostraDadosDoNascimento = true;
                dados.MostraDataDoNascimentoEHora = true;
            }

            // Tipo de Parto e Fórcipe
            dados.listaTipoDeParto = TipoDeParto(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (dados.listaTipoDeParto.FirstOrDefault().MostraTipoDeParto)
                dados.MostraDadosDoNascimento = true;
            //

            // Peso e Idade Gestacional
            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.Peso.IsNotNull())
                    dados.PesoEIdadeGestacional = "Peso: " + pWrpSumarioAvaliacaoMedicaCTINEO.Peso + " g";

                if (pWrpSumarioAvaliacaoMedicaCTINEO.IdadeDesconhecida.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.IdadeDesconhecida == SimNao.Sim)
                        if (dados.PesoEIdadeGestacional.IsEmpty())
                            dados.PesoEIdadeGestacional = "Idade Gestacional: Desconhecida";
                        else
                            dados.PesoEIdadeGestacional += "     Idade Gestacional: Desconhecida";
                }

                string idade = string.Empty;

                if (pWrpSumarioAvaliacaoMedicaCTINEO.IdadeSemana.IsNotNull())
                    idade = "Idade Gestacional: " + pWrpSumarioAvaliacaoMedicaCTINEO.IdadeSemana + " semanas";

                if (pWrpSumarioAvaliacaoMedicaCTINEO.IdadeDias.IsNotNull())
                    if (idade.IsEmpty())
                        idade = "Idade Gestacional: " + pWrpSumarioAvaliacaoMedicaCTINEO.IdadeDias + " dias";
                    else
                        idade += " e " + pWrpSumarioAvaliacaoMedicaCTINEO.IdadeDias + " dias";

                if (idade.IsNotEmpty())
                    if (dados.PesoEIdadeGestacional.IsEmpty())
                        dados.PesoEIdadeGestacional = idade;
                    else
                        dados.PesoEIdadeGestacional += "     " + idade;

                if (dados.PesoEIdadeGestacional.IsNotEmpty())
                {
                    dados.MostraDadosDoNascimento = true;
                    dados.MostraPesoEIdadeGestacional = true;
                }
                //

                // Apgar
                if (pWrpSumarioAvaliacaoMedicaCTINEO.ApgarPrimeiro.IsNotNull())
                {
                    dados.TituloPrimeiro = "1º";
                    dados.Primeiro = pWrpSumarioAvaliacaoMedicaCTINEO.ApgarPrimeiro.ToString();
                    dados.MostraDadosDoNascimento = true;
                    dados.MostraApgarPrimeiro = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCTINEO.ApgarQuinto.IsNotNull())
                {
                    if (dados.Primeiro.ConvertNullToStringEmpty().IsEmpty())
                    {
                        dados.TituloPrimeiro = "5º";
                        dados.Primeiro = pWrpSumarioAvaliacaoMedicaCTINEO.ApgarQuinto.ToString();
                        dados.MostraApgarPrimeiro = true;
                    }
                    else
                    {
                        dados.TituloQuinto = "5º";
                        dados.Quinto = pWrpSumarioAvaliacaoMedicaCTINEO.ApgarQuinto.ToString();
                        dados.MostraApgarSegundo = true;
                    }

                    dados.MostraDadosDoNascimento = true;
                }

                if (pWrpSumarioAvaliacaoMedicaCTINEO.ApgarDessimo.IsNotNull())
                {
                    if (dados.Primeiro.ConvertNullToStringEmpty().IsEmpty())
                    {
                        dados.TituloPrimeiro = "10º";
                        dados.Primeiro = pWrpSumarioAvaliacaoMedicaCTINEO.ApgarDessimo.ToString();
                        dados.MostraApgarPrimeiro = true;
                    }
                    else if (dados.Quinto.ConvertNullToStringEmpty().IsEmpty())
                    {
                        dados.TituloQuinto = "10º";
                        dados.Quinto = pWrpSumarioAvaliacaoMedicaCTINEO.ApgarDessimo.ToString();
                        dados.MostraApgarSegundo = true;
                    }
                    else
                    {
                        dados.TituloDecimo = "10º";
                        dados.Decimo = pWrpSumarioAvaliacaoMedicaCTINEO.ApgarDessimo.ToString();
                        dados.MostraApgarTerceiro = true;
                    }

                    dados.MostraDadosDoNascimento = true;
                }
            }
            //

            // Observações
            dados.listaObservacoes = DadosDoNascimentoObservacoes(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (dados.listaObservacoes.FirstOrDefault().MostraObservacoes)
                dados.MostraDadosDoNascimento = true;
            //

            lista.Add(dados);

            return lista;
        }

        private List<TipoDePartoCTINEO> TipoDeParto(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<TipoDePartoCTINEO> lista = new List<TipoDePartoCTINEO>();
            TipoDePartoCTINEO parto = new TipoDePartoCTINEO();
            parto.MostraTipoDeParto = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.TipoParto.IsNotNull())
                    parto.Descricao = pWrpSumarioAvaliacaoMedicaCTINEO.TipoParto.Value.GetEnumCustomDisplay();

                if (pWrpSumarioAvaliacaoMedicaCTINEO.MotivoCesarianaUrgencia.ConvertNullToStringEmpty().IsNotEmpty())
                    if (parto.Descricao.IsEmpty())
                        parto.Descricao = "Motivo: " + pWrpSumarioAvaliacaoMedicaCTINEO.MotivoCesarianaUrgencia;
                    else
                        parto.Descricao += "     Motivo: " + pWrpSumarioAvaliacaoMedicaCTINEO.MotivoCesarianaUrgencia;

                if (pWrpSumarioAvaliacaoMedicaCTINEO.IsForcipe.IsNotNull())
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.IsForcipe == SimNao.Sim)
                    {
                        if (parto.Descricao.IsEmpty())
                            parto.Descricao = "Fórcipe: " + pWrpSumarioAvaliacaoMedicaCTINEO.IsForcipe.Value.GetEnumCustomDisplay();
                        else
                            parto.Descricao += "     Fórcipe: " + pWrpSumarioAvaliacaoMedicaCTINEO.IsForcipe.Value.GetEnumCustomDisplay();
                    }

                if (parto.Descricao.IsNotEmpty())
                    parto.MostraTipoDeParto = true;
            }

            lista.Add(parto);

            return lista;
        }

        private List<DadosDoNascimentoObservacoesCTINEO> DadosDoNascimentoObservacoes(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<DadosDoNascimentoObservacoesCTINEO> lista = new List<DadosDoNascimentoObservacoesCTINEO>();
            DadosDoNascimentoObservacoesCTINEO observacoes = new DadosDoNascimentoObservacoesCTINEO();
            observacoes.MostraObservacoes = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.Observacoes.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    observacoes.Obs = pWrpSumarioAvaliacaoMedicaCTINEO.Observacoes;
                    observacoes.MostraObservacoes = true;
                }
            }

            lista.Add(observacoes);

            return lista;
        }

        private List<ExameFisicoCTINEO> ExameFisicoCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<ExameFisicoCTINEO> lista = new List<ExameFisicoCTINEO>();
            ExameFisicoCTINEO exame = new ExameFisicoCTINEO();
            exame.MostraExameFisico = false;

            exame.listaParte1 = Parte1(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (exame.listaParte1.FirstOrDefault().MostraParte1)
                exame.MostraExameFisico = true;

            exame.listaParte2 = Parte2(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (exame.listaParte2.FirstOrDefault().MostraParte2)
                exame.MostraExameFisico = true;

            exame.listaParte3 = Parte3(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (exame.listaParte3.FirstOrDefault().MostraParte3)
                exame.MostraExameFisico = true;

            lista.Add(exame);

            return lista;
        }

        private List<Parte1CTINEO> Parte1(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<Parte1CTINEO> lista = new List<Parte1CTINEO>();
            Parte1CTINEO parte1 = new Parte1CTINEO();
            parte1.MostraParte1 = false;

            parte1.listaSinaisVitais = SinaisVitais(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte1.listaSinaisVitais.FirstOrDefault().MostraSinaisVitais)
                parte1.MostraParte1 = true;

            parte1.listaAspectoGeral = AspectoGeral(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte1.listaAspectoGeral.FirstOrDefault().MostraAspectoGeral)
                parte1.MostraParte1 = true;

            parte1.listaCabecaEPescoco = CabecaEPescoco(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte1.listaCabecaEPescoco.FirstOrDefault().MostraCabecaEPescoco)
                parte1.MostraParte1 = true;

            lista.Add(parte1);

            return lista;
        }

        private List<SinaisVitaisCTINEO> SinaisVitais(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<SinaisVitaisCTINEO> lista = new List<SinaisVitaisCTINEO>();
            SinaisVitaisCTINEO sinais = new SinaisVitaisCTINEO();
            sinais.MostraSinaisVitais = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FC.IsNotNull())
                        sinais.Descricao = "FC: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FC + " bpm";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FR.IsNotNull())
                        if (sinais.Descricao.IsEmpty())
                            sinais.Descricao = "FR: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FR + " mrpm";
                        else
                            sinais.Descricao += "     FR: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FR + " mrpm";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX.IsNotNull())
                        if (sinais.Descricao.IsEmpty())
                            sinais.Descricao = "Tax: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX + " °C";
                        else
                            sinais.Descricao += "     Tax: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX + " °C";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT.IsNotNull())
                        if (sinais.Descricao.IsEmpty())
                            sinais.Descricao = "SAT: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT + " %";
                        else
                            sinais.Descricao += "     SAT: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT + " %";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM.IsNotNull())
                        if (sinais.Descricao.IsEmpty())
                            sinais.Descricao = "PAM: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM + " mmhg";
                        else
                            sinais.Descricao += "     PAM: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM + " mmhg";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.DOR.IsNotNull())
                        if (sinais.Descricao.IsEmpty())
                            sinais.Descricao = "Dor (NIPS): " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.DOR.Value.GetEnumCustomDisplay();
                        else
                            sinais.Descricao += "     Dor (NIPS) : " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.DOR.Value.GetEnumCustomDisplay();

                    if (sinais.Descricao.IsNotEmpty())
                        sinais.MostraSinaisVitais = true;
                }
            }

            lista.Add(sinais);

            return lista;
        }

        private List<AspectoGeralCTINEO> AspectoGeral(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<AspectoGeralCTINEO> lista = new List<AspectoGeralCTINEO>();
            AspectoGeralCTINEO aspecto = new AspectoGeralCTINEO();
            aspecto.MostraAspectoGeral = false;
            aspecto.MostraFacies = false;
            aspecto.MostraCorETonus = false;
            aspecto.MostraAtividadeECotoUmbilical = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNotNull())
                {
                    // Facies
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsFacies.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsFacies == SimNao.Sim)
                            aspecto.Facies = "Facies: Atípica";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FaciesObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                        aspecto.Facies = "Facies: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.FaciesObservacao;

                    if (aspecto.Facies.IsNotEmpty())
                    {
                        aspecto.MostraAspectoGeral = true;
                        aspecto.MostraFacies = true;
                    }
                    //

                    // Cor e Tônus
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorRosada.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorRosada == SimNao.Sim)
                            aspecto.CorETonus = "Cor: Rosada";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorPalida.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorPalida == SimNao.Sim)
                            if (aspecto.CorETonus.IsEmpty())
                                aspecto.CorETonus = "Cor: Pálida";
                            else
                                aspecto.CorETonus += ", Pálida";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianose.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianose == SimNao.Sim)
                            if (aspecto.CorETonus.IsEmpty())
                                aspecto.CorETonus = "Cor: Cianose de Extremidades";
                            else
                                aspecto.CorETonus += ", Cianose de Extremidades";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianoseGeneralizada.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianoseGeneralizada == SimNao.Sim)
                            if (aspecto.CorETonus.IsEmpty())
                                aspecto.CorETonus = "Cor: Cianose Generalizada";
                            else
                                aspecto.CorETonus += ", Cianose Generalizada";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.IsNotNull())
                        if (aspecto.CorETonus.IsEmpty())
                            aspecto.CorETonus = "Tônus: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.Value.GetEnumCustomDisplay();
                        else
                            aspecto.CorETonus += "     Tônus: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.Value.GetEnumCustomDisplay();

                    if (aspecto.CorETonus.IsNotEmpty())
                    {
                        aspecto.MostraAspectoGeral = true;
                        aspecto.MostraCorETonus = true;
                    }
                    //

                    // Atividade e Coto Umbilical
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Atividade == AtividadeRN.Reativo)
                            aspecto.AtividadeECotoUmbilical = "Atividade: " + "Reativo ao Manuseio";
                        else
                            aspecto.AtividadeECotoUmbilical = "Atividade: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.Value.GetEnumCustomDisplay();

                    string coto = string.Empty;

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical == SimNao.Sim)
                            coto = "Coto Umbilical: 2 artérias e 1 veia";

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.UmbilicalAlteracoes.ConvertNullToStringEmpty().IsNotEmpty())
                        coto = "Coto Umbilical: " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.UmbilicalAlteracoes;

                    if (coto.IsNotEmpty())
                    {
                        if (aspecto.AtividadeECotoUmbilical.IsEmpty())
                            aspecto.AtividadeECotoUmbilical = coto;
                        else
                            aspecto.AtividadeECotoUmbilical += "     " + coto;

                        aspecto.MostraAspectoGeral = true;
                        aspecto.MostraAtividadeECotoUmbilical = true;
                    }

                    string pele = string.Empty;

                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Pele.IsNotNull())
                        if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Pele == Pele.Normal)
                            if (aspecto.AtividadeECotoUmbilical.IsNotEmpty())
                                aspecto.AtividadeECotoUmbilical += "     Pele: " + "Normal";
                            else
                                aspecto.AtividadeECotoUmbilical = "Pele: " + "Normal";
                        else
                            if (aspecto.AtividadeECotoUmbilical.IsNotEmpty())
                                aspecto.AtividadeECotoUmbilical += "     Pele: " + " " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.PeleOutros;
                            else
                                aspecto.AtividadeECotoUmbilical = "Pele: " + " " + pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.PeleOutros;

                    if (pele.IsNotEmpty())
                    {
                        if (aspecto.AtividadeECotoUmbilical.IsEmpty())
                            aspecto.AtividadeECotoUmbilical = coto;
                        else
                            aspecto.AtividadeECotoUmbilical += "     " + coto;

                        aspecto.MostraAspectoGeral = true;
                        aspecto.MostraAtividadeECotoUmbilical = true;
                    }
                    //
                }
            }

            lista.Add(aspecto);

            return lista;
        }

        private List<CabecaEPescocoCTINEO> CabecaEPescoco(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<CabecaEPescocoCTINEO> lista = new List<CabecaEPescocoCTINEO>();
            List<CabecaEPescocoCTINEO> listaNormais = new List<CabecaEPescocoCTINEO>();
            CabecaEPescocoCTINEO cabecaEPescoco;
            string normais = string.Empty;
            string olhinhos = string.Empty;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.CabecaPescoco == SimNao.Sim))
                    {
                        if (item.IsNormal.IsNotNull())
                        {
                            if (item.IsNormal == SimNao.Sim)
                            {
                                if (item.ItemCO.Descricao == "Teste do Olhinho")
                                {
                                    olhinhos = "Teste do Olhinho: Normal";
                                }
                                else
                                {
                                    if (normais.IsNotEmpty())
                                        normais += ", " + item.ItemCO.Descricao;
                                    else
                                        normais += "Normal: " + item.ItemCO.Descricao;
                                }
                            }
                            else
                            {
                                cabecaEPescoco = new CabecaEPescocoCTINEO();
                                cabecaEPescoco.Descricao = item.ItemCO.Descricao + ": " + item.Observacao;
                                cabecaEPescoco.MostraCabecaEPescoco = true;
                                lista.Add(cabecaEPescoco);
                            }
                        }
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new CabecaEPescocoCTINEO { Descricao = normais, MostraCabecaEPescoco = true });

            if (olhinhos.IsNotEmpty())
                listaNormais.Add(new CabecaEPescocoCTINEO { Descricao = olhinhos, MostraCabecaEPescoco = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                cabecaEPescoco = new CabecaEPescocoCTINEO();
                cabecaEPescoco.MostraCabecaEPescoco = false;
                lista.Add(cabecaEPescoco);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<Parte2CTINEO> Parte2(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<Parte2CTINEO> lista = new List<Parte2CTINEO>();
            Parte2CTINEO parte2 = new Parte2CTINEO();
            parte2.MostraParte2 = false;

            parte2.listaRespiratorio = Respiratorio(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte2.listaRespiratorio.FirstOrDefault().MostraRespiratorio)
                parte2.MostraParte2 = true;

            parte2.listaCardioVascular = CardioVascular(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte2.listaCardioVascular.FirstOrDefault().MostraCardioVascular)
                parte2.MostraParte2 = true;

            parte2.listaSemNome = SemNome(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte2.listaSemNome.FirstOrDefault().MostraSemNome)
                parte2.MostraParte2 = true;

            lista.Add(parte2);

            return lista;
        }

        private List<RespiratorioCTINEO> Respiratorio(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<RespiratorioCTINEO> lista = new List<RespiratorioCTINEO>();
            List<RespiratorioCTINEO> listaNormais = new List<RespiratorioCTINEO>();
            RespiratorioCTINEO respiratorio;
            string normais = string.Empty;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.Respiratorio == SimNao.Sim))
                    {
                        if (item.IsNormal.IsNotNull())
                        {
                            if (item.IsNormal == SimNao.Sim)
                            {
                                if (normais.IsNotEmpty())
                                    normais += ", " + item.ItemCO.Descricao;
                                else
                                    normais += "Normal: " + item.ItemCO.Descricao;
                            }
                            else
                            {
                                respiratorio = new RespiratorioCTINEO();
                                respiratorio.Descricao = item.ItemCO.Descricao + ": " + item.Observacao;
                                respiratorio.MostraRespiratorio = true;
                                lista.Add(respiratorio);
                            }
                        }
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new RespiratorioCTINEO { Descricao = normais, MostraRespiratorio = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                respiratorio = new RespiratorioCTINEO();
                respiratorio.MostraRespiratorio = false;
                lista.Add(respiratorio);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<CardioVascularCTINEO> CardioVascular(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<CardioVascularCTINEO> lista = new List<CardioVascularCTINEO>();
            List<CardioVascularCTINEO> listaNormais = new List<CardioVascularCTINEO>();
            CardioVascularCTINEO cardioVascular;
            string normais = string.Empty;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.Cardio == SimNao.Sim))
                    {
                        if (item.IsNormal.IsNotNull())
                        {
                            if (item.IsNormal == SimNao.Sim)
                            {
                                if (normais.IsNotEmpty())
                                    normais += ", " + item.ItemCO.Descricao;
                                else
                                    normais += "Normal: " + item.ItemCO.Descricao;
                            }
                            else
                            {
                                cardioVascular = new CardioVascularCTINEO();
                                cardioVascular.Descricao = item.ItemCO.Descricao + ": " + item.Observacao;
                                cardioVascular.MostraCardioVascular = true;
                                lista.Add(cardioVascular);
                            }
                        }
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new CardioVascularCTINEO { Descricao = normais, MostraCardioVascular = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                cardioVascular = new CardioVascularCTINEO();
                cardioVascular.MostraCardioVascular = false;
                lista.Add(cardioVascular);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<SemNomeCTINEO> SemNome(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<SemNomeCTINEO> lista = new List<SemNomeCTINEO>();
            List<SemNomeCTINEO> listaNormais = new List<SemNomeCTINEO>();
            SemNomeCTINEO semNome;
            string normais = string.Empty;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.Outros == SimNao.Sim))
                    {
                        if (item.IsNormal.IsNotNull())
                        {
                            if (item.IsNormal == SimNao.Sim)
                            {
                                if (normais.IsNotEmpty())
                                    normais += ", " + item.ItemCO.Descricao;
                                else
                                    normais += "Normal: " + item.ItemCO.Descricao;
                            }
                            else
                            {
                                semNome = new SemNomeCTINEO();
                                semNome.Descricao = item.ItemCO.Descricao + ": " + item.Observacao;
                                semNome.MostraSemNome = true;
                                lista.Add(semNome);
                            }
                        }
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new SemNomeCTINEO { Descricao = normais, MostraSemNome = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                semNome = new SemNomeCTINEO();
                semNome.MostraSemNome = false;
                lista.Add(semNome);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<Parte3CTINEO> Parte3(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<Parte3CTINEO> lista = new List<Parte3CTINEO>();
            Parte3CTINEO parte3 = new Parte3CTINEO();
            parte3.MostraParte3 = false;

            parte3.listaOsteoarticular = Osteoarticular(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte3.listaOsteoarticular.FirstOrDefault().MostraOsteoarticular)
                parte3.MostraParte3 = true;

            parte3.listaObservacoes = ExameFisicoObservacoes(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (parte3.listaObservacoes.FirstOrDefault().MostraObservacoes)
                parte3.MostraParte3 = true;

            lista.Add(parte3);

            return lista;
        }

        private List<OsteoarticularCTINEO> Osteoarticular(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<OsteoarticularCTINEO> lista = new List<OsteoarticularCTINEO>();
            List<OsteoarticularCTINEO> listaNormais = new List<OsteoarticularCTINEO>();
            OsteoarticularCTINEO osteoarticular;
            string normais = string.Empty;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.OsteoArticular == SimNao.Sim))
                    {
                        if (item.IsNormal.IsNotNull())
                        {
                            if (item.IsNormal == SimNao.Sim)
                            {
                                if (normais.IsNotEmpty())
                                    normais += ", " + item.ItemCO.Descricao;
                                else
                                    normais += "Normal: " + item.ItemCO.Descricao;
                            }
                            else
                            {
                                osteoarticular = new OsteoarticularCTINEO();
                                osteoarticular.Descricao = item.ItemCO.Descricao + ": " + item.Observacao;
                                osteoarticular.MostraOsteoarticular = true;
                                lista.Add(osteoarticular);
                            }
                        }
                    }
                }
            }

            if (normais.IsNotEmpty())
                listaNormais.Add(new OsteoarticularCTINEO { Descricao = normais, MostraOsteoarticular = true });

            if (lista.Count == 0 && listaNormais.Count == 0)
            {
                osteoarticular = new OsteoarticularCTINEO();
                osteoarticular.MostraOsteoarticular = false;
                lista.Add(osteoarticular);
            }
            else
            {
                listaNormais.AddRange(lista);
                return listaNormais;
            }

            return lista;
        }

        private List<ExameFisicoObservacoesCTINEO> ExameFisicoObservacoes(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<ExameFisicoObservacoesCTINEO> lista = new List<ExameFisicoObservacoesCTINEO>();
            ExameFisicoObservacoesCTINEO observacoes = new ExameFisicoObservacoesCTINEO();
            observacoes.MostraObservacoes = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Observacao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        observacoes.Descricao = pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Observacao;
                        observacoes.MostraObservacoes = true;
                    }
                }
            }

            lista.Add(observacoes);

            return lista;
        }

        private List<DiagnosticosHipoteseDiagnosticasCTINEO> DiagnosticosHipoteseDiagnosticasCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<DiagnosticosHipoteseDiagnosticasCTINEO> lista = new List<DiagnosticosHipoteseDiagnosticasCTINEO>();
            DiagnosticosHipoteseDiagnosticasCTINEO diagnostico = new DiagnosticosHipoteseDiagnosticasCTINEO();
            diagnostico.MostraDiagnosticosHipoteseDiagnosticas = false;

            diagnostico.listaCidPrincipalDoAtendimento = CidPrincipalDoAtendimento(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (diagnostico.listaCidPrincipalDoAtendimento.FirstOrDefault().MostraCidPrincipalDoAtendimento)
                diagnostico.MostraDiagnosticosHipoteseDiagnosticas = true;

            diagnostico.listaOutrosCids = OutrosCids(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (diagnostico.listaOutrosCids.FirstOrDefault().MostraOutrosCids)
                diagnostico.MostraDiagnosticosHipoteseDiagnosticas = true;

            diagnostico.listaHipotesesDiagnosticas = HipotesesDiagnosticas(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (diagnostico.listaHipotesesDiagnosticas.FirstOrDefault().MostraHipotesesDiagnosticas)
                diagnostico.MostraDiagnosticosHipoteseDiagnosticas = true;

            lista.Add(diagnostico);

            return lista;
        }

        private List<CidPrincipalDoAtendimentoCTINEO> CidPrincipalDoAtendimento(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<CidPrincipalDoAtendimentoCTINEO> lista = new List<CidPrincipalDoAtendimentoCTINEO>();
            CidPrincipalDoAtendimentoCTINEO cid = new CidPrincipalDoAtendimentoCTINEO();
            cid.MostraCidPrincipalDoAtendimento = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.Atendimento.IsNotNull())
                {
                    if (pWrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Cid.IsNotNull())
                    {
                        cid.Descricao = "CID: " + pWrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Cid.Id + "  -  " + pWrpSumarioAvaliacaoMedicaCTINEO.Atendimento.Cid.Descricao;
                        cid.MostraCidPrincipalDoAtendimento = true;
                    }
                }
            }

            lista.Add(cid);

            return lista;
        }

        private List<OutrosCidsCTINEO> OutrosCids(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<OutrosCidsCTINEO> lista = new List<OutrosCidsCTINEO>();
            OutrosCidsCTINEO outrosCids;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEODiagnostico.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEODiagnostico)
                    {
                        outrosCids = new OutrosCidsCTINEO();
                        outrosCids.CID = item.CID10.Id;
                        outrosCids.DescricaoCID = item.CID10.Descricao;
                        outrosCids.Complemento = item.Complemento;
                        outrosCids.MostraOutrosCids = true;
                        lista.Add(outrosCids);
                    }
                }
            }

            if (lista.Count == 0)
            {
                outrosCids = new OutrosCidsCTINEO();
                outrosCids.MostraOutrosCids = false;
                lista.Add(outrosCids);
            }

            return lista;
        }

        private List<HipotesesDiagnosticasCTINEO> HipotesesDiagnosticas(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<HipotesesDiagnosticasCTINEO> lista = new List<HipotesesDiagnosticasCTINEO>();
            HipotesesDiagnosticasCTINEO hipoteses;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOHipotese.IsNotNull())
                {
                    foreach (var item in pWrpSumarioAvaliacaoMedicaCTINEO.SumarioAvaliacaoMedicaCTINEOHipotese)
                    {
                        hipoteses = new HipotesesDiagnosticasCTINEO();
                        hipoteses.Hipotese = item.Hipotese;
                        hipoteses.MostraHipotesesDiagnosticas = true;
                        lista.Add(hipoteses);
                    }
                }
            }

            if (lista.Count == 0)
            {
                hipoteses = new HipotesesDiagnosticasCTINEO();
                hipoteses.MostraHipotesesDiagnosticas = false;
                lista.Add(hipoteses);
            }

            return lista;
        }

        private List<PlanoDiagnosticoETerapeuticoCTINEO> PlanoDiagnosticoETerapeuticoCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<PlanoDiagnosticoETerapeuticoCTINEO> lista = new List<PlanoDiagnosticoETerapeuticoCTINEO>();
            PlanoDiagnosticoETerapeuticoCTINEO plano = new PlanoDiagnosticoETerapeuticoCTINEO();
            plano.MostraPlanoDiagnosticoETerapeutico = false;

            plano.listaExame = Exame(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (plano.listaExame.FirstOrDefault().MostraExame)
                plano.MostraPlanoDiagnosticoETerapeutico = true;

            plano.listaConduta = Conduta(pWrpSumarioAvaliacaoMedicaCTINEO);
            if (plano.listaConduta.FirstOrDefault().MostraConduta)
                plano.MostraPlanoDiagnosticoETerapeutico = true;

            lista.Add(plano);

            return lista;
        }

        private List<ExameCTINEO> Exame(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<ExameCTINEO> lista = new List<ExameCTINEO>();
            ExameCTINEO exame = new ExameCTINEO();
            exame.MostraExame = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.Exames.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    exame.Exames = pWrpSumarioAvaliacaoMedicaCTINEO.Exames;
                    exame.MostraExame = true;
                }
            }

            lista.Add(exame);

            return lista;
        }

        private List<CondutaCTINEO> Conduta(wrpSumarioAvaliacaoMedicaCTINEO pWrpSumarioAvaliacaoMedicaCTINEO)
        {
            List<CondutaCTINEO> lista = new List<CondutaCTINEO>();
            CondutaCTINEO conduta = new CondutaCTINEO();
            conduta.MostraConduta = false;

            if (pWrpSumarioAvaliacaoMedicaCTINEO.IsNotNull())
            {
                if (pWrpSumarioAvaliacaoMedicaCTINEO.Conduta.ConvertNullToStringEmpty().IsNotEmpty())
                {
                    conduta.Conduta = pWrpSumarioAvaliacaoMedicaCTINEO.Conduta;
                    conduta.MostraConduta = true;
                }
            }

            lista.Add(conduta);

            return lista;
        }

        #endregion

    }

    public class SumarioAvaliacaoMedicaCTINEO
    {
        public string Nome { get; set; }
        public string Prontuario { get; set; }
        public string Atendimento { get; set; }
        public string DataAtendimento { get; set; }
        public string Sexo { get; set; }
        public string Cor { get; set; }
        public string EstadoCivil { get; set; }
        public string Idade { get; set; }

        public string NomePaciente { get; set; }
        public string NomeResumo { get; set; }
        public int IDPaciente { get; set; }
        public string NomePrestador { get; set; }
        public string Registro { get; set; }
        public string CodigoBarras { get; set; }

        public bool MostraCodigoBarras { get; set; }
        public bool MostraIDPaciente { get; set; }
    }

    public class AssinaturaCTINEO
    {
        public string Assinatura { get; set; }
        public string DataEncerramento { get; set; }
    }

    public class MotivoInternacaoHistoriaCTINEO
    {
        public string Procedencia { get; set; }

        public bool MostraMotivoInternacaoHistoria { get; set; }
        public bool MostraProcedencia { get; set; }

        public List<MotivoDeInternacaoCTINEO> listaMotivoDeInternacao { get; set; }
        public List<HistoriaAtualCTINEO> listaHistoriaAtual { get; set; }
    }

    public class MotivoDeInternacaoCTINEO
    {
        public string Motivo { get; set; }

        public bool MostraMotivoDeInternacao { get; set; }
    }

    public class HistoriaAtualCTINEO
    {
        public string Historia { get; set; }

        public bool MostraHistoriaAtual { get; set; }
    }

    public class DadosDoNascimentoCTINEO
    {
        public string DataDoNascimentoEHora { get; set; }
        public string PesoEIdadeGestacional { get; set; }
        public string TituloPrimeiro { get; set; }
        public string TituloQuinto { get; set; }
        public string TituloDecimo { get; set; }
        public string Primeiro { get; set; }
        public string Quinto { get; set; }
        public string Decimo { get; set; }

        public bool MostraDadosDoNascimento { get; set; }
        public bool MostraDataDoNascimentoEHora { get; set; }
        public bool MostraPesoEIdadeGestacional { get; set; }
        public bool MostraApgarPrimeiro { get; set; }
        public bool MostraApgarSegundo { get; set; }
        public bool MostraApgarTerceiro { get; set; }

        public List<TipoDePartoCTINEO> listaTipoDeParto { get; set; }
        public List<DadosDoNascimentoObservacoesCTINEO> listaObservacoes { get; set; }
    }

    public class TipoDePartoCTINEO
    {
        public string Descricao { get; set; }

        public bool MostraTipoDeParto { get; set; }
    }

    public class DadosDoNascimentoObservacoesCTINEO
    {
        public string Obs { get; set; }

        public bool MostraObservacoes { get; set; }
    }

    public class ExameFisicoCTINEO
    {
        public List<Parte1CTINEO> listaParte1 { get; set; }
        public List<Parte2CTINEO> listaParte2 { get; set; }
        public List<Parte3CTINEO> listaParte3 { get; set; }

        public bool MostraExameFisico { get; set; }
    }

    public class Parte1CTINEO
    {
        public List<SinaisVitaisCTINEO> listaSinaisVitais { get; set; }
        public List<AspectoGeralCTINEO> listaAspectoGeral { get; set; }
        public List<CabecaEPescocoCTINEO> listaCabecaEPescoco { get; set; }

        public bool MostraParte1 { get; set; }
    }

    public class SinaisVitaisCTINEO
    {
        public string Descricao { get; set; }

        public bool MostraSinaisVitais { get; set; }
    }

    public class AspectoGeralCTINEO
    {
        public string Facies { get; set; }
        public string CorETonus { get; set; }
        public string AtividadeECotoUmbilical { get; set; }

        public bool MostraAspectoGeral { get; set; }
        public bool MostraFacies { get; set; }
        public bool MostraCorETonus { get; set; }
        public bool MostraAtividadeECotoUmbilical { get; set; }
    }

    public class CabecaEPescocoCTINEO
    {
        public string Descricao { get; set; }
        public string Normais { get; set; }
        public string Observacoes { get; set; }

        public bool MostraCabecaEPescoco { get; set; }
    }

    public class Parte2CTINEO
    {
        public List<RespiratorioCTINEO> listaRespiratorio { get; set; }
        public List<CardioVascularCTINEO> listaCardioVascular { get; set; }
        public List<SemNomeCTINEO> listaSemNome { get; set; }

        public bool MostraParte2 { get; set; }
    }

    public class RespiratorioCTINEO
    {
        public string Descricao { get; set; }
        public string Normais { get; set; }
        public string Observacoes { get; set; }

        public bool MostraRespiratorio { get; set; }
    }

    public class CardioVascularCTINEO
    {
        public string Descricao { get; set; }
        public string Normais { get; set; }
        public string Observacoes { get; set; }

        public bool MostraCardioVascular { get; set; }
    }

    public class SemNomeCTINEO
    {
        public string Descricao { get; set; }
        public string Normais { get; set; }
        public string Observacoes { get; set; }

        public bool MostraSemNome { get; set; }
    }

    public class Parte3CTINEO
    {
        public List<OsteoarticularCTINEO> listaOsteoarticular { get; set; }
        public List<ExameFisicoObservacoesCTINEO> listaObservacoes { get; set; }

        public bool MostraParte3 { get; set; }
    }

    public class OsteoarticularCTINEO
    {
        public string Descricao { get; set; }
        public string Normais { get; set; }
        public string Observacoes { get; set; }

        public bool MostraOsteoarticular { get; set; }
    }

    public class ExameFisicoObservacoesCTINEO
    {
        public string Descricao { get; set; }

        public bool MostraObservacoes { get; set; }
    }

    public class DiagnosticosHipoteseDiagnosticasCTINEO
    {
        public bool MostraDiagnosticosHipoteseDiagnosticas { get; set; }

        public List<CidPrincipalDoAtendimentoCTINEO> listaCidPrincipalDoAtendimento { get; set; }
        public List<OutrosCidsCTINEO> listaOutrosCids { get; set; }
        public List<HipotesesDiagnosticasCTINEO> listaHipotesesDiagnosticas { get; set; }
    }

    public class CidPrincipalDoAtendimentoCTINEO
    {
        public string Descricao { get; set; }

        public bool MostraCidPrincipalDoAtendimento { get; set; }
    }

    public class OutrosCidsCTINEO
    {
        public string CID { get; set; }
        public string DescricaoCID { get; set; }
        public string Complemento { get; set; }

        public bool MostraOutrosCids { get; set; }
    }

    public class HipotesesDiagnosticasCTINEO
    {
        public string Hipotese { get; set; }

        public bool MostraHipotesesDiagnosticas { get; set; }
    }

    public class PlanoDiagnosticoETerapeuticoCTINEO
    {
        public List<ExameCTINEO> listaExame { get; set; }
        public List<CondutaCTINEO> listaConduta { get; set; }

        public bool MostraPlanoDiagnosticoETerapeutico { get; set; }
    }

    public class ExameCTINEO
    {
        public string Exames { get; set; }

        public bool MostraExame { get; set; }
    }

    public class CondutaCTINEO
    {
        public string Conduta { get; set; }

        public bool MostraConduta { get; set; }
    }
}
