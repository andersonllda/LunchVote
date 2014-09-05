using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Repository.Anticoagulante;
using HMV.Core.Domain.Repository.AtendimentoPaciente;
using HMV.Core.Domain.Repository.Ativo;
using HMV.Core.Domain.Repository.AvaliacaoMobilidade;
using HMV.Core.Domain.Repository.ClassificacaoPaciente;
using HMV.Core.Domain.Repository.EAD;
using HMV.Core.Domain.Repository.EAD.Colaboradores;
using HMV.Core.Domain.Repository.ExamesMonitoramento;
using HMV.Core.Domain.Repository.IEP.POS;
using HMV.Core.Domain.Repository.OrdemDeCompra;
using HMV.Core.Domain.Repository.PEP;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Domain.Repository.PEP.CheckUpProntuario;
using HMV.Core.Domain.Repository.PEP.Enfermagem;
using HMV.Core.Domain.Repository.PEP.Evolucao;
using HMV.Core.Domain.Repository.PEP.PassagemPlantaoE;
using HMV.Core.Domain.Repository.PEP.PreCadastro;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Repository.PortalMobile;
using HMV.Core.Domain.Repository.PreAgendaObst;
using HMV.Core.Domain.Repository.PreCheckInWeb;
using HMV.Core.Domain.Repository.ProfissionaisEServicos;
using HMV.Core.Domain.Repository.RPC;
using HMV.Core.Domain.Repository.SumarioAvaliacaoMedica;
using HMV.Core.Domain.Repository.TransferenciaAssistencial;
using HMV.Core.Domain.Repository.Transporte;
using HMV.Core.Domain.Repository.Views.PEP.CheckUpProntuario;
using HMV.Core.NH.Repository;
using HMV.Core.NH.Repository.Anticoagulante;
using HMV.Core.NH.Repository.AtendimentoPaciente;
using HMV.Core.NH.Repository.Ativo;
using HMV.Core.NH.Repository.AvaliacaoMobilidade;
using HMV.Core.NH.Repository.EAD;
using HMV.Core.NH.Repository.EAD.Colaboradores;
using HMV.Core.NH.Repository.Evolucao;
using HMV.Core.NH.Repository.ExamesMonitoramento;
using HMV.Core.NH.Repository.IEP.POS;
using HMV.Core.NH.Repository.PEP;
using HMV.Core.NH.Repository.PEP.CentroObstetrico;
using HMV.Core.NH.Repository.PEP.CheckUpProntuario;
using HMV.Core.NH.Repository.PEP.Enfermagem;
using HMV.Core.NH.Repository.PEP.PassagemPlantaoE;
using HMV.Core.NH.Repository.PEP.PreCadastro;
using HMV.Core.NH.Repository.PEP.ProcessoDeEnfermagem;
using HMV.Core.NH.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.NH.Repository.PEP.ProcessoDeEnfermagem.ClassificacaoPaciente;
using HMV.Core.NH.Repository.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.NH.Repository.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.NH.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.NH.Repository.PortalMobile;
using HMV.Core.NH.Repository.PreAgendaObst;
using HMV.Core.NH.Repository.PreCheckInWeb;
using HMV.Core.NH.Repository.ProfissionaisEServicos;
using HMV.Core.NH.Repository.RPC;
using HMV.Core.NH.Repository.TransferenciaAssistencial;
using HMV.Core.NH.Repository.Transporte;
using HMV.Core.NH.Repository.Views.PEP.CheckUpProntuario;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using CTNH = HMV.Core.NH.Repository.CertificadoTecnico;
using CTRep = HMV.Core.Domain.Repository.CertificadoTecnico;
using EADCCNH = HMV.Core.NH.Repository.EAD.CorpoClinico;
using EADCCRep = HMV.Core.Domain.Repository.EAD.CorpoClinico;
using HMV.Core.NH.Repository.PEP.PrevAlta;
using HMV.Core.Domain.Repository.PEP.PrevAlta;

namespace HMV.Core.IoC
{
    public class RepositoryRegistry : Registry
    {
        public RepositoryRegistry()
        {
            //Configura para acessar o IRepositoryFilter
            Scan(assemblyScanner =>
            {
                assemblyScanner.TheCallingAssembly();
                assemblyScanner.AddAllTypesOf(typeof(IRepositoryFilter<>));
                assemblyScanner.AssemblyContainingType(typeof(IRepositoryFilter<>));
            });

            ForRequestedType(typeof(IRepositoryFilter<>))
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType(typeof(RepositoryFilter<>));

            ForRequestedType<IRepositorioDeSeguranca>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSeguranca>();

            ForRequestedType<IRepositorioDeUsuarios>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeUsuarios>();

            ForRequestedType<IRepositorioDoMenu>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefault.Is.OfConcreteType<RepositorioDoMenu>();

            ForRequestedType<IRepositorioAgendaMedica>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioAgendaMedica>();

            ForRequestedType<IRepositorioDeSolicitacaoAgenda>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSolicitacaoAgenda>();

            ForRequestedType<IRepositorioDeSolicitacaoAgendaConfirmada>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSolicitacaoAgendaConfirmada>();

            ForRequestedType<IRepositorioDeSolicitacaoAgendaNaoConfirmada>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSolicitacaoAgendaNaoConfirmada>();

            ForRequestedType<IRepositorioTipoSangue>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioTipoSangue>();

            ForRequestedType<IRepositorioDeCirurgias>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeCirurgias>();

            ForRequestedType<IRepositorioDeCirurgiasAtivas>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeCirurgiasAtivas>();

            ForRequestedType<IRepositorioTipoSangue>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioTipoSangue>();

            ForRequestedType<IRepositorioDeParametrosInternet>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeParametroInternet>();

            ForRequestedType<IRepositorioDeConvenios>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeConvenios>();

            ForRequestedType<IRepositorioDeConveniosAtivos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeConveniosValidos>();

            ForRequestedType<IRepositorioDePrestadores>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePrestadores>();

            ForRequestedType<IRepositorioDePrestadoresAtivos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePrestadoresAtivos>();

            ForRequestedType<IRepositorioDePacientes>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePacientes>();

            ForRequestedType<IRepositorioDePacientesAtivos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePacientesAtivos>();

            ForRequestedType<IRepositorioAvisosDeCirurgia>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioAvisosDeCirurgia>();

            ForRequestedType<IRepositorioDeAtendimento>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAtendimento>();

            ForRequestedType<IRepositorioDeAtendimentoInternado>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAtendimentoInternado>();

            ForRequestedType<IRepositorioDeAtendimentoEmergencia>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAtendimentoEmergencia>();

            ForRequestedType<IRepositorioLogsAcessoAoSistema>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefault.Is.OfConcreteType<RepositorioLogsAcessoAoSistema>();

            ForRequestedType<IRepositorioDeEspecialidadesValidas>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeEspecialidadesValidas>();

            ForRequestedType<IRepositorioDeEspecialidades>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeEspecialidades>();

            ForRequestedType<IRepositorioDeProtocolosClinicos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeProtocolosClinicos>();

            ForRequestedType<IRepositorioDeProcessos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeProcessos>();

            ForRequestedType<IRepositorioDePais>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePais>();

            ForRequestedType<IRepositorioDeSumariosAvaliacaoMedicaAbertos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSumariosAvaliacaoMedicaAbertos>();

            ForRequestedType<IRepositorioDeCid>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeCid>();

            ForRequestedType<IRepositorioDeTiposLogradouro>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefault.Is.OfConcreteType<RepositorioDeTiposLogradouro>();

            ForRequestedType<IRepositorioDeItensSumarioAvaliacaoMedica>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeItensSumarioAvaliacaoMedica>();

            ForRequestedType<IRepositorioAlergiaTipo>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAlergiaTipo>();

            ForRequestedType<IRepositorioDeCurriculo>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeCurriculo>();

            ForRequestedType<IRepositorioDeCirurgiasSumarioAlta>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeCirurgiasSumarioAlta>();

            ForRequestedType<IRepositorioDeProduto>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeProduto>();

            ForRequestedType<IRepositorioDeOrigemAtendimento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeOrigemAtendimento>();

            ForRequestedType<IRepositorioDeSetor>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSetor>();

            ForRequestedType<IRepositorioDeEscala>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEscala>();

            ForRequestedType<IRepositorioDeTiposProtocolo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTiposProtocolo>();

            ForRequestedType<IRepositorioDeClassificacaoProtocolo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeClassificacaoProtocolo>();

            ForRequestedType<IRepositorioDeTipoAnexoProtocolo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoAnexoProtocolo>();

            ForRequestedType<IRepositorioDeSistema>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSistema>();

            ForRequestedType<IRepositorioDeUsuarioClasseSistema>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<RepositorioDeUsuarioClasseSistema>();

            ForRequestedType<IRepositorioDeCapitulos>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<RepositorioDeCapitulos>();

            ForRequestedType<IRepositorioDeEntregaSolicitacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEntregaSolicitacao>();

            ForRequestedType<IRepositorioDeSolicitacaoProduto>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacaoProduto>();

            ForRequestedType<IRepositorioDeColaboradores>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeColaboradores>();

            ForRequestedType<IRepositorioLocalTrabalho>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioLocalTrabalho>();

            ForRequestedType<IRepositorioDePrestadoresComLocalDeTrabalho>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefault.Is.OfConcreteType<RepositorioDePrestadoresComLocalDeTrabalho>();

            ForRequestedType<IRepositorioDeServico>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefault.Is.OfConcreteType<RepositorioDeServico>();

            ForRequestedType<IRepositorioDeExame>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeExame>();

            ForRequestedType<IRepositorioDeRedeSemFioOutros>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRedeSemFioOutros>();

            ForRequestedType<IRepositorioDeRedeSemFio>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRedeSemFio>();

            ForRequestedType<IRepositorioDeUnidadeDeInternacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUnidadeDeInternacao>();

            ForRequestedType<IRepositorioDeClinica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeClinica>();

            ForRequestedType<IRepositorioDeAtendimentoAmbulatorial>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAtendimentoAmbulatorial>();

            ForRequestedType<IRepositorioDePacotesInstitucionais>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePacotesInstitucionais>();

            ForRequestedType<IRepositorioDePrestadoresComExcecao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadoresComExcecao>();

            ForRequestedType<IRepositorioDeGuiaDeRamais>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeGuiaDeRamais>();

            ForRequestedType<IRepositorioDeCriterio>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCriterio>();

            ForRequestedType<IRepositorioDeImagens>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeImagens>();

            ForRequestedType<IRepositorioDeCiclo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCiclo>();

            ForRequestedType<IRepositorioDeReunioesCientificas>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeReunioesCientificas>();

            ForRequestedType<IRepositorioDeGrandRound>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeGrandRound>();

            ForRequestedType<IRepositorioDeArquivosEmpresas>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeArquivosEmpresas>();

            ForRequestedType<IRepositorioDeFornecedor>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeFornecedor>();

            ForRequestedType<IRepositorioDeSetorPAMQ>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSetorPAMQ>();

            ForRequestedType<IRepositorioDeConfiguracaoCiclo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeConfiguracaoCiclo>();

            ForRequestedType<IRepositorioDePrestadoresComEspecialidadePorValidar>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadoresComEspecialidadePorValidar>();

            ForRequestedType<IRepositorioDeAreaAtuacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAreaAtuacao>();

            ForRequestedType<IRepositorioDePrestadorComAreaAtuacaoPorValidar>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadorComAreaAtuacaoPorValidar>();

            ForRequestedType<IRepositorioDeCursosMoodle>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCursosMoodle>();

            ForRequestedType<IRepositorioDeSetorGDO>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSetorGDO>();

            ForRequestedType<IRepositorioDeHierarquiaDeSetoresGDO>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeHierarquiaDeSetoresGDO>();

            ForRequestedType<IRepositorioDeContaContabil>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeContaContabil>();

            ForRequestedType<IRepositorioDeOrcado>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeOrcado>();

            ForRequestedType<IRepositorioDeRealizado>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRealizado>();

            ForRequestedType<IRepositorioDePrestadoresMaternidade>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadoresMaternidade>();

            ForRequestedType<IRepositorioDePrestadoresComCurriculo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadoresComCurriculo>();

            ForRequestedType<IRepositorioDeContasPagar>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeContasPagar>();

            ForRequestedType<IRepositorioDePacote>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePacote>();

            ForRequestedType<IRepositorioDePeriodoGMD>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePeriodoGMD>();

            ForRequestedType<IRepositorioDePlanoAcaoSetor>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePlanoAcaoSetor>();

            ForRequestedType<IRepositorioDeContrato>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeContrato>();

            ForRequestedType<IRepositorioDicasTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDicasTipo>();

            ForRequestedType<IRepositorioDeProntuarioOftalmo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeProntuarioOftalmo>();

            ForRequestedType<IRepositorioDeProntuarioOftalmoTipoExame>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeProntuarioOftalmoTipoExame>();

            ForRequestedType<IRepositorioDeProntuarioOftalmoConclusaoAdmissao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeProntuarioOftalmoConclusaoAdmissao>();

            ForRequestedType<IRepositorioDeSumariosAvaliacaoMedicaTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSumariosAvaliacaoMedicaTipo>();

            ForRequestedType<IRepositorioDeImunizacaoTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeImunizacaoTipo>();

            ForRequestedType<IRepositorioDeMotivoAlta>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeMotivoAlta>();

            ForRequestedType<IRepositorioDeSumarioAlta>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSumarioAlta>();

            ForRequestedType<IRepositorioDeDadosNascimento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeDadosNascimento>();

            ForRequestedType<IRepositorioDeMotivoAlta>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeMotivoAlta>();

            ForRequestedType<Core.Domain.Repository.IRepositorioDeProcedimento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<Core.NH.Repository.RepositorioDeProcedimento>();

            ForRequestedType<IRepositorioDeItensContaAmbulatorial>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeItensContaAmbulatorial>();

            ForRequestedType<IRepositorioDeItensContaHospitalar>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeItensContaHospitalar>();

            ForRequestedType<IRepositorioDeTipoPrescricaoMedica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoPrescricaoMedica>();

            ForRequestedType<IRepositorioDePerguntasPaciente>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePerguntasPaciente>();

            ForRequestedType<IRepositorioDeCidade>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCidade>();

            ForRequestedType<IRepositorioDeMeioDeTransporte>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeMeioDeTransporte>();

            ForRequestedType<IRepositorioDeRecomendacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRecomendacao>();

            ForRequestedType<IRepositorioDeProcedimentoSumarioAlta>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeProcedimentoSumarioAlta>();

            ForRequestedType<IRepositorioGlossarioDeSiglas>()
                      .CacheBy(InstanceScope.PerRequest)
                      .TheDefaultIsConcreteType<RepositorioGlossarioDeSiglas>();

            ForRequestedType<IRepositorioDeParametrosClinicas>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeParametrosClinicas>();

            ForRequestedType<IRepositorioDeProdutoSumarioAlta>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeProdutoSumarioAlta>();

            ForRequestedType<IRepositorioDePrescricaoSumarioAlta>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrescricaoSumarioAlta>();

            ForRequestedType<IRepositorioDePrestadorRelatorioTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadorRelatorioTipo>();

            ForRequestedType<IRepositorioDeUrgenciaPediatrica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUrgenciaPediatrica>();

            ForRequestedType<IRepositorioDeUrgenciaPediatricaGrupo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUrgenciaPediatricaGrupo>();

            ForRequestedType<IRepositorioDeAuditoriaItem>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAuditoriaItem>();

            ForRequestedType<IRepositorioDeAuditoriaFechamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAuditoriaFechamento>();

            ForRequestedType<IRepositorioDeUrgenciaPediatrica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUrgenciaPediatrica>();

            ForRequestedType<IRepositorioDeUrgenciaPediatricaGrupo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUrgenciaPediatricaGrupo>();

            ForRequestedType<IRepositorioSubClinica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioSubClinica>();

            ForRequestedType<IRepositorioDeBoletimDeEmergencia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeBoletimDeEmergencia>();

            ForRequestedType<IRepositorioSinaisVitaisTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioSinaisVitaisTipo>();

            ForRequestedType<IRepositorioDeSinaisVitais>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSinaisVitais>();

            ForRequestedType<IRepositorioCorRisco>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioCorRisco>();

            ForRequestedType<IRepositorioSinaisVitaisTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioSinaisVitaisTipo>();

            ForRequestedType<IRepositorioDeTipoAvaliacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoAvaliacao>();


            ForRequestedType<IRepositorioDeServicos>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeServicos>();

            ForRequestedType<IRepositorioDeServicoTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeServicoTipo>();

            ForRequestedType<IRepositorioDeIdioma>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeIdioma>();

            ForRequestedType<IRepositorioDeIdiomaColaborador>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeIdiomaColaborador>();

            ForRequestedType<IRepositorioDePacienteEmergencia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePacienteEmergencia>();

            ForRequestedType<IRepositorioDeNotasMoodle>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeNotasMoodle>();

            ForRequestedType<IRepositorioDeHabitacaoRecurso>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeHabitacaoRecurso>();

            ForRequestedType<IRepositorioDeTipoHabitacao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoHabitacao>();

            ForRequestedType<IRepositorioDeTipoAvaliacaoPaciente>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoAvaliacaoPaciente>();

            ForRequestedType<IRepositorioDeTipoAvaliacaoSCP>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoAvaliacaoSCP>();

            ForRequestedType<IRepositorioDeTipoAvaliacaoNAS>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoAvaliacaoNAS>();

            ForRequestedType<HMV.Core.Domain.Repository.ClassificacaoPaciente.IRepositorioDeSCP>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSCP>();

            ForRequestedType<IRepositorioDeMucosa>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeMucosa>();

            ForRequestedType<IRepositorioDeEstadoNeurologico>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEstadoNeurologico>();

            ForRequestedType<IRepositorioDeProcedencia>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeProcedencia>();

            ForRequestedType<IRepositorioDeIntestinal>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeIntestinal>();

            ForRequestedType<IRepositorioDeDeficiencia>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeDeficiencia>();

            ForRequestedType<IRepositorioDeTipoProtese>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoProtese>();

            ForRequestedType<IRepositorioDeTipoFuncaoMotora>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoFuncaoMotora>();

            ForRequestedType<IRepositorioDeResideCom>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeResideCom>();

            ForRequestedType<IRepositorioDeDoencaPrevia>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeDoencaPrevia>();

            ForRequestedType<IRepositorioDeMaterialPresente>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeMaterialPresente>();

            ForRequestedType<IRepositorioDeAvaliacaoEmocional>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAvaliacaoEmocional>();

            ForRequestedType<IRepositorioDeExameAdmissao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeExameAdmissao>();

            ForRequestedType<IRepositorioDeLocalUlcera>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeLocalUlcera>();

            ForRequestedType<IRepositorioDeHabito>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeHabito>();

            ForRequestedType<IRepositorioDeAdmissaoAssistencial>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAdmissaoAssistencial>();

            ForRequestedType<IRepositorioDeOstomia>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeOstomia>();

            ForRequestedType<IRepositorioDeAplicacaoPrescricao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAplicacaoPrescricao>();

            ForRequestedType<IRepositorioDeFrequenciaItemPrescricao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeFrequenciaItemPrescricao>();

            ForRequestedType<IRepositorioDeManifestoPaciente>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeManifestoPaciente>();

            ForRequestedType<IRepositorioDeEnfermagemDiagnostico>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEnfermagemDiagnostico>();

            ForRequestedType<IRepositorioDeSumariosAvaliacaoMedica>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<HMV.Core.NH.Repository.SumarioAvaliacaoMedica.RepositorioDeSumarioAvaliacaoMedica>();

            ForRequestedType<IRepositorioDeSumariosAvaliacaoMedicaFechados>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<HMV.Core.NH.Repository.SumarioAvaliacaoMedica.RepositorioDeSumariosAvaliacaoMedicaFechados>();

            ForRequestedType<IRepositorioDeGrauInstrucao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeGrauInstrucao>();

            ForRequestedType<IRepositorioDeTipoProfissional>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDeTipoProfissional>();

            ForRequestedType<IRepositorioDeTipoPergunta>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDeTipoPergunta>();

            ForRequestedType<IRepositorioDeInstituicao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeInstituicao>();

            ForRequestedType<IRepositorioDeCargo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCargo>();

            ForRequestedType<IRepositorioDeDocente>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeDocente>();

            ForRequestedType<IRepositorioDeAvaliacaoRiscoPergunta>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoPergunta>();

            ForRequestedType<IRepositorioDeAluno>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAluno>();

            ForRequestedType<IRepositorioDeDisciplina>()
                    .CacheBy(InstanceScope.PerRequest)
                    .TheDefaultIsConcreteType<RepositorioDeDisciplina>();

            ForRequestedType<IRepositorioDeCheckListDocumento>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeCheckListDocumento>();

            ForRequestedType<IRepositorioDePerguntasEnfermagem>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDePerguntasEnfermagem>();

            ForRequestedType<IRepositorioDeContaHospitalar>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDeContaHospitalar>();

            ForRequestedType<IRepositorioDeRedeSocial>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDeRedeSocial>();

            ForRequestedType<IRepositorioDeProcedimentoPlanoEducacional>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<RepositorioDeProcedimentoPlanoEducacional>();

            ForRequestedType<IRepositorioDeCurso>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<RepositorioDeCurso>();

            ForRequestedType<IRepositorioDeAreaAtuacaoCurso>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<RepositorioDeAreaAtuacaoCurso>();

            ForRequestedType<IRepositorioDeLocalAtividade>()
                         .CacheBy(InstanceScope.PerRequest)
                         .TheDefaultIsConcreteType<RepositorioDeLocalAtividade>();

            ForRequestedType<IRepositorioDeTipoAtividade>()
                        .CacheBy(InstanceScope.PerRequest)
                        .TheDefaultIsConcreteType<RepositorioDeTipoAtividade>();

            ForRequestedType<IRepositorioDeMatricula>()
                        .CacheBy(InstanceScope.PerRequest)
                        .TheDefaultIsConcreteType<RepositorioDeMatricula>();

            ForRequestedType<IRepositorioDeBibliografia>()
                        .CacheBy(InstanceScope.PerRequest)
                        .TheDefaultIsConcreteType<RepositorioDeBibliografia>();

            ForRequestedType<IRepositorioDeAreaAtuacaoDisciplina>()
                       .CacheBy(InstanceScope.PerRequest)
                       .TheDefaultIsConcreteType<RepositorioDeAreaAtuacaoDisciplina>();

            ForRequestedType<IRepositorioDeEstrategiaEnsino>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEstrategiaEnsino>();

            ForRequestedType<IRepositorioDeTipoAvaliacaoDisciplina>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoAvaliacaoDisciplina>();

            ForRequestedType<IRepositorioDeTipoDisciplina>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoDisciplina>();

            ForRequestedType<IRepositorioDeProfissao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeProfissao>();

            ForRequestedType<IRepositorioDeReligiao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeReligiao>();

            ForRequestedType<IRepositorioDeTipoParentesco>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeTipoParentesco>();

            ForRequestedType<IRepositorioDeEmpresaCarteira>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEmpresaCarteira>();

            ForRequestedType<IRepositorioDeCurriculoDisciplina>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCurriculoDisciplina>();


            ForRequestedType<IRepositoriovEmergenciaPEP>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoriovEmergenciaPEP>();


            ForRequestedType<IRepositorioDeAltaDestino>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAltaDestino>();

            ForRequestedType<IRepositorioDeSigaProblemaLog>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeSigaProblemaLog>();

            ForRequestedType<IRepositoriovPacienteInternado>()
                         .CacheBy(InstanceScope.PerRequest)
                         .TheDefaultIsConcreteType<RepositoriovPacienteInternado>();

            ForRequestedType<IRepositorioDeAnestesiaTipo>()
                         .CacheBy(InstanceScope.PerRequest)
                         .TheDefaultIsConcreteType<RepositorioDeAnestesiaTipo>();

            ForRequestedType<IRepositorioDeEventoAnestesia>()
                         .CacheBy(InstanceScope.PerRequest)
                         .TheDefaultIsConcreteType<RepositorioDeEventoAnestesia>();

            ForRequestedType<IRepositorioDeEventoAvaliacaoMedica>()
                         .CacheBy(InstanceScope.PerRequest)
                         .TheDefaultIsConcreteType<RepositorioDeEventoAvaliacaoMedica>();

            ForRequestedType<IRepositorioDeSumarioAvaliacaoPreAnestesica>()
                         .CacheBy(InstanceScope.PerRequest)
                         .TheDefaultIsConcreteType<RepositorioDeSumarioAvaliacaoPreAnestesica>();

            ForRequestedType<IRepositorioDeAtividade>()
                 .CacheBy(InstanceScope.PerRequest)
                 .TheDefaultIsConcreteType<RepositorioDeAtividade>();

            ForRequestedType<IRepositorioDePacientePreCadastro>()
                 .CacheBy(InstanceScope.PerRequest)
                 .TheDefaultIsConcreteType<RepositorioDePacientePreCadastro>();

            ForRequestedType<IRepositorioDePacientePreCadastroWeb>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePacientePreCadastroWeb>();
            
            ForRequestedType<IRepositorioDePacientePCI>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDePacientePCI>();

            ForRequestedType<IRepositorioDeEnderecosConsult>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEnderecosConsult>();

            ForRequestedType<IRepositorioMotivoItem>()
                 .CacheBy(InstanceScope.PerRequest)
                 .TheDefaultIsConcreteType<RepositorioMotivoItem>();

            ForRequestedType<IRepositorioMediaPonderadaSetorCiclo>()
                   .CacheBy(InstanceScope.PerRequest)
                   .TheDefaultIsConcreteType<RepositorioMediaPonderadaSetorCiclo>();

            ForRequestedType<IRepositorioDeTipo>()
                 .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipo>();

            ForRequestedType<IRepositorioDeParecer>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeParecer>();

            ForRequestedType<IRepositorioDeTipoAnexo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoAnexo>();

            ForRequestedType<IRepositorioDeSolicitacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacao>();

            ForRequestedType<IRepositorioDeEventoAdmissaoAssistencial>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoAdmissaoAssistencial>();

            ForRequestedType<IRepositorioDeEventoAdmissaoAssistencialEndoscopia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoAdmissaoAssistencialEndoscopia>();

            ForRequestedType<IRepositorioDeControleEntrega>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeControleEntrega>();

            ForRequestedType<IRepositorioDeNotaFiscal>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeNotaFiscal>();

            ForRequestedType<IRepositorioDeRPAcomISSQN>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPAcomISSQN>();

            ForRequestedType<IRepositorioDeRPAsemISSQN>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPAsemISSQN>();

            ForRequestedType<IRepositorioDeContraCheque>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeContraCheque>();

            ForRequestedType<IRepositorioDeFormaPagto>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeFormaPagto>();

            ForRequestedType<IRepositorioDeCheckList>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCheckList>();

            ForRequestedType<IRepositorioDeUsuario>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuario>();

            ForRequestedType<IRepositorioDeRecurso>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRecurso>();

            ForRequestedType<IRepositorioDeSiga_Profissional>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSiga_Profissional>();

            ForRequestedType<IRepositorioDePagamentoAutorizacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePagamentoAutorizacao>();

            ForRequestedType<IRepositorioDeFormacaoValor>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeFormacaoValor>();

            ForRequestedType<IRepositorioDeEventoCheckListCirurgia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoCheckListCirurgia>();

            ForRequestedType<IRepositorioDeCalculoPagamentos>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCalculoPagamentos>();

            ForRequestedType<IRepositorioDePagamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePagamento>();

            ForRequestedType<IRepositorioDeLoteNotaFiscal>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeLoteNotaFiscal>();

            ForRequestedType<IRepositorioDeLoteRPAComISSQN>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeLoteRPAComISSQN>();

            ForRequestedType<IRepositorioDeLoteRPASemISSQN>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeLoteRPASemISSQN>();

            ForRequestedType<IRepositoriovEmergenciaPAME>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositoriovEmergenciaPAME>();

            ForRequestedType<IRepositorioDePrestadorRelatorioDesempenho>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadorRelatorioDesempenho>();

            ForRequestedType<IRepositorioDeListaDeProblemaAdmissao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeListaDeProblemaAdmissao>();

            ForRequestedType<IRepositorioDeAlergia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAlergia>();

            ForRequestedType<IRepositorioDeEvento>()
                 .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEvento>();

            ForRequestedType<IRepositorioDeEventoMedicamentosEmUso>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoMedicamentosEmUso>();

            ForRequestedType<IRepositorioDeEventoAlergias>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoAlergias>();

            ForRequestedType<IRepositorioDeDREGrupo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeDREGrupo>();

            ForRequestedType<IRepositorioDeDREOperacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeDREOperacao>();

            ForRequestedType<IRepositorioDeEspecie>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEspecie>();

            ForRequestedType<IRepositorioDeParecer>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeParecer>();

            ForRequestedType<IRepositorioDeAtivoGrupo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAtivoGrupo>();

            #region ---------- RN ----------
            ForRequestedType<IRepositorioDeSumarioDeAvaliacaoMedicaRN>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSumarioDeAvaliacaoMedicaRN>();

            ForRequestedType<IRepositorioDeEventoSumarioAvaliacaoMedicaRN>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoSumarioAvaliacaoMedicaRN>();
            #endregion

            #region ---------- RPC ----------
            ForRequestedType<IRepositorioDeRPCSetor>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCSetor>();

            ForRequestedType<IRepositorioDeRPCEspecialidade>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCEspecialidade>();

            ForRequestedType<IRepositorioDeRPCCargo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCCargo>();

            ForRequestedType<IRepositorioDeRPCProfissao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCProfissao>();

            ForRequestedType<IRepositorioDeRPCCurso>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCCurso>();

            ForRequestedType<IRepositorioDeRPCTipoCurso>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCTipoCurso>();

            ForRequestedType<IRepositorioDeRPCTipoAnaisEvento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCTipoAnaisEvento>();

            ForRequestedType<IRepositorioDeRPCAbrangenciaEvento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCAbrangenciaEvento>();

            ForRequestedType<IRepositorioDeRPCClassificacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCClassificacao>();

            ForRequestedType<IRepositorioDeRPCTipoApresentacao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCTipoApresentacao>();

            ForRequestedType<IRepositorioDeRPCIdioma>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCIdioma>();

            ForRequestedType<IRepositorioDeRPCDefesaTipo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCDefesaTipo>();

            ForRequestedType<IRepositorioDeRPCBaseCientifica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCBaseCientifica>();

            ForRequestedType<IRepositorioDeRPCMotivo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRPCMotivo>();

            ForRequestedType<IRepositorioDeRPCPessoa>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeRPCPessoa>();

            ForRequestedType<IRepositorioDeRPCPessoaSetor>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeRPCPessoaSetor>();

            ForRequestedType<IRepositorioDeRPCArea>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<RepositorioDeRPCArea>();

            ForRequestedType<IRepositorioDeRPCProducao>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<RepositorioDeRPCProducao>();

            #endregion

            #region ---------- CheckUp ----------

            ForRequestedType<IRepositorioDeAgendaCheckUp>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<RepositorioDeAgendaCheckUp>();

            ForRequestedType<IRepositorioDeLaboratorio>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<RepositorioDeLaboratorio>();

            ForRequestedType<IRepositorioDeExameCheckUp>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeExameCheckUp>();

            ForRequestedType<IRepositorioDeGrupoCheckUp>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeGrupoCheckUp>();

            ForRequestedType<IRepositorioDeCheckUp>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeCheckUp>();

            ForRequestedType<IRepositorioDeEspecialistaCheckUp>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeEspecialistaCheckUp>();

            ForRequestedType<IRepositorioDeLaboratorioHorario>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeLaboratorioHorario>();

            ForRequestedType<IRepositorioDeDicionarioCheckUp>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeDicionarioCheckUp>();

            ForRequestedType<IRepositorioDeDicionarioGrupoCheckUp>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeDicionarioGrupoCheckUp>();

            ForRequestedType<IRepositorioDeRiscoFraminghamParametros>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeRiscoFraminghamParametros>();


            ForRequestedType<IRepositorioDevPacienteAgendaCheckUp>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDevPacienteAgendaCheckUp>();

            ForRequestedType<IRepositorioDeAgendaPacienteCheckUp>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAgendaPacienteCheckUp>();

            ForRequestedType<IRepositorioDeEventoCheckup>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoCheckup>();

            ForRequestedType<IRepositorioDeAgendamentoPacienteCheckUp>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAgendamentoPacienteCheckUp>();

            ForRequestedType<IRepositorioDeContatoPaciente>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeContatoPaciente>();

            ForRequestedType<IRepositorioDeCheckUpPasta>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeCheckUpPasta>();

            ForRequestedType<IRepositorioDeProntuario>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeProntuario>();

            #endregion

            #region ---------- EAD ----------

            ForRequestedType<IRepositorioDeCategoriaTreinamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCategoriaTreinamento>();

            ForRequestedType<IRepositorioDePublicoAlvo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePublicoAlvo>();

            ForRequestedType<IRepositorioDeTreinamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTreinamento>();

            ForRequestedType<IRepositoriovUGBSetorCargo>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<RepositoriovUGBSetorCargo>();

            ForRequestedType<IRepositoriovUsuarioUgbSetorCargo>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<RepositoriovUsuarioUgbSetorCargo>();

            ForRequestedType<IRepositoriovLiderancaUsuario>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<RepositoriovLiderancaUsuario>();

            ForRequestedType<IRepositorioDeUsuarioCargo>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<RepositorioDeUsuarioCargo>();

            ForRequestedType<IRepositorioDeUsuarioLog>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<RepositorioDeUsuarioLog>();

            ForRequestedType<IRepositorioDeAvaliacao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAvaliacao>();

            ForRequestedType<IRepositorioDePerguntaEAD>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePerguntaEAD>();

            ForRequestedType<IRepositorioDePerguntaEAD>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePerguntaEAD>();

            ForRequestedType<IRepositorioDeMaterialTreinamento>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeMaterialTreinamento>();

            ForRequestedType<IRepositorioDeTipoTreinamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoTreinamento>();

            ForRequestedType<IRepositorioDeOpiniao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeOpiniao>();

            ForRequestedType<IRepositorioDeScormTreinamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeScormTreinamento>();

            ForRequestedType<IRepositorioDeLideranca>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeLideranca>();

            ForRequestedType<IRepositorioDeImagemTreinamento>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeImagemTreinamento>();

            ForRequestedType<IRepositoriovUsuarioTreinamento>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoriovUsuarioTreinamento>();

            ForRequestedType<IRepositoriovAvaliacaoEAD>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoriovAvaliacaoEAD>();

            #endregion

            #region ----------EAD CC-----------

            ForRequestedType<EADCCRep.IRepositorioDeAvaliacaoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDeAvaliacaoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeCategoriaTreinamentoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDeCategoriaTreinamentoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeImagemTreinamentoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDeImagemTreinamentoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeMaterialTreinamentoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDeMaterialTreinamentoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeOpiniaoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDeOpiniaoCC>();

            ForRequestedType<EADCCRep.IRepositorioDePerguntaCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDePerguntaCC>();

            ForRequestedType<EADCCRep.IRepositorioDePublicoAlvoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDePublicoAlvoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeScormTreinamentoCC>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<EADCCNH.RepositorioDeScormTreinamentoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeTreinamentoCC>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<EADCCNH.RepositorioDeTreinamentoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeComentarioTreinamentoCC>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<EADCCNH.RepositorioDeComentarioTreinamentoCC>();

            ForRequestedType<EADCCRep.IRepositorioDeUsuarioAceiteCC>()
             .CacheBy(InstanceScope.PerRequest)
             .TheDefaultIsConcreteType<EADCCNH.RepositorioDeUsuarioAceiteCC>();

            #endregion

            #region ---------- ADMISSAO ASSISTENCIAL ENDOSCOPIA ----------

            ForRequestedType<IRepositorioDeItemEndoscopia>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeItemEndoscopia>();

            ForRequestedType<IRepositorioDeAdmissaoAssistencialEndoscopia>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAdmissaoAssistencialEndoscopia>();


            ForRequestedType<IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSumarioDeAvaliacaoMedicaEndoscopia>();

            ForRequestedType<IRepositorioDeEventoSumarioAvaliacaoEndoscopia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoSumarioAvaliacaoEndoscopia>();

            #endregion

            #region ---------- Centro Obstetrico ----------

            ForRequestedType<IRepositorioDeAdmissaoAssistencialCO>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAdmissaoAssistencialCO>();

            ForRequestedType<IRepositorioDeItensCO>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeItensCO>();

            ForRequestedType<IRepositorioDeEventoAdmissaoAssistencialCO>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEventoAdmissaoAssistencialCO>();

            ForRequestedType<IRepositorioDeSumarioAvaliacaoMedicaCO>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeSumarioAvaliacaoMedicaCO>();

            ForRequestedType<IRepositorioDeEventoSumarioAvaliacaoMedicaCO>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeEventoSumarioAvaliacaoMedicaCO>();

            #endregion

            #region ---------- Sumario CTI NEO ----------
            ForRequestedType<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSumarioAvaliacaoMedicaCTINEO>();
            #endregion

            #region ---------- AdmissaoAssistencial CTI NEO ----------
            ForRequestedType<IRepositorioDeProcedenciaCTINEO>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeProcedenciaCTINEO>();

            ForRequestedType<IRepositorioDeAdmissaoAssistencialCTINEO>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAdmissaoAssistencialCTINEO>();
            #endregion

            ForRequestedType<IRepositorioDeProntuario>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeProntuario>();

            ForRequestedType<IRepositorioDeProcedimentoCbhpm>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeProcedimentoCbhpm>();

            ForRequestedType<IRepositorioDeHMVMessengerTipo>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeHMVMessengerTipo>();

            ForRequestedType<IRepositorioDeHMVMessenger>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeHMVMessenger>();

            ForRequestedType<IRepositorioDeSMSPrestadorExcecao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeSMSPrestadorExcecao>();

            ForRequestedType<IRepositorioDeUsuarioAceite>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuarioAceite>();

            ForRequestedType<IRepositorioDePrestadoreResumido>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestadoreResumido>();

            ForRequestedType<IRepositoriovAtendimentoGED>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositoriovAtendimentoGED>();

            ForRequestedType<HMV.Core.Domain.Repository.IRepositorioDoProntuario>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<HMV.Core.NH.Repository.RepositorioDoProntuario>();

            ForRequestedType<IRepositorioDeAtivoEspecie>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAtivoEspecie>();

            ForRequestedType<IRepositorioDeCidMobile>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCidMobile>();

            ForRequestedType<IRepositorioDeTussMobile>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTussMobile>();

            ForRequestedType<IRepositorioDeMotivoCancelamentoDebito>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeMotivoCancelamentoDebito>();

            ForRequestedType<IRepositorioDeDadosDoBoleto>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeDadosDoBoleto>();

            ForRequestedType<IRepositorioDeProjeto>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeProjeto>();

            ForRequestedType<IRepositorioDeAvaliacaoRiscoOrigemTempo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoOrigemTempo>();

            ForRequestedType<Core.Domain.Repository.AtendimentoPaciente.IRepositorioDeProcedimento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<Core.NH.Repository.AtendimentoPaciente.RepositorioDeProcedimento>();
            ForRequestedType<IRepositorioCertificadoPresencial>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioCertificadoPresencial>();

            ForRequestedType<IRepositorioDeSolicitacaoStatusCompras>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacaoStatusCompras>();

            ForRequestedType<IRepositorioDeMatriculaDisciplina>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeMatriculaDisciplina>();

            ForRequestedType<IRepositorioDePrestador>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrestador>();

            ForRequestedType<IRepositorioDeLegendaHistoricoAluno>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeLegendaHistoricoAluno>();

            ForRequestedType<IRepositorioDeOrdemCompra>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeOrdemCompra>();

            ForRequestedType<IRepositorioDeSeniorFuncionario>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSeniorFuncionario>();

            ForRequestedType<IRepositorioDeSeniorCursoAperfeicoamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSeniorCursoAperfeicoamento>();

            ForRequestedType<IRepositorioDeSeniorTurma>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSeniorTurma>();

            ForRequestedType<IRepositorioDeSeniorParticipante>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSeniorParticipante>();

            ForRequestedType<IRepositorioDeSeniorParticipanteHistorico>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSeniorParticipanteHistorico>();

            ForRequestedType<IRepositorioDeSeniorParticipanteHistorico>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSeniorParticipanteHistorico>();

            ForRequestedType<IRepositorioDeCheckListUDI>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeCheckListUDI>();

            ForRequestedType<IRepositorioDeVeiculo>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeVeiculo>();

            ForRequestedType<IRepositorioDeTipoIsolamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoIsolamento>();

            ForRequestedType<IRepositorioDeSolicitacaoDeEquipamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacaoDeEquipamento>();

            ForRequestedType<IRepositorioDeSolicitacaoDePaciente>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacaoDePaciente>();

            ForRequestedType<IRepositorioDeUsuarioFila>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuarioFila>();

            ForRequestedType<IRepositorioDeUsuarioTransporte>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuarioTransporte>();

            ForRequestedType<IRepositorioDeMotivoCancelamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeMotivoCancelamento>();

            ForRequestedType<IRepositorioDeSetorTransporte>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSetorTransporte>();

            #region CallCenter

            ForRequestedType<HMV.Core.Domain.Repository.CallCenter.IRepositorioDeExameCallCenter>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<HMV.Core.NH.Repository.CallCenter.RepositorioDeExameCallCenter>();

            ForRequestedType<HMV.Core.Domain.Repository.CallCenter.IRepositorioDeInformativos>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<HMV.Core.NH.Repository.CallCenter.RepositorioDeInformativos>();

            ForRequestedType<HMV.Core.Domain.Repository.CallCenter.IRepositorioDeContatoCallCenter>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<HMV.Core.NH.Repository.CallCenter.RepositorioDeContatoCallCenter>();

            ForRequestedType<HMV.Core.Domain.Repository.CallCenter.IRepositorioDeDetalhesMedicos>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<HMV.Core.NH.Repository.CallCenter.RepositorioDeDetalhesMedicos>();


            #endregion CallCenter

            #region CertificadoTecnico

            ForRequestedType<CTRep.IRepositorioDeInstituicao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<CTNH.RepositorioDeInstituicao>();

            ForRequestedType<CTRep.IRepositorioDeAssunto>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<CTNH.RepositorioDeAssunto>();

            ForRequestedType<CTRep.IRepositorioDeSubassunto>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<CTNH.RepositorioDeSubassunto>();

            ForRequestedType<CTRep.IRepositorioDeTipo>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<CTNH.RepositorioDeTipo>();

            ForRequestedType<CTRep.IRepositorioDeAgrupador>()
              .CacheBy(InstanceScope.PerRequest)
              .TheDefaultIsConcreteType<CTNH.RepositorioDeAgrupador>();

            ForRequestedType<CTRep.IRepositorioDeSetor>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<CTNH.RepositorioDeSetor>();

            ForRequestedType<CTRep.IRepositorioDeCargo>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<CTNH.RepositorioDeCargo>();

            ForRequestedType<CTRep.IRepositorioDeQuestao>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<CTNH.RepositorioDeQuestao>();

            ForRequestedType<CTRep.IRepositorioDeResposta>()
          .CacheBy(InstanceScope.PerRequest)
          .TheDefaultIsConcreteType<CTNH.RepositorioDeResposta>();

            ForRequestedType<CTRep.IRepositorioDeProva>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<CTNH.RepositorioDeProva>();

            ForRequestedType<CTRep.IRepositorioDeCertificacao>()
           .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<CTNH.RepositorioDeCertificacao>();

            ForRequestedType<CTRep.IRepositorioDeProvaCertificacao>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<CTNH.RepositorioDeProvaCertificacao>();

            ForRequestedType<CTRep.IRepositorioDeProvaQuestao>()
         .CacheBy(InstanceScope.PerRequest)
         .TheDefaultIsConcreteType<CTNH.RepositorioDeProvaQuestao>();

            ForRequestedType<CTRep.IRepositorioDeColaborador>()
       .CacheBy(InstanceScope.PerRequest)
       .TheDefaultIsConcreteType<CTNH.RepositorioDeColaborador>();

            ForRequestedType<CTRep.IRepositorioDeCertificacaoColaborado>()
       .CacheBy(InstanceScope.PerRequest)
       .TheDefaultIsConcreteType<CTNH.RepositorioDeCertificacaoColaborado>();

            ForRequestedType<CTRep.IRepositorioDeJustificativa>()
    .CacheBy(InstanceScope.PerRequest)
    .TheDefaultIsConcreteType<CTNH.RepositorioDeJustificativa>();

            ForRequestedType<CTRep.IRepositorioDeAvaliacao>()
     .CacheBy(InstanceScope.PerRequest)
     .TheDefaultIsConcreteType<CTNH.RepositorioDeAvaliacao>();

            ForRequestedType<CTRep.IRepositorioDeAvaliacaoNota>()
   .CacheBy(InstanceScope.PerRequest)
   .TheDefaultIsConcreteType<CTNH.RepositorioDeAvaliacaoNota>();

            #endregion

            ForRequestedType<IRepositorioDeSCIJustificativa>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSCIJustificativa>();

            ForRequestedType<IRepositorioDeSCIParecer>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSCIParecer>();

            ForRequestedType<IRepositorioDePrescricao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrescricao>();

            ForRequestedType<IRepositorioDeSCIAntimicrobiano>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSCIAntimicrobiano>();


            #region Avaliacao de Risco
            ForRequestedType<IRepositorioDeAvaliacaoRiscoPerguntaUlceraPressao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoPerguntaUlceraPressao>();

            ForRequestedType<IRepositorioDeAvaliacaoRiscoPerguntaQueda>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoPerguntaQueda>();

            ForRequestedType<IRepositorioDeAvaliacaoRiscoPerguntaTriagem>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoPerguntaTriagem>();

            ForRequestedType<IRepositorioDeAvaliacaoRiscoPerguntaRiscoPsicologico>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoPerguntaRiscoPsicologico>();

            ForRequestedType<IRepositorioDeAvaliacaoRiscoPerguntaRiscoSocial>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeAvaliacaoRiscoPerguntaRiscoSocial>();
            #endregion


            ForRequestedType<IRepositorioDeDiagnostico>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeDiagnostico>();

            ForRequestedType<IRepositorioDeEvidencia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEvidencia>();

            ForRequestedType<IRepositorioDeRelacionado>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeRelacionado>();

            ForRequestedType<IRepositorioDePrePaciente>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDePrePaciente>();

            ForRequestedType<IRepositorioDeItemPrescricao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeItemPrescricao>();

            ForRequestedType<IRepositorioDePassagemPlantaoLocalPaciente>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePassagemPlantaoLocalPaciente>();

            ForRequestedType<IRepositorioDePassagemPlantaoLocalizacao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePassagemPlantaoLocalizacao>();

            ForRequestedType<IRepositorioDePassagemPlantaoEmergencia>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePassagemPlantaoEmergencia>();

            ForRequestedType<IRepositorioDeTipoAcomodacao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeTipoAcomodacao>();

            ForRequestedType<IRepositorioDeTermoNaoCoberturaConvenio>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeTermoNaoCoberturaConvenio>();

            ForRequestedType<IRepositorioDeGrupoTipoEsquema>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeGrupoTipoEsquema>();

            ForRequestedType<IRepositorioDeProibicaoConvenio>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeProibicaoConvenio>();

            ForRequestedType<IRepositorioDeSCIAntimicrobianoControle>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSCIAntimicrobianoControle>();

            ForRequestedType<IRepositorioDeReceituario>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeReceituario>();

            ForRequestedType<IRepositorioDeReceituarioPadrao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeReceituarioPadrao>();
            
            #region AvaliacaoMobilidade

            ForRequestedType<IRepositoriodePacienteAvaliacaoMobilidade>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositoriodePacienteAvaliacaoMobilidade>();

            ForRequestedType<IRepositorioDeMobilidadeCiclos>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeMobilidadeCiclos>();

            ForRequestedType<IRepositorioDeAvaliacaoMobilidade>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAvaliacaoMobilidade>();

            ForRequestedType<IRepositorioDeAvaliacaoMobilidadeRelatorio>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeAvaliacaoMobilidadeRelatorio>();

            #endregion

            #region ExamesMonitoramento
            ForRequestedType<IRepositorioDeExameImagem>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeExameImagem>();

            ForRequestedType<IRepositorioDeLocalExame>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeLocalExame>();

            ForRequestedType<IRepositorioDePedidoExame>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePedidoExame>();

            ForRequestedType<IRepositorioDePreparoExame>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePreparoExame>();

            ForRequestedType<IRepositorioDeSetorExame>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSetorExame>();

            ForRequestedType<IRepositorioDeSolicitacaoExame>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeSolicitacaoExame>();

            ForRequestedType<IRepositorioDeTurnoExame>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDeTurnoExame>();

            ForRequestedType<IRepositorioDePassagemPlantaoEmergenciaObservacao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePassagemPlantaoEmergenciaObservacao>();
            #endregion

            #region PreAgendPo

            ForRequestedType<IRepositorioDePeriodoPo>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePeriodoPo>();

            ForRequestedType<IRepositorioDePreAgendaPortalPo>()
           .CacheBy(InstanceScope.PerRequest)
           .TheDefaultIsConcreteType<RepositorioDePreAgendaPortalPo>();

            #endregion PreAgendPo

            ForRequestedType<IRepositorioDePEPEvolucao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePEPEvolucao>();

            ForRequestedType<IRepositorioDePEPEvolucaoPadrao>()
            .CacheBy(InstanceScope.PerRequest)
            .TheDefaultIsConcreteType<RepositorioDePEPEvolucaoPadrao>();

            ForRequestedType<IRepositorioDeTransferenciaDeEnfermagem>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTransferenciaDeEnfermagem>();

            ForRequestedType<IRepositorioDeTransferenciaDeNutricao>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTransferenciaDeNutricao>();

            ForRequestedType<IRepositorioDeTransferenciaDeFisioterapia>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTransferenciaDeFisioterapia>();

            ForRequestedType<IRepositorioDeNivelMobilidade>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeNivelMobilidade>();

            ForRequestedType<IRepositorioDevPrestadorSite>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDevPrestadorSite>();

            ForRequestedType<IRepositorioDevPacienteSite>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDevPacienteSite>();

            ForRequestedType<IRepositorioDeServiceSite>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeServiceSite>();

            ForRequestedType<IRepositorioDeAdmissaoAssistencialUrodinamica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAdmissaoAssistencialUrodinamica>();

            ForRequestedType<IRepositorioDeItemUrodinamica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeItemUrodinamica>();

            ForRequestedType<IRepositorioDeEventoAdmissaoAssistencialUrodinamica>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeEventoAdmissaoAssistencialUrodinamica>();
            ForRequestedType<IRepositorioDeFarJustificativa>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeFarJustificativa>();


            ForRequestedType<IRepositorioDeFarAnticoagulanteItem>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeFarAnticoagulanteItem>();

            ForRequestedType<IRepositorioDePrevAltaMotivo>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDePrevAltaMotivo>();

            ForRequestedType<IRepositorioDePrevisaoAlta>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDePrevisaoAlta>();
            
            ForRequestedType<IRepositorioDeMedicamentoEmUsoProntuario>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeMedicamentoEmUsoProntuario>();

            ForRequestedType<IRepositorioDeExameFisicoEvento>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeExameFisicoEvento>();
        
        }
    }
}
