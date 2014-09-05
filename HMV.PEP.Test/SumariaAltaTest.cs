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
using HMV.PEP.DTO;
using HMV.PEP.Consult;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SumarioAltaTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_data_de_impressao_do_sumario_de_alta_do_atendimento_2845615()
        {
            SumarioAlta sum = retornaSumario(2845615);
            Assert.AreEqual(sum.UsuarioImpressao.cd_usuario , "M13250RS");
            Assert.AreEqual(sum.DataImpressao.Value.ToShortDateString() , "09/11/2010");
        }

        private SumarioAlta retornaSumario(int pAtendimento)
        {
            IAtendimentoService servAte = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = servAte.FiltraPorID(pAtendimento);
            return ate.SumarioAlta;
        }

        [TestMethod]
        public void busca_causa_externa_do_atendimento()
        {
            SumarioAlta sum = retornaSumario(2698754);
            Assert.IsNotNull(sum.CausaExterna.FirstOrDefault(x=>x.Cid.Id == "S271"));
            Assert.AreEqual(sum.CausaExterna.FirstOrDefault(x => x.Cid.Id == "S271").Observacao, "QUEDA AO SOLO DA PRÓPRIA ALTURA");
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessValidatorException))]
        public void cria_um_novo_sumario_de_alta_1327910()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1327910);
            Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single(); 
            SumarioAlta sum = new SumarioAlta(ate,usu);
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            serv.Save(sum);
            Assert.IsTrue(sum.Atendimento.ID == 1327910);
        }

        [TestMethod]
        public void adiciona_uma_nova_causa_externa_para_o_atendimento_2698754()
        {
            CausaExterna causa = new CausaExterna(ObjectFactory.GetInstance<ICidService>().FiltraCIDMVPorId("Q165"));
            causa.Observacao = "TESTE";

            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2698754);
            ate.SumarioAlta.AddCausaExterna(causa);
            serv.Save(ate);

            Assert.IsNotNull(ate.SumarioAlta.CausaExterna.FirstOrDefault(x=>x.Cid.Id == "Q165"));
            Assert.AreEqual(ate.SumarioAlta.CausaExterna.FirstOrDefault(x => x.Cid.Id == "Q165").Observacao, "TESTE");
        }

        [TestMethod]
        public void adiciona_um_novo_procedimentio_NO_sumarioDeAlta_do_atendimento_3278594()
        {
            HMV.Core.Domain.Repository.IRepositorioDeCirurgiasSumarioAlta rep = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeCirurgiasSumarioAlta>();
            Cirurgia cir = rep.List().FirstOrDefault();
            Usuarios usu = ObjectFactory.GetInstance<HMV.Core.Interfaces.IUsuariosService>().FiltraPorID("H2555HO");

            ProcedimentoAlta obj = new ProcedimentoAlta(cir,usu);

            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(3278594);
            ate.SumarioAlta.SemProcedimentoInvasivo = SimNao.Sim;
            
            ate.SumarioAlta.AddProcedimentoAlta(obj);
            ate.SumarioAlta.AddProcedimentoAlta(obj);
            serv.Save(ate);

            Assert.AreEqual(ate.SumarioAlta.SemProcedimentoInvasivo, SimNao.Nao);
            Assert.AreEqual(ate.SumarioAlta.ProcedimentosAlta.Count(x=>x.Data.Date == DateTime.Now.Date), 1);
            Assert.AreEqual(ate.SumarioAlta.ProcedimentosAlta.FirstOrDefault().Usuario.cd_usuario, "H2555HO");
            Assert.AreEqual(ate.SumarioAlta.ProcedimentosAlta.FirstOrDefault().Cirurgia.cd_cirurgia, cir.cd_cirurgia);
        }

        [TestMethod]
        public void seta_O_sumario_para_sem_procedimento_invasivo_com_procedimento_alta_informado_NAO_ALTERAR()
        {
            HMV.Core.Domain.Repository.IRepositorioDeCirurgiasSumarioAlta rep = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeCirurgiasSumarioAlta>();
            Cirurgia cir = rep.List().FirstOrDefault();
            Usuarios usu = ObjectFactory.GetInstance<HMV.Core.Interfaces.IUsuariosService>().FiltraPorID("H2555HO");

            ProcedimentoAlta obj = new ProcedimentoAlta(cir, usu);

            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2845348);
            ate.SumarioAlta.RemoveProcedimentoAlta(obj);
            ate.SumarioAlta.AddProcedimentoAlta(obj);
            serv.Save(ate);

            Assert.AreEqual(ate.SumarioAlta.SemProcedimentoInvasivo, SimNao.Nao);

            ate.SumarioAlta.SemProcedimentoInvasivo = SimNao.Sim;

            Assert.AreEqual(ate.SumarioAlta.SemProcedimentoInvasivo, SimNao.Nao);
        }

        [TestMethod]
        public void adiciona_um_sumario_de_alta_sem_informar_O_procedimeto_alta_retornar_um_erro()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300266);
            wrpSumarioAlta sumario = new wrpSumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
             
            IList<InvalidValue> erros = sumario.ValidarProcedimentoAlta();
            Assert.IsTrue(erros.Count > 0 );
        }

        [TestMethod]
        public void adiciona_um_sumario_de_alta_sem_procedimento_invasivos()
        {
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300266);
            wrpSumarioAlta sumario = new wrpSumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            sumario.SemProcedimentoInvasivo = SimNao.Sim;
            sumario.Evolucao = "sdsd";
            IList<InvalidValue> erros = sumario.ValidarProcedimentoAlta();
            Assert.IsTrue(erros.Count == 0);
        }

        [TestMethod]
        public void adiciona_um_sumario_de_alta_sem_informar_a_causa_externa_retornar_um_erro()
        {
            ISumarioAltaService servSum = ObjectFactory.GetInstance<ISumarioAltaService>();
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300266);
            wrpSumarioAlta sum = new wrpSumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            IList<InvalidValue> erros = sum.ValidarCausaExterna();
            Assert.IsTrue(erros.Count > 0);
        }

        [TestMethod]
        public void adiciona_um_sumario_de_alta_sem_causa_externa()
        {
            ISumarioAltaService servSum = ObjectFactory.GetInstance<ISumarioAltaService>();
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300266);
            wrpSumarioAlta sum = new wrpSumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            sum.SemCausaExterna = SimNao.Sim;
            IList<InvalidValue> erros = sum.ValidarCausaExterna();
            Assert.IsTrue(erros.Count == 0);
        }

        [TestMethod]
        public void adiciona_um_sumario_de_alta_sem_informar_os_farmacos_retornar_um_erro()
        {
            ISumarioAltaService servSum = ObjectFactory.GetInstance<ISumarioAltaService>();
            Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300266);
            wrpSumarioAlta sum = new wrpSumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            IList<InvalidValue> erros = sum.ValidarFarmacos();
            Assert.IsTrue(erros.Count > 0);
        }

        [TestMethod]
        public void adiciona_um_sumario_de_alta_sem_farmacos()
        {
            ISumarioAltaService servSum = ObjectFactory.GetInstance<ISumarioAltaService>();
            wrpSumarioAlta sum = new wrpSumarioAlta(ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300266), ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            sum.SemFarmaco = SimNao.Sim;
            IList<InvalidValue> erros = sum.ValidarFarmacos();
            Assert.IsTrue(erros.Count == 0);
        }

        [TestMethod]
        public void insere_uma_descrição_para_os_farmacos_do_sumario_2849240()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2849240);
            ate.SumarioAlta.FarmacoObservacao = "";

            serv.Save(ate);

            Assert.IsNull(retornaSumario(2849240).FarmacoObservacao);
        }

        [TestMethod]
        public void insere_farmacos_no_sumario_2444969()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2698754);
            ate.SumarioAlta.SemFarmaco = SimNao.Sim;

            Produto pro = ObjectFactory.GetInstance<IRepositorioDeProduto>().OndeIdIgual(1202).Single();
            Usuarios usu = ObjectFactory.GetInstance<HMV.Core.Interfaces.IUsuariosService>().FiltraPorID("H2555HO");

            Farmaco farmaco = new Farmaco(pro,usu);
            ate.SumarioAlta.AddFarmaco(farmaco);

            serv.Save(ate);

            Assert.AreEqual(ate.SumarioAlta.SemFarmaco, SimNao.Nao);
            Assert.IsNotNull(ate.SumarioAlta.Farmacos.FirstOrDefault(x => x.Produto.Id == 1202));
            Assert.AreEqual(ate.SumarioAlta.Farmacos.FirstOrDefault(x => x.Produto.Id == 1202).Usuario.cd_usuario, "H2555HO");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void insere_farmacos_no_sumario_2444969_sem_informar_o_produto_retornar_um_erro()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            
            Produto pro = null;
            Usuarios usu = ObjectFactory.GetInstance<HMV.Core.Interfaces.IUsuariosService>().FiltraPorID("H2555HO");

            Farmaco farmaco = new Farmaco(pro, usu);
            SumarioAlta sum = serv.FiltraPorID(2444969);
            sum.AddFarmaco(farmaco);

            serv.Save(sum);         
        }

        [TestMethod]
        public void insere_evolucao_no_sumario_de_alta_2444969()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2444969);
            ate.SumarioAlta.Evolucao = "sem nenhuma complicação";

            serv.Save(ate);

            Assert.AreEqual(ate.SumarioAlta.Evolucao, "sem nenhuma complicação");
        }

        [TestMethod]
        public void insere_evolucao_no_sumario_de_alta_e_valida_()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            wrpSumarioAlta sum = new wrpSumarioAlta(serv.FiltraPorID(2768370).SumarioAlta);
            sum.Evolucao = "";
            IList<InvalidValue> erros = sum.ValidarEvolucao();
            Assert.IsTrue(erros.Count > 0);


        }

        [TestMethod]
        public void altera_SumarioOutros_semCausaExterna_atendimento_2117068_para_Nao()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2768370);
            ate.SumarioAlta.SemCausaExterna = SimNao.Nao;
            serv.Save(ate);
            Assert.IsTrue(serv.FiltraPorID(2768370).SumarioAlta.SemCausaExterna == SimNao.Nao);
        }    

        [TestMethod]
        public void rbusca_causa_externa_do_atendimento_2824767()
        {
            adiciona_nova_causa_externa_do_atendimento_2824767_cid_J312();
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2824767);
            Assert.IsTrue(ate.SumarioAlta.CausaExterna.Count() > 0);
        }

        [TestMethod]
        public void adiciona_nova_causa_externa_do_atendimento_2824767_cid_J312()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2824767);
            CausaExterna causa = new CausaExterna(ObjectFactory.GetInstance<IRepositorioDeCid>().OndeCidMVIgual("J312").Single().CidMV);
            causa.Observacao = "TESTE";
            ate.SumarioAlta.AddCausaExterna(causa);
            serv.Save(ate);
            Assert.IsTrue(serv.FiltraPorID(2824767).SumarioAlta.CausaExterna.Where(x => x.Cid.Id == "J312").Count() == 1);
        }

        [TestMethod]
        public void remove_causa_externa_do_atendimento_2768370_cid_J312()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2768370);
            CausaExterna causa = new CausaExterna(ObjectFactory.GetInstance<IRepositorioDeCid>().OndeCidMVIgual("J312").Single().CidMV);
            causa.Observacao = "TESTE";
            ate.SumarioAlta.AddCausaExterna(causa);

            IList<CausaExterna> causas = ate.SumarioAlta.CausaExterna.Where(x => x.Cid.Id == "J312").ToList();
            ate.SumarioAlta.SemFarmaco = SimNao.Sim;
            ate.SumarioAlta.RemoveCausaExterna(causas.First());
            serv.Save(ate);
            Assert.IsTrue(serv.FiltraPorID(2768370).SumarioAlta.CausaExterna.Where(x => x.Cid.Id == "J312").Count() == 0);
        }

        [TestMethod]
        public void busca_SumarioOutros_atendimento_1300207()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(1300207);
            ate.SumarioAlta.SemCausaExterna = SimNao.Nao;
            serv.Save(ate);

            ate = serv.FiltraPorID(1300207);
            Assert.IsTrue(ate.SumarioAlta.SemCausaExterna == SimNao.Nao);

        }

        [TestMethod]
        public void insere_sumario_de_alta_sem_causa_externa_informado_atendimento_1300074()
        {

            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            wrpSumarioAlta sum = new wrpSumarioAlta(ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300074), ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
            sum.SemProcedimentoInvasivo = SimNao.Nao;
            Assert.IsTrue(sum.ValidarProcedimentoAlta().Count > 0);
        }

        [TestMethod]
        public void insere_sumario_de_alta_sem_procedimento_informado_atendimento_1300074()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(1300074);
            wrpSumarioAlta sum = new wrpSumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());

            sum.SemCausaExterna = SimNao.Nao;
            Assert.IsTrue(sum.ValidarCausaExterna().Count > 0);
        }

        [TestMethod]
        public void d_altera_sumario_alta_sem_farmaco_sumario_de_alta_atendimento_2451406()
        {
            IAtendimentoService serv = ObjectFactory.GetInstance<IAtendimentoService>();
            Atendimento ate = serv.FiltraPorID(2453289);
            wrpSumarioAlta sumario = new wrpSumarioAlta(ate.SumarioAlta);
            ate.SumarioAlta.SemFarmaco = SimNao.Nao;
            Assert.IsTrue(sumario.ValidarFarmacos().Count > 0);
        }

        [TestMethod]
        public void insere_no_campo_ObservacaoExame_sumario_de_alta_atendimento_2444969()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2444969);
            sum.ExameObservacao = "TESTE";
            serv.Save(sum);
            Assert.AreEqual(retornaSumario(2444969).ExameObservacao, "TESTE");
        }

        [TestMethod]
        public void insere_resultados_sumario_de_alta_atendimento_2444969()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();

            SumarioAlta sum = serv.FiltraPorID(2444969);
            Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimento>().OndeIdIgual("32050054").Single();
            SumarioExame exa = new SumarioExame(procedimento);
            exa.Observacao = "TESTE";
            sum.AddSumarioExame(exa);
            serv.Save(sum);
            Assert.AreEqual(sum.SumarioExames.FirstOrDefault(x => x.Procedimento.ID == procedimento.ID).Observacao, "TESTE");
            Assert.IsNotNull(sum.SumarioExames.FirstOrDefault(x => x.Procedimento.ID == procedimento.ID));

            sum.RemoveSumarioExame(exa);
            serv.Save(sum);
            Assert.IsNull(sum.SumarioExames.FirstOrDefault(x => x.Procedimento.ID == procedimento.ID));

        }

        [TestMethod]
        public void insere_um_plano_posAlta_do_sumario_2838069_com_produto()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2838069);
            Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
            TipoPrescricaoMedica tipoPresc = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>().OndeIdIgual(31540).Single();
            
            PlanoPosAlta plano = new PlanoPosAlta(usu, sum.Atendimento);
            plano.TipoPrescricaoMedica = tipoPresc;
            plano.Dose = "500 MG";
            plano.Via = "ORAL";
            plano.Frequencia = "12/12h";
            plano.Tempo = "14 dias";        

            sum.AddPlanoPosAlta(plano);
            serv.Save(sum);

            Assert.AreEqual(sum.PlanoPosAlta.FirstOrDefault(x => x.Id == plano.Id).Produto.Id, 2113);
            Assert.AreEqual(sum.PlanoPosAlta.FirstOrDefault(x => x.Id == plano.Id).TipoPrescricaoMedica.Descricao, "NARCAN (0,4MG/ML) 1ML INJ NEO");
            Assert.AreEqual(sum.PlanoPosAlta.FirstOrDefault(x => x.Id == plano.Id).Usuario.cd_usuario, "H2555HO");

            sum.RemovePlanoPosAlta(plano);
            serv.Save(sum);
            Assert.IsNull(sum.PlanoPosAlta.FirstOrDefault(x => x.Id == plano.Id));

        }

        [TestMethod]
        [ExpectedException (typeof(BusinessValidatorException))]
        public void insere_um_plano_posAlta_do_sumario_2444969_sem_informar_o_produto_e_nome_comercial()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2444969);
            Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

            PlanoPosAlta plano = new PlanoPosAlta(usu, sum.Atendimento);
            plano.Dose = "12/15H";

            sum.AddPlanoPosAlta(plano);
        }

        [TestMethod]
        public void insere_um_plano_posAlta_do_sumario_2444969_sem_informar_o_produto()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2444969);
            Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
            sum.SemMedPosAlta = SimNao.Sim;

            PlanoPosAlta plano = new PlanoPosAlta(usu, sum.Atendimento);
            plano.NomeComercial = "TESTE";
            sum.AddPlanoPosAlta(plano);
           
            serv.Save(sum);
            Assert.IsNotNull(sum.PlanoPosAlta.FirstOrDefault(x => x.Id == plano.Id));
            Assert.AreEqual(sum.SemMedPosAlta,SimNao.Nao);

            sum.RemovePlanoPosAlta(plano);
            serv.Save(sum);
            Assert.IsNull(sum.PlanoPosAlta.FirstOrDefault(x => x.Id == plano.Id));

        }

        [TestMethod]
        public void quando_alterar_para_sem_plano_posalta_igual_sim_e_tiver_plano_pos_alta_nao_alterar()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2444969);
            Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

            PlanoPosAlta plano = new PlanoPosAlta(usu, sum.Atendimento);
            plano.NomeComercial = "TESTE";
            sum.AddPlanoPosAlta(plano);
            sum.SemMedPosAlta = SimNao.Sim;

            Assert.AreEqual(sum.SemMedPosAlta, SimNao.Nao);

        }

        [TestMethod]
        public void insere_um_prestador_no_sumario_2444969()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2444969);

            Prestador prestador = ObjectFactory.GetInstance<IRepositorioDePrestadores>().OndeCodigoIgual(277).Single();
            sum.addPrestador(prestador);
            sum.addPrestador(prestador);

            serv.Save(sum);
            Assert.AreEqual(sum.Prestadores.Count(x=>x.Id == prestador.Id) , 1);
            Assert.IsTrue(sum.Prestadores.Count > 0);
            Assert.IsNotNull(sum.Prestadores.FirstOrDefault(x => x.Id == prestador.Id));

            sum.RemovePrestador(prestador);
            serv.Save(sum);
            Assert.IsNull(sum.Prestadores.FirstOrDefault(x => x.Id == prestador.Id));

        }

        [TestMethod]
        public void busca_meios_de_transporte()
        {
            IRepositorioDeMeioDeTransporte rep = ObjectFactory.GetInstance<IRepositorioDeMeioDeTransporte>();
            IList<MeioDeTransporte> lista = rep.List();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void insere_transferencia_no_sumario_de_alta_2444969()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2444969);

            MeioDeTransporte meio = ObjectFactory.GetInstance<IRepositorioDeMeioDeTransporte>().List().FirstOrDefault();
            Transferencia trans = new Transferencia(meio);
            trans.Oxigenio = SimNao.Sim;
            trans.Ventilacao = SimNao.Sim;
            trans.Motorizacao = SimNao.Sim;
            trans.AcompanhaMedico = SimNao.Sim;
            trans.ContatoInstituicao = "Dr. Luis Carlos";
            trans.NomeHospitalDestino = "HOSPITAL DE CLINICAS";
            trans.FrequenciaCardiaca = 100;
            trans.PressaoArterial = new PressaoArterial(100,20);
            trans.FrequenciaRespiratoria = 100;
            trans.UsuarioLeitoReserva = "Dr. fulano";
            trans.ExameFisicoSumario = "Normal";

            Cidade cidade = ObjectFactory.GetInstance<IRepositorioDeCidade>().OndeIdIgual(7951).Single();
            trans.Municipio = cidade;

            sum.Transferencia = trans;
             
            serv.Save(sum);
            Assert.IsNotNull(sum.Transferencia);
            Assert.AreEqual(sum.Transferencia.Oxigenio, SimNao.Sim);
            Assert.AreEqual(sum.Transferencia.PressaoArterial.Alta, 100);
            Assert.AreEqual(sum.Transferencia.PressaoArterial.Baixa, 20);
            Assert.AreEqual(sum.Transferencia.Municipio.Descricao, "PORTO ALEGRE");            
        }

        [TestMethod]
        public void exclui_dados_da_transferencia_no_sumario_de_alta_3278473()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(3278473);

            MeioDeTransporte meio = ObjectFactory.GetInstance<IRepositorioDeMeioDeTransporte>().List().FirstOrDefault();
            Transferencia trans = new Transferencia(meio);
            trans.Oxigenio = SimNao.Sim;
            trans.Ventilacao = SimNao.Sim;
            sum.Transferencia = trans;
            
            serv.Save(sum);
            Assert.IsNotNull(sum.Transferencia);

            sum.Transferencia = null;

            serv.Save(sum);
            Assert.IsNull(sum.Transferencia);

        }

        [TestMethod]
        public void altera_revisao_medica_ecaso_de_urgencia_no_sumario_de_alta_2846943()
        {
            ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
            SumarioAlta sum = serv.FiltraPorID(2846943);

            sum.RevisaoMedicaEm = "30 dias";
            sum.EmCasoDeUrgencia = "ligar para a mãe";

            serv.Save(sum);

            Assert.IsNotNull(retornaSumario(2846943).RevisaoMedicaEm);
            Assert.IsNotNull(retornaSumario(2846943).EmCasoDeUrgencia);

        }

        [TestMethod]
        public void busca_recomencoes_somente_ativas()
        {
            IRepositorioDeRecomendacao rep = ObjectFactory.GetInstance<IRepositorioDeRecomendacao>();
            IList<Recomendacao> lista = rep.List();
            Assert.AreEqual(lista.Count , 5);
        }

       [TestMethod]
        public void adiciona_recomendacaoes_padrao_no_sumario_de_alta_3281904()
       {
           ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
           SumarioAlta sum = serv.FiltraPorID(3281904);

           Recomendacao recomendacao = ObjectFactory.GetInstance<IRepositorioDeRecomendacao>().OndeIdIgual(2).Single();
           SumarioRecomendacao sumRec = new SumarioRecomendacao(recomendacao,"comer mais", sum.Atendimento);
           sum.AddRecomendacao(sumRec);
           sum.AddRecomendacao(sumRec);
           serv.Save(sum);

           Assert.AreEqual(retornaSumario(3281904).recomendacoes.Count(x => x.Recomendacao.Id == recomendacao.Id), 1);
           Assert.IsNotNull(retornaSumario(3281904).recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == recomendacao.Id));
           Assert.AreEqual(retornaSumario(3281904).recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == recomendacao.Id).Descricao, "comer mais");
           
           sum.RemoveRecomendacao(sumRec);
           serv.Save(sum);
           Assert.IsNull(retornaSumario(3281904).recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == recomendacao.Id));

       }

       [TestMethod]
       public void quando_adicinar_uma_recomendacao_setar_sem_recomendacao_para_nao_no_sumario_de_alta_2833249()
       {
           ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
           SumarioAlta sum = serv.FiltraPorID(2833249);

           Recomendacao recomendacao = ObjectFactory.GetInstance<IRepositorioDeRecomendacao>().OndeIdIgual(2).Single();
           SumarioRecomendacao sumRec = new SumarioRecomendacao(recomendacao, "comer mais", sum.Atendimento);
           sum.SemRecomendacao = SimNao.Sim;
           sum.AddRecomendacao(sumRec);
           serv.Save(sum);

           Assert.AreEqual(sum.SemRecomendacao, SimNao.Nao);

           sum.RemoveRecomendacao(sumRec);
           serv.Save(sum);
           Assert.IsNull(retornaSumario(2833249).recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == recomendacao.Id));

       }

       [TestMethod]
       public void não_deixar_alterar_para_sem_recomendacao_quando_existir_recomendacao_no_sumario_de_alta_2444969()
       {
           //Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2444969);
           //Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           //vmRecomendacao sum = new vmRecomendacao(ate, usu);
           
           

           //Recomendacao recomendacao = ObjectFactory.GetInstance<IRepositorioDeRecomendacao>().OndeIdIgual(2).Single();
           //SumarioRecomendacao sumRec = new SumarioRecomendacao(recomendacao, "comer mais");
           //sum.SumarioAlta.recomendacoes.Add(new wrpSumarioRecomendacao(sumRec));
           //sum.SumarioAlta.Save();

           //sum.SumarioAlta.SemRecomendacao = SimNao.Sim;
           //Assert.AreEqual(sum.SumarioAlta.SemRecomendacao, SimNao.Nao);

           //sum.SumarioAlta.recomendacoes.Remove(new wrpSumarioRecomendacao(sumRec));
           //sum.SumarioAlta.Save();
           //Assert.IsNull(retornaSumario(2444969).recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == recomendacao.Id));
       }

       //[TestMethod]
       //public void busca_sumario_nulo()
       //{           
       //    IAtendimentoService servAte = ObjectFactory.GetInstance<IAtendimentoService>();
       //    Atendimento ate = servAte.FiltraPorID(2845616);
       //    SumarioAlta sum = ate.SumarioAlta;
       //    Assert.IsNull(sum);
       //}       
       
       [TestMethod]
       public void valida_cid_selecionado_para_O_paciente_9013455_sexo_do_cid_diferente_do_paciente()
       {
           IPacienteService servPac = ObjectFactory.GetInstance<IPacienteService>();
           Paciente paciente = servPac.FiltraPorID(9013455);

           ICidService serv = ObjectFactory.GetInstance<ICidService>();
           Cid cid = serv.FiltraCIDMVPorId("C621").Cid;
           IList<InvalidValue> erros = cid.Valida(paciente);
           Assert.IsTrue(erros.Count > 0);
       }

       [TestMethod]
       public void valida_cid_selecionado_para_O_paciente_9013455_ocp_igual_a_nao()
       {
           IPacienteService servPac = ObjectFactory.GetInstance<IPacienteService>();
           Paciente paciente = servPac.FiltraPorID(9013455);

           ICidService serv = ObjectFactory.GetInstance<ICidService>();
           Cid cid = serv.FiltraCIDMVPorId("I25").Cid;
           IList<InvalidValue> erros = cid.Valida(paciente);
           Assert.IsTrue(erros.Count > 0);
       }

       [TestMethod]
       public void busca_cids_do_sexomasculino()
       {
           ICidService serv = ObjectFactory.GetInstance<ICidService>();
           IList<Cid> lista = serv.Carrega();
           serv.FiltraPorSexoDoPaciente(Sexo.Masculino);
           
           Assert.AreEqual(lista.Count(x => x.OPC == SimNao.Nao), 0);
           Assert.AreEqual(lista.Count(x => x.Sexo == SexoCid.Feminino), 699);
           Assert.IsTrue(lista.Count() > 0);

       }

       [TestMethod]
       [ExpectedException (typeof(BusinessValidatorException))]
       public void adiciona_cid_secundario_no_sumario_de_alta_retorna_um_erro()
       {
           ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
           SumarioAlta sum = serv.FiltraPorID(3279884);
           Cid cid = ObjectFactory.GetInstance<ICidService>().FiltraCIDMVPorId("I25").Cid;
           sum.AddCid(cid);
       }

       [TestMethod]
       public void adiciona_cid_secundario_no_sumario_de_alta()
       {
           ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
           SumarioAlta sum = serv.FiltraPorID(3281904);
           Cid cid = ObjectFactory.GetInstance<ICidService>().FiltraCIDMVPorId("J22").Cid;
           sum.AddCid(cid);
           serv.Save(sum);
           Assert.IsNotNull(sum.Cids.FirstOrDefault(x => x.Id == cid.Id));
           sum.removeCid(cid);
           serv.Save(sum);
           Assert.IsNull(sum.Cids.FirstOrDefault(x => x.Id == cid.Id));
       }

       [TestMethod]
       public void busca_data_de_alta_do_dia_seguinte_do_atendimento_2180906()
       {
           ISumarioAltaService serv = ObjectFactory.GetInstance<ISumarioAltaService>();
           SumarioAlta sum = serv.FiltraPorID(2180906);
           Assert.IsTrue(sum.DataAlta.HasValue);
           Assert.AreEqual(sum.MotivoAltaDiaSeguinte.Id, 12);
           Assert.IsFalse(sum.Atendimento.DataAltaMedica.HasValue);
       }

       [TestMethod]
       public void busca_os_produtos_item_de_prescricao_famarcos_do_sumario_1300075()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300075);
           IRepositorioDeProdutoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProdutoSumarioAlta>();
           rep.OndeProdudoIsItemDePrescricao(ate);
           IList<Produto> produtos = rep.List();
           Assert.AreEqual(produtos.Count , 22 );
       }

       [TestMethod]
       public void busca_os_produtos_componentes_de_prescricao_famarcos_do_sumario_1300075()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300075);
           IRepositorioDeProdutoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProdutoSumarioAlta>();
           rep.OndeProdudoIsComponenteDaPrescricao(ate);
           IList<Produto> produtos = rep.List();
           Assert.AreEqual(produtos.Count, 5);
       }

       [TestMethod]
       public void busca_os_produtos_famarcos_do_sumario_1300075()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300075);
           IProdutoConsult consult = ObjectFactory.GetInstance<IProdutoConsult>();
           IList<Produto> produtos = consult.carregaProdutoFarmaco(ate);
           Assert.AreEqual(produtos.Count, 25);
       }

       [TestMethod]
       public void valida_aba_exame_obrigatorio_marca_semParicularidade_ou_informar_um_exame()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2922443);
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrpSumarioAlta sum = new wrpSumarioAlta(ate, usu);
           Assert.AreNotEqual(sum.SemParticularidadeExames, SimNao.Nao);

           sum.SumarioExames.Remove(sum.SumarioExames.FirstOrDefault());

           IList<InvalidValue> erros = sum.ValidarExames();
           Assert.IsTrue(erros.Count == 0);
       }

       [TestMethod]
       public void valida_aba_exame_obrigatorio_marca_sem_exames_realizados_ou_informar_um_exame()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2880431);
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrpSumarioAlta sum = new wrpSumarioAlta(ate, usu);

           Assert.AreEqual(sum.SemExamesRealizados, SimNao.Sim);
           Assert.IsNull(sum.SemParticularidadeExames);
           Assert.AreEqual(sum.SumarioExames.Count, 0);
           
           sum.SemExamesRealizados = SimNao.Nao;

           IList<InvalidValue> erros = sum.ValidarExames();
           Assert.IsTrue(erros.Count > 0);

       }

       [TestMethod]
       public void não_permitir_alterar_SemExamesRealizados_se_existir_exames_realizados()
       {
       //    Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2880431);
       //    Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
       //    SumarioAlta sum = new SumarioAlta(ate, usu);

       //    Assert.AreEqual(sum.SemExamesRealizados, SimNao.Sim);
       //    Assert.IsNull(sum.SemParticularidadeExames);
       //    Assert.AreEqual(sum.SumarioExames.Count, 0);

       //    sum.AddSumarioExame(new SumarioExame(ObjectFactory.GetInstance<IRepositorioDeProcedimento>().OndeIdIgual("32050160").Single()));
           
       //    Assert.AreEqual(sum.SemExamesRealizados, SimNao.Nao);
           
       //    sum.SemExamesRealizados = SimNao.Sim;
       //    Assert.AreEqual(sum.SemExamesRealizados, SimNao.Nao);
       }

       [TestMethod]
       public void busca_lista_de_medicamentos_da_ultima_prescrisao_do_sumario_1300485()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(1300485);
           IPosAltaConsult iConsult = ObjectFactory.GetInstance<IPosAltaConsult>();
           IList<MedicamentoPosAltaDTO> lista = iConsult.carregaMedicametosUltimaPrescricao(ate);

           Assert.AreEqual(lista.Count, 3);
           MedicamentoPosAltaDTO dto = lista.FirstOrDefault(x => x.IdTipoPrescricao == 28308);
           Assert.AreEqual(dto.DescricaoProduto, "TYLEX 7,5MG CP");
           Assert.AreEqual(dto.Via, "VIA ORAL");
           Assert.AreEqual(dto.Frequencia, "4x/dia");
       }

       [TestMethod]
       public void busca_lista_de_medicamentos_prescritos_do_sumario_1300485()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2967667);
           IPosAltaConsult iConsult = ObjectFactory.GetInstance<IPosAltaConsult>();
           IList<MedicamentoPosAltaDTO> lista = iConsult.carregaMedicametosPrescritos(ate);

           Assert.AreEqual(lista.Count, 24);
           MedicamentoPosAltaDTO dto = lista.FirstOrDefault(x => x.IdTipoPrescricao == 31587);
           Assert.AreEqual(dto.DescricaoProduto, "NEOZINE 4% AD 20ML GTS");
       }

       [TestMethod]
       public void busca_os_medicamentos_possiveis_para_pos_alta()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2967667);
           IPosAltaConsult iConsult = ObjectFactory.GetInstance<IPosAltaConsult>();
           IList<MedicamentoPosAltaDTO> lista = iConsult.carregaMedicametos();

           //Assert.AreEqual(lista.Count, 3281);
           Assert.IsTrue(lista.Count(x => x.IdTipoPrescricao == null) > 0);
           Assert.IsTrue(lista.Count(x => x.IdProduto == null) > 0);
           Assert.AreEqual(lista.FirstOrDefault(x => x.IdProduto == 1462).DescricaoProduto, "PORTO SAUDE MIDAZOLAM 5MG/ML 3ML INJ (INATIVO)");
           Assert.AreEqual(lista.FirstOrDefault(x => x.IdTipoPrescricao == 43268).DescricaoProduto, "INFLAMENE (9MG/ML) 10ML GTS");
           Assert.IsNull(lista.FirstOrDefault(x => x.IdTipoPrescricao == 43268).IdProduto);
       }

       [TestMethod]
       public void busca_os_tipo_prescricao_medica_onde_são_medicamentos_da_prescricao()
       {
           IRepositorioDeTipoPrescricaoMedica repTipoPrescricao = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>();
           repTipoPrescricao.ondeMedicamentoDaPrescricao();
           IList<TipoPrescricaoMedica> listaTipoPrescricao = repTipoPrescricao.List();
           Assert.IsTrue(listaTipoPrescricao.Count > 0);
       }

    /*   [TestMethod]
       public void cria_um_sumario_novo_deve_busca_do_atendimento_o_prestador_para_ListaDeMedicos_do_seumario()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2925814);
           SumarioAlta sum = new SumarioAlta(ate, ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single());
           Assert.IsTrue(sum.Prestadores.Count > 0);
           Assert.IsNotNull(sum.Prestadores.FirstOrDefault(x=>x.Id == 627));
           IRepositorioDeSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
           rep.Save(sum);
           rep.Delete(sum);
       }*/

       [TestMethod]
       public void cria_um_novo_sumario_de_alta_2925815_e_exclui()
       {
           Atendimento ate = ObjectFactory.GetInstance<IAtendimentoService>().FiltraPorID(2925815);
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           SumarioAlta sum = new SumarioAlta(ate, usu);
           IRepositorioDeSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
           rep.Save(sum);
           Assert.IsTrue(sum.Atendimento.ID == 2925815);
           rep.Delete(sum);
       }

       [TestMethod]
       [ExpectedException (typeof(BusinessValidatorException))]
       public void realiza_alta_para_o_sumario_2925099_com_a_data_menor_que_a_data_de_internacao_retornar_um_erro()
       {

           IRepositorioDeSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
           SumarioAlta sum = rep.OndeCodigoAtendimentoIgual(2925099).Single();
           Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>().OndeIdIgual("48030023").Single();
           MotivoAlta motivo = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>().OndeIdIgual(72).Single();
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrpSumarioAlta wrp = new wrpSumarioAlta(sum);

           wrp.RealizaAltaInternado(new DateTime(), procedimento, /*procedimento.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS,*/motivo, null, sum.Atendimento.Cid, usu);

       }

       [TestMethod]
       [ExpectedException(typeof(BusinessValidatorException))]
       public void realiza_alta_dia_seguinte_para_o_sumario_2925099_com_a_data_menor_que_a_data_de_internacao_retornar_um_erro()
       {

           IRepositorioDeSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
           SumarioAlta sum = rep.OndeCodigoAtendimentoIgual(2925099).Single();
           Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>().OndeIdIgual("48030023").Single();
           MotivoAlta motivo = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>().OndeIdIgual(72).Single();
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrpSumarioAlta wrp = new wrpSumarioAlta(sum);

           wrp.RealizaAltaInternado(DateTime.Now.AddYears(-2), procedimento, /*procedimento.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS,*/ motivo, null, sum.Atendimento.Cid,usu);

       }        

       [TestMethod]
       [ExpectedException(typeof(BusinessValidatorException))]
       public void realiza_alta_para_o_sumario_2096121_sem_procedimento_sus()
       {
           IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
           Atendimento atendimento = repAte.OndeCodigoAtendimentoIgual(2096121).Single();
           atendimento.DataAltaMedica = null;
           repAte.Save(atendimento);
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();          
           wrpSumarioAlta wrp = new wrpSumarioAlta(atendimento.SumarioAlta);
           wrp.RealizaAltaInternado(DateTime.Now.Date, atendimento.ProcedimentoAMB, /*atendimento.ProcedimentoSUS,*/ atendimento.MotivoAlta, null, atendimento.Cid,usu);
                    
       }

       [TestMethod]
       public void realiza_alta_para_o_sumario_3005056()
       {
           IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
           Atendimento atendimento = repAte.OndeCodigoAtendimentoIgual(3005056).Single();
           atendimento.DataAltaMedica = null;
           repAte.Save(atendimento);

           Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>().OndeIdIgual("48030023").Single();

           var t = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>();
           var tt = t.List();

           MotivoAlta motivo = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>().OndeIdIgual(72).Single();
           ProcedimentoSUS sus = procedimento.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS;

           wrpSumarioAlta wrp = new wrpSumarioAlta(atendimento.SumarioAlta);
           wrp.SemFarmaco = SimNao.Sim;
           wrp.SemCausaExterna = SimNao.Sim;
           wrp.SemParticularidadeExames = SimNao.Sim;
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrp.RealizaAltaInternado(DateTime.Now.Date.AddDays(1), procedimento,/*sus,*/motivo, null, atendimento.Cid,usu);

           Assert.IsNotNull(wrp.DataAlta);

       }

       [TestMethod]
       [ExpectedException (typeof(BusinessValidatorException))]
       public void realiza_alta__para_um_sumario_ja_com_alta_1837122()
       {
           IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
           Atendimento atendimento = repAte.OndeCodigoAtendimentoIgual(1837122).Single();
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrpSumarioAlta wrp = new wrpSumarioAlta(atendimento.SumarioAlta);
           wrp.RealizaAltaInternado(DateTime.Now.Date, atendimento.ProcedimentoAMB, /*atendimento.ProcedimentoSUS,*/ atendimento.MotivoAlta, null, atendimento.Cid, usu);

       }  

       [TestMethod]
       [ExpectedException(typeof(BusinessValidatorException))]
       public void realiza_alta_sem_informar_as_recomendacoes_retornar_um_erro()
       {
           IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
           Atendimento atendimento = repAte.OndeCodigoAtendimentoIgual(2929453).Single();
           atendimento.DataAltaMedica = null;
           repAte.Save(atendimento);

           wrpSumarioAlta wrp = new wrpSumarioAlta(atendimento.SumarioAlta);

           Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>().OndeIdIgual("48030023").Single();
           MotivoAlta motivo = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>().OndeIdIgual(72).Single();
           ProcedimentoSUS sus = procedimento.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS;
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrp.RealizaAltaInternado(DateTime.Now.Date, procedimento, /*sus,*/ motivo, null, atendimento.Cid,usu);
       }

       [TestMethod]
       [ExpectedException(typeof(BusinessValidatorException))]
       public void realiza_alta_sem_informar_as_revisao_medica_retornar_um_erro()
       {
           IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
           Atendimento atendimento = repAte.OndeCodigoAtendimentoIgual(2929453).Single();
           atendimento.DataAltaMedica = null;
           repAte.Save(atendimento);

           wrpSumarioAlta wrp = new wrpSumarioAlta(atendimento.SumarioAlta);

           Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>().OndeIdIgual("48030023").Single();
           MotivoAlta motivo = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>().OndeIdIgual(72).Single();
           ProcedimentoSUS sus = procedimento.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS;
           wrp.SemRecomendacao = SimNao.Sim;
           wrp.RevisaoMedicaEm = String.Empty;
           Usuarios usu = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();
           wrp.RealizaAltaInternado(DateTime.Now.Date, procedimento, /*sus,*/ motivo, null, atendimento.Cid,usu);

       }

       [TestMethod]
       public void busca_parametro_dados_emergencia_rodape_sumario_de_alta()
       {
           IRepositorioDeParametrosInternet rep = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
           ParametroInternet par = rep.OndeDadosEmergenciaRodapeSumarioDeAlta().Single();
           Assert.IsNotNull(par.valor);
       }

       [TestMethod]
       public void busca_parametro_dados_endereco_rodape_sumario_de_alta()
       {
           IRepositorioDeParametrosInternet rep = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
           ParametroInternet par = rep.OndeDadosEnderecoRodapeSumarioDeAlta().Single();
           Assert.IsNotNull(par.valor);
       }

    }
}
