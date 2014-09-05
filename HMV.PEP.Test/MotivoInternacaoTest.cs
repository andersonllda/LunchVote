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
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.Test
{
    [TestClass]
    public class MotivoInternacaoTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void inseri_um_novo_motivo_internacao_id_1()
        {
            IRepositorioMotivoItem rep = ObjectFactory.GetInstance<IRepositorioMotivoItem>();
            wrpMotivoItemCollection MotivoItemCollection = new wrpMotivoItemCollection(rep.List());

            IAtendimentoService srv = ObjectFactory.GetInstance<IAtendimentoService>();

            wrpAtendimento atendimento = new wrpAtendimento(srv.FiltraPorID(1911486));
            wrpMotivoInternacao motivo = new wrpMotivoInternacao(atendimento.DomainObject, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());

            motivo.SubItem = MotivoItemCollection.SelectMany(x => x.SubItens.Where(xx => xx.ID == 1)).Single();
            atendimento.MotivoInternacao.Add(motivo);

            atendimento.Save();

            Assert.IsTrue(atendimento.MotivoInternacao.Count(x => x.SubItem.ID == 1) > 0);

        }

           [TestMethod]
        public void remove_o_motivo_internacao_id_igual_1()
        {
            IRepositorioMotivoItem rep = ObjectFactory.GetInstance<IRepositorioMotivoItem>();
            wrpMotivoItemCollection MotivoItemCollection = new wrpMotivoItemCollection(rep.List());
            IAtendimentoService srv = ObjectFactory.GetInstance<IAtendimentoService>();
            wrpAtendimento atendimento = new wrpAtendimento(srv.FiltraPorID(1911486));          

            while (atendimento.MotivoInternacao.Count(x=>x.SubItem.ID == 1) > 0)
            {
                wrpMotivoInternacao motivo = atendimento.MotivoInternacao.FirstOrDefault(x => x.SubItem.ID == 1);
                atendimento.MotivoInternacao.Remove(motivo);
            }
         

            atendimento.Save();

            Assert.IsTrue(atendimento.MotivoInternacao.Count(x => x.ID == 1) == 0);
        }
    }
}
