using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository;
using HMV.Core.Interfaces;
using HMV.Core.Framework.Types;
using HMV.Core.Domain.Views.PEP;
using HMV.PEP.Services;
using HMV.PEP.DTO;

namespace HMV.PEP.Test
{
    [TestClass]
    public class InicialEmergenciaTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
            PrepareTests();

        }

        private static void PrepareTests()
        {
            IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            repAtend.OndeCodigoAtendimentoIgual(2655189);
            Atendimento atend = repAtend.Single();
            atend.DataAtendimento = DateTime.Now.Date;
            repAtend.Save(atend);

            repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            repAtend.OndeCodigoAtendimentoIgual(2655194);
            Atendimento atend2 = repAtend.Single();
            atend2.DataAtendimento = DateTime.Now.Date;
            repAtend.Save(atend2);
        }

        [TestMethod]
        public void quando_pesquisar_pacientes_na_emergencia_deve_retornar_uma_lista_de_Atendimentos_com_IsEmergencia_TRUE()
        {
            IEmergenciaService srvEmerg = ObjectFactory.GetInstance<IEmergenciaService>();
            IList<Atendimento> lstAtend = srvEmerg.BuscaAtendimentos();

            Assert.IsTrue(lstAtend.Count > 0);
            Assert.AreNotEqual(lstAtend[0].Convenio.Id, 2);
            Assert.IsNotNull(lstAtend[0].Prestador);
            Assert.AreEqual(lstAtend[0].TipoDeAtendimento, TipoAtendimento.Urgencia);
            Assert.IsTrue(lstAtend[0].DataAtendimento >= DateTime.Now.Date.AddDays(-2));
        }

        [TestMethod]
        public void quando_pesquisa_pacientes_internados_na_emergencia_deve_retornar_uma_lista_de_atendimento_com_unidade_internacao_igual_ao_parametro()
        {
            IEmergenciaService srvEmerg = ObjectFactory.GetInstance<IEmergenciaService>();
            IList<vPacienteInternado> lstAtend = srvEmerg.BuscaAtendimentosInternados();
            IClinicaService clinServ = ObjectFactory.GetInstance<IClinicaService>();
            Parametro param = clinServ.FiltraPorID(11).getParametroSetoresDaEmergencia();
            IList<string> lstParam = param.Valor.Split(',');

            Assert.IsTrue(lstParam.Contains(lstAtend[0].IDUnidade.ToString()));
        }

        [TestMethod]
        public void verifica_performance_consulta_emergencias()
        {          
            IEmergenciaService emerServ = ObjectFactory.GetInstance<IEmergenciaService>();
            IList<Atendimento> lstAtend = emerServ.BuscaAtendimentos();
            DateTime? dateSample = null;
            var atendEmerg = (from x in lstAtend
                             select new
                             {
                                 TipoPaciente = Enum<TipoPaciente>.GetDescriptionOf(x.Paciente.TipoDoPaciente),
                                 Paciente = x.Paciente.Nome,
                                 Prontuario = x.Paciente.ID,
                                 DtAdmissao = x.DataAtendimento,
                                 Admissao = x.ID,
                                 DataClassif = (x.UltimoBoletimDeEmergencia != null ? x.UltimoBoletimDeEmergencia.DataInclusao : dateSample),
                                 //Classif = (x.UltimoBoletimDeEmergencia.Classificacoes.ToString() != "") ? x.UltimoBoletimDeEmergencia.Classificacoes.OrderByDescending(y => y.DataInclusao).FirstOrDefault().Cor.CodigoCor : null,
                                 Classif = (x.UltimoBoletimDeEmergencia != null && x.UltimoBoletimDeEmergencia.Classificacoes.ToString() != "") ? (x.UltimoBoletimDeEmergencia.Classificacoes.OrderByDescending(y => y.DataInclusao).FirstOrDefault().Cor.CodigoCor.ToString()) : (""),
                                 Ordem = (x.UltimoBoletimDeEmergencia != null && x.UltimoBoletimDeEmergencia.Classificacoes.ToString() != "") ? x.UltimoBoletimDeEmergencia.Classificacoes.OrderByDescending(y => y.DataInclusao).FirstOrDefault().Cor.Ordem : 10

                             });
            Assert.IsTrue(atendEmerg.OrderBy(x => x.Ordem).Count() > 0);
        }

        [TestMethod]
        public void busca_pacientes_obstetricos_nova_aba_emergencia()
        {
            IRepositoriovPacienteInternado rep = ObjectFactory.GetInstance<IRepositoriovPacienteInternado>();
            rep.InternadoNaEmergencia();
            IList<vPacienteInternado> lista = new List<vPacienteInternado>();
            rep.OndePacienteObstetrico();
            lista = rep.List();

            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_pacientes_externos()
        {
          /*  InternacaoService serv = ObjectFactory.GetInstance<InternacaoService>();
            IList<PacienteExternoDTO> ret = serv.ListaPacientesExternos(0, string.Empty, 0);

            Assert.IsTrue(ret.Count > 0);
            */
            IInternacaoService serv2 = ObjectFactory.GetInstance<IInternacaoService>();
            IList<UnidadeInternacaoDTO> lista = serv2.ListaSetoresPacientesExternos();

        }

      


    }
}
