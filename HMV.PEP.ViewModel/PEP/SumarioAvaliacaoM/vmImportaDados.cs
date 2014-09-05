using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.SumarioAvaliacaoM
{
    public class vmImportaDados : ViewModelBase
    {
        #region Contrutor
        public vmImportaDados(Atendimento pAtendimento, Usuarios pUsuarioLogado)
        {
            _UsuarioLogado = pUsuarioLogado;
            this._Atendimento = new wrpAtendimento(pAtendimento);
        }
        #endregion

        #region Propriedades Publicas
        public wrpAtendimentoCollection ListaAtendimentos
        {
            get
            {
                var qr = from T in _Atendimento.Paciente.Atendimentos
                         where T.ID != _Atendimento.ID
                         select T;

                wrpAtendimentoCollection qry = new wrpAtendimentoCollection(
                    (from S in qr
                     where
                       S.SumarioAvaliacaoMedica != null &&
                       S.SumarioAvaliacaoMedica.Tipo.ID == this._Atendimento.SumarioAvaliacaoMedica.Tipo.ID
                     orderby S.DataAtendimento descending
                     select S.DomainObject).ToList());

                //var rT = new wrpAtendimentoCollection(qr.Where(y => y.SumarioAvaliacaoMedica != null && y.SumarioAvaliacaoMedica.Tipo.ID == this._Atendimento.SumarioAvaliacaoMedica.Tipo.ID).Select(y => y.DomainObject).OrderByDescending(x => x.DataAtendimento).ToList());
                return qry;
            }
        }

        public wrpAtendimento AtendimentoSelecionado
        {
            get
            {
                return _AtendimentoSelecionado;
            }
            set
            {
                _AtendimentoSelecionado = value;
            }
        }

        public void Importa()
        {
            string vTexto = string.Empty;
            #region ---------- Anamnese ----------
            //MOTIVO INTERNAÇÃO                        
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.MotivoInternacao) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.MotivoInternacao : _Atendimento.SumarioAvaliacaoMedica.MotivoInternacao + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.MotivoInternacao;
            _Atendimento.SumarioAvaliacaoMedica.MotivoInternacao = vTexto.IsEmpty() ? string.Empty : vTexto.Left(4000);

            //HISTÓRIA DA DOENÇA ATUAL
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.HistoriaDoencaAtual) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaDoencaAtual : _Atendimento.SumarioAvaliacaoMedica.HistoriaDoencaAtual + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaDoencaAtual;
            _Atendimento.SumarioAvaliacaoMedica.HistoriaDoencaAtual = vTexto.IsEmpty() ? string.Empty : vTexto.Left(3000);

            ////REVISÃO DE SISTEMAS
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros : _Atendimento.SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros;            
            _Atendimento.SumarioAvaliacaoMedica.RevisaoDeSistemas.DomainObject = _AtendimentoSelecionado.SumarioAvaliacaoMedica.RevisaoDeSistemas.DomainObject;
            _Atendimento.SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros = vTexto.IsEmpty() ? string.Empty : vTexto.Left(3000);

            ////HISTÓRIA PREGRESSA            
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.HistoriaPregressa.Outros) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaPregressa.Outros : _Atendimento.SumarioAvaliacaoMedica.HistoriaPregressa.Outros + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaPregressa.Outros;
            _Atendimento.SumarioAvaliacaoMedica.HistoriaPregressa.DomainObject.ItensHistoriaPregressa = _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaPregressa.DomainObject.ItensHistoriaPregressa;
            _Atendimento.SumarioAvaliacaoMedica.HistoriaPregressa.Outros = vTexto.IsEmpty() ? string.Empty : vTexto.Left(3000);

            //HISTÓRIA FAMILIAR            
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.HistoriaFamiliar.Outros) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaFamiliar.Outros : _Atendimento.SumarioAvaliacaoMedica.HistoriaFamiliar.Outros + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaFamiliar.Outros;
            _Atendimento.SumarioAvaliacaoMedica.HistoriaFamiliar.DomainObject.ItensHistoriaFamiliar = _AtendimentoSelecionado.SumarioAvaliacaoMedica.HistoriaFamiliar.DomainObject.ItensHistoriaFamiliar;
            _Atendimento.SumarioAvaliacaoMedica.HistoriaFamiliar.Outros = vTexto.IsEmpty() ? string.Empty : vTexto.Left(3000);

            ////PERFIL PSICOSOCIAL            
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros : _Atendimento.SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros;
            _Atendimento.SumarioAvaliacaoMedica.PerfilPsicoSocial.DomainObject.ItensPerfilPsicoSocial = _AtendimentoSelecionado.SumarioAvaliacaoMedica.PerfilPsicoSocial.DomainObject.ItensPerfilPsicoSocial;
            _Atendimento.SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros = vTexto.IsEmpty() ? string.Empty : vTexto.Left(3000);
            #endregion

            #region ---------- Exame Fisico ----------
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.Altura = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.Altura == 0 ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.Altura : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.Altura;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.Peso = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.Peso == 0.0 ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.Peso : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.Peso;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Alta = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Alta == null ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Alta : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Alta;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Baixa = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Baixa == null ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Baixa : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Baixa;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.FrequenciaCardiaca = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.FrequenciaCardiaca == 0 ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.FrequenciaCardiaca : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.FrequenciaCardiaca;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.FrequenciaRespiratoria = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.FrequenciaRespiratoria == 0 ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.FrequenciaRespiratoria : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.FrequenciaRespiratoria;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.TemperaturaAxila = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.TemperaturaAxila == null ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.TemperaturaAxila : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.TemperaturaAxila;
            //_Atendimento.SumarioAvaliacaoMedica.ExameFisico.SC = _Atendimento.SumarioAvaliacaoMedica.ExameFisico.SC == 0 ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.SC : _Atendimento.SumarioAvaliacaoMedica.ExameFisico.SC;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.EstadoGeral = _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.EstadoGeral;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.MucosaEstado = _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.MucosaEstado;
            _Atendimento.SumarioAvaliacaoMedica.ExameFisico.MucosaSituacao = _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExameFisico.MucosaSituacao;
            #endregion

            #region ---------- Hipoteses ----------
            foreach (var item in _AtendimentoSelecionado.SumarioAvaliacaoMedica.Hipoteses.ToList())
            {
                _Atendimento.SumarioAvaliacaoMedica.Hipoteses.Add(new wrpHipotese()
                                                                 {
                                                                     AvaliacaoMedica = item.AvaliacaoMedica,
                                                                     Complemento = item.Complemento
                                                                 });
            }
            #endregion

            #region ---------- Diagnostico ----------
            foreach (var item in _AtendimentoSelecionado.SumarioAvaliacaoMedica.Diagnosticos.ToList())
            {
                _Atendimento.SumarioAvaliacaoMedica.Diagnosticos.Add(new wrpDiagnostico()
               {

                   AvaliacaoMedica = _Atendimento.SumarioAvaliacaoMedica,
                   Complemento = item.Complemento,
                   Usuario = new wrpUsuarios(_UsuarioLogado),
                   Cid = item.Cid,
               });
            }
            #endregion

            #region ---------- Exames Realizados ----------
            _Atendimento.SumarioAvaliacaoMedica.ExamesRealizados.NaoForamRealizadosExames = _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExamesRealizados.NaoForamRealizadosExames;
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.ExamesRealizados.Descricao) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExamesRealizados.Descricao : _Atendimento.SumarioAvaliacaoMedica.ExamesRealizados.Descricao + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.ExamesRealizados.Descricao;
            //Caso seja um sumario antigo verifica se é nao realizado exames se sim marca como SIM.
            if (_Atendimento.SumarioAvaliacaoMedica.ExamesRealizados.NaoForamRealizadosExames.IsNull() &&
                vTexto == HMV.Core.Domain.Constant.Constantes.coNaoForamRealizadosExames)
                _Atendimento.SumarioAvaliacaoMedica.ExamesRealizados.NaoForamRealizadosExames = SimNao.Sim;

            _Atendimento.SumarioAvaliacaoMedica.ExamesRealizados.Descricao = vTexto.IsEmpty() ? string.Empty : vTexto.Left(2000);
            #endregion

            #region ---------- Plano Diagnostico ----------
            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.CirurgiaProposta) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.CirurgiaProposta : _Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.CirurgiaProposta + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.CirurgiaProposta;
            _Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.CirurgiaProposta = vTexto.IsEmpty() ? string.Empty : vTexto.Left(4000);

            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.Conduta) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.Conduta : _Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.Conduta + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.Conduta;
            _Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.Conduta = vTexto.IsEmpty() ? string.Empty : vTexto.Left(4000);

            vTexto = string.IsNullOrWhiteSpace(_Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.ExamesSolicitados) ? _AtendimentoSelecionado.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.ExamesSolicitados : _Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.ExamesSolicitados + Environment.NewLine + _AtendimentoSelecionado.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.ExamesSolicitados;
            _Atendimento.SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.ExamesSolicitados = vTexto.IsEmpty() ? string.Empty : vTexto.Left(4000);
            #endregion
        }

        public wrpSumarioAvaliacaoMedica SumarioAvalicaoMedicaSelecionado
        {
            get
            {
                if (this._AtendimentoSelecionado == null)
                    return ListaAtendimentos[0].SumarioAvaliacaoMedica;
                return this._AtendimentoSelecionado.SumarioAvaliacaoMedica;
            }
        }
        #endregion

        #region propriedades privada
        private Usuarios _UsuarioLogado;
        private wrpAtendimento _AtendimentoSelecionado { get; set; }
        private wrpAtendimento _Atendimento { get; set; }
        private wrpSumarioAvaliacaoMedica _Sumario { get; set; }
        #endregion
    }
}
