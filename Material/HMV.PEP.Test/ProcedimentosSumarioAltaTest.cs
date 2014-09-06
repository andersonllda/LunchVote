using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Model;
using System.Linq;
using HMV.Core.Framework.Exception;
using System;
using System.Collections.Generic;
using NHibernate.Validator.Engine;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.Test
{
    [TestClass]
    public class ProcedimentosSumarioAltaTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_os_procedimentos_amb_alta()
        {
            IRepositorioDeProcedimentoSumarioAlta serv = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            IList<Procedimento> procedimentos = serv.ondeProcedimentoPermitidoAlta().List();
            Assert.AreNotEqual(procedimentos.Count(x => x.GrupoDeProcedimento.GrupoDeFaturamento.ID == 7), 0);
            //Assert.AreNotEqual(procedimentos.Count(x => x.GrupoDeProcedimento.GrupoDeFaturamento.ID == 20), 0);
            Assert.AreNotEqual(procedimentos.Count(x => x.GrupoDeProcedimento.TipoGrupoDeFaturamento == TipoGrupoDeFaturamento.ServicosProfissionais), 0);
            Assert.AreEqual(procedimentos.Count(x => x.GrupoDeProcedimento.TipoGrupoDeFaturamento == TipoGrupoDeFaturamento.ServicosHospitalares), 0);
            Assert.AreEqual(procedimentos.Count(x => x.GrupoDeProcedimento.ID == 70), 0);
            Assert.AreEqual(procedimentos.Count(x => x.Ativo == SimNao.Nao), 0);
        }

        [TestMethod]
        public void busca_os_procedimenro_sus_do_procedimento_48030023()
        {
            IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            Procedimento procedimento = rep.OndeIdIgual("48030023").Single();
            IList<ProcedimentoMVSUS> sus = procedimento.ProcedimentoMVSUS;

            Assert.AreEqual(sus.Count, 2);
            Assert.IsNotNull(sus.FirstOrDefault(x => x.ProcedimentoSUS.ID == "0408060468"));
            Assert.IsNotNull(sus.FirstOrDefault(x => x.ProcedimentoSUS.ID == "0408060034"));
            Assert.IsNotNull(sus.FirstOrDefault(x => x.Procedimento.ID == "X3900218"));
            Assert.IsNotNull(sus.FirstOrDefault(x => x.Procedimento.ID == "X3900508"));
        }

        [TestMethod]
        public void busca_cirurgia_2961_que_nao_pode_ser_inserida_no_sumario_de_alta()
        {
            IRepositorioDeCirurgiasSumarioAlta serv = ObjectFactory.GetInstance<IRepositorioDeCirurgiasSumarioAlta>();
            Cirurgia cirurgia = serv.OndeOProcedimentoEhPermitidoNoSumarioDeAlta().OndeCodigoDaCirurgiaIgual(2961).Single();
            Assert.IsNull(cirurgia);
        }

        [TestMethod]
        public void busca_cirurgia_2631_que_pode_ser_inserida_no_sumario_de_alta()
        {
            IRepositorioDeCirurgiasSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeCirurgiasSumarioAlta>();
            rep.OndeOProcedimentoEhPermitidoNoSumarioDeAlta();
            Cirurgia cirurgia = rep.OndeCodigoDaCirurgiaIgual(2631).Single();
            Assert.IsNotNull(cirurgia);
        }

        [TestMethod]
        public void busca_as_cirugia_do_paciente_no_atendimento_1300208()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300208);
            IRepositorioDeCirurgiasSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeCirurgiasSumarioAlta>();
            rep.OndeCirugiaRealizadoDoPacienteNoAtendimento(ate);
            IList<Cirurgia> cirurgias = rep.List();
            Assert.AreEqual(cirurgias.Count, 2);
        }

        [TestMethod]
        public void quando_inicia_um_sumario_de_alta_deve_inserrir_os_procedimentos_da_cirurgia_nos_procedimentos_de_alta()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1302310);
            Usuarios usuario = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
            SumarioAlta sum = new SumarioAlta(ate, usuario);

            Assert.IsTrue(sum.ProcedimentosAlta.Count > 0);
        }

        [TestMethod]
        public void busca_exames_permitidos_so_sumario()
        {
            IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            rep.ondeProcedimentoIsExame();
            IList<Procedimento> lista = rep.List();
            Assert.IsTrue(lista.Count(x => x.GrupoDeProcedimento.ID == 20) > 0);
            Assert.IsTrue(lista.Count(x => x.GrupoDeProcedimento.ID == 32) > 0);
            Assert.IsTrue(lista.Count(x => x.GrupoDeProcedimento.ID == 33) > 0);
            Assert.IsTrue(lista.Count(x => x.GrupoDeProcedimento.ID == 34) > 0);
            Assert.IsTrue(lista.Count(x => x.GrupoDeProcedimento.ID == 35) > 0);
            Assert.IsTrue(lista.Count(x => x.GrupoDeProcedimento.ID == 36) > 0);
            Assert.AreEqual(lista.Count(x => x.GrupoDeProcedimento.ID == 26), 0);
            Assert.AreEqual(lista.Count(x => x.GrupoDeProcedimento.ID == 40), 0);
        }

        [TestMethod]
        public void busca_exame_que_estao_na_conta_amb_do_sumario_2822779()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2822779);
            IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            rep.ondeProcedimentoEstaNaContaAmbulatorial(ate);
            IList<Procedimento> lista = rep.List();
            Assert.AreEqual(lista.Count, 1);
        }

        [TestMethod]
        public void busca_exame_que_estao_na_conta_fat_do_sumario_2905174()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2905174);
            IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            rep.ondeProcedimentoEstaNaContaHospitalar(ate);
            IList<Procedimento> lista = rep.List();
            Assert.AreEqual(lista.Count, 18);
        }

        //[TestMethod]
        //public void quando_iniciar_um_sumario_novo_deve_adicionar_automaticamente_os_exames_da_conta_2922383()
        //{
        //    Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2922383);
        //    Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

        //    wrpSumarioAlta sum = new wrpSumarioAlta(ate, usu);
        //    Assert.IsTrue(sum.SumarioExames.Count > 0);
        //    Assert.AreEqual(sum.SemParticularidadeExames, SimNao.Nao);
        //    Assert.IsNull(sum.SemExamesRealizados);
        //}

        //[TestMethod]
        //public void quando_O_semExamesRealizado_Iniciar_nulo_não_deve_permitir_alterar_o_seu_valor()
        //{
        //    Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2922383);
        //    Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

        //    SumarioAlta sum = new SumarioAlta(ate, usu);
        //    Assert.IsNull(sum.SemExamesRealizados);

        //    sum.SemExamesRealizados = SimNao.Sim;
        //    Assert.IsNull(sum.SemExamesRealizados);
        //}

        [TestMethod]
        public void quando_O_semParticularidade_Iniciar_nulo_não_deve_permitir_alterar_o_seu_valor()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2866403);
            Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

            wrpSumarioAlta sum = new wrpSumarioAlta(ate, usu);
            Assert.IsNull(sum.SemParticularidadeExames);

            sum.SemParticularidadeExames = SimNao.Sim;
            Assert.IsNull(sum.SemParticularidadeExames);
        }

        [TestMethod]
        public void procedimentosalta_procedimentoAMB()
        {
            IRepositorioDeProcedimentoSumarioAlta serv = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            var procedimento = serv.OndeIdIgual("45080038").Single();

            Assert.IsTrue(procedimento.MotivosAlta.Count > 0);
        }
    }
}
