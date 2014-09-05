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

namespace HMV.PEP.Test
{
    [TestClass]
    public class ProblemasPacienteTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void exclui_problema_do_paciente_1104180()
        {
            Paciente paciente = ObjectFactory.GetInstance<IPacienteService>().FiltraPorID(1104180);

            IList<ProblemasPaciente> problemas = paciente.ProblemasPaciente;

            ProblemasPaciente problema = problemas.First();

            IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();

            paciente.RemoveProblemasPaciente(problema);

            srv.Salvar(paciente);
        }
    }
}
