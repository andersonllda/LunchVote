using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System.Linq;
using HMV.Core.Domain.Repository;
using System.Collections.Generic;

namespace HMV.PEP.Test
{
    [TestClass]
    public class AtendimentoTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
            AmbulatorioTest2.preparar_dados_para_teste();
        }
     
        [TestMethod]
        public void busca_atendimento_por_id()
        {           
            IAtendimentoService srv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = srv.FiltraPorID(5);
            //ate.Convenio = null;
            //srv.Save(ate);
            Assert.IsNotNull(ate);
        }


        [TestMethod]
        public void busca_Sumario_do_atendimento()
        {
            IAtendimentoService srv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = srv.FiltraPorID(2320050);

            SumarioAvaliacaoMedica sumario = ate.SumarioAvaliacaoMedica;     

            Assert.IsNotNull(sumario);
        }



        [TestMethod]
        public void insere_restricao_atendimento()
        {
            IAtendimentoService srv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = srv.FiltraPorID(2320050);

            int cout = 0;
            if (ate.VisitanteRestricao != null)
                cout = ate.VisitanteRestricao.Count;

            HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem.VisitanteRestricao newVisitanteRestricao = new Core.Domain.Model.PEP.ProcessoDeEnfermagem.VisitanteRestricao(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            newVisitanteRestricao.Observacao = "XXXX";
            ate.VisitanteRestricao.Add(newVisitanteRestricao);

            srv.Save(ate);

            IAtendimentoService srv2 = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate2 = srv2.FiltraPorID(2320050);
            
            Assert.AreEqual(ate2.VisitanteRestricao.Count, cout + 1);
        }

        [TestMethod]
        public void busca_somente_os_atendimento_com_registro_de_dor_para_o_paciente_9127186() {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.OndeCodigoPacienteIgual(9610064);
            IList<Atendimento> lista = rep.OndeExisteRegistroDeDor().List();
            Assert.AreEqual(lista.Count(x=>x.RegistroDor.Count == 0), 0);
        }

    }
}
