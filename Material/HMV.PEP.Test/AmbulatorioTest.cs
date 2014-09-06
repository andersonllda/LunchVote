using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using HMV.PEP.DTO;
using HMV.Core.Domain.Repository;
using HMV.PEP.Consult;

namespace HMV.PEP.Test
{
    [TestClass]
    public class AmbulatorioTest2 : BaseTestClass
    {

        public static void preparar_dados_para_teste()
        {

            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2634876).Single();
            ate.DataAtendimento = DateTime.Now.Date;
            rep.Save(ate);

            rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            ate = rep.OndeCodigoAtendimentoIgual(2616611).Single();
            ate.DataAtendimento = DateTime.Now.Date;
            rep.Save(ate);

        }
        
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
            AmbulatorioTest2.preparar_dados_para_teste();
        }
     
        [TestMethod]
        public void busca_aviso_cirurgia_2616611_do_atendimento_2616611()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2616611).Single();

            IList<AvisoCirurgia> avisoCirurgias = ate.DescricaoCirurgica;
            Assert.IsNotNull(avisoCirurgias.Where(x => x.Id == 125820));
        }

        [TestMethod]
        public void busca_todas_as_cirurgias_do_atendimento_2616611()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2616611).Single();
            AvisoCirurgia avisoCirurgias = ate.DescricaoCirurgica.Where(x => x.Id == 125820).Single();
            Assert.AreEqual(avisoCirurgias.ProcedimentosCirurgicos.Count,2);
        }

        [TestMethod]
        public void busca_cirurgia_1755_do_atendimento_2616611()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2616611).Single();
            AvisoCirurgia avisoCirurgias = ate.DescricaoCirurgica.Where(x => x.Id == 125820).Single();
            Assert.IsNotNull(avisoCirurgias.ProcedimentosCirurgicos.Count(x => x.Cirurgia.cd_cirurgia == 1755));
        }

        [TestMethod]
        public void busca_atendimento_2616611_pelo_repositorio_de_atendimentos_ambulatoriais()
        {
            IRepositorioDeAtendimentoAmbulatorial rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2616611).Single();
            Assert.IsNotNull(ate);
        }

        [TestMethod]
        public void quando_buscar_pelo_servico_de_pacientes_ambulatorios_retona_lista_com_todos_os_pacinetes_ambulatoriais()
        {
            IRepositorioDeAtendimentoAmbulatorial rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("rep", rep);
            IListaPacienteAmbulatorialConsult consult = ObjectFactory.GetInstance<IListaPacienteAmbulatorialConsult>(args);
            IList<PacienteAmbulatorialDTO> amb = consult.ListaPacientes(0, String.Empty);
            Assert.IsTrue(amb.Count() > 0 );

        }

        [TestMethod]
        public void busca_todos_os_atendimentos_ambulatoriais()
        {
            IAmbulatorioService serv = ObjectFactory.GetInstance<IAmbulatorioService>();
            IList<PacienteAmbulatorialDTO> amb = serv.ListaPacientes(0, String.Empty);
            Assert.IsTrue(amb.Count > 0);
        }

        [TestMethod]
        public void busca_atendimentos_ambulatoriais_onde_nome_do_paciente_Igual_NEREU_MARTINELI()
        {
            IAmbulatorioService serv = ObjectFactory.GetInstance<IAmbulatorioService>();
            IList<PacienteAmbulatorialDTO> amb = serv.ListaPacientes(0, "NEREU MARTINELI");
            Assert.IsTrue(amb.Count == 1);
        }

        [TestMethod]
        public void busca_atendimentos_ambulatoriais_onde_id_do_paciente_Igual_10629122()
        {
            IAmbulatorioService serv = ObjectFactory.GetInstance<IAmbulatorioService>();
            IList<PacienteAmbulatorialDTO> amb = serv.ListaPacientes(10629122, String.Empty);
            Assert.IsTrue(amb.Count == 1);
        }

        [TestMethod]
        public void busca_atendimentos_ambulatoriais_onde_nome_do_paciente_Igual_NEREU_MARTINELI_e_id_igual_9884796()
        {
            IAmbulatorioService serv = ObjectFactory.GetInstance<IAmbulatorioService>();
            IList<PacienteAmbulatorialDTO> amb = serv.ListaPacientes(9884796, "NEREU MARTINELI");
            Assert.IsTrue(amb.Count == 1);
        }

        
        [TestMethod]
        public void quando_buscar_paciente_ambulatorial_com_id_9884796_o_procedimento_deve_ser_null()
        {
            IAmbulatorioService serv = ObjectFactory.GetInstance<IAmbulatorioService>();
            IList<PacienteAmbulatorialDTO> amb = serv.ListaPacientes(9884796, string.Empty);
            Assert.AreEqual(amb.Count(),1);
            Assert.IsNull(amb[0].Procedimento);
        }
      

    }
}
