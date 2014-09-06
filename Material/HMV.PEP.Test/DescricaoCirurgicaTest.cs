using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using System.Configuration;
using HMV.PEP.DTO;
using HMV.Core.Domain.Enum;
using HMV.PEP.Consult;

namespace HMV.PEP.Test
{
    [TestClass]
    public class DescricaoCirurgicaTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void verifica_campos_avisocirurgia_id_120()
        {
            IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
            AvisoCirurgia ac = rep.OndeCodigoDoAvisoIgual(120).Single();
            Assert.AreEqual(ac.Atendimento.ID, 1302310);
            Assert.IsNotNull(ac.DescricaoCirurgia);
            Assert.AreEqual(ac.DataAviso.Date, DateTime.Parse("28/04/07"));
            Assert.AreEqual(ac.DataInicio.Value.Date, DateTime.Parse("03/05/07"));
            Assert.AreEqual(ac.DataFim.Value.Date, DateTime.Parse("03/05/07"));
            Assert.AreEqual(ac.DataRealizacao.Value.Date, DateTime.Parse("03/05/07"));
            Assert.AreEqual(ac.TempoDuracao.Value.Date, DateTime.Parse("03/05/07"));
            Assert.IsNotNull(ac.TipoAnestesia);
            Assert.IsNotNull(ac.SalaCirurgia);
        }

        [TestMethod]
        public void consulta_descricao_cirurgica_36230()
        { 
            IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("rep", rep);
            IDescricaoCirurgicaConsult consult = ObjectFactory.GetInstance<IDescricaoCirurgicaConsult>(args);

            //DescricaoCirurgicaDTO ret = consult.DescricaoCirurgica(36230).ToList();
            //Assert.IsNotNull(ret.Situacao.CustomDisplay());
        
        }
        [TestMethod]
        public void verificar_equipe_medica()
        {
            IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("rep", rep);
            IDescricaoCirurgicaConsult consult = ObjectFactory.GetInstance<IDescricaoCirurgicaConsult>(args);

           //DescricaoCirurgicaDTO ret = consult.DescricaoCirurgica(36230);
        //    Assert.IsNotNull(ret.Situacao.CustomDisplay());

        }

        [TestMethod]
        public void verificar_achadoscirurgicos_71212()
        {
            IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
            AvisoCirurgia ac = rep.OndeCodigoDoAvisoIgual(71212).Single();
            Assert.IsTrue(ac.AchadosCirurgicosItens.Count() > 0);
            Assert.AreEqual(ac.AchadosCirurgicosItens.Count(), 1);
            Assert.AreEqual(ac.AchadosCirurgicosItens[0].Complemento, "HEMATOMA NA MAMA DIR PÓS QUADRANTECTOMIA MAMARIA");
        }
    }
}
