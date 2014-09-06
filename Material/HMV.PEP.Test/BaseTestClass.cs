using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMV.Core.DataAccess;
using System.Configuration;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;

namespace HMV.PEP.Test
{
    [TestClass]
    public abstract class BaseTestClass
    {
        private IUnitOfWork _unitOfWork;
        public static Usuarios Usuario;

        protected static void BaseTestInitialize(TestContext testContext)
        {
            SessionManager.ConfigureDataAccess(@"Data Source=homolog_01;User ID=DBAHMV;Password=DBAHMV"//ConfigurationManager.ConnectionStrings["HOMOLOG"].ConnectionString
                //, System.Configuration.ConfigurationManager.AppSettings["ConfigNHibernate"].ToString()
                );
            HMV.PEP.IoC.IoCWorker.ConfigureWIN();
            Usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
        }

        /*protected static void BaseTestInitialize2(TestContext testContext)
        {
            SessionManager.ConfigureDataAccess(ConfigurationManager.ConnectionStrings["HOMOLOG1"].ConnectionString, System.Configuration.ConfigurationManager.AppSettings["ConfigNHibernate"].ToString());
            HMV.PEP.IoC.IoCWorker.ConfigureWIN();
        }*/

     
        [TestInitialize]
        public void InitializeTest()
        {
            _unitOfWork = ObjectFactory.GetInstance<IUnitOfWork>();
        }

        [TestCleanup]
        public void FinalizeTest()
        {
            _unitOfWork.Dispose();
            //_unitOfWork = null;
        }
    }
}
