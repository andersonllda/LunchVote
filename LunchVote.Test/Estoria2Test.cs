using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using LunchVote.Domain.Repository;
using LunchVote.Domain;
using System.Collections.Generic;
using LunchVote.Core;
using System.Linq;

namespace LunchVote.Test
{
    [TestClass]
    public class Estoria2Test : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void ProfissionalFacilitadorTestVotacao()
        {
            var prof = BaseTestClass.ProfissionalFacilitador;
            var votacao = new Votacao() { Profissional = prof, Data = DateTime.Now.Date };
            IRepositoryRestaurante repres = ObjectFactory.GetInstance<IRepositoryRestaurante>();            
            votacao.Restaurantes.Add(repres.OndeIdIgual(1).Single());
            votacao.Restaurantes.Add(repres.OndeIdIgual(2).Single());
            votacao.Restaurantes.Add(repres.OndeIdIgual(3).Single());

            IRepositoryVotacao repvota = ObjectFactory.GetInstance<IRepositoryVotacao>();
            repvota.Save(votacao);

            Assert.IsTrue(repvota.List().Contains(votacao));
        }

        [TestMethod]
        public void ProfissionalFacilitadorTestVotacaoSemRepetirRestauranteSemana()
        {
            var prof = BaseTestClass.ProfissionalFacilitador;
            var votacao = new Votacao() { Profissional = prof, Data = DateTime.Now.Date };
            IRepositoryRestaurante repres = ObjectFactory.GetInstance<IRepositoryRestaurante>();
            
            Random r = new Random();
            List<Restaurante> restaurantesganhadores = new List<Restaurante>();
            var diasdasemana = DateTime.Now.Date.GetDaysInWeek(DayOfWeek.Monday).Take(5).ToList();
            foreach (var dia in diasdasemana)
            {
                var votacaoant = ObjectFactory.GetInstance<IRepositoryVotacao>().OndeDataIgual(dia).Single();
                votacaoant.RestauranteMaisVotado = ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(1, 20)).Single();
                restaurantesganhadores.Add(votacaoant.RestauranteMaisVotado);
            }
            //adiciona apenas os restaurantes que não foram mais votados durante a semana.
            while (votacao.Restaurantes.Count() < 3)
            {
                var rest = ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(1, 20)).Single();
                if (restaurantesganhadores.Count(x => x.Id == rest.Id) == 0)
                    votacao.Restaurantes.Add(rest);
                else
                    Console.Write("Restaurante " + rest.Descricao + " já foi selecionado na semana!\r\n");
            }          

            IRepositoryVotacao repvota = ObjectFactory.GetInstance<IRepositoryVotacao>();
            repvota.Save(votacao);

            Assert.IsTrue(repvota.List().Contains(votacao));
        }        
    }
}
