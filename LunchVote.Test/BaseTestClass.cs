using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchVote.IoC;
using StructureMap;
using LunchVote.Domain;

namespace LunchVote.Test
{
    [TestClass]
    public abstract class BaseTestClass
    {
        public static Profissional Profissional;

        protected static void BaseTestInitialize(TestContext testContext)
        {           
            IoCWorker.ConfigureFake();
            Profissional = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
        }      

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
