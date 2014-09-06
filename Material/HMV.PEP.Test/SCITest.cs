using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.SCI;
using HMV.Core.Domain.Repository;
using HMV.PEP.Interfaces;
using HMV.PEP.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System.Linq;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SCITest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }
       
        [TestMethod]
        public void busca_antimicrobianos()
        {
            IRepositorioDeSCIAntimicrobiano rep = ObjectFactory.GetInstance<IRepositorioDeSCIAntimicrobiano>();
            var lista = rep.List();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void salva_antimicrobianos()
        {
            var ins = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>().ondeAntimicrobiano().List().Where(x => x.Id == 28548).First();

            IRepositorioDeSCIAntimicrobiano rep = ObjectFactory.GetInstance<IRepositorioDeSCIAntimicrobiano>();
            SCIAntimicrobiano ant = new SCIAntimicrobiano();
            ant.TipoPrescricaoMedica = ins;

            rep.Save(ant);
            
        }

        [TestMethod]
        public void remove_antimicrobianos()
        {
            var del = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>().ondeAntimicrobiano().List().Where(x => x.Id == 28548).First();

            IRepositorioDeSCIAntimicrobiano rep = ObjectFactory.GetInstance<IRepositorioDeSCIAntimicrobiano>();
            SCIAntimicrobiano ant = rep.ondeIdIgual(del).Single();

            rep.Delete(ant);

        }

/*        [TestInitialize]
        public static void limpa_dados()
        {
        }*/

        [TestMethod]
        public void gera_parecer_para_atendimento_4010587()
        {
            IRepositorioDeSCIParecer rep = ObjectFactory.GetInstance<IRepositorioDeSCIParecer>();
            foreach (var item in rep.List())
                rep.Delete(item);
            
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(4010587).Single(); 

            ISCIService serv = ObjectFactory.GetInstance<ISCIService>();
            var lista = serv.BuscaParecerPendentes(ate, BaseTestClass.Usuario);
            SCIParecer parecer = lista.First();

            Assert.IsTrue(lista.Count > 0);

            serv.AlteraJustificativaMedica(parecer.ID, "nova justificativa", Core.Domain.Enum.SindromeInfecciosaSCI.BACTEREMIA);

            Assert.AreEqual(parecer.JustificativaMedica, "nova justificativa");
            Assert.IsNotNull(parecer.ItemPrescricao.Dose);
            Assert.IsNotNull(parecer.ItemPrescricao.MedicamentoUnidadeMedida.Descricao);
            Assert.IsNotNull(parecer.ItemPrescricao.FrequenciaItemPrescricaoMedica.Descricao);
            Assert.IsNotNull(parecer.Prescricao.DataReferencia);
            Assert.IsNotNull(parecer.ItemPrescricao.NumeroDias);
            Assert.IsNotNull(parecer.ItemPrescricao.QuantidadeDias);
            
            lista = serv.BuscaParecerPendentes(ate, BaseTestClass.Usuario);
            Assert.IsTrue(lista.Count == 0);


        }

        [TestMethod]
        public void verifica_se_deve_gerar_parecer_4010587()
        {
           Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(4010587).Single();

            ISCIService serv = ObjectFactory.GetInstance<ISCIService>();
            var lista = serv.BuscaParecerPendentes(ate, BaseTestClass.Usuario);

            Assert.IsTrue(lista.Count > 0);


        }
       
    }
}
