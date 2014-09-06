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
using System.IO;

namespace HMV.PEP.Test
{
    [TestClass]
    public class ParametroPEPTest : BaseTestClass
    {


        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_link_fleury()
        {
            IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
            Assert.IsNotNull(srv.LinkFleury());
        }

        [TestMethod]
        public void busca_path_microdata()
        {
            IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
            Assert.IsNotNull(srv.PathMicroData());
        }


        [TestMethod]
        public void busca_path_compartilhamentoMV()
        {
            IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
            Assert.IsNotNull(srv.PathCompartilhamentoMV());
           
            //Parametro parametro = srv.PathCompartilhamentoMV();

            //System.Diagnostics.Process.Start("net.exe", "use E: /delete");
            //Assert.IsFalse(!Directory.Exists("E:\\"));           

            //System.Diagnostics.Process.Start("net.exe", "use E: " + parametro.Valor);
            //Assert.IsTrue(Directory.Exists("E:\\"));   
        }
    }
}