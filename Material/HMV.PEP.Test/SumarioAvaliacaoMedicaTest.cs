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
using HMV.Core.Interfaces;
using System.Configuration;
using HMV.Core.Domain.Enum;
using HMV.PEP.DTO;
using HMV.Core.DTO;
using HMV.PEP.Consult;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SumarioAvaliacaoMedicaTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_sumario_258911()
        {
            ISumarioAvaliacaoMedicaService serv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
            SumarioAvaliacaoMedica sumario = serv.FiltraPorID(258911);
            Assert.IsTrue(sumario.ID == 258911);
        }

        // ver 
        //[TestMethod]
        //public void busca_sumarios_por_tipo_adulto()
        //{
        //    ISumarioAvaliacaoMedicaService serv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
        //    List<SumarioAvaliacaoMedica> sumarios = serv.FiltraPorTipo(TipoPaciente.Adulto).Take(1).ToList();

        //    SumarioAvaliacaoMedica sumario = sumarios.First();

        //    Assert.IsTrue(sumario != null);
        //}

        [TestMethod]
        public void busca_lista_revisoes_de_sistemas()
        {
            SumarioAvaliacaoMedica sumario = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>().FiltraPorID(258911);

            IItensSumarioAvaliacaoMedicaService srv = ObjectFactory.GetInstance<IItensSumarioAvaliacaoMedicaService>();

            IList<ItensSumarioAvaliacaoMedica> lst = srv.CarregaRevisaoDeSistemas(sumario.Tipo.ID);

            RevisaoDeSistemasPEP revisao = new RevisaoDeSistemasPEP(sumario);

            IList<SumarioAvaliacaoMedicaItensDetalheDTO> revisoes = sumario.RevisaoDeSistemas.ItensRevisaoDeSistemas;

            revisoes.Add(new SumarioAvaliacaoMedicaItensDetalheDTO { ID = 6, Observacoes = string.Empty, SemParticularidades = true });

            var ret = (from x in lst
                       join y in revisoes on x.CodItemDetalhe equals y.ID into w
                       from z in w.DefaultIfEmpty()
                       select new SumarioAvaliacaoMedicaItensDetalheDTO()
                        {
                            ID = x.CodItemDetalhe,
                            Descricao = x.Descricao,
                            Observacoes = z != null ? z.Observacoes : String.Empty,
                            SemParticularidades = z != null && z.SemParticularidades == true ? true : false
                        }).ToList();

            Assert.AreEqual(ret.Count, 18);
            Assert.AreEqual(ret.Where(x => x.SemParticularidades).Count(), 1);
        }

        [TestMethod]
        public void Salva_sumario()
        {
            SumarioAvaliacaoMedica sumario = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>().FiltraPorID(258911);

            sumario.ExamesRealizados.NaoForamRealizadosExames = SimNao.Nao;
            sumario.ExamesRealizados.Descricao = "teste";

            ISumarioAvaliacaoMedicaService srv = ObjectFactory.GetInstance<ISumarioAvaliacaoMedicaService>();
            srv.Save(sumario);
        }


        [TestMethod]
        public void TestaConsult_sumarioweb()
        {
            Usuarios usu = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
            ISumarioDeAvaliacaoMedicaConsult consult = ObjectFactory.GetInstance<ISumarioDeAvaliacaoMedicaConsult>();
            var teste = consult.carregaSumarioAvaliacoesWeb(usu);

            Assert.IsTrue(teste.Count > 0);
        }

        [TestMethod]
        public void carrega_sumario_avaliacao_medica_598474()
        {
            IRepositorioDeSumariosAvaliacaoMedicaFechados rep = ObjectFactory.GetInstance<IRepositorioDeSumariosAvaliacaoMedicaFechados>();

            SumarioAvaliacaoMedica sumario = rep.OndeIDdoSumarioIgual(598474).Single();

            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.HistoriaDoencaAtual));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.QueixaPrincipal));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.RevisaoDeSistemas.Outros));
            //Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.MedicamentosUso));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.AlergiasAmbulatorio));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.HistoriaPregressa.Outros));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.HistoriaFamiliar.Outros));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.PerfilPsicoSocial.Outros));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.ExameFisico.Observacoes));
            Assert.IsNotNull(sumario.Diagnosticos);
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.PlanoDiagnosticoExamesSolicitados));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.PlanoDiagnosticoConduta));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.NotasPessoaisMedico));
        }

        [TestMethod]
        public void carrega_sumario_avaliacao_medica_598481()
        {
            IRepositorioDeSumariosAvaliacaoMedicaFechados rep = ObjectFactory.GetInstance<IRepositorioDeSumariosAvaliacaoMedicaFechados>();

            SumarioAvaliacaoMedica sumario = rep.OndeIDdoSumarioIgual(598481).Single();

            Assert.IsTrue(sumario.Diagnosticos.Count > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.PlanoDiagnosticoExamesSolicitados));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.PlanoDiagnosticoConduta));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.NotasPessoaisMedico));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.Subjetivo));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.Objetivo));
            Assert.IsFalse(string.IsNullOrWhiteSpace(sumario.Impressao));
        }


        [TestMethod]
        public void carrega_eventos_lixo()
        {
            IRepositorioDeEvento rep = ObjectFactory.GetInstance<IRepositorioDeEvento>();
            Evento evento = rep.List().FirstOrDefault(x => x.ID == 1);

            Assert.IsNotNull(evento);
        }

       
    }
}
