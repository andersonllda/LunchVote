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

namespace HMV.PEP.Test
{
    [TestClass]
    public class PIN2Test : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void adiciona_PIN2_atendimento()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                        
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(2320050).Single();
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");

            PIN2 pin2 = new PIN2(ate);
            pin2.Status = Status.Ativo;
            pin2.Total = 100;
            pin2.Usuario = usu;
            pin2.BAS = 0;
            pin2.BE = 0;
            pin2.Bypass = 0;
            pin2.DataInclusao = DateTime.Now;
            pin2.Elective = 0;
            pin2.Haut = 0;
            pin2.PAS = 0;
            pin2.Pupilles = 0;
            pin2.Ratio = 0;
            pin2.Recovery = 0;
            pin2.Ventil = 0;

            ate.PIN2.Add(pin2);

            rep.Save(ate);
            rep.Refresh(ate);
            
            Assert.IsTrue(ate.PIN2.Count > 0);
        }
    }
}
