using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System.Linq;
using HMV.Core.Domain.Repository;
using System.Collections;
using HMV.Core.DTO;
using System.Collections.Generic;

namespace HMV.PEP.Test
{
    [TestClass]
    public class PacienteTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_os_atendimento_paciente_10743023()
        {
            IPacienteService serv = ObjectFactory.GetInstance<IPacienteService>();
            Paciente paciente = serv.FiltraPorID(10743023);
            Assert.IsTrue(paciente.Atendimentos.Count > 0);
            Assert.AreEqual(paciente.Atendimentos.FirstOrDefault().Paciente.ID, paciente.ID);
        }

        [TestMethod]
        public void adiciona_imunizacao_detalhe()
        {
            IRepositorioDePacientes rep = ObjectFactory.GetInstance<IRepositorioDePacientes>();
            Paciente pac = rep.OndeCodigoIgual(10713739).Single();
            Imunizacao imu = pac.Imunizacoes.FirstOrDefault();

            IList<ImunizacaoDetalheDTO> lista = new List<ImunizacaoDetalheDTO>();
            lista.Add(new ImunizacaoDetalheDTO() { Ativo = true, Descricao = "TESTE", ID = 1});
            
            imu.ImunizacaoTodas = lista;
            
            rep.Save(pac);
        }
        
        [TestMethod]
        public void busca_cid_inexistente_da_lista_de_problema_do_paciente()
        {
            IRepositorioDePacientes rep = ObjectFactory.GetInstance<IRepositorioDePacientes>();
            Paciente pac = rep.OndeCodigoIgual(9791405).Single();
            IList<Cid> cids = new List<Cid>();
            foreach (var item in pac.ProblemasPaciente)
                cids.Add(item.CID);

            Assert.IsTrue(cids.Count > 0);

        }
    }
}
