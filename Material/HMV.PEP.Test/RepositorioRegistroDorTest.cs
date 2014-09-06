using HMV.Core.NH.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HMV.Core.DataAccess;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Linq;
using System.Collections.Generic;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.Test
{


    [TestClass()]
    public class RepositorioRegistroDorTest:BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }


        /// <summary>
        ///Teste de RegistroDor filtrado por atendimento
        ///</summary>
        [TestMethod]
        public void busca_lista_de_registro_de_dor_do_atendimento_2656124() 
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2656124).Single();
            Assert.IsTrue(ate.RegistroDor.Count > 0);
        }

        [TestMethod]
        public void verifica_ponodeReferencia_e_Articulacao()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2656124).Single();
            RegistroDor reg = ate.RegistroDor.First();
            Assert.IsNotNull(reg.PontoReferencia);
            Assert.IsNotNull(reg.PontoReferencia.Articulacao);

        }

        [TestMethod]
        public void transforma_data_e_hora_em_um_campo_unico()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2566050).Single();
            RegistroDor reg = ate.RegistroDor.First();
            Assert.AreEqual(reg.DataInclusao,new DateTime(2010,4,26,19,12,6));

        }

        [TestMethod]
        public void busca_lista_de_articulacao_da_lista_do_registro_de_dor()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2566050).Single();
            Assert.IsTrue(ate.RegistroDor.Count(x => x.PontoReferencia.Articulacao.Status == Status.Ativo) > 0);
            //Assert.IsTrue(ate.RegistroDor.Count(x => x.PontoReferencia.Articulacao.Status == Status.Inativo) > 0);
        }         


        ///// <summary>
        /////efetuar filtro registro de dor considerando o intervalo de datas e articulcao.
        /////</summary>
        //[TestMethod()]
        //public void FiltroDeEvolucaoTest()
        //{
        //    IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
        //    Atendimento ate = rep.OndeCodigoAtendimentoIgual(2566050).Single();
            
        //    RegistroDor reg = ate.RegistroDor.FirstOrDefault(); 
            
        //    Articulacao Articulacao = new Articulacao();
        //    Articulacao.ID = 11;
        //    DateTime DtInicial = new DateTime(01/01/2009);
        //    DateTime DtFinal = new DateTime(01/12/2010);

        //    IRepositorioDeRegistroDor actual;
        //    actual = reg.FiltroDeEvolucao(Articulacao, DtInicial, DtFinal);
        //    Assert.IsNotNull(actual);
        //}
    }
}

