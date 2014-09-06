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
using HMV.Core.Interfaces;
using System.Configuration;
using HMV.PEP.Consult;
using HMV.PEP.DTO;

namespace HMV.PEP.Test
{
    [TestClass]
    public class CIDTest : BaseTestClass
    {


        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void carrega_arvore_de_cid()
        {
            ICidConsult consult = ObjectFactory.GetInstance<ICidConsult>();
            IList<CidDTO> listaCid = consult.ListaCids();
            Assert.IsTrue(listaCid.Count > 0);
            Assert.IsNotNull(listaCid.FirstOrDefault().IdCid);
            Assert.IsNotNull(listaCid.FirstOrDefault().DescricaoCid);
        }

        [TestMethod]
        public void carrega_arvore_de_cid_com_os_capitulos()
        {
            ICidConsult consult = ObjectFactory.GetInstance<ICidConsult>();
            IList<CidDTO> listaCid = consult.ListaCids();
            Assert.IsTrue(listaCid.Count(x => x.IdCapitulo != 0) > 0);
            Assert.IsTrue(listaCid.Count(x => !String.IsNullOrEmpty(x.DescricaoCapitulo)) > 0);
        }

        [TestMethod]
        public void carrega_arvore_de_cid_com_as_categorias()
        {
            ICidConsult consult = ObjectFactory.GetInstance<ICidConsult>();
            IList<CidDTO> listaCid = consult.ListaCids();
            //Assert.IsTrue(listaCid.Count(x => x.IdCategoria != 0) > 0);
            Assert.IsTrue(listaCid.Count(x => !String.IsNullOrEmpty(x.DescricaoCategoria)) > 0);
        }

        [TestMethod]
        public void carrega_arvore_de_cid_com_as_subcategorias()
        {
            ICidConsult consult = ObjectFactory.GetInstance<ICidConsult>();
            IList<CidDTO> listaCid = consult.ListaCids();
            //Assert.IsTrue(listaCid.Count(x => x.IdCategoria != 0) > 0);
            Assert.IsTrue(listaCid.Count(x => !String.IsNullOrEmpty(x.DescricaoSubCategoria)) > 0);
        }

        [TestMethod]
        public void busca_somente_os_cids_que_tem_cidmv()
        {
            ICidService srv = ObjectFactory.GetInstance<ICidService>();
            IList<CidDTO> lista = srv.ListaCIDs(true);
            Assert.AreEqual(lista.Count(x =>  x.IdCidMV == null), 0);

        }
    }
}
