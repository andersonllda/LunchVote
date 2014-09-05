using System;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Constant;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmRelatorioSumarioAvaliacaoMedicaEndoscopia
    {
        public vmRelatorioSumarioAvaliacaoMedicaEndoscopia(vmSumarioAvaliacaoMedicaEndoscopia pvmSumarioAvaliacaoMedicaEndoscopia)
        {
            _vm = pvmSumarioAvaliacaoMedicaEndoscopia;
        }

        #region Relatório

        private List<SumarioAvaliacaoMedicaEndoscopia> _listaRelatorio = new List<SumarioAvaliacaoMedicaEndoscopia>();
        private vmSumarioAvaliacaoMedicaEndoscopia _vm;

        public List<SumarioAvaliacaoMedicaEndoscopia> Relatorio()
        {
            SumarioAvaliacaoMedicaEndoscopia sumario = new SumarioAvaliacaoMedicaEndoscopia();

            sumario.Nome = this._vm.SumarioAvaliacaoMedicaEndoscopia.Paciente.Nome;
            sumario.Prontuario = this._vm.SumarioAvaliacaoMedicaEndoscopia.Paciente.ID.ToString();
            sumario.Atendimento = this._vm.SumarioAvaliacaoMedicaEndoscopia.Atendimento.ID.ToString();
            sumario.DataAtendimento = this._vm.SumarioAvaliacaoMedicaEndoscopia.Atendimento.DataAtendimento.ToShortDateString();
            sumario.Sexo = this._vm.SumarioAvaliacaoMedicaEndoscopia.Paciente.Sexo.ToString();
            sumario.Cor = this._vm.SumarioAvaliacaoMedicaEndoscopia.Paciente.Cor.ToString();

            if (_vm.Atendimento.IsNotNull())
            {
                if (_vm.Atendimento.Paciente.IsNotNull())
                {
                    sumario.NomePaciente = _vm.Atendimento.Paciente.Nome;
                    sumario.IDPaciente = _vm.Atendimento.Paciente.ID.ToString();
                }

                sumario.NomeResumo = _vm.Atendimento.Leito.IsNotNull() ? _vm.Atendimento.Leito.Descricao : string.Empty;
                sumario.CodigoBarras = _vm.Atendimento.ID.ToString();
                sumario.MostraCodigoBarras = true;

                if (_vm.Atendimento.Prestador.IsNotNull())
                {
                    sumario.NomePrestador = _vm.Atendimento.Prestador.Nome;
                    sumario.Registro = _vm.Atendimento.Prestador.Registro;
                }
            }

            if (this._vm.SumarioAvaliacaoMedicaEndoscopia.Paciente.IdadeExtenso.ConvertNullToStringEmpty().IsNotEmpty())
            {
                sumario.Idade = this._vm.SumarioAvaliacaoMedicaEndoscopia.Paciente.Idade.ToString(2);
            }

            // Assinatura
            AssinaturaEndoscopia assinatura = new AssinaturaEndoscopia();
            _listaAssinaturaSource = new List<AssinaturaEndoscopia>();

            if (_vm.SumarioAvaliacaoMedicaEndoscopia.DataEncerramento.IsNotNull())
            {
                assinatura.Assinatura = _vm.SumarioAvaliacaoMedicaEndoscopia.Usuario.AssinaturaNaLinhaSemColchetes;
                assinatura.DataEncerramento = _vm.SumarioAvaliacaoMedicaEndoscopia.DataEncerramento.Value.ToString();
            }
            else
            {
                assinatura.Assinatura = _vm.SumarioAvaliacaoMedicaEndoscopia.Usuario.AssinaturaNaLinhaSemColchetes;
            }

            _listaAssinaturaSource.Add(assinatura);

            // Procedimentos
            _listaProcedimentoSource = Procedimento(_vm);

            // Alergias
            _listaAlergiasSource = Alergias(_vm);

            // Perfil Psico-Social
            _listaPerfilPsicoSocialSource = PerfilPsicoSocial(_vm);

            // Comorbidades
            _listaComorbidadesSource = Comorbidades(_vm);

            // História Familiar
            _listaHistoriaFamiliarSource = HistoriaFamiliar(_vm);

            // Medicamentos em Uso
            _listaMedicamentosEmUsoSource = MedicamentosEmUso(_vm);

            // Exame Físico
            _listaExameFisicoSource = ExameFisico(_vm);

            _listaRelatorio.Add(sumario);

            return _listaRelatorio;
        }

        #endregion Relatório

        #region Propriedades Públicas

        public List<SumarioAvaliacaoMedicaEndoscopia> _listaSumarioAvaliacaoMedicaEndoscopiaSource { get; set; }
        public List<AssinaturaEndoscopia> _listaAssinaturaSource { get; set; }
        public List<MedicamentosEmUsoEndoscopia> _listaMedicamentosEmUsoSource { get; set; }
        public List<MedicamentosDeRiscoEndoscopia> _listaMedicamentosDeRiscoSource { get; set; }
        public List<HistoriaFamiliarEndoscopia> _listaHistoriaFamiliarSource { get; set; }
        public List<ComorbidadesEndoscopia> _listaComorbidadesSource { get; set; }
        public List<ProcedimentosRealizadosNoHMVEndoscopia> _listaProcedimentosRealizadosHMVSource { get; set; }
        public List<PerfilPsicoSocialEndoscopia> _listaPerfilPsicoSocialSource { get; set; }
        public List<AlergiasEndoscopia> _listaAlergiasSource { get; set; }
        public List<ProcedimentoEndoscopia> _listaProcedimentoSource { get; set; }
        public List<ExameFisicoEndoscopia> _listaExameFisicoSource { get; set; }

        private double IMC
        {
            get
            {
                double peso = 0;
                peso = Convert.ToDouble(this._vm.SumarioAvaliacaoMedicaEndoscopia.Peso);
                if (this._vm.SumarioAvaliacaoMedicaEndoscopia.Altura == 0 || this._vm.SumarioAvaliacaoMedicaEndoscopia.Peso == 0) return 0;
                return Math.Round(peso / Math.Pow((double)this._vm.SumarioAvaliacaoMedicaEndoscopia.Altura / 100, 2), 2);
            }
        }

        private double SC
        {
            get
            {
                double peso = 0;
                peso = Convert.ToDouble(this._vm.SumarioAvaliacaoMedicaEndoscopia.Peso);
                if (this._vm.SumarioAvaliacaoMedicaEndoscopia.Altura == 0 || this._vm.SumarioAvaliacaoMedicaEndoscopia.Peso == 0) return 0;
                return Math.Round(0.007184 * Math.Pow(peso, 0.425) * Math.Pow(this._vm.SumarioAvaliacaoMedicaEndoscopia.Altura, 0.725), 2);
            }
        }

        #endregion Propriedades Públicas

        #region Métodos

        public List<MedicamentosEmUsoEndoscopia> MedicamentosEmUso(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<MedicamentosEmUsoEndoscopia> lista = new List<MedicamentosEmUsoEndoscopia>();
            MedicamentosEmUsoEndoscopia medicamento = new MedicamentosEmUsoEndoscopia();
            medicamento.MostraMedicamentosEmUso = false;
            medicamento.MostraNaoFazUsoDeMedicamentos = false;

            // Medicamentos Item
            medicamento.listaMedicamentosEmUsoItem = MedicamentosEmUsoItem(pVm);
            if (medicamento.listaMedicamentosEmUsoItem.FirstOrDefault().isNaoFazUsoDeMedicamentos)
            {
                medicamento.NaoFazUsoDeMedicamentos = medicamento.listaMedicamentosEmUsoItem.FirstOrDefault().Medicamento;
                medicamento.MostraMedicamentosEmUso = true;
                medicamento.MostraNaoFazUsoDeMedicamentos = true;
            }
            else if (medicamento.listaMedicamentosEmUsoItem.FirstOrDefault().MostraMedicamentosEmUsoItem)
                medicamento.MostraMedicamentosEmUso = true;

            medicamento.listaMedicamentosDeRiscoItem = MedicamentosDeRisco(pVm);
            if (medicamento.listaMedicamentosDeRiscoItem.FirstOrDefault().MostraMedicamentosDeRisco)
                medicamento.MostraMedicamentosEmUso = true;

            lista.Add(medicamento);

            return lista;
        }

        public List<MedicamentosEmUsoItemEndoscopia> MedicamentosEmUsoItem(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<MedicamentosEmUsoItemEndoscopia> lista = new List<MedicamentosEmUsoItemEndoscopia>();
            MedicamentosEmUsoItemEndoscopia medicamento;

            foreach (var item in pVm.vmSumarioAvaliacaoMedicaMedicamentosEndoscopia.vmMedicamentosEmUsoEvento.MedicamentosCollection.Where(x => x.Status != StatusMedicamentosEmUso.Excluído))
            {
                medicamento = new MedicamentosEmUsoItemEndoscopia();
                if (item.Status == StatusMedicamentosEmUso.NaoFazUso)
                {
                    lista = new List<MedicamentosEmUsoItemEndoscopia>();
                    medicamento.Medicamento = item.Medicamento;
                    medicamento.MostraMedicamentosEmUsoItem = false;
                    medicamento.isNaoFazUsoDeMedicamentos = true;
                    lista.Add(medicamento);
                    break;
                }

                //if (pVm.Evento.MedicamentosEmUsoEventos.Count(x => x.Chave == pVm.SumarioAvaliacaoMedicaEndoscopia.ID && x.MedicamentosEmUso.ID == item.ID) > 0)
                //{
                medicamento = new MedicamentosEmUsoItemEndoscopia();
                medicamento.Medicamento = item.Medicamento;
                medicamento.Dose = item.Dose;
                medicamento.Frequencia = item.Frequencia;
                medicamento.Via = item.Via;
                medicamento.Status = item.Status.GetEnumCustomDisplay();
                medicamento.MostraMedicamentosEmUsoItem = true;
                medicamento.isNaoFazUsoDeMedicamentos = false;
                lista.Add(medicamento);
                //}
            }

            if (lista.Count == 0)
            {
                medicamento = new MedicamentosEmUsoItemEndoscopia();
                medicamento.MostraMedicamentosEmUsoItem = false;
                medicamento.isNaoFazUsoDeMedicamentos = false;
                lista.Add(medicamento);
            }

            return lista;
        }

        public List<MedicamentosDeRiscoEndoscopia> MedicamentosDeRisco(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<MedicamentosDeRiscoEndoscopia> lista = new List<MedicamentosDeRiscoEndoscopia>();
            MedicamentosDeRiscoEndoscopia medicamentoDeRisco;

            string nega = string.Empty;

            foreach (var item in pVm.SumarioAvaliacaoMedicaEndoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.MedicamentoRisco == SimNao.Sim))
            {
                if (item.Nega == SimNao.Sim)
                {
                    if (item.ItemEndoscopia.Descricao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        if (nega.IsEmpty())
                            nega = "Nega: " + item.ItemEndoscopia.Descricao;
                        else
                            nega = nega + ", " + item.ItemEndoscopia.Descricao;
                    }
                }
                else
                {
                    if (item.Observacao.IsNotEmpty())
                    {
                        medicamentoDeRisco = new MedicamentosDeRiscoEndoscopia();
                        medicamentoDeRisco.Medicamento = item.ItemEndoscopia.Descricao + ": " + item.Observacao;
                        medicamentoDeRisco.MostraMedicamentosDeRisco = true;
                        lista.Add(medicamentoDeRisco);
                    }
                }
            }

            if (nega.IsNotEmpty())
            {
                medicamentoDeRisco = new MedicamentosDeRiscoEndoscopia();
                medicamentoDeRisco.Medicamento = nega;
                medicamentoDeRisco.MostraMedicamentosDeRisco = true;
                lista.Add(medicamentoDeRisco);
            }

            if (lista.Count == 0)
            {
                medicamentoDeRisco = new MedicamentosDeRiscoEndoscopia();
                medicamentoDeRisco.MostraMedicamentosDeRisco = false;
                lista.Add(medicamentoDeRisco);
            }

            return lista;
        }

        public List<HistoriaFamiliarEndoscopia> HistoriaFamiliar(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<HistoriaFamiliarEndoscopia> lista = new List<HistoriaFamiliarEndoscopia>();
            HistoriaFamiliarEndoscopia historia;

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Ausencia == SimNao.Sim)
            {
                historia = new HistoriaFamiliarEndoscopia();
                historia.Descricao = "<< Ausência de histórico familiar relacionado ao procedimento proposto >>";
                historia.MostraHistoriaFamiliar = true;
                lista.Add(historia);
            }
            else
            {
                if (pVm.SumarioAvaliacaoMedicaEndoscopia.Cancer == SimNao.Sim)
                {
                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.CancerObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        historia = new HistoriaFamiliarEndoscopia();
                        historia.Descricao = "Câncer Gástrico: " + pVm.SumarioAvaliacaoMedicaEndoscopia.CancerObservacao;
                        historia.MostraHistoriaFamiliar = true;
                        lista.Add(historia);
                    }
                }

                if (pVm.SumarioAvaliacaoMedicaEndoscopia.CancerIntestino == SimNao.Sim)
                {
                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.CancerIntestinoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        historia = new HistoriaFamiliarEndoscopia();
                        historia.Descricao = "Câncer Intestino: " + pVm.SumarioAvaliacaoMedicaEndoscopia.CancerIntestinoObservacao;
                        historia.MostraHistoriaFamiliar = true;
                        lista.Add(historia);
                    }
                }

                if (pVm.SumarioAvaliacaoMedicaEndoscopia.Outro == SimNao.Sim)
                {
                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.OutroObservacao.ConvertNullToStringEmpty().IsNotEmpty())
                    {
                        historia = new HistoriaFamiliarEndoscopia();
                        historia.Descricao = pVm.SumarioAvaliacaoMedicaEndoscopia.OutroObservacao; //"Outro: " +
                        historia.MostraHistoriaFamiliar = true;
                        lista.Add(historia);
                    }
                }
            }

            if (lista.Count == 0)
            {
                historia = new HistoriaFamiliarEndoscopia();
                historia.MostraHistoriaFamiliar = false;
                lista.Add(historia);
            }

            return lista;
        }

        public List<ComorbidadesEndoscopia> Comorbidades(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ComorbidadesEndoscopia> lista = new List<ComorbidadesEndoscopia>();
            ComorbidadesEndoscopia comorbidades = new ComorbidadesEndoscopia();
            comorbidades.MostraComorbidades = false;

            // Comorbidades Item
            comorbidades.listaComorbidadesItem = ComorbidadesItem(pVm);
            if (comorbidades.listaComorbidadesItem.FirstOrDefault().MostraComorbidadesItem)
                comorbidades.MostraComorbidades = true;

            // Outras Doenças
            comorbidades.listaOutrasDoencas = ComorbidadeOutrasDoencasEndoscopia(pVm);
            if (comorbidades.listaOutrasDoencas.FirstOrDefault().MostraOutrasDoencas)
                comorbidades.MostraComorbidades = true;

            // Procedimentos Realizados no HMV
            comorbidades.listaProcedimentosRealizadosNoHMVEndoscopia = ProcedimentosRealizadosHMV(pVm);
            if (comorbidades.listaProcedimentosRealizadosNoHMVEndoscopia.FirstOrDefault().MostraProcedimentosRealizadosNoHMV)
                comorbidades.MostraComorbidades = true;

            // Outros Procedimentos Realizados no HMV
            comorbidades.listaOutrosProcedimentosRealizadosNoHMVEndoscopia = OutrosProcedimentosRealizadosNoHMV(pVm);
            if (comorbidades.listaOutrosProcedimentosRealizadosNoHMVEndoscopia.FirstOrDefault().MostraOutrosProcedimentosRealizadosNoHMV)
                comorbidades.MostraComorbidades = true;

            lista.Add(comorbidades);

            return lista;
        }

        public List<ComorbidadesItemEndoscopia> ComorbidadesItem(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ComorbidadesItemEndoscopia> lista = new List<ComorbidadesItemEndoscopia>();
            ComorbidadesItemEndoscopia comorbidades;

            string nega = string.Empty;

            foreach (var item in pVm.SumarioAvaliacaoMedicaEndoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.Comorbidade == SimNao.Sim))
            {
                if (item.Nega == SimNao.Sim)
                {
                    if (nega.IsEmpty())
                        nega = "Nega: " + item.ItemEndoscopia.Descricao;
                    else
                        nega += ", " + item.ItemEndoscopia.Descricao;
                }
                else
                {
                    if (item.Observacao.IsNotEmpty())
                    {
                        comorbidades = new ComorbidadesItemEndoscopia();
                        comorbidades.Descricao = item.ItemEndoscopia.Descricao + ": " + item.Observacao;
                        comorbidades.MostraComorbidadesItem = true;
                        lista.Add(comorbidades);
                    }
                }
            }

            if (nega.IsNotEmpty())
            {
                comorbidades = new ComorbidadesItemEndoscopia();
                comorbidades.Descricao = nega;
                comorbidades.MostraComorbidadesItem = true;
                lista.Add(comorbidades);
            }

            if (lista.Count == 0)
            {
                comorbidades = new ComorbidadesItemEndoscopia();
                comorbidades.MostraComorbidadesItem = false;
                lista.Add(comorbidades);
            }

            return lista;
        }

        public List<ComorbidadeOutrasDoencasEndoscopia> ComorbidadeOutrasDoencasEndoscopia(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ComorbidadeOutrasDoencasEndoscopia> lista = new List<ComorbidadeOutrasDoencasEndoscopia>();
            ComorbidadeOutrasDoencasEndoscopia doenca = new ComorbidadeOutrasDoencasEndoscopia();
            doenca.MostraOutrasDoencas = false;

            if (pVm.vmSumarioAvaliacaoMedicaComorbidadesEndoscopia.Outras.ConvertNullToStringEmpty().IsNotEmpty())
            {
                doenca.Descricao = pVm.vmSumarioAvaliacaoMedicaComorbidadesEndoscopia.Outras;
                doenca.MostraOutrasDoencas = true;
            }

            lista.Add(doenca);

            return lista;
        }

        public List<ProcedimentosRealizadosNoHMVEndoscopia> ProcedimentosRealizadosHMV(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ProcedimentosRealizadosNoHMVEndoscopia> lista = new List<ProcedimentosRealizadosNoHMVEndoscopia>();
            ProcedimentosRealizadosNoHMVEndoscopia procedimento = new ProcedimentosRealizadosNoHMVEndoscopia();
            procedimento.MostraProcedimentosRealizadosNoHMV = false;

            foreach (var itemP in pVm.vmSumarioAvaliacaoMedicaComorbidadesEndoscopia.vmProcedimentosRealizados.ProcedimentosRealizados)
            {
                procedimento = new ProcedimentosRealizadosNoHMVEndoscopia();
                procedimento.Procedimento = itemP.Procedimento;
                procedimento.Atendimento = itemP.IdAtendimento.ToString();
                procedimento.DataAtendimento = itemP.DataAtendimento.IsNotNull() ? itemP.DataAtendimento.Value.ToShortDateString() : string.Empty;
                procedimento.Profissional = itemP.NomePrestador;
                procedimento.MostraProcedimentosRealizadosNoHMV = true;
                lista.Add(procedimento);
            }

            if (lista.Count == 0)
            {
                procedimento = new ProcedimentosRealizadosNoHMVEndoscopia();
                procedimento.MostraProcedimentosRealizadosNoHMV = false;
                lista.Add(procedimento);
            }

            return lista;
        }

        public List<OutrosProcedimentosRealizadosNoHMVEndoscopia> OutrosProcedimentosRealizadosNoHMV(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<OutrosProcedimentosRealizadosNoHMVEndoscopia> lista = new List<OutrosProcedimentosRealizadosNoHMVEndoscopia>();
            OutrosProcedimentosRealizadosNoHMVEndoscopia procedimento = new OutrosProcedimentosRealizadosNoHMVEndoscopia();
            procedimento.MostraOutrosProcedimentosRealizadosNoHMV = false;

            foreach (var item in pVm.SumarioAvaliacaoMedicaEndoscopia.ProcedimentosEndoscopia.Where(x => x.IdCirurgia.IsNull() || x.IdCirurgia == 0).ToList())
            {
                procedimento.Procedimento = item.Descricao;
                procedimento.Data = item.Data.HasValue ? item.Data.Value.ToShortDateString() : string.Empty;
                procedimento.Ano = item.Ano;
                procedimento.Observacao = item.Observacao;
                procedimento.MostraOutrosProcedimentosRealizadosNoHMV = true;
                lista.Add(procedimento);
            }            

            return lista;
        }

        public List<AlergiasEndoscopia> Alergias(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<AlergiasEndoscopia> lista = new List<AlergiasEndoscopia>();
            AlergiasEndoscopia alergia = new AlergiasEndoscopia();
            alergia.MostraAlergias = false;
            alergia.MostraSemAlergiasConhecidas = false;

            alergia.listaAlergiasItem = AlergiasItem(pVm);
            if (alergia.listaAlergiasItem.FirstOrDefault().isSemAlergiasConhecidas)
            {
                alergia.SemAlergiasConhecidas = alergia.listaAlergiasItem.FirstOrDefault().Agente;
                alergia.MostraAlergias = true;
                alergia.MostraSemAlergiasConhecidas = true;
            }
            else if (alergia.listaAlergiasItem.FirstOrDefault().MostraAlergiasItem)
                alergia.MostraAlergias = true;

            lista.Add(alergia);

            return lista;
        }

        public List<AlergiasItemEndoscopia> AlergiasItem(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<AlergiasItemEndoscopia> lista = new List<AlergiasItemEndoscopia>();
            AlergiasItemEndoscopia alergia;

            foreach (var item in pVm.vmAlergiasEvento.AlergiaCollection.Where(x=> x.Status != StatusAlergiaProblema.Excluído))
            {
                if (item.IsNotNull())
                {
                    //if (pVm.Evento.AlergiaEventos.Count(x => x.Chave == pVm.SumarioAvaliacaoMedicaEndoscopia.ID && x.Alergia.ID == item.ID && x.Alergia.Status == StatusAlergiaProblema.Ativo) > 0)
                    //{
                        alergia = new AlergiasItemEndoscopia();
                        alergia.Agente = item.Agente;

                        if (alergia.Agente == Constantes.coSemAlergiasConhecidas)
                        {
                            alergia.MostraAlergiasItem = false;
                            alergia.isSemAlergiasConhecidas = true;
                        }
                        else
                        {
                            alergia.Comentario = item.Comentario;
                            alergia.Data = item.DataInicio.IsNotNull() ? item.DataInicio.Value.ToShortDateString() : string.Empty;
                            alergia.Profissional = item.Profissional.IsNull() ? string.Empty : item.Profissional.nome;
                            alergia.Status = item.Status.GetEnumCustomDisplay();
                            alergia.Tipo = item.AlergiaTipo.IsNotNull() ? item.AlergiaTipo.Descricao : string.Empty;
                            alergia.MostraAlergiasItem = true;
                            alergia.isSemAlergiasConhecidas = false;
                        }
                        lista.Add(alergia);
                    //}
                }
            }

            if (lista.Count == 0)
            {
                alergia = new AlergiasItemEndoscopia();
                alergia.MostraAlergiasItem = false;
                alergia.isSemAlergiasConhecidas = false;
                lista.Add(alergia);
            }

            return lista;
        }

        public List<PerfilPsicoSocialEndoscopia> PerfilPsicoSocial(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<PerfilPsicoSocialEndoscopia> lista = new List<PerfilPsicoSocialEndoscopia>();
            PerfilPsicoSocialEndoscopia perfil;

            string nega = string.Empty;

            foreach (var item in pVm.SumarioAvaliacaoMedicaEndoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.PerfilPscicoSocial == SimNao.Sim))
            {
                if (item.Nega == SimNao.Sim)
                {
                    if (item.ItemEndoscopia.Descricao.IsNotEmpty())
                    {
                        nega = nega + item.ItemEndoscopia.Descricao + ", ";
                    }
                }
                else
                {
                    if (item.Observacao.IsNotEmpty())
                    {
                        perfil = new PerfilPsicoSocialEndoscopia();
                        perfil.MostraPerfilPsicoSocial = true;
                        perfil.Descricao = item.ItemEndoscopia.Descricao + ": " + item.Observacao;
                        lista.Add(perfil);
                    }
                }
            }

            if (nega.IsNotEmpty())
            {
                nega = nega.Remove(nega.Length - 2);
                perfil = new PerfilPsicoSocialEndoscopia();
                perfil.MostraPerfilPsicoSocial = true;
                perfil.Descricao = "Nega: " + nega;
                lista.Add(perfil);
            }

            if (lista.Count == 0)
            {
                perfil = new PerfilPsicoSocialEndoscopia();
                perfil.MostraPerfilPsicoSocial = false;
                lista.Add(perfil);
            }

            return lista;
        }

        public List<ProcedimentoEndoscopia> Procedimento(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ProcedimentoEndoscopia> lista = new List<ProcedimentoEndoscopia>();
            ProcedimentoEndoscopia procedimento = new ProcedimentoEndoscopia();
            procedimento.MostraProcedimento = false;

            procedimento.listaMotivoDoProcedimentoHistoriaDaDoencaAtual = MotivoDoProcedimentoHistoriaDaDoencaAtual(pVm);
            if (procedimento.listaMotivoDoProcedimentoHistoriaDaDoencaAtual.FirstOrDefault().MostraMotivoDoProcedimentoHistoriaDaDoencaAtual)
                procedimento.MostraProcedimento = true;

            procedimento.listaProcedimentoPlanejado = ProcedimentoPlanejado(pVm);
            if (procedimento.listaProcedimentoPlanejado.FirstOrDefault().MostraProcedimentoPlanejado)
                procedimento.MostraProcedimento = true;

            lista.Add(procedimento);

            return lista;
        }

        public List<MotivoDoProcedimentoHistoriaDaDoencaAtual> MotivoDoProcedimentoHistoriaDaDoencaAtual(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<MotivoDoProcedimentoHistoriaDaDoencaAtual> lista = new List<MotivoDoProcedimentoHistoriaDaDoencaAtual>();
            MotivoDoProcedimentoHistoriaDaDoencaAtual motivo = new MotivoDoProcedimentoHistoriaDaDoencaAtual();
            motivo.MostraMotivoDoProcedimentoHistoriaDaDoencaAtual = false;

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Motivo.ConvertNullToStringEmpty().IsNotEmpty())
            {
                motivo.Motivo = pVm.SumarioAvaliacaoMedicaEndoscopia.Motivo;
                motivo.MostraMotivoDoProcedimentoHistoriaDaDoencaAtual = true;
            }

            lista.Add(motivo);

            return lista;
        }

        public List<ProcedimentoPlanejadoEndoscopia> ProcedimentoPlanejado(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ProcedimentoPlanejadoEndoscopia> lista = new List<ProcedimentoPlanejadoEndoscopia>();
            ProcedimentoPlanejadoEndoscopia procedimento = new ProcedimentoPlanejadoEndoscopia();
            procedimento.MostraProcedimentoPlanejado = false;


            if (pVm.SumarioAvaliacaoMedicaEndoscopia.EndoscopiaDigestivaAlta == SimNao.Sim)
            {
                procedimento.Procedimento += "Endoscopia Digestiva Alta, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Colonoscopia == SimNao.Sim)
            {
                procedimento.Procedimento += "Colonoscopia, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Colangiopancreatografia == SimNao.Sim)
            {
                procedimento.Procedimento += "Colangiopancreatografia Endoscópica, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.EcoendoscopiaAlta == SimNao.Sim)
            {
                procedimento.Procedimento += "Ecoendoscopia Alta, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.EcoendoscopiaBaixa == SimNao.Sim)
            {
                procedimento.Procedimento += "Ecoendoscopia Baixa, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Fibrobroncospia == SimNao.Sim)
            {
                procedimento.Procedimento += "Fibrobroncoscopia, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Laringoscopia == SimNao.Sim)
            {
                procedimento.Procedimento += "Laringoscopia, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Retossigmoidoscopia == SimNao.Sim)
            {
                procedimento.Procedimento += "Retossigmoidoscopia, ";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Enema == SimNao.Sim)
            {
                procedimento.Procedimento += "Enema";
                procedimento.MostraProcedimentoPlanejado = true;
            }
            if (!procedimento.MostraProcedimentoPlanejado)
                procedimento.Procedimento = string.Empty;
            else
                procedimento.Procedimento = procedimento.Procedimento.Trim().TrimEnd(',');

            //if (pVm.SumarioAvaliacaoMedicaEndoscopia.ProcedimetoPlanejado.IsNotNull())
            //{
            //    procedimento.Procedimento = pVm.SumarioAvaliacaoMedicaEndoscopia.ProcedimetoPlanejado.Value.GetEnumCustomDisplay();
            //    procedimento.MostraProcedimentoPlanejado = true;
            //}

            lista.Add(procedimento);

            return lista;
        }

        public List<ExameFisicoEndoscopia> ExameFisico(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ExameFisicoEndoscopia> lista = new List<ExameFisicoEndoscopia>();
            ExameFisicoEndoscopia exame = new ExameFisicoEndoscopia();
            exame.MostraExameFisico = false;
            exame.MostraExame = false;
            exame.MostraEstadoGeralMucosasENivelConsciencia = false;

            // Exame
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.PA.IsNotNull())
            {
                if (pVm.SumarioAvaliacaoMedicaEndoscopia.PA.Alta.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.PA.Alta != 0)
                    exame.Exame = exame.Exame + "PA: " + pVm.SumarioAvaliacaoMedicaEndoscopia.PA.Alta.Value.ToString();

                if (pVm.SumarioAvaliacaoMedicaEndoscopia.PA.Baixa.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.PA.Baixa.Value != 0)
                    exame.Exame = exame.Exame + "/" + pVm.SumarioAvaliacaoMedicaEndoscopia.PA.Baixa.Value.ToString() + " mmHg";
            }

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.FR.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.FR != 0)
                exame.Exame = exame.Exame + "    FR: " + pVm.SumarioAvaliacaoMedicaEndoscopia.FR.ToString() + " mpm";

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.SAT.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.SAT != 0)
                exame.Exame = exame.Exame + "    SAT: " + pVm.SumarioAvaliacaoMedicaEndoscopia.SAT.ToString() + " %";

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.TAX.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.TAX != 0)
                exame.Exame = exame.Exame + "    TAX: " + pVm.SumarioAvaliacaoMedicaEndoscopia.TAX.ToString() + " °C";

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.FC.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.FC != 0)
                exame.Exame = exame.Exame + "    FC: " + pVm.SumarioAvaliacaoMedicaEndoscopia.FC.ToString() + " bpm";

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Peso.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.Peso != 0)
                exame.Exame = exame.Exame + "    Peso: " + pVm.SumarioAvaliacaoMedicaEndoscopia.Peso.ToString() + " kg";

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Altura.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.Altura != 0)
                exame.Exame = exame.Exame + "    Altura: " + pVm.SumarioAvaliacaoMedicaEndoscopia.Altura.ToString() + " cm";

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.Peso.IsNotNull() && pVm.SumarioAvaliacaoMedicaEndoscopia.Altura.IsNotNull())
            {
                if (pVm.SumarioAvaliacaoMedicaEndoscopia.Peso != 0 && pVm.SumarioAvaliacaoMedicaEndoscopia.Altura != 0)
                {
                    exame.Exame = exame.Exame + "    ICM: " + IMC.ToString();
                    exame.Exame = exame.Exame + "    SC: " + SC.ToString();
                }
            }

            if (exame.Exame.IsNotEmpty())
            {
                exame.MostraExameFisico = true;
                exame.MostraExame = true;
            }
            //

            // Estado Geral, Mucosas e Nível de Consciência
            if (pVm.SumarioAvaliacaoMedicaEndoscopia.EstadoGeral.IsNotNull())
                exame.EstadoGeralMucosasENivelConsciencia = "Estado Geral: " + pVm.SumarioAvaliacaoMedicaEndoscopia.EstadoGeral.Value.GetEnumCustomDisplay();

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.IsNotNull() || pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaEstado.IsNotNull())
            {
                if (exame.EstadoGeralMucosasENivelConsciencia.IsEmpty())
                {
                    exame.EstadoGeralMucosasENivelConsciencia = "Mucosas: ";
                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.HasValue)
                        exame.EstadoGeralMucosasENivelConsciencia += pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.Value.GetEnumCustomDisplay();

                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaEstado.HasValue)
                    {
                        if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.HasValue)
                            exame.EstadoGeralMucosasENivelConsciencia += ", ";
                        exame.EstadoGeralMucosasENivelConsciencia += pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaEstado.Value.GetEnumCustomDisplay();
                    }
                }
                else
                {
                    exame.EstadoGeralMucosasENivelConsciencia += "     Mucosas: ";

                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.HasValue)
                        exame.EstadoGeralMucosasENivelConsciencia += pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.Value.GetEnumCustomDisplay();

                    if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaEstado.HasValue)
                    {
                        if (pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaSituacao.HasValue)
                            exame.EstadoGeralMucosasENivelConsciencia += ", ";
                        exame.EstadoGeralMucosasENivelConsciencia += pVm.SumarioAvaliacaoMedicaEndoscopia.MucosaEstado.Value.GetEnumCustomDisplay();
                    }
                }
            }

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.NivelConsciencia.IsNotNull())
            {
                if (exame.EstadoGeralMucosasENivelConsciencia.IsEmpty())
                    exame.EstadoGeralMucosasENivelConsciencia = "Nível de Consciência: " + pVm.SumarioAvaliacaoMedicaEndoscopia.NivelConsciencia.Value.GetEnumCustomDisplay();
                else
                    exame.EstadoGeralMucosasENivelConsciencia += "     Nível de Consciência: " + pVm.SumarioAvaliacaoMedicaEndoscopia.NivelConsciencia.Value.GetEnumCustomDisplay();
            }

            if (exame.EstadoGeralMucosasENivelConsciencia.IsNotEmpty())
            {
                exame.MostraExameFisico = true;
                exame.MostraEstadoGeralMucosasENivelConsciencia = true;
            }
            //

            // Observação
            exame.listaObservacao = ExameFisicoObservacaoEndoscopia(pVm);
            if (exame.listaObservacao.FirstOrDefault().MostraObservacao)
                exame.MostraExameFisico = true;

            lista.Add(exame);

            return lista;
        }

        public List<ExameFisicoObservacaoEndoscopia> ExameFisicoObservacaoEndoscopia(vmSumarioAvaliacaoMedicaEndoscopia pVm)
        {
            List<ExameFisicoObservacaoEndoscopia> lista = new List<ExameFisicoObservacaoEndoscopia>();
            ExameFisicoObservacaoEndoscopia observacao = new ExameFisicoObservacaoEndoscopia();
            observacao.MostraObservacao = false;

            if (pVm.SumarioAvaliacaoMedicaEndoscopia.ExameFisicoObservacao.ConvertNullToStringEmpty().IsNotEmpty())
            {
                observacao.Descricao = pVm.SumarioAvaliacaoMedicaEndoscopia.ExameFisicoObservacao;
                observacao.MostraObservacao = true;
            }

            lista.Add(observacao);

            return lista;
        }

        #endregion Métodos
    }

    public class SumarioAvaliacaoMedicaEndoscopia
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
        public string IDPaciente { get; set; }
        public string NomePrestador { get; set; }
        public string Registro { get; set; }
        public string CodigoBarras { get; set; }

        public bool MostraCodigoBarras { get; set; }
    }

    public class AssinaturaEndoscopia
    {
        public string Assinatura { get; set; }
        public string DataEncerramento { get; set; }
    }

    public class MedicamentosEmUsoEndoscopia
    {
        public string NaoFazUsoDeMedicamentos { get; set; }

        public bool MostraMedicamentosEmUso { get; set; }
        public bool MostraNaoFazUsoDeMedicamentos { get; set; }

        public List<MedicamentosEmUsoItemEndoscopia> listaMedicamentosEmUsoItem { get; set; }
        public List<MedicamentosDeRiscoEndoscopia> listaMedicamentosDeRiscoItem { get; set; }
    }

    public class MedicamentosEmUsoItemEndoscopia
    {
        public string Medicamento { get; set; }
        public string Dose { get; set; }
        public string Frequencia { get; set; }
        public string Via { get; set; }
        public string Status { get; set; }

        public bool MostraMedicamentosEmUsoItem { get; set; }
        public bool isNaoFazUsoDeMedicamentos { get; set; }
    }

    public class MedicamentosDeRiscoEndoscopia
    {
        public string Medicamento { get; set; }

        public bool MostraMedicamentosDeRisco { get; set; }
    }

    public class HistoriaFamiliarEndoscopia
    {
        public string Descricao { get; set; }

        public bool MostraHistoriaFamiliar { get; set; }
    }

    public class ComorbidadesEndoscopia
    {
        public bool MostraComorbidades { get; set; }

        public List<ComorbidadesItemEndoscopia> listaComorbidadesItem { get; set; }
        public List<ComorbidadeOutrasDoencasEndoscopia> listaOutrasDoencas { get; set; }
        public List<ProcedimentosRealizadosNoHMVEndoscopia> listaProcedimentosRealizadosNoHMVEndoscopia { get; set; }
        public List<OutrosProcedimentosRealizadosNoHMVEndoscopia> listaOutrosProcedimentosRealizadosNoHMVEndoscopia { get; set; }
    }

    public class ComorbidadesItemEndoscopia
    {
        public string Descricao { get; set; }

        public bool MostraComorbidadesItem { get; set; }
    }

    public class ComorbidadeOutrasDoencasEndoscopia
    {
        public string Descricao { get; set; }

        public bool MostraOutrasDoencas { get; set; }
    }

    public class ProcedimentosRealizadosNoHMVEndoscopia
    {
        public string Atendimento { get; set; }
        public string DataAtendimento { get; set; }
        public string Procedimento { get; set; }
        public string Profissional { get; set; }
        public string IdCirurgia { get; set; }

        public bool MostraProcedimentosRealizadosNoHMV { get; set; }
    }

    public class OutrosProcedimentosRealizadosNoHMVEndoscopia
    {
        public string Procedimento { get; set; }
        public string Data { get; set; }
        public int? Ano { get; set; }
        public string Observacao { get; set; }

        public bool MostraOutrosProcedimentosRealizadosNoHMV { get; set; }
    }

    public class PerfilPsicoSocialEndoscopia
    {
        public string Descricao { get; set; }

        public bool MostraPerfilPsicoSocial { get; set; }
    }

    public class AlergiasEndoscopia
    {
        public string SemAlergiasConhecidas { get; set; }

        public bool MostraAlergias { get; set; }
        public bool MostraSemAlergiasConhecidas { get; set; }

        public List<AlergiasItemEndoscopia> listaAlergiasItem { get; set; }
    }

    public class AlergiasItemEndoscopia
    {
        public string Agente { get; set; }
        public string Tipo { get; set; }
        public string Data { get; set; }
        public string Status { get; set; }
        public string Profissional { get; set; }
        public string Comentario { get; set; }

        public bool MostraAlergiasItem { get; set; }
        public bool isSemAlergiasConhecidas { get; set; }
    }

    public class ProcedimentoEndoscopia
    {
        public bool MostraProcedimento { get; set; }

        public List<MotivoDoProcedimentoHistoriaDaDoencaAtual> listaMotivoDoProcedimentoHistoriaDaDoencaAtual { get; set; }
        public List<ProcedimentoPlanejadoEndoscopia> listaProcedimentoPlanejado { get; set; }
    }

    public class MotivoDoProcedimentoHistoriaDaDoencaAtual
    {
        public string Motivo { get; set; }

        public bool MostraMotivoDoProcedimentoHistoriaDaDoencaAtual { get; set; }
    }

    public class ProcedimentoPlanejadoEndoscopia
    {
        public string Procedimento { get; set; }

        public bool MostraProcedimentoPlanejado { get; set; }
    }

    public class ExameFisicoEndoscopia
    {
        public string Exame { get; set; }
        public string EstadoGeralMucosasENivelConsciencia { get; set; }

        public bool MostraExameFisico { get; set; }
        public bool MostraExame { get; set; }
        public bool MostraEstadoGeralMucosasENivelConsciencia { get; set; }

        public List<ExameFisicoObservacaoEndoscopia> listaObservacao { get; set; }
    }

    public class ExameFisicoObservacaoEndoscopia
    {
        public string Descricao { get; set; }

        public bool MostraObservacao { get; set; }
    }
}
