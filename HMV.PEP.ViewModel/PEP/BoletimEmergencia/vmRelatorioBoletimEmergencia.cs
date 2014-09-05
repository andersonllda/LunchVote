using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Model.DocumentosEletronicos;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Windows.Documents;
using System.Windows;
using System.IO;
using System.Configuration;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.PEP.BoletimEmergencia
{
    public class vmRelatorioBoletimEmergencia
    {
        public vmRelatorioBoletimEmergencia(wrpBoletimDeEmergencia pBoletimDeEmergencia, bool pIsPame)
        {
            this._ispame = pIsPame;
            _BoletimDeEmergencia = pBoletimDeEmergencia;
        }

        public vmRelatorioBoletimEmergencia()
        {
        }

        #region Relatorio
        private List<BoletimEmergenciaRelatorio> Listarel = new List<BoletimEmergenciaRelatorio>();
        private bool _ispame;
        SimNao _simnao = new SimNao();

        public List<BoletimEmergenciaRelatorio> Relatorio()
        {
            if (this._BoletimDeEmergencia != null)
            {
                BoletimEmergenciaRelatorio b = new BoletimEmergenciaRelatorio();
                b.MostraCodigoBarras = false;
                b.MostraIDPaciente = false;

                if (this._BoletimDeEmergencia.UsuarioAlta.IsNotNull())
                    b.Dr = this._BoletimDeEmergencia.UsuarioAlta.Assinatura;
                //if (this._BoletimDeEmergencia.UsuarioAlta != null && this._BoletimDeEmergencia.UsuarioAlta.Profissional != null)
                //b.Dr = (this._BoletimDeEmergencia.UsuarioAlta.Profissional.tratamento != null ? this._BoletimDeEmergencia.UsuarioAlta.Profissional.tratamento : " - ") + " " + this._BoletimDeEmergencia.UsuarioAlta.Nome + " (" + this._BoletimDeEmergencia.UsuarioAlta.Profissional.conselho + " " + this._BoletimDeEmergencia.UsuarioAlta.Profissional.registro + ")";
                //else if (this._BoletimDeEmergencia.UsuarioAlta != null && this._BoletimDeEmergencia.UsuarioAlta.Prestador != null)
                //b.Dr = this._BoletimDeEmergencia.UsuarioAlta.Prestador.NomeExibicaoPrestador + " (" + this._BoletimDeEmergencia.UsuarioAlta.Prestador.Conselho + " " + this._BoletimDeEmergencia.UsuarioAlta.Prestador.Registro + ")";

                b.ListaSinais = new List<ListaSinaisVitais>();
                b.ListaAvaliacao = new List<ListaAvaliacao>();

                if (this._BoletimDeEmergencia.Atendimento.IsNotNull())
                {
                    b.AtendimentoId = _BoletimDeEmergencia.Atendimento.ID.ToString();
                    b.AtendimentoInicio = this._BoletimDeEmergencia.Atendimento.DataAtendimento.ToString("d");
                    b.PacienteNome = this._BoletimDeEmergencia.Atendimento.Paciente.ID + " - " + this._BoletimDeEmergencia.Atendimento.Paciente.Nome;
                    b.PacienteIdade = this._BoletimDeEmergencia.Atendimento.Paciente.Idade.GetDate(this._BoletimDeEmergencia.Atendimento.HoraAtendimento);
                    b.PacienteDataCadastro = this._BoletimDeEmergencia.Atendimento.DataAtendimento.ToString("d") + " - " + this._BoletimDeEmergencia.Atendimento.HoraAtendimento.ToString("HH:mm");

                    if (this._BoletimDeEmergencia.Atendimento.Paciente.IsNotNull())
                    {
                        b.NomePaciente = this._BoletimDeEmergencia.Atendimento.Paciente.Nome;
                        b.IDPaciente = this._BoletimDeEmergencia.Atendimento.Paciente.ID;
                        b.MostraIDPaciente = true;
                    }

                    b.NomeResumo = this._BoletimDeEmergencia.Atendimento.Leito.IsNotNull() ? this._BoletimDeEmergencia.Atendimento.Leito.Descricao : string.Empty;
                    b.CodigoBarras = this._BoletimDeEmergencia.Atendimento.ID.ToString();
                    b.MostraCodigoBarras = true;

                    if (this._BoletimDeEmergencia.Atendimento.Prestador.IsNotNull())
                    {
                        b.NomePrestador = this._BoletimDeEmergencia.Atendimento.Prestador.Nome;
                        b.Registro = this._BoletimDeEmergencia.Atendimento.Prestador.Registro;
                    }
                }

                #region SINAIS VITAIS
                if (this._BoletimDeEmergencia.SinaisVitais.HasItems())
                    if (this._BoletimDeEmergencia.SinaisVitais.Select(x => x.Data).OrderBy(x => x).Distinct().Count() > 0)
                    {
                        foreach (var data in this._BoletimDeEmergencia.SinaisVitais.Select(x => x.Data).OrderBy(x => x).Distinct())
                        {
                            ListaSinaisVitais sinais = new ListaSinaisVitais();
                            foreach (var item in this._BoletimDeEmergencia.SinaisVitais.Where(x => x.Data.Equals(data)))
                            {
                                switch (item.Sigla.Descricao)
                                {
                                    case "FC":
                                        sinais.SinaisFC = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                    case "FR":
                                        sinais.SinaisFR = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                    case "PA":
                                        sinais.SinaisPA = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                    case "SAT":
                                        sinais.SinaisSat = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                    case "TAX":
                                        sinais.SinaisTax = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                    case "DOR":
                                        sinais.SinaisDor = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                    case "PESO":
                                        sinais.SinaisPeso = item.Valor.IsEmptyOrWhiteSpace() ? " - " : item.Valor;
                                        break;
                                }



                                sinais.SinaisUsuario = item.Usuario.IsNotNull() ? item.Usuario.nm_usuario : " "
                                    + (item.Usuario.DomainObject.Prestador.IsNull() ? string.Empty : " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : "CRM ")
                                    + item.Usuario.DomainObject.Prestador.Registro
                                    + ")");

                                if (item.Data.IsNotNull())
                                    sinais.SinaisData = item.Data.ToString("dd/MM/yyyy HH:mm");
                            }

                            sinais.SinaisFC = sinais.SinaisFC.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisFC;
                            sinais.SinaisFR = sinais.SinaisFR.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisFR;
                            sinais.SinaisPA = sinais.SinaisPA.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisPA;
                            sinais.SinaisSat = sinais.SinaisSat.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisSat;
                            sinais.SinaisTax = sinais.SinaisTax.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisTax;
                            sinais.SinaisDor = sinais.SinaisDor.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisDor;
                            sinais.SinaisPeso = sinais.SinaisPeso.IsEmptyOrWhiteSpace() ? " - " : sinais.SinaisPeso;

                            b.ListaSinais.Add(sinais);
                        }
                    }
                    else
                    {
                        b.ListaSinais.Add(new ListaSinaisVitais() { SinaisData = "-", SinaisDor = "-", SinaisFC = "-", SinaisFR = "-", SinaisPA = "-", SinaisPeso = "-", SinaisSat = "-", SinaisTax = "-", SinaisUsuario = "-" });
                    }
                #endregion

                //var te = this._BoletimDeEmergencia.BoletimAvaliacao.OrderBy(x => x.TipoAvaliacao.ID).Select(x => x.TipoAvaliacao.Descricao).Distinct();
                string ultimo = string.Empty;
                string tipoDeAvaliacao = string.Empty;
                string especialidade = string.Empty;
                ListaAvaliacao avaliacao = new ListaAvaliacao();
                List<ListaAvaliacaoDescricao> avaliacoes = new List<ListaAvaliacaoDescricao>();
                List<ListaAvaliacaoDescricao> motivos = new List<ListaAvaliacaoDescricao>();

                // Para cada tipo de avaliação diferente existente
                if (_BoletimDeEmergencia.BoletimAvaliacao.HasItems())
                    foreach (var tipoaval in _BoletimDeEmergencia.BoletimAvaliacao.OrderBy(x => x.DataHoraInclusao).Select(x => x.TipoAvaliacao.Descricao).Distinct())
                    {
                        ListaAvaliacao listaaval = new ListaAvaliacao();
                        listaaval.ListaAvaliacaoDescricao = new List<ListaAvaliacaoDescricao>();
                        tipoDeAvaliacao = tipoaval == "Motivo Consulta" ? string.Empty : " - " + tipoaval;

                        foreach (var item in this._BoletimDeEmergencia.BoletimAvaliacao.Where(x => x.TipoAvaliacao.Descricao.Equals(tipoaval)).Distinct()
                            .OrderBy(x => x.TipoAvaliacao.OrdemRelatorio).OrderBy(x => x.ID).Distinct())
                        {
                            ListaAvaliacaoDescricao desc = new ListaAvaliacaoDescricao();
                            _simnao = item.TipoAvaliacao.ObrigatorioBoletim;

                            // Separa os tipos de avaliações do relatório por essa linha
                            if (ultimo != tipoaval && tipoaval != "Motivo Consulta")
                            {
                                desc.SN = "__________________________________________________________________________________________________________________________________________";
                                ultimo = tipoaval;
                            }

                            string _Descricao = string.Empty;
                            DateTime _DataInclusao;

                            if (item.TipoAvaliacao.MostraEspecialidade == Core.Domain.Enum.SimNao.Sim && item.Usuario.Prestador.IsNotNull())
                            {
                                especialidade = " - ";
                                especialidade += item.Usuario.Prestador.EspecialidadePrincipal.IsNull() ? string.Empty : " - " + item.Usuario.Prestador.EspecialidadePrincipal.Descricao;
                            }

                            if (item.TipoAvaliacao.ObrigatorioBoletim == SimNao.Sim)
                            {
                                if (!item.Usuario.Profissional.IsNull())
                                {
                                    _Descricao += Convert.ToChar(61) + "" + Convert.ToChar(187) + " " + item.DataInclusao.ToString("d") + " - " + item.HoraInclusao + tipoDeAvaliacao
                                        + (item.Usuario.Profissional.IsNotNull() ? " - " + item.Usuario.Profissional.tratamento : " - " + string.Empty) + " " + item.Usuario.nm_usuario.ToString().ToTitle()
                                        + " (" + (item.Usuario.DomainObject.Prestador.Conselho.IsNotNull() ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : " ")
                                        + item.Usuario.DomainObject.Prestador.Registro + ")";

                                    _Descricao += _RemoveColchetes(item.Descricao).RemoveXmlInvalidCharacters();

                                    string[] horaInclusao = item.HoraInclusao.Split(new char[] { ':' });
                                    int hora = int.Parse(horaInclusao[0]);
                                    int minuto = int.Parse(horaInclusao[1]);

                                    _DataInclusao = item.DataInclusao.AddHours(hora).AddMinutes(minuto);
                                }
                                else
                                {
                                    _Descricao += Convert.ToChar(61) + "" + Convert.ToChar(187) + " " + item.DataInclusao.ToString("d") + " - " + item.HoraInclusao + tipoDeAvaliacao + " - " + ("(" + (item.Usuario.DomainObject.Prestador.Conselho.IsNotNull() ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : item.Usuario.Profissional.conselho) + (item.Usuario.DomainObject.Prestador.Registro.IsNotNull() ? item.Usuario.DomainObject.Prestador.Registro.Trim() : " ") + ")")
                                        .Replace(item.Usuario.DomainObject.nm_usuario, item.Usuario.NomeExibicao) + _RemoveColchetes(item.Descricao);

                                    string[] horaInclusao = item.HoraInclusao.Split(new char[] { ':' });
                                    int hora = int.Parse(horaInclusao[0]);
                                    int minuto = int.Parse(horaInclusao[1]);
                                    _DataInclusao = item.DataInclusao.AddHours(hora).AddMinutes(minuto);
                                }
                            }
                            else
                            {
                                _Descricao += Convert.ToChar(61) + "" + Convert.ToChar(187) + " " + item.DataInclusao.ToString("d") + " - " + item.HoraInclusao + tipoDeAvaliacao
                                    + (item.Usuario.Profissional.IsNotNull() ? " - " + item.Usuario.Profissional.tratamento : " - " + string.Empty) + " " + item.Usuario.nm_usuario.ToString().ToTitle()
                                    + " (" + (item.Usuario.DomainObject.Prestador.Conselho.IsNotNull() ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : " ")
                                    + item.Usuario.DomainObject.Prestador.Registro + ")" + Environment.NewLine + _RemoveColchetes(item.Descricao);

                                string[] horaInclusao = item.HoraInclusao.Split(new char[] { ':' });
                                int hora = int.Parse(horaInclusao[0]);
                                int minuto = int.Parse(horaInclusao[1]);
                                _DataInclusao = item.DataInclusao.AddHours(hora).AddMinutes(minuto);
                            }

                            if (item.TipoAvaliacao.Exame == SimNao.Sim)
                            {
                                _Descricao = _RemoveColchetes(item.Descricao).RemoveXmlInvalidCharacters();
                            }

                            // O Motivo Consulta é o único agrupado separadamente dos demais, título e deve aparecer primeiramente no relatório
                            if (tipoaval == "Motivo Consulta")
                            {
                                if (motivos.Count == 0)
                                {
                                    // Insere um título quando a avaliação for um Motivo Consulta                                   
                                    desc.Descricao = "Motivo Consulta" + especialidade + Environment.NewLine + _Descricao.RemoveXmlInvalidCharacters();
                                    desc.DataInclusao = _DataInclusao;

                                    if (_BoletimDeEmergencia.DescricaoMedicamentoProcedimento.IsNotEmptyOrWhiteSpace())
                                    {
                                        string medicamentorealizado = "Medicamentos/ Procedimentos realizados durante a classificação de risco:" + Environment.NewLine
                                                                                          + _BoletimDeEmergencia.DescricaoMedicamentoProcedimento + Environment.NewLine
                                                                                          + (_BoletimDeEmergencia.PrestadorMedicamentoProcedimento.IsNotNull() ?
                                                                                          "Por orientação do " + _BoletimDeEmergencia.PrestadorMedicamentoProcedimento.NomeExibicao :
                                                                                          string.Empty);
                                        desc.Descricao += Environment.NewLine + Environment.NewLine + medicamentorealizado.RemoveXmlInvalidCharacters();
                                    }
                                    if (_BoletimDeEmergencia.MedicoAssistenteCiente.IsNotEmptyOrWhiteSpace())
                                        desc.Descricao += Environment.NewLine + "Médico Assistente: " + _BoletimDeEmergencia.MedicoAssistenteCiente;
                                }
                                else
                                {
                                    desc.Descricao = _Descricao.RemoveXmlInvalidCharacters();
                                    desc.DataInclusao = _DataInclusao;
                                }

                                motivos.Add(desc);
                            }
                            else
                            {
                                desc.Descricao = _Descricao.RemoveXmlInvalidCharacters();
                                desc.DataInclusao = _DataInclusao;

                                avaliacoes.Add(desc);
                            }
                        }
                    }



                if (motivos.HasItems())
                    motivos = motivos.OrderBy(x => x.DataInclusao).ToList();
                if (avaliacoes.HasItems())
                    avaliacoes = avaliacoes.OrderBy(x => x.DataInclusao).ToList();

                avaliacao.ListaAvaliacaoDescricao = motivos;
                avaliacao.ListaAvaliacaoDescricao.AddRange(avaliacoes);

                b.ListaAvaliacao.Add(avaliacao);

                #region antigo

                //foreach (var tipoaval in this._BoletimDeEmergencia.BoletimAvaliacao.OrderBy(x => x.TipoAvaliacao.OrdemRelatorio).Select(x => x.TipoAvaliacao.Descricao).Distinct())
                //{
                //    ListaAvaliacao listaaval = new ListaAvaliacao();

                //    listaaval.ListaAvaliacaoDescricao = new List<ListaAvaliacaoDescricao>();

                //    foreach (var item in this._BoletimDeEmergencia.BoletimAvaliacao.Where(x => x.TipoAvaliacao.Descricao.Equals(tipoaval)).Distinct().OrderBy(x => x.TipoAvaliacao.OrdemRelatorio).OrderBy(x => x.ID).Distinct())
                //    {
                //        _simnao = item.TipoAvaliacao.ObrigatorioBoletim;

                //        if (_simnao == SimNao.Nao)
                //        {
                //            listaaval = new ListaAvaliacao();
                //            listaaval.ListaAvaliacaoDescricao = new List<ListaAvaliacaoDescricao>();
                //            listaaval.SN = string.Empty;
                //        }

                //        if (ultimo != tipoaval)
                //        {
                //            if (ultimo != "")
                //                listaaval.SN = "___________________________________________________________________________________________________________________________________________";
                //            ultimo = tipoaval;
                //        }
                //        string _Descricao = string.Empty;
                //        if (item.TipoAvaliacao.ObrigatorioBoletim == SimNao.Sim)
                //        {
                //            listaaval.Titulo = tipoaval;
                //            listaaval.DataInicio = "Inicio Atendimento " + this._BoletimDeEmergencia.DataInclusao.Value.ToString("d") + " " + this._BoletimDeEmergencia.DataInclusao.Value.ToString("HH:mm"); ;

                //            if (!item.Usuario.Profissional.IsNull())
                //                _Descricao = Convert.ToChar(61) + "" + Convert.ToChar(187) + " "
                //                             + (item.Descricao.Trim().Replace(item.Usuario.nm_usuario.ToString(),
                //                             ((item.Usuario.Profissional != null) ? " - " + item.Usuario.Profissional.tratamento : " - " + string.Empty) + " " + item.Usuario.nm_usuario.ToString().ToTitle() + " " + " (" + item.Usuario.Profissional.conselho + " " + item.Usuario.Profissional.registro + ") " + Environment.NewLine)).Replace("[", "").Replace("]", "").Trim();
                //            else
                //                _Descricao = Convert.ToChar(61) + "" + Convert.ToChar(187) + " "
                //                             + (item.Descricao.Trim().Replace("[", " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : item.Usuario.Profissional.conselho) + (item.Usuario.DomainObject.Prestador.Registro != null ? item.Usuario.DomainObject.Prestador.Registro.Trim() : " ") + ")" + Environment.NewLine).Replace("]", ""))
                //                             .Replace(item.Usuario.DomainObject.nm_usuario, item.Usuario.DomainObject.Prestador.NomeExibicaoPrestador);
                //        }
                //        else
                //        {
                //            listaaval.Titulo = Convert.ToChar(61) + "" + Convert.ToChar(187) + " " + item.DataInclusao.ToString("d") + " - " + item.HoraInclusao + " - " + tipoaval + ((item.Usuario.Profissional != null) ? " - " + item.Usuario.Profissional.tratamento : " - " + string.Empty) + " " + item.Usuario.nm_usuario.ToString().ToTitle() + " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : " ") + item.Usuario.DomainObject.Prestador.Registro + ")";
                //            string[] text = item.Descricao.Split(new char[] { '[' });
                //            //_Descricao = Convert.ToChar(61) + "" + Convert.ToChar(187) + text[1].Replace("]", "");
                //            _Descricao = text[1].Replace("]", "");
                //        }

                //        if (item.TipoAvaliacao.Exame == SimNao.Sim)
                //        {
                //            string[] text = item.Descricao.Split(new char[] { '[' });
                //            _Descricao = text[1].Replace("]", "");
                //        }

                //        if (item.TipoAvaliacao.MostraEspecialidade == Core.Domain.Enum.SimNao.Sim)
                //            listaaval.Titulo += item.Usuario.Prestador.EspecialidadePrincipal.IsNull() ? string.Empty : " - " + item.Usuario.Prestador.EspecialidadePrincipal.Descricao;

                //        listaaval.ListaAvaliacaoDescricao.Add(new ListaAvaliacaoDescricao()
                //        {
                //            Descricao = _Descricao
                //        });

                //        if (_simnao == SimNao.Nao)
                //            b.ListaAvaliacao.Add(listaaval);
                //    }

                //    if (_simnao == SimNao.Sim)
                //        b.ListaAvaliacao.Add(listaaval);

                //    listaaval.ListaAvaliacaoDescricao = listaaval.ListaAvaliacaoDescricao.OrderBy(x => x.Descricao).ToList();
                //}

                #endregion

                #region CLASSIFICACAO
                b.ListaClassificacao = new List<ListaClassificacao>();
                foreach (var item in this._BoletimDeEmergencia.Classificacoes)
                {
                    b.ListaClassificacao.Add(new ListaClassificacao()
                    {
                        Cor = item.ClassificacaoCor.ToUpper(),
                        Data = item.DataHoraInclusao,
                        Usuario = item.UsuarioNome + " (" + ((item.Usuario.DomainObject.Prestador.Conselho != null) ? item.Usuario.DomainObject.Prestador.Conselho.ds_conselho + " " : "CRM ")
                        + item.Usuario.DomainObject.Prestador.Registro + ")"
                    });
                }
                #endregion

                #region CIDS
                if (this._BoletimDeEmergencia.Atendimento.CIDs.Count > 0)
                {
                    b.CIDDiagnostico = new List<CIDsDiagnostico>();
                    foreach (var item in this._BoletimDeEmergencia.Atendimento.CIDs.Select(x => x.Id + " - " + x.Descricao))
                    {
                        b.CIDDiagnostico.Add(new CIDsDiagnostico() { Descricao = item });
                    }
                }
                #endregion

                #region ALTA
                b.DadosAltaDataHora = this._BoletimDeEmergencia.DataAlta == null ? " - " : this._BoletimDeEmergencia.DataAlta.Value.ToString("dd/MM/yyyy HH:mm");
                b.DadosAltaDestino = this._BoletimDeEmergencia.AltaDestino == null ? " - " : this._BoletimDeEmergencia.AltaDestino.Descricao;
                b.DadosAltaCondicao = this._BoletimDeEmergencia.CondicaoAlta == null ? " - " : this._BoletimDeEmergencia.CondicaoAlta;
                b.DadosAltaOrientacao = this._BoletimDeEmergencia.ObservacaoAlta == null ? " - " : this._BoletimDeEmergencia.ObservacaoAlta;
                #endregion

                #region TRANSFERENCIA
                if (_BoletimDeEmergencia.AltaDestino != null && _BoletimDeEmergencia.AltaDestino.Descricao == "Transferência")
                {
                    b.Transferecia = new List<Transferecia>();
                    b.Transferecia.Add(new Transferecia()
                    {
                        AcompMedico = this._BoletimDeEmergencia.AcompMedico != null && this._BoletimDeEmergencia.AcompMedico == SimNao.Sim ? "Sim" : "Não",
                        ExameFisico = this._BoletimDeEmergencia.ExameFisico,
                        MedicamentosPrescritos = this._BoletimDeEmergencia.MedicamentosPrescritos,
                        MedicoInstituicaoDestino = this._BoletimDeEmergencia.MedicoInstituicaoDestino,
                        MeioDeTransporte = this._BoletimDeEmergencia.MeioTransporte,
                        Monitorizacao = this._BoletimDeEmergencia.Monitorizacao != null && this._BoletimDeEmergencia.Monitorizacao == SimNao.Sim ? "Sim" : "Não",
                        MunicipioDestino = this._BoletimDeEmergencia.MunicipioDestino,
                        NomeProficional = this._BoletimDeEmergencia.NomeProfissionalReservouLeito,
                        Oxigenio = this._BoletimDeEmergencia.Oxigenio != null && this._BoletimDeEmergencia.Oxigenio == SimNao.Sim ? "Sim" : "Não",
                        Ventilacao = this._BoletimDeEmergencia.Ventilacao != null && this._BoletimDeEmergencia.Ventilacao == SimNao.Sim ? "Sim" : "Não"
                    });
                }
                #endregion

                if (!string.IsNullOrWhiteSpace(this._BoletimDeEmergencia.Cargo))
                    b.Cargo = this._BoletimDeEmergencia.Cargo;
                else if (this._BoletimDeEmergencia.Atendimento.IsNotNull() && this._BoletimDeEmergencia.Atendimento.Paciente.Profissao.IsNotNull())
                    b.Cargo = this._BoletimDeEmergencia.Atendimento.Paciente.Profissao.Descricao;

                if (!_ispame)
                    b.Empresa = this._BoletimDeEmergencia.Empresa;
                else
                    if (this._BoletimDeEmergencia.Atendimento.IsNotNull() && this._BoletimDeEmergencia.Atendimento.Paciente.IsNotNull() && this._BoletimDeEmergencia.Atendimento.Plano.IsNotNull())
                        b.Empresa = this._BoletimDeEmergencia.Atendimento.Paciente.Trabalho + " / " + this._BoletimDeEmergencia.Atendimento.Plano.Descricao;

                #region PAME
                if (this._BoletimDeEmergencia.PAME.IsNotNull() && this._BoletimDeEmergencia.PAME.TipoAtendimento.IsNotNull())
                {
                    b.PAME = new PAME();
                    b.PAME.RegistroEvento = this._BoletimDeEmergencia.PAME.TipoAtendimento.Equals(TipoAtendimentoPAME.RegistroEvento);
                    b.PAME.TipoRegistro = this._BoletimDeEmergencia.PAME.TipoAtendimento.Value.CustomDisplay();
                    if (b.PAME.RegistroEvento)
                    {
                        b.PAME.DataHora = this._BoletimDeEmergencia.PAME.Data.HasValue ? this._BoletimDeEmergencia.PAME.Data.Value.ToString("dd/MM/yyyy - HH:mm") : "-";
                        b.PAME.Local = this._BoletimDeEmergencia.PAME.Local;
                        b.PAME.Relato = this._BoletimDeEmergencia.PAME.Relato;
                    }
                }

                #endregion

                Listarel.Add(b);
                return Listarel;
            }

            return null;
        }
        #endregion

        #region Relatório MV
        public List<PerguntasRespostas> RelatorioMV(wrpDocumentos pDocumento)
        {
            if (!pDocumento.TiposDocumentos.Id.Equals(2))
            {
                IRepositorioDeParametrosInternet rep = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
                ParametroInternet par = rep.OndePerguntasNaoVinculadas().Single();
                List<int> Lista = par.valor.Split(',').Select(x => Convert.ToInt32(x.ToString())).ToList();
                foreach (var item in pDocumento.Respostas.Where(x => Lista.Count(y => y == x.Pergunta.ID) == 0).OrderBy(o => o.Pergunta.Descricao).ToList())
                {
                    if (item.Descricao != null || item.Descricao != "0")
                        _PerguntasAndRespostas.Add(new PerguntasRespostas()
                        {
                            Texto = TrataTexto(item.Pergunta.Descricao, item.Descricao, false, null)
                        });
                }
            }
            else
            {
                if (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1142)).HasItems())
                    _PerguntasAndRespostas.Add(new PerguntasRespostas()
                    {
                        Texto = TrataTexto("Nome do Paciente", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1142)).Select(r => r.Descricao).Single(), false, null)
                    });

                if (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(193)).HasItems())
                    _PerguntasAndRespostas.Add(new PerguntasRespostas()
                    {
                        Texto = TrataTexto("Idade", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(193)).Select(r => r.Descricao).Single(), false, null)
                    });

                if (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(218)).HasItems())
                    _PerguntasAndRespostas.Add(new PerguntasRespostas()
                    {
                        Texto = TrataTexto("Atendimento do Paciente", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(218)).Select(r => r.Descricao).Single(), false, null)
                    });

                string Data = string.Empty;
                if (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1225)).HasItems())
                    Data = DateTime.Parse(pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1225)).Select(r => r.Descricao).Single()).ToString("d");
                string Hora = string.Empty;
                if (pDocumento.Respostas.Count(x => x.Pergunta.ID.Equals(1262)) > 0)
                    Hora = pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1262)).Select(r => r.Descricao).Single();
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Data-Hora Atendimento", Data + " " + Hora, false, null)
                });
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Origem do Atendimento", pDocumento.Atendimento.OrigemAtendimento.Descricao, false, null)
                });

                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Motivo da Consulta", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1148)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1148)).Select(r => r.Descricao).Single(), true, null)
                });
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Avaliação Clínica", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1149)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1149)).Select(r => r.Descricao).Single(), true, null)
                });

                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Avaliação e Orientação do Médico Assistente"
                    , pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1223)).Count() == 0 ? string.Empty
                    : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1223)).Select(r => r.Descricao).Single()
                    , true
                    , null)
                });

                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Médico especialista indicado e chamado pelo HMV"
                    , pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1302)).Count() == 0 ? null
                    : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1302)).Select(r => r.Descricao).Single()
                    , false
                    , pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1304)).Count() == 0 ? string.Empty
                    : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1304)).Select(r => r.Descricao).Single())
                });

                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Procedimentos Realizados", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(765)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(765)).Select(r => r.Descricao).Single(), false, null)
                });
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Diagnóstico", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(762)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(762)).Select(r => r.Descricao).Single(), true, null)
                });
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Conduta", pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1151)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1151)).Select(r => r.Descricao).Single(), true, null)
                });
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("Data/Hora da Alta",
                    (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1621)).Count() == 0 ? " - " : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1621)).Select(r => r.Descricao).Single()) + " " +
                    (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1623)).Count() == 0 ? " - " : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1623)).Select(r => r.Descricao).Single()), false, null)
                });
                string CRMCRO = pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1222)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1222)).Select(r => r.Descricao).Single();
                CRMCRO += Environment.NewLine + (pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1282)).Count() == 0 ? string.Empty : pDocumento.Respostas.Where(x => x.Pergunta.ID.Equals(1282)).Select(r => r.Descricao).Single());
                _PerguntasAndRespostas.Add(new PerguntasRespostas()
                {
                    Texto = TrataTexto("CRM/CRO Nome do Médico (assistente/plantonista)", CRMCRO, true, null)
                });
            }
            return _PerguntasAndRespostas;
        }
        #endregion

        private List<PerguntasRespostas> _PerguntasAndRespostas = new List<PerguntasRespostas>();
        private wrpBoletimDeEmergencia _BoletimDeEmergencia { get; set; }

        private string _RemoveColchetes(string pDescricao)
        {
            try
            {
                string[] text = pDescricao.Split(new char[] { '[' }, 2);
                return Environment.NewLine + text[1].Remove(text[1].Length - 1);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string TrataTexto(string pPergunta, string pResposta, bool pQuebraLinha, string pCheckBox)
        {
            System.Windows.Forms.RichTextBox rtf = new System.Windows.Forms.RichTextBox();
            if (pQuebraLinha)
            {
                rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                rtf.SelectionColor = System.Drawing.Color.Navy;
                rtf.SelectionLength = 15;
                rtf.SelectedText = pPergunta.Trim() + Environment.NewLine;
                rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                rtf.SelectionColor = System.Drawing.Color.Black;
                rtf.SelectionLength = 15;
                rtf.SelectedText = pResposta == null ? string.Empty : pResposta;
            }
            else if (pCheckBox != null)
            {
                rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                rtf.SelectionColor = System.Drawing.Color.Navy;
                rtf.SelectionLength = 15;
                rtf.SelectedText = pPergunta.Trim() + Environment.NewLine;
                rtf.SelectionColor = System.Drawing.Color.Black;
                if (pResposta != null)
                {
                    rtf.SelectionFont = new System.Drawing.Font("WingDings", 10F);
                    rtf.SelectedText = "þ";
                    rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                    rtf.SelectedText = "Sim  ";
                    rtf.SelectionFont = new System.Drawing.Font("WingDings", 10F);
                    rtf.SelectedText = "¨";
                    rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                    rtf.SelectedText = "Não   ";
                    rtf.SelectedText = pCheckBox;
                }
                else
                {
                    rtf.SelectionFont = new System.Drawing.Font("WingDings", 10F);
                    rtf.SelectedText = "¨";
                    rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                    rtf.SelectedText = "Sim  ";
                    rtf.SelectionFont = new System.Drawing.Font("WingDings", 10F);
                    rtf.SelectedText = "þ";
                    rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                    rtf.SelectedText = "Não ";
                }
            }
            else
            {
                rtf.SelectionFont = new System.Drawing.Font("Calibri", 10F);
                rtf.SelectionColor = System.Drawing.Color.Navy;
                rtf.SelectionLength = 15;
                rtf.SelectedText = pPergunta + ": ";
                rtf.SelectionColor = System.Drawing.Color.Black;
                rtf.SelectedText = pResposta == null ? string.Empty : pResposta;
            }
            return rtf.Rtf;
        }
    }

    public class PerguntasRespostas
    {
        public virtual string Texto { get; set; }
    }

    public class BoletimEmergenciaRelatorio
    {
        public virtual string AtendimentoId { get; set; }
        public virtual string AtendimentoInicio { get; set; }
        public virtual string PacienteNome { get; set; }
        public virtual string PacienteIdade { get; set; }
        public virtual string PacienteDataCadastro { get; set; }
        public virtual List<ListaSinaisVitais> ListaSinais { get; set; }
        public virtual List<ListaClassificacao> ListaClassificacao { get; set; }
        public virtual List<ListaAvaliacao> ListaAvaliacao { get; set; }
        public virtual List<MotivoDaConsulta> motivos { get; set; }
        public virtual List<CIDsDiagnostico> CIDDiagnostico { get; set; }
        public virtual string DadosAltaDataHora { get; set; }
        public virtual string DadosAltaDestino { get; set; }
        public virtual string DadosAltaCondicao { get; set; }
        public virtual string DadosAltaOrientacao { get; set; }
        public virtual string Dr { get; set; }
        public virtual List<Transferecia> Transferecia { get; set; }
        public virtual PAME PAME { get; set; }
        public virtual string Empresa { get; set; }
        public virtual string Cargo { get; set; }

        public string NomePaciente { get; set; }
        public string NomeResumo { get; set; }
        public int IDPaciente { get; set; }
        public string NomePrestador { get; set; }
        public string Registro { get; set; }
        public string CodigoBarras { get; set; }

        public bool MostraCodigoBarras { get; set; }
        public bool MostraIDPaciente { get; set; }
    }

    public class ListaSinaisVitais
    {
        public virtual string SinaisData { get; set; }
        public virtual string SinaisUsuario { get; set; }
        public virtual string SinaisDor { get; set; }
        public virtual string SinaisFC { get; set; }
        public virtual string SinaisFR { get; set; }
        public virtual string SinaisPA { get; set; }
        public virtual string SinaisSat { get; set; }
        public virtual string SinaisTax { get; set; }
        public virtual string SinaisPeso { get; set; }
    }

    public class ListaClassificacao
    {
        public virtual string Data { get; set; }
        public virtual string Cor { get; set; }
        public virtual string Usuario { get; set; }
    }

    public class ListaAvaliacao
    {
        //public virtual string DataInicio { get; set; }
        //public virtual string Titulo { get; set; }
        public virtual List<ListaAvaliacaoDescricao> ListaAvaliacaoDescricao { get; set; }
        //public virtual string SN { get; set; }
    }

    public class MotivoDaConsulta
    {
        public virtual string DataInicio { get; set; }
        public virtual string Titulo { get; set; }
        public virtual List<MotivoDaConsultaDescricao> descricoes { get; set; }
        public virtual string SN { get; set; }
    }

    public class ListaAvaliacaoDescricao
    {
        public virtual string Titulo { get; set; }
        public virtual string Descricao { get; set; }
        public virtual DateTime DataInclusao { get; set; }
        public virtual string Linha { get; set; }
        public virtual string SN { get; set; }
    }

    public class MotivoDaConsultaDescricao
    {
        public virtual string Descricao { get; set; }
    }

    public class CIDsDiagnostico
    {
        public virtual string Descricao { get; set; }
    }

    public class Transferecia
    {
        public virtual wrpMeioDeTransporte MeioDeTransporte { get; set; }
        public virtual string MedicoInstituicaoDestino { get; set; }
        public virtual string MunicipioDestino { get; set; }
        public virtual string ExameFisico { get; set; }
        public virtual string MedicamentosPrescritos { get; set; }
        public virtual string NomeProficional { get; set; }
        public virtual string Oxigenio { get; set; }
        public virtual string Ventilacao { get; set; }
        public virtual string Monitorizacao { get; set; }
        public virtual string AcompMedico { get; set; }
    }

    public class PAME
    {
        public virtual string TipoRegistro { get; set; }
        public virtual string Local { get; set; }
        public virtual string Relato { get; set; }
        public virtual string DataHora { get; set; }
        public virtual bool RegistroEvento { get; set; }
    }
}
