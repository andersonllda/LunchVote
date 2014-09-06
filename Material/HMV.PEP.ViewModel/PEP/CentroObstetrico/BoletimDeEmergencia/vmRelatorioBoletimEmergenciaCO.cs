using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmRelatorioBoletimEmergenciaCO
    {
        public vmRelatorioBoletimEmergenciaCO(Core.Wrappers.ObjectWrappers.wrpBoletimDeEmergencia wrpBoletimDeEmergencia)
        {
            this.wrpBoletimDeEmergencia = wrpBoletimDeEmergencia;
        }

        #region Relatorio

        private List<BoletimEmergenciaRelatorioCo> LisRelatorio = new List<BoletimEmergenciaRelatorioCo>();
        private Core.Wrappers.ObjectWrappers.wrpBoletimDeEmergencia wrpBoletimDeEmergencia;
        public List<BoletimEmergenciaRelatorioCo> Relatorio()
        {
            BoletimEmergenciaRelatorioCo boletim = new BoletimEmergenciaRelatorioCo();
            boletim.MostraCodigoBarras = false;
            boletim.MostraIDPaciente = false;

            boletim.AtendimentoId = this.wrpBoletimDeEmergencia.Atendimento.ID.ToString();
            boletim.PacienteDataCadastro = this.wrpBoletimDeEmergencia.DataInclusao.Value.ToString("dd/MM/yyyy HH:mm");
            boletim.PacienteIdade = this.wrpBoletimDeEmergencia.Atendimento.Paciente.Idade.GetDate(this.wrpBoletimDeEmergencia.Atendimento.HoraAtendimento);
            //boletim.PacienteIdade = this.wrpBoletimDeEmergencia.Atendimento.Paciente.Idade.ToString();
            boletim.PacienteNome = this.wrpBoletimDeEmergencia.Atendimento.Paciente.Nome;

            if (this.wrpBoletimDeEmergencia.Atendimento.IsNotNull())
            {
                if (this.wrpBoletimDeEmergencia.Atendimento.Paciente.IsNotNull())
                {
                    boletim.NomePaciente = this.wrpBoletimDeEmergencia.Atendimento.Paciente.Nome;
                    boletim.IDPaciente = this.wrpBoletimDeEmergencia.Atendimento.Paciente.ID;
                    boletim.MostraIDPaciente = true;
                }

                boletim.NomeResumo = this.wrpBoletimDeEmergencia.Atendimento.Leito.IsNotNull() ? this.wrpBoletimDeEmergencia.Atendimento.Leito.Descricao : string.Empty;
                boletim.CodigoBarras = this.wrpBoletimDeEmergencia.Atendimento.ID.ToString();
                boletim.MostraCodigoBarras = true;

                if (this.wrpBoletimDeEmergencia.Atendimento.Prestador.IsNotNull())
                {
                    boletim.NomePrestador = this.wrpBoletimDeEmergencia.Atendimento.Prestador.Nome;
                    boletim.Registro = this.wrpBoletimDeEmergencia.Atendimento.Prestador.Registro;
                }
            }


            if (this.wrpBoletimDeEmergencia.UsuarioAlta.IsNotNull())
                boletim.DR = this.wrpBoletimDeEmergencia.UsuarioAlta.Assinatura;
            
            //if (this.wrpBoletimDeEmergencia.UsuarioAlta != null && this.wrpBoletimDeEmergencia.UsuarioAlta.Profissional != null)
                //boletim.DR = (this.wrpBoletimDeEmergencia.UsuarioAlta.Profissional.tratamento != null ? this.wrpBoletimDeEmergencia.UsuarioAlta.Profissional.tratamento : " - ") + " " + this.wrpBoletimDeEmergencia.UsuarioAlta.Nome + " (" + this.wrpBoletimDeEmergencia.UsuarioAlta.Profissional.conselho + " " + this.wrpBoletimDeEmergencia.UsuarioAlta.Profissional.registro + ")";
            //else if (this.wrpBoletimDeEmergencia.UsuarioAlta != null && this.wrpBoletimDeEmergencia.UsuarioAlta.Prestador != null)
                //boletim.DR = this.wrpBoletimDeEmergencia.UsuarioAlta.Prestador.NomeExibicao + " (" + this.wrpBoletimDeEmergencia.UsuarioAlta.Prestador.Conselho + " " + this.wrpBoletimDeEmergencia.UsuarioAlta.Prestador.Registro + ")";
            //.NomeExibicaoPrestador

            LisRelatorio.Clear();

            #region Classificacao

            boletim.ListaClassificacao = new List<ListaClassificacao>();

            foreach (var item in this.wrpBoletimDeEmergencia.Classificacoes.OrderBy(x => x.DataHoraInclusaoDate))
            {
                boletim.ListaClassificacao.Add(new ListaClassificacao()
                {
                    Cor = item.Cor.Descricao,
                    Data = item.DataHoraInclusaoDate.ToString("dd/MM/yyyy HH:mm"),
                    //Usuario = item.Usuario.Prestador.Nome + " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : "")
                    //        + item.Usuario.cd_usuario + ")" 
                    Usuario = item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")"),
                });
            }

            _listaClassificacaoSource = boletim.ListaClassificacao.ToList();

            #endregion Classificacao

            #region SinaisVitais

            boletim.ListaSinaisVitais = new List<ListaSinaisVitais>();

            if (this.wrpBoletimDeEmergencia.SinaisVitais.Select(x => x.DataHoraInclusaoDate).OrderBy(x => x).Distinct().Count() > 0)
            {
                foreach (var data in this.wrpBoletimDeEmergencia.SinaisVitais.OrderBy(x => x.DataHoraInclusaoDate).Select(x => x.DataHoraInclusaoDate).OrderBy(x => x).Distinct())
                {
                    ListaSinaisVitais sinais = new ListaSinaisVitais();
                    foreach (var item in this.wrpBoletimDeEmergencia.SinaisVitais.Where(x => x.Data.Equals(data)))
                    {
                        switch (item.Sigla.Descricao)
                        {
                            case "FC":
                                sinais.Fc = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                break;
                            case "FR":
                                sinais.Fr = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                break;
                            case "PA":
                                sinais.Pa = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                break;
                            case "TAX":
                                sinais.Tax = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                break;
                            case "DOR":
                                sinais.Dor = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                break;
                            case "BCF":
                                sinais.Bcf = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                break;
                        }
                        //sinais.Usuario = item.Usuario.Prestador.Nome
                        //    + " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : "CRM ")
                        //    + item.Usuario.DomainObject.Prestador.Registro
                        //    + ")";
                        sinais.Usuario = item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")");
                        sinais.Data = item.Data.ToString("dd/MM/yyyy HH:mm");
                    }

                    sinais.Fc = sinais.Fc.IsEmptyOrWhiteSpace() ? " - " : sinais.Fc;
                    sinais.Fr = sinais.Fr.IsEmptyOrWhiteSpace() ? " - " : sinais.Fr;
                    sinais.Pa = sinais.Pa.IsEmptyOrWhiteSpace() ? " - " : sinais.Pa;
                    sinais.Tax = sinais.Tax.IsEmptyOrWhiteSpace() ? " - " : sinais.Tax;
                    sinais.Dor = sinais.Dor.IsEmptyOrWhiteSpace() ? " - " : sinais.Dor;
                    sinais.Bcf = sinais.Bcf.IsEmptyOrWhiteSpace() ? " - " : sinais.Bcf;

                    boletim.ListaSinaisVitais.Add(sinais);
                }
            }
            else
            {
                boletim.ListaSinaisVitais.Add(new ListaSinaisVitais() { Data = "-", Dor = "-", Fc = "-", Fr = "-", Pa = "-", Tax = "-", Bcf = "-", Usuario = "-", TemReg = "" });
            }

            _listaSinaisVitaisSource = boletim.ListaSinaisVitais.OrderBy(x => x.Data).ToList();

            #endregion SinaisVitais

            #region MotivoConsulta

            boletim.ListaMotivoConsulta = new List<ListaMotivoConsulta>();

            foreach (var item in this.wrpBoletimDeEmergencia.MotivoConsultaCO.OrderBy(x => x.DataHoraInclusaoDate))
            {
                boletim.ListaMotivoConsulta.Add(new ListaMotivoConsulta()
                {

                    Descricao = item.Texto,
                    Profissional = item.DataHoraInclusao + " - " + item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")"),
                });
            }

            _listaMotivoConsultaSource = boletim.ListaMotivoConsulta.ToList();

            #endregion MotivoConsulta

            #region AvalicaoClinica

            boletim.ListaAvaliacaoClinica = new List<ListaAvaliacaoClinica>();

            foreach (var item in this.wrpBoletimDeEmergencia.CentroObstetrico.AvaliacoesClinicas.OrderBy(x => x.DataInclusao))
            {
                List<ListaAvaliacaoAnamnese> sublista = null;
                List<ListaAvaliacaoExameFisico> sublistaex = null;

                sublista = ListaAvalicaoAnamnese(item);

                if (sublista.FirstOrDefault().TemReg == "")
                {
                    sublistaex = ListaAvalicaoExameFisicos(item, false);
                }
                else
                {
                    sublistaex = ListaAvalicaoExameFisicos(item, true);
                }

                boletim.ListaAvaliacaoClinica.Add(new ListaAvaliacaoClinica()
                {
                    Data = item.DataInclusao.ToShortDateString(),
                    listaAvaliacaoAnamnese = sublista,
                    listaExameFisico = sublistaex
                });
            }

            _listaAvaliacaoClinicaSource = boletim.ListaAvaliacaoClinica.OrderByDescending(x => x.Data).ToList();

            #endregion AvalicaoClinica

            #region ProcedimentoExames

            boletim.ListaProcedimentoExames = new List<ListaProcedimentoExames>();

            foreach (var item in this.wrpBoletimDeEmergencia.CentroObstetrico.Procedimentos.OrderBy(x => x.DataInclusao))
            {
                boletim.ListaProcedimentoExames.Add(new ListaProcedimentoExames()
                {
                    // ProcedimentoExame = item.Realizado == SimNao.Sim ? "þ" : "",
                    Usuario = item.DataInclusao.ToString("dd/MM/yyyy HH:mm") + " - " + item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")"),
                    Descricao = item.Descricao,
                });
            }

            _listaProcedimentoExamesSource = boletim.ListaProcedimentoExames.ToList();

            #endregion ProcedimentoExames

            #region OrientacoesMedicoAssistente

            boletim.ListaOrientacoesMedicoAssistente = new List<ListaOrientacoesMedicoAssistente>();

            foreach (var item in this.wrpBoletimDeEmergencia.CentroObstetrico.Orientacoes.OrderBy(x => x.DataInclusao))
            {
                boletim.ListaOrientacoesMedicoAssistente.Add(new ListaOrientacoesMedicoAssistente()
                {
                    Usuario = item.DataInclusao.ToString("dd/MM/yyyy HH:mm") + " - " + item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")"),
                    //OrientacoesMedico = item.MedicoNaoDefinido == SimNao.Sim ? "þ" : "",
                    MedicoAssistenteDescricao = item.Prestador != null ? "Médico Assistente: " + item.Prestador.Nome + " " + Environment.NewLine + item.Descricao : " " + item.Descricao

                });
            }

            _listaOrientacoesMedicoAssistenteSource = boletim.ListaOrientacoesMedicoAssistente.ToList();

            #endregion OrientacoesMedicoAssistente

            #region CondutaReavaliacao

            boletim.ListaCondutaReavaliacoes = new List<ListaCondutaReavaliacoes>();

            foreach (var item in this.wrpBoletimDeEmergencia.CentroObstetrico.Condutas.OrderBy(x => x.DataInclusao))
            {
                boletim.ListaCondutaReavaliacoes.Add(new ListaCondutaReavaliacoes()
                {
                    //Usuario = item.DataInclusao.ToString("dd/MM/yyyy HH:mm") + " - " + item.Usuario.Prestador.Nome + " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : " ")
                    //     + item.Usuario.DomainObject.Prestador.Registro + ")",
                    Usuario = item.DataInclusao.ToString("dd/MM/yyyy HH:mm") + " - " + item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")"),
                    Descricao = item.Descricao
                });
            }

            _listaCondutaReavaliacoesSource = boletim.ListaCondutaReavaliacoes.ToList();

            #endregion CondutaReavaliacao

            #region CIDS

            boletim.ListaCIDS = new List<ListaCIDS>();

            foreach (var item in this.wrpBoletimDeEmergencia.Atendimento.CIDs)
            {
                boletim.ListaCIDS.Add(new ListaCIDS()
                {
                    Descricao = item.CidMV.Id + " - " + item.Descricao,
                });
            }

            _listaCIDSSource = boletim.ListaCIDS;

            #endregion CIDS

            #region DadosAlta

            boletim.DadosAlta = new List<DadosAlta>();

            if (this.wrpBoletimDeEmergencia.AltaCO != null)
            {
                DadosAlta dados = new DadosAlta();

                dados.Condicoes = this.wrpBoletimDeEmergencia.AltaCO.Condicao;
                dados.Data = this.wrpBoletimDeEmergencia.AltaCO.DataInclusao.ToString("dd/MM/yyyy HH:mm");
                dados.Orientacoes = this.wrpBoletimDeEmergencia.AltaCO.Orientacao;
                dados.ExamesEntreguesDescricao = this.wrpBoletimDeEmergencia.AltaCO.ExameObservacao;
                dados.Usuario = this.wrpBoletimDeEmergencia.AltaCO.DataInclusao.ToString("dd/MM/yyyy HH:mm") + " - " + this.wrpBoletimDeEmergencia.AltaCO.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")");

                dados.SimNao = this.wrpBoletimDeEmergencia.AltaCO.ExameEntregue.GetEnumCustomDisplay();

                if (this.wrpBoletimDeEmergencia.AltaCO.DestinoAlta.IsNotNull())
                    dados.Destino = this.wrpBoletimDeEmergencia.AltaCO.DestinoAlta.Value.GetEnumCustomDisplay();                

                boletim.DadosAlta.Add(dados);
            }

            _dadosAltaSource = boletim.DadosAlta;

            #endregion DadosAlta

            LisRelatorio.Add(boletim);
            _relatorio = LisRelatorio;

            return LisRelatorio;
        }

        #endregion Relatorio

        #region Propriedades
        public List<ListaClassificacao> _listaClassificacaoSource { get; set; }
        public List<ListaSinaisVitais> _listaSinaisVitaisSource { get; set; }
        public List<ListaMotivoConsulta> _listaMotivoConsultaSource { get; set; }
        public List<ListaAvaliacaoClinica> _listaAvaliacaoClinicaSource { get; set; }
        public List<ListaProcedimentoExames> _listaProcedimentoExamesSource { get; set; }
        public List<ListaOrientacoesMedicoAssistente> _listaOrientacoesMedicoAssistenteSource { get; set; }
        public List<ListaCondutaReavaliacoes> _listaCondutaReavaliacoesSource { get; set; }
        public List<ListaCIDS> _listaCIDSSource { get; set; }
        public List<BoletimEmergenciaRelatorioCo> _relatorio { get; set; }
        public List<DadosAlta> _dadosAltaSource { get; set; }
        #endregion Propriedades

        #region DadosAux

        private List<ListaAvaliacaoAnamnese> ListaAvalicaoAnamnese(wrpAvaliacaoClinica item)
        {
            string idadeGesta = "";
            string primeiro = "";
            string segundo = "";
            string terceiro = "";

            List<ListaAvaliacaoAnamnese> lista = new List<ListaAvaliacaoAnamnese>();

            ListaAvaliacaoAnamnese anamnese = new ListaAvaliacaoAnamnese();

            anamnese.TemReg = "";

            anamnese.TituloGestacoes = item.TemGestacao == true ? "Gestações " : "";
            anamnese.TemReg = anamnese.TituloGestacoes;
            primeiro = anamnese.TituloGestacoes;

            if (item.Gestacao != null)
            {
                anamnese.AnamneseTitulo1 = "Gestação";
                if (item.Gestacao.Gesta.HasValue)
                {
                    anamnese.AnamneseItem1 += "Gesta:" + item.Gestacao.Gesta.ToString().PadRight(10, ' ');
                }
                if (item.Gestacao.Para.HasValue)
                {
                    anamnese.AnamneseItem1 += "Para:" + item.Gestacao.Para.ToString().PadRight(10, ' ');
                }
                if (item.Gestacao.Cesarea.HasValue)
                {
                    anamnese.AnamneseItem1 += "Cesarea:" + item.Gestacao.Cesarea.ToString().PadRight(10, ' ');
                }
                if (item.Gestacao.Aborto.HasValue)
                {
                    anamnese.AnamneseItem1 += "Aborto:" + item.Gestacao.Aborto.ToString().PadRight(10, ' ');
                }
                primeiro = "gestacao";
            }

            if (item.TemGestacaoIdade)
            {
                if (!string.IsNullOrWhiteSpace(anamnese.AnamneseItem1))
                    anamnese.AnamneseItem1.PadRight(5, ' ');
                else
                    anamnese.AnamneseItem1 = "";

                // Multiplica para colocar o titulo da idade na posicao correta 
                anamnese.AnamneseTitulo1 += "Idade Gestacional".PadLeft(Convert.ToInt32(anamnese.AnamneseItem1.Length * 1.395), ' ');

                if (item.GestacaoIdade.IdadeSemana != null)
                    anamnese.AnamneseItem1 += item.GestacaoIdade.IdadeSemana.ToString() + " Semana(s)".PadLeft(10, ' ');

                if (item.GestacaoIdade.IdadeDia != null)
                    anamnese.AnamneseItem1 += idadeGesta + " e " + item.GestacaoIdade.IdadeDia.ToString() + " dia(s) ";

                if (item.GestacaoIdade.Desconhecido == SimNao.Sim)
                    anamnese.AnamneseItem1 += "Desconhecida";

                primeiro = "idade";
            }

            anamnese.TemReg = anamnese.TemReg == "" ? idadeGesta : anamnese.TemReg;
            primeiro = primeiro == "" ? anamnese.Semana : primeiro;

            //anamnese.Desconhecido = item.GestacaoIdade.Desconhecido == SimNao.Sim ? "þ" : "";
            anamnese.TemReg = anamnese.TemReg == "" ? anamnese.Desconhecido : anamnese.TemReg;
            primeiro = primeiro == "" ? anamnese.Desconhecido : primeiro;

            anamnese.DesDesconhecido = item.GestacaoIdade.Desconhecido == SimNao.Sim ? "Desconhecido: " : "";
            anamnese.TemReg = anamnese.TemReg == "" ? anamnese.Desconhecido : anamnese.TemReg;
            primeiro = primeiro == "" ? anamnese.DesDesconhecido : primeiro;


            //anamnese.TituloDinamica = item.TemDinamica == true ? "Dinâmica: " : "";
            //anamnese.TemReg = anamnese.TemReg == "" ? anamnese.DesDesconhecido : anamnese.TemReg;
            //segundo = segundo == "" ? anamnese.TituloDinamica : segundo;

            //anamnese.Dinamica = item.TemDinamica == true ? item.Dinamica.ToString() : "";
            //anamnese.TemReg = anamnese.TemReg == "" ? anamnese.Dinamica : anamnese.TemReg;
            //segundo = segundo == "" ? anamnese.Dinamica : segundo;

            //anamnese.NA = item.Dinamica.NaoAvaliado == SimNao.Sim ? "þ" : "";
            anamnese.TemReg = anamnese.TemReg == "" ? anamnese.NA : anamnese.TemReg;
            segundo = segundo == "" ? anamnese.NA : segundo;

            //anamnese.TituloNadaDinamica = item.Dinamica.NaoAvaliado == SimNao.Sim ? "" : "";
            //anamnese.TemReg = anamnese.TemReg == "" ? anamnese.TituloNadaDinamica : anamnese.TemReg;
            //segundo = segundo == "" ? anamnese.TituloNadaDinamica : segundo;

            anamnese.HistoriaTitulo = item.HistoriaAtual.IsNotEmptyOrWhiteSpace() ? "História Atual:" : "";
            anamnese.TemReg = anamnese.TemReg == "" ? anamnese.HistoriaTitulo : anamnese.TemReg;
            segundo = segundo == "" ? anamnese.HistoriaTitulo : segundo;

            anamnese.HistoriaAtual = item.HistoriaAtual.IsNotEmptyOrWhiteSpace() ? item.HistoriaAtual : "";
            anamnese.TemReg = anamnese.TemReg == "" ? anamnese.HistoriaAtual : anamnese.TemReg;
            terceiro = terceiro == "" ? anamnese.HistoriaAtual : terceiro;

            if (!string.IsNullOrWhiteSpace(primeiro))
                anamnese.Primeiro = true;

            if (!string.IsNullOrWhiteSpace(segundo))
                anamnese.Segundo = true;

            if (!string.IsNullOrWhiteSpace(terceiro))
                anamnese.Terceiro = true;

            anamnese.TemReg = "xxx";
            if (item != null && item.Usuario != null)
                anamnese.Usuario = item.DataInclusao.ToString("dd/MM/yyyy HH:mm") + " - " + item.Usuario.AssinaturaNaLinha.Replace("[", "(").Replace("]", ")");

            lista.Add(anamnese);

            return lista;
        }

        private void addItensGestacao(ListaAvaliacaoAnamnese ana, string descricao)
        {
            if (string.IsNullOrWhiteSpace(ana.GestacaoItem1))
                ana.GestacaoItem1 = descricao;
            else if (string.IsNullOrWhiteSpace(ana.GestacaoItem2))
                ana.GestacaoItem2 = descricao;
            else if (string.IsNullOrWhiteSpace(ana.GestacaoItem3))
                ana.GestacaoItem3 = descricao;
            else if (string.IsNullOrWhiteSpace(ana.GestacaoItem4))
                ana.GestacaoItem4 = descricao;
        }

        private List<ListaAvaliacaoExameFisico> ListaAvalicaoExameFisicos(wrpAvaliacaoClinica item, bool temDataAnam)
        {

            List<ListaAvaliacaoExameFisico> lista = new List<ListaAvaliacaoExameFisico>();
            ListaAvaliacaoExameFisico exa = new ListaAvaliacaoExameFisico();

            if (item.TemDinamica)
            {
                exa.Primeiro = true;
                exa.TituloDinamica = "Dinâmica:";
                exa.DescricaoDinamica = item.Dinamica.ToString();
            }

            if (item.TemTonus)
                addItensExameFisicoPrimeiraLinha(exa, "Tõnus", item.Tonus.Descricao);

            if (item.TemExameEspecular)
                addItensExameFisicoPrimeiraLinha(exa, "Exame especular", item.ExameEspecular.Descricao);

            if (item.TemToque)
                addItensExameFisicoPrimeiraLinha(exa, "Toque", item.Toque.Descricao);

            if (item.TemMonitorizacao)
                addItensExameFisicoSegundaLinha(exa, "Monitorização Ante-parto", item.Monitorizacao.Descricao);

            if (item.TemEcografia)
                addItensExameFisicoSegundaLinha(exa, "Ecografia no Centro Obstétrico", item.Ecografia.Descricao);

            if (item.ExameFisico.IsNotEmptyOrWhiteSpace())
                addItensExameFisicoSegundaLinha(exa, "Demais aspectos do Exame Físico", item.ExameFisico);

            lista.Add(exa);
            return lista;
        }

        private void addItensExameFisicoPrimeiraLinha(ListaAvaliacaoExameFisico exa, string titulo, string valor)
        {
            exa.Primeiro = true;
            if (string.IsNullOrWhiteSpace(exa.TituloItem1))
            {
                exa.TituloItem1 = titulo;
                exa.DescricaoItem1 = valor;
            }
            else if (string.IsNullOrWhiteSpace(exa.TituloItem2))
            {
                exa.TituloItem2 = titulo;
                exa.DescricaoItem2 = valor;
            }
            else if (string.IsNullOrWhiteSpace(exa.TituloItem3))
            {
                exa.TituloItem3 = titulo;
                exa.DescricaoItem3 = valor;
            }
        }

        private void addItensExameFisicoSegundaLinha(ListaAvaliacaoExameFisico exa, string titulo, string valor)
        {
            exa.Segundo = true;
            if (string.IsNullOrWhiteSpace(exa.TituloItem4))
            {
                exa.TituloItem4 = titulo;
                exa.DescricaoItem4 = valor;
            }
            else if (string.IsNullOrWhiteSpace(exa.TituloItem5))
            {
                exa.TituloItem5 = titulo;
                exa.DescricaoItem5 = valor;
            }
            else if (string.IsNullOrWhiteSpace(exa.TituloItem6))
            {
                exa.TituloItem6 = titulo;
                exa.DescricaoItem6 = valor;
            }
        }

        #endregion DadosAux

    }


    #region classesAux

    public class BoletimEmergenciaRelatorioCo
    {
        public BoletimEmergenciaRelatorioCo()
        {

        }
        public virtual string AtendimentoId { get; set; }
        public virtual string AtendimentoInicio { get; set; }
        public virtual string PacienteNome { get; set; }
        public virtual string DR { get; set; }
        public virtual string PacienteIdade { get; set; }
        public virtual string PacienteDataCadastro { get; set; }
        public virtual List<ListaClassificacao> ListaClassificacao { get; set; }
        public virtual List<ListaSinaisVitais> ListaSinaisVitais { get; set; }
        public virtual List<ListaMotivoConsulta> ListaMotivoConsulta { get; set; }
        public virtual List<ListaAvaliacaoClinica> ListaAvaliacaoClinica { get; set; }
        public virtual List<ListaAvaliacaoExameFisico> ListaExameFisico { get; set; }
        public virtual List<ListaProcedimentoExames> ListaProcedimentoExames { get; set; }
        public virtual List<ListaOrientacoesMedicoAssistente> ListaOrientacoesMedicoAssistente { get; set; }
        public virtual List<ListaCondutaReavaliacoes> ListaCondutaReavaliacoes { get; set; }
        public virtual List<ListaCIDS> ListaCIDS { get; set; }
        public virtual List<DadosAlta> DadosAlta { get; set; }

        public string NomePaciente { get; set; }
        public string NomeResumo { get; set; }
        public int IDPaciente { get; set; }
        public string NomePrestador { get; set; }
        public string Registro { get; set; }
        public string CodigoBarras { get; set; }

        public bool MostraCodigoBarras { get; set; }
        public bool MostraIDPaciente { get; set; }
    }

    public class ListaClassificacao
    {
        public virtual string Data { get; set; }
        public virtual string Cor { get; set; }
        public virtual string Usuario { get; set; }
    }

    public class ListaSinaisVitais
    {
        public virtual string Data { get; set; }
        public virtual string Pa { get; set; }
        public virtual string Tax { get; set; }
        public virtual string Fc { get; set; }
        public virtual string Fr { get; set; }
        public virtual string Dor { get; set; }
        public virtual string Bcf { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string TemReg { get; set; }
    }

    public class ListaMotivoConsulta
    {
        public virtual string Data { get; set; }
        public virtual string Profissional { get; set; }
        public virtual string Descricao { get; set; }
    }

    public class ListaAvaliacaoClinica
    {
        public List<ListaAvaliacaoAnamnese> listaAvaliacaoAnamnese { get; set; }
        public List<ListaAvaliacaoExameFisico> listaExameFisico { get; set; }
        public virtual string Data { get; set; }
    }

    public class ListaAvaliacaoAnamnese
    {
        public virtual string AnamneseItem1 { get; set; }
        public virtual string AnamneseTitulo1 { get; set; }

        public virtual string GestacaoItem1 { get; set; }
        public virtual string GestacaoItem2 { get; set; }
        public virtual string GestacaoItem3 { get; set; }
        public virtual string GestacaoItem4 { get; set; }

        public virtual string TituloGestacoes { get; set; }
        /*public virtual string Gesta { get; set; }
        public virtual string Para { get; set; }
        public virtual string Cesarea { get; set; }
        public virtual string Aborto { get; set; }*/
        public virtual string TituloIndadeGesta { get; set; }
        public virtual string Semana { get; set; }
        public virtual string Dias { get; set; }
        public virtual string Desconhecido { get; set; }
        public virtual string DesDesconhecido { get; set; }
        public virtual string TituloDinamica { get; set; }
        public virtual string Dinamica { get; set; }
        public virtual string NA { get; set; }
        public virtual string TituloNadaDinamica { get; set; }
        public virtual string HistoriaAtual { get; set; }
        public virtual string HistoriaTitulo { get; set; }
        public virtual string Data { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string TemReg { get; set; }
        public virtual bool Primeiro { get; set; }
        public virtual bool Segundo { get; set; }
        public virtual bool Terceiro { get; set; }
        public virtual bool TemData { get; set; }
    }

    public class ListaAvaliacaoExameFisico
    {
        /*public virtual string TituloToque { get; set; }
        public virtual string Toque { get; set; }
        public virtual string TituloToqueNaoRealizado { get; set; }
        public virtual string ToqueValor { get; set; }
        public virtual string TituloMonitorizacao { get; set; }
        public virtual string Monitorizacao { get; set; }
        public virtual string TituloNaoRealMonitorizacao { get; set; }
        public virtual string MonitorizacaoValor { get; set; }
        public virtual string TituloEcografia { get; set; }
        public virtual string Ecografia { get; set; }
        public virtual string TitulonaoRealizadoEcografia { get; set; }
        public virtual string EcografiaValor { get; set; }
        public virtual string TituloDemaisAspectos { get; set; }
        public virtual string DemaisAspectosValor { get; set; }
        public virtual string TituloTonus { get; set; }
        public virtual string Tonus { get; set; }
        public virtual string NaoRealizadoTonus { get; set; }
        public virtual string TonusValor { get; set; }
        public virtual string TituloExameEspecular { get; set; }
        public virtual string ExameEspecular { get; set; }
        public virtual string naoRealizadoExameEspecular { get; set; }
        public virtual string ExameEspecularValor { get; set; }
        public virtual string Data { get; set; }
        public virtual bool TemData { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string TemReg { get; set; }*/
        public virtual bool Primeiro { get; set; }
        public virtual bool Segundo { get; set; }
        public virtual string Indicador { get; set; }
        public virtual string TituloDinamica { get; set; }
        public virtual string TituloItem1 { get; set; }
        public virtual string TituloItem2 { get; set; }
        public virtual string TituloItem3 { get; set; }
        public virtual string TituloItem4 { get; set; }
        public virtual string TituloItem5 { get; set; }
        public virtual string TituloItem6 { get; set; }
        public virtual string DescricaoDinamica { get; set; }
        public virtual string DescricaoItem1 { get; set; }
        public virtual string DescricaoItem2 { get; set; }
        public virtual string DescricaoItem3 { get; set; }
        public virtual string DescricaoItem4 { get; set; }
        public virtual string DescricaoItem5 { get; set; }
        public virtual string DescricaoItem6 { get; set; }
    }

    public class ListaProcedimentoExames
    {
        public virtual string TituloProcedimentoExame { get; set; }
        public virtual string ProcedimentoExame { get; set; }
        public virtual string TituloNaoRealizado { get; set; }
        public virtual string Data { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Descricao { get; set; }
    }

    public class ListaOrientacoesMedicoAssistente
    {
        public virtual string OrientacoesMedico { get; set; }
        public virtual string TitulonaoDefinido { get; set; }
        public virtual string Data { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string TituloMedicoAssistente { get; set; }
        public virtual string MedicoAssistenteDescricao { get; set; }
        public virtual string MedicoAssistenteValor { get; set; }
    }

    public class ListaCondutaReavaliacoes
    {
        public virtual string Indicador { get; set; }
        public virtual string Data { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Descricao { get; set; }
    }

    public class ListaCIDS
    {
        public virtual string Codigo { get; set; }
        public virtual string Descricao { get; set; }
    }

    public class DadosAlta
    {
        public virtual string Destino { get; set; }
        public virtual string Condicoes { get; set; }
        //public virtual string Domicilio { get; set; }
        //public virtual string Internacao { get; set; }
        //public virtual string OutroHospital { get; set; }
        //public virtual string Retorno { get; set; }
        //public virtual string RetornoHoras { get; set; }
        //public virtual string SimExames { get; set; }
        //public virtual string NaoExames { get; set; }
        public virtual string SimNao { get; set; }
        public virtual string Orientacoes { get; set; }
        public virtual string Data { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string ExamesEntreguesDescricao { get; set; }

        public virtual string DestinoDesc { get; set; }
        //public virtual string DomicilioDesc { get; set; }
        //public virtual string InternacaoDesc { get; set; }
        //public virtual string OutroHospitalDesc { get; set; }
        //public virtual string RetornoDesc { get; set; }
        //public virtual string RetornoHoraDes { get; set; }
        public virtual string Sim { get; set; }
        public virtual string Nao { get; set; }

    }

    #endregion classesAux

}

