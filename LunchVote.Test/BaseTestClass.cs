using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchVote.IoC;
using StructureMap;
using LunchVote.Domain;
using LunchVote.Domain.Repository;
using System.Linq;

namespace LunchVote.Test
{
    [TestClass]
    public abstract class BaseTestClass
    {
        public static Profissional ProfissionalFaminto;
        public static Profissional ProfissionalFacilitador;

        protected static void BaseTestInitialize(TestContext testContext)
        {           
            IoCWorker.ConfigureFake();
            ProfissionalFaminto = ObjectFactory.GetInstance<IRepositoryProfissional>().OndeIdIgual(new Random().Next(10)).Single();
            ProfissionalFacilitador = ObjectFactory.GetInstance<IRepositoryProfissional>().OndeIsFacilitador().List().FirstOrDefault();
        }      

        [TestInitialize]
        public void InitializeTest()
        {
         //
        }

        [TestCleanup]
        public void FinalizeTest()
        {
         //
        }
    }
}
