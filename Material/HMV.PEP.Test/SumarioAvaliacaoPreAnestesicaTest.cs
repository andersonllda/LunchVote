using System;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.ObjectWrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoPreAnestesica;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SumarioAvaliacaoPreAnestesicaTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void Busca_tipos_avaliacao_pre_anestesica()
        {
            IRepositorioDeAnestesiaTipo rep = ObjectFactory.GetInstance<IRepositorioDeAnestesiaTipo>();
            var tipoanestesiasemwrapper = rep.OndeIDIgual((int)AnestesiaTipos.PreAnestesica).Single();
            var tipoanestesia = new wrpAnestesiaTipo(rep.OndeIDIgual((int)AnestesiaTipos.PreAnestesica).Single());
        }


        [TestMethod]
        public void Busca_tipos_avaliacao_pre_anestesica_itens()
        {
            IRepositorioDeAnestesiaTipo rep = ObjectFactory.GetInstance<IRepositorioDeAnestesiaTipo>();
            AnestesiaTipo tp =  rep.OndeIDIgual((int)AnestesiaTipos.PreAnestesica).Single();
            Assert.IsTrue(tp.AnestesiaGrupos.FirstOrDefault().AnestesiaItens.Count > 0);
        }

        [TestMethod]
        public void Busca_avisos_cirurgias()
        {
            IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();

            var teste = repav.OndeCodigoPacienteIgual(9969607).List();

            var avcir = repav.OndeCodigoPacienteIgual(9969607).List()
                .Where(x => x.Situacao == SituacaoAviso.Agendada
                        && x.AgendaCirurgias.Count(y => y.DataInicio >= DateTime.Today.AddDays(-1) && y.DataInicio <= DateTime.Today.AddDays(1)) > 0).ToList();
            if (avcir.Count == 0)
                avcir = repav.OndeCodigoPacienteIgual(9969607).List()
                .Where(x => x.Situacao == SituacaoAviso.Agendada && x.AgendaCirurgias.Count(y => y.DataInicio >= DateTime.Today) > 0).ToList();
        }
    }
}
