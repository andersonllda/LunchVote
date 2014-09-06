using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using StructureMap;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SumarioAvaliacaoMedicaRNTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }


        [TestMethod]
        public void cria_sumario_rn()
        {
            IRepositorioDeSumarioDeAvaliacaoMedicaRN rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
            IRepositorioDeAtendimento repAtendimento = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            IRepositorioDeUsuarios repUsuario= ObjectFactory.GetInstance<IRepositorioDeUsuarios>();
            IRepositorioDePacientes repPaciente = ObjectFactory.GetInstance<IRepositorioDePacientes>();

            Atendimento atend = repAtendimento.OndeCodigoAtendimentoIgual(5).Single();
            //Paciente pac =  repPaciente.OndeCodigoIgual(9812474).Single();
            Usuarios usu = repUsuario.OndeCodigoIgual("H2555HO").Single();

            SumarioAvaliacaoMedicaRN sumario = new SumarioAvaliacaoMedicaRN(atend, usu, atend.Paciente, DateTime.Now);
            
            sumario.IsGestacaoAnterior = SimNao.Sim;
            sumario.IsMedicacao = SimNao.Sim;
            sumario.IsPatologia = SimNao.Sim;
            sumario.IsForcipe = SimNao.Sim;
            sumario.IsSituacaoFetalNaoTraquilizadora = SimNao.Nao;

            rep.Save(sumario);

            Assert.IsTrue(sumario.Id > 0);
        }

        [TestMethod]
        public void consulta_sumario_rn()
        {
            IRepositorioDeSumarioDeAvaliacaoMedicaRN rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
            var teste = rep.List();

            Assert.IsTrue(teste.Count > 0);
        }
    }
}
