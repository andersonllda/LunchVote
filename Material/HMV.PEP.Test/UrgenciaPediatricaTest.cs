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
    public class UrgenciaPediatricaTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_Urgencia_pediatrica_atendimento_1911486()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();

            Atendimento ate = rep.OndeCodigoAtendimentoIgual(1911486).Single();

            var teste = ate.UrgenciasPediatricas;
            Assert.IsTrue(teste.Count > 0);
        }

        [TestMethod]
        public void busca_Urgencia_pediatrica_atendimento_itens_1911486()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();

            Atendimento ate = rep.OndeCodigoAtendimentoIgual(1911486).Single();

            var teste = ate.UrgenciasPediatricas.FirstOrDefault();
            Assert.IsTrue(teste.UrgenciaPediatricaAtendimentoItens.Count > 0);
        }

        [TestMethod]
        public void busca_Urgencia_pediatrica_itens()
        {
            IRepositorioDeUrgenciaPediatricaGrupo rep = ObjectFactory.GetInstance<IRepositorioDeUrgenciaPediatricaGrupo>();

            var teste = rep.List();

            Assert.IsTrue(teste.Count > 0);
        }

        [TestMethod]
        public void busca_Urgencia_pediatrica_save()
        {
            IRepositorioDeUrgenciaPediatrica rep = ObjectFactory.GetInstance<IRepositorioDeUrgenciaPediatrica>();

            IRepositorioDeUrgenciaPediatricaGrupo rep2 = ObjectFactory.GetInstance<IRepositorioDeUrgenciaPediatricaGrupo>();
            var grupo = rep2.OndeIdIgual(1).List().First();

            var item = grupo.UrgenciaPediatricaItens.FirstOrDefault();

            var teste = rep.OndeIdIgual(144).Single();

            teste.UrgenciaPediatricaAtendimentoItens = new List<UrgenciaPediatricaAtendimentoItem>();


            var novo = new UrgenciaPediatricaAtendimentoItem(teste);
            novo.Apresentacao = "teste1";
            novo.DoseCalculada = "teste";
            novo.Droga = "teste1";
            novo.UrgenciaPediatricaItem = item;

            teste.UrgenciaPediatricaAtendimentoItens.Add(novo);

            rep.Save(teste);

            teste = rep.OndeIdIgual(144).Single();

            Assert.IsTrue(teste.UrgenciaPediatricaAtendimentoItens.Count > 0);

            teste.UrgenciaPediatricaAtendimentoItens = null;

            rep.Save(teste);
        }

    }
}
