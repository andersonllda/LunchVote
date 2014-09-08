using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using LunchVote.Domain.Repository;
using LunchVote.Domain;
using System.Linq;

namespace LunchVote.Test
{
    [TestClass]
    public class Estoria1Test : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void ProfissionalFamintoTestVoto()
        {
            var prof = BaseTestClass.ProfissionalFaminto;
            var votacao = ObjectFactory.GetInstance<IRepositoryVotacao>().Single();
            var restaurantes = ObjectFactory.GetInstance<IRepositoryRestaurante>().List();
            for (int i = 0; i <= 2; i++)
                votacao.Restaurantes.Add(restaurantes[i]);
            
            var restaurantevotado = votacao.Restaurantes.FirstOrDefault();
            var voto = new Voto() { Profissional = prof, Restaurante = restaurantevotado, Votacao = votacao };

            IRepositoryVoto rep = ObjectFactory.GetInstance<IRepositoryVoto>();
            rep.Save(voto);

            Assert.IsTrue(rep.List().Contains(voto));
        }

        [TestMethod]
        public void ProfissionalFamintoTestVotoIgual()
        {
            var prof = BaseTestClass.ProfissionalFaminto;
            var votacao = ObjectFactory.GetInstance<IRepositoryVotacao>().Single();
            var restaurantes = ObjectFactory.GetInstance<IRepositoryRestaurante>().List();
            for (int i = 0; i <= 2; i++)
                votacao.Restaurantes.Add(restaurantes[i]);

            var restaurantevotado = votacao.Restaurantes.FirstOrDefault();
            var voto = new Voto() { Profissional = prof, Restaurante = restaurantevotado, Votacao = votacao };

            IRepositoryVoto rep = ObjectFactory.GetInstance<IRepositoryVoto>();
            rep.Save(voto);
            votacao.Votos.Add(voto);
            IRepositoryVotacao repv = ObjectFactory.GetInstance<IRepositoryVotacao>();
            repv.Save(votacao);

            restaurantevotado = votacao.Restaurantes.LastOrDefault();
            voto = new Voto() { Profissional = prof, Restaurante = restaurantevotado, Votacao = votacao };

            if (votacao.Votos.Count(x => x.Profissional == voto.Profissional) > 0)
                Console.Write("O Profissional já votou!");
            Assert.IsTrue(votacao.Votos.Count(x => x.Profissional == voto.Profissional) > 0);
        }
    }
}
