using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Model;
using System.Linq;
using HMV.Core.Framework.Exception;
using System;
using System.Collections.Generic;
using NHibernate.Validator.Engine;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository;
using HMV.PEP.DTO;
using HMV.PEP.Consult;
using HMV.Core.Wrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.SumarioDeAtendimento;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SumarioAtendimentoTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_sumarios_de_atendimento()
        {
            ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();

            var teste = consult.carregaSumarioDeAtendimentos(new Paciente { ID = 10739786 }, TipoAtendimentoSumario.Todos, null);

            Assert.IsTrue(teste.Count > 0);
        }

        [TestMethod]
        public void busca_procedimentos_realizados()
        {            
            ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();

            var teste = consult.carregaProcedimentosRealizados(new Paciente { ID = 1191769 });

            Assert.IsTrue(teste.Count > 0);
        }


        //[TestMethod]
        //public void testa_vmSumarioAtendimento()
        //{
        //    IRepositorioDePacientes rep = ObjectFactory.GetInstance<IRepositorioDePacientes>();
        //    var pac = rep.OndeCodigoIgual(1000425).Single();
        //    var teste = new vmSumarioAtendimento(pac, TipoAtendimentoSumario.Todos,false,false);

        //    Assert.AreEqual(teste.ProcedimentosRealizados.Count, 177);
        //}
    }
}
