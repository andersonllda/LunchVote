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
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem;

namespace HMV.PEP.Test
{
    [TestClass]
    public class PlanoEducacionalTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void Verifica_conselho_prestador()
        {
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");

            Assert.IsNotNull(usu.Prestador.Conselho.ds_conselho);
        }

        [TestMethod]
        public void busca_Tipo_de_profissional()
        {
            IRepositorioDeTipoProfissional rep = ObjectFactory.GetInstance<IRepositorioDeTipoProfissional>();
            var lista = rep.List();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void Tipo_de_profissional_por_CRM()
        {
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
            var tipos = ObjectFactory.GetInstance<IRepositorioDeTipoProfissional>().OndeConselhoIgual(usu.Prestador.Conselho.ds_conselho).List();

            Assert.IsTrue(tipos.Count > 0);
        }

        [TestMethod]
        public void Carrega_grid_por_tipo_profissional()
        {
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
            var tipo = ObjectFactory.GetInstance<IRepositorioDeTipoProfissional>().OndeConselhoIgual(usu.Prestador.Conselho.ds_conselho).List().FirstOrDefault();

            IRepositorioDePerguntasEnfermagem rep = ObjectFactory.GetInstance<IRepositorioDePerguntasEnfermagem>();

            rep.OndeTipoPerguntaAtivo();
            rep.OndeIdTipoProfissionalIgualOuNull(tipo.Id);

            Assert.IsTrue(rep.List().Count > 0);
        }

        [TestMethod]
        public void Busca_Perguntas_Paciente_do_Atendimento_3219452()
        {
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3219452).Single();

            Assert.IsTrue(ate.PerguntasPaciente.Count == 3);
        }

        [TestMethod]
        public void Busca_exames_plano_educacional_3219452()
        {
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3219452).Single();
            var exas = ObjectFactory.GetInstance<IRepositorioDeContaHospitalar>().OndeAtendimentoIgual(ate).List();

            Assert.IsTrue(exas.Count() > 0);
        }

        [TestMethod]
        public void Busca_exames_Atendimento_3150938()
        {
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3150938).Single();

            Assert.IsTrue(ate.ExamesPaciente.Count() > 0);
        }

        [TestMethod]
        public void busca_os_procedimento_da_conta_hospitalar_no_atendimento_3150938()
        {
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3150938).Single();
            ContaHospitalar conta = ObjectFactory.GetInstance<IRepositorioDeContaHospitalar>().OndeAtendimentoIgual(ate).List().FirstOrDefault(x => x.ID == 80220238);
            Assert.AreEqual(conta.Itens.Count, 4);
            Assert.AreEqual(conta.Itens.Count(x => x.ProcedimentoMV.ID == "28314592"), 1);
        }

        [TestMethod]
        public void busca_os_procedimento_da_conta_ambulatorial_no_atendimento_1301372()
        {
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(1301372).Single();
            IList<ItensContaAmbulatorial> itens = ate.ContaAmbulatorial;
            Assert.AreEqual(itens.Count, 2);
            Assert.AreEqual(itens.First().ContaAmbulatorial.ID, 389);
            Assert.AreEqual(itens.Count(x => x.ProcedimentoMV.ID == "33010021"), 1);
        }

        [TestMethod]
        public void Busca_exames_plano_educacional_3150938()
        {
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3150938).Single();

            IRepositorioDeProcedimentoPlanoEducacional rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoPlanoEducacional>();
            rep.FiltraGrupoProcedimento(ate);

            IList<Procedimento> exas = rep.List();

            Assert.IsTrue(exas.Count() > 0);
        }

        [TestMethod]
        public void salva_plano_educacional_1301372()
        {
            wrpAtendimento ate =  new wrpAtendimento(ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3150938).Single());

            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");

            var perguntapac = new wrpPerguntasPaciente();
            perguntapac.Atendimento = ate;
            perguntapac.Usuario = new wrpUsuarios(usu);
            perguntapac.TipoProfissional = new wrpTipoProfissional(ObjectFactory.GetInstance<IRepositorioDeTipoProfissional>().OndeConselhoIgual(usu.Prestador.Conselho.ds_conselho).List().FirstOrDefault());

            IRepositorioDePerguntasEnfermagem rep = ObjectFactory.GetInstance<IRepositorioDePerguntasEnfermagem>();
            rep.OndeTipoPerguntaAtivo();
            rep.OndeIdTipoProfissionalIgualOuNull(perguntapac.TipoProfissional.Id);

            var perguntaenfermagem = rep.List().FirstOrDefault();

            wrpPerguntasPacienteResposta novo = new wrpPerguntasPacienteResposta();
            novo.PerguntasPaciente = perguntapac;
            novo.PerguntasEnfermagem = new wrpPerguntasEnfermagem(perguntaenfermagem);
            novo.Observacao = "TESTE";
            novo.Status = Status.Ativo;
            perguntapac.PerguntasPacienteResposta.Add(novo);

            rep = ObjectFactory.GetInstance<IRepositorioDePerguntasEnfermagem>();
            rep.OndeTipoPerguntaAtivo();
            rep.OndeIdTipoProfissionalIgualOuNull(6);
            perguntaenfermagem = rep.List().LastOrDefault();

            novo = new wrpPerguntasPacienteResposta();
            novo.PerguntasPaciente = perguntapac;
            novo.PerguntasEnfermagem = new wrpPerguntasEnfermagem(perguntaenfermagem);
            novo.Observacao = "TESTE2";
            novo.Status = Status.Ativo;
            perguntapac.PerguntasPacienteResposta.Add(novo);

            ate.PerguntasPaciente.Add(perguntapac);

            ate.Save();

            Assert.IsTrue(ate.PerguntasPaciente.Count > 1);
        }

        [TestMethod]
        public void salva_plano_educacional_dominio_1301372()
        {
            var rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento ate = rep.OndeCodigoAtendimentoIgual(3150938).Single();
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");

            
            var pergunta = new Core.Domain.Model.PEP.ProcessoDeEnfermagem.PerguntasPaciente(){
                Atendimento = ate,
                TipoProfissional = ObjectFactory.GetInstance<IRepositorioDeTipoProfissional>().OndeConselhoIgual(usu.Prestador.Conselho.ds_conselho).List().FirstOrDefault(),
                Usuario = usu                
            };

            ate.PerguntasPaciente.Add(pergunta);
            rep.Save(ate);

            Assert.IsTrue(ate.PerguntasPaciente.Count(x=>x.Id == pergunta.Id) == 1);

            //IRepositorioDePerguntasEnfermagem repEnf = ObjectFactory.GetInstance<IRepositorioDePerguntasEnfermagem>()
            //    .OndeTipoPerguntaAtivo()
            //    .OndeIdTipoProfissionalIgualOuNull(pergunta.TipoProfissional.Id);

            //var resp = new PerguntasPacienteResposta()
            //{
            //    Observacao = "TESTE",
            //    Status = Status.Ativo,
            //    PerguntasPaciente = pergunta,
            //    PerguntasEnfermagem = repEnf.List().FirstOrDefault()
            //};

            //pergunta.PerguntasPacienteResposta = new List<PerguntasPacienteResposta>();

            //pergunta.PerguntasPacienteResposta.Add(resp);
            //rep.Save(ate);

            //Assert.AreEqual(ate.PerguntasPaciente.FirstOrDefault(x=>x.Id == pergunta.Id).PerguntasPacienteResposta.Count, 1);
        }

        [TestMethod]
        public void salva_plano_educacional_exames()
        {
            Procedimento procedimento = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>().OndeIdIgual("15020037").Single();
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3878664).Single();
            IRepositorioDePerguntasPaciente rep = ObjectFactory.GetInstance<IRepositorioDePerguntasPaciente>();
            var plan = rep.OndeIdIgual(63028).Single();

            plan.ExamesPaciente.Add(new ExamesPaciente { Atendimento = ate, Observacao = "teste", PerguntasPaciente = plan, Procedimento = procedimento, Quantidade = 99, Usuario = usu });

            rep.Save(plan);

            Assert.IsTrue(plan.Id > 0);
        }
    }
}
