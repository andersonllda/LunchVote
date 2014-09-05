using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Consult;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Views.PEP;

namespace HMV.PEP.Test
{
    [TestClass]
    public class InternacaoTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }
     
        [TestMethod]
        public void busca_pacientes_internados_por_nome_paciente_ADAO_DOMINGUES_DIOGO()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();

            IList<vPacienteInternado> ate = srv.ListaPacientesInternados(0, "ADAO DOMINGUES DIOGO", 0, 0);

            Assert.AreEqual(ate.Count, 1);
        }  

        [TestMethod]
        public void busca_pacientes_internados_Onde_Unidade_Internacao_B2()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();

            IList<vPacienteInternado> ate = srv.ListaPacientesInternados(0, string.Empty, 3, 0);
            Assert.IsTrue(ate.Count > 0);
        }
      
        [TestMethod]
        public void busca_pacientes_internados_DTO()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();

            IList<vPacienteInternado> ate = srv.ListaPacientesInternados(0, string.Empty, 0, 0);
            Assert.IsTrue(ate.Count > 0);
        }

        [TestMethod]
        public void busca_medicos_com_pacientes_internados_DTO()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();

            IList<MedicoAssistenteDTO> ate = srv.ListaMedicosComPacientesInternados();
            Assert.AreNotEqual(ate.Count, 0);
        }     

        [TestMethod]
        public void busca_UnidadesInternacao_com_pacientes_internados_DTO()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();

            IList<UnidadeInternacaoDTO> ate = srv.ListaUnidadesInternacaoComPacientesInternados();
            Assert.IsTrue(ate.Count > 0);
        }

        [TestMethod]
        public void verifica_se_retornou_atendimento_do_servico_pacientes_internados_busca_por_ID_paciente_10775483()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();
            vPacienteInternado ate = srv.ListaPacientesInternados(10775483, string.Empty, 0, 0).FirstOrDefault();

            Assert.IsTrue(ate.Atendimento > 0);
        }

        [TestMethod]
        public void busca_atendimento_onde_id_igual_3282009()
        {
            IInternacaoService srv = ObjectFactory.GetInstance<IInternacaoService>();
            Atendimento ate = srv.FiltraPorId(3250022);
            Assert.IsNotNull(ate);
        }

        [TestMethod]
        public void busca_minha_lista_pacientes_prestador_3151()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(3151);
            Assert.IsTrue(list.Count > 0);            
        }


        [TestMethod]
        public void busca_minha_lista_pacientes_prestador_1585()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(1585);
            Assert.IsTrue(list.Count > 0);
        }


        [TestMethod]
        public void busca_minha_lista_pacientes_prestador_2151_onde_existe_paciente_ambulatorial()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(2151);
            Assert.IsTrue(list.Count(x => x.TipoAtendimento.Equals("Ambulatorial")) > 0);
        }

        [TestMethod]
        public void busca_minha_lista_pacientes_prestador_3151_buscando_dados_minha_equipe()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(3151);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void busca_minha_lista_pacientes_internado_prestador_4690()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(4690);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void testa_funcionalidade_servico_minha_lista_prestador_3151()
        {
            IMeusPacientesService srv = ObjectFactory.GetInstance<IMeusPacientesService>();
            IList<MinhaListaDTO> list = srv.ListaMeusPacientes(3151);
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void quando_buscar_lista_de_pacientes_do_prestador_3151_retornar_o_atendimento_na_MinhaListaDTO()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(3151);
            Assert.AreNotEqual(list.Count(x=>x.Atendimento > 0 ), 0);
        }

        [TestMethod]
        public void busca_pacientes_internados_vinculoweb()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("rep", rep);
            IListaPacientesInternadosConsult consult = ObjectFactory.GetInstance<IListaPacientesInternadosConsult>(args);
            var pacs = consult.ListaPacientesInternados();

            Assert.IsTrue(pacs.Count >= 90);
        }


        [TestMethod]
        public void busca_minha_lista_pacientes_prestador_1395()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            IRepositorioDeAtendimentoAmbulatorial repA = ObjectFactory.GetInstance<IRepositorioDeAtendimentoAmbulatorial>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("repInternado", rep);
            args.SetArg("repAmbulatorial", repA);

            IMinhaListaPacientesConsult srv = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>(args);
            IList<MinhaListaDTO> list = srv.MinhaListaPacientes(1395);
            Assert.IsTrue(list.Count > 0);
        }
    }
}
