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
using HMV.Core.Domain.Model.PEP;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Repository.ClassificacaoPaciente;
using HMV.PEP.Consult;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SCPTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void Filtra_SCP_por_periodo()
        {
            IRepositorioDeSCP rep = ObjectFactory.GetInstance<IRepositorioDeSCP>();
            rep.FiltraPorPeriodo(DateTime.Now.Date.Subtract(new TimeSpan(96, 0, 0)), DateTime.Now.Date);

            Assert.IsTrue(rep.List().Count > 0);
        }

        [TestMethod]
        public void Filtra_SCP_por_periodo_pacientes_internados()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            rep.FiltraPorPeriodoSCP(DateTime.Now.Date.Subtract(new TimeSpan(100,0, 0, 0)), DateTime.Now.Date);

            Assert.IsTrue(rep.List().Count > 0);
        }

        
        [TestMethod]
        public void consult_scp_internados()
        {
            IListaPacientesInternadosConsult rep = ObjectFactory.GetInstance<IListaPacientesInternadosConsult>();
            var teste = rep.ListaSCPInternados(DateTime.Now.Date.Subtract(new TimeSpan(100, 0, 0, 0)), DateTime.Now.Date);            

            Assert.IsTrue(teste.Count > 0);
        }

    /*     
       [TestMethod]
        public void consult_scp_internadosnaoavaliados()
        {
            IListaPacientesInternadosConsult rep = ObjectFactory.GetInstance<IListaPacientesInternadosConsult>();
            var teste = rep.ListaSCPInternadosNaoAvaliados(DateTime.Now.Date.Subtract(new TimeSpan(100, 0, 0, 0)), DateTime.Now.Date);

            Assert.IsTrue(teste.ToList().Count > 0);
        } */
    }
}
