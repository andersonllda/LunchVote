using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using LunchVote.Domain.Repository;
using System.Linq;
using LunchVote.Domain;
using System.Collections.Generic;

namespace LunchVote.Test
{
    [TestClass]
    public class Estoria3Test : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void ProfissionalFamintoTestVerificaVotos()
        {
            var votacao = ObjectFactory.GetInstance<IRepositoryVotacao>().OndeDataIgual(DateTime.Now.Date).Single();
            votacao.Profissional = ObjectFactory.GetInstance<IRepositoryProfissional>().OndeIsFacilitador().List().FirstOrDefault();
            var restaurantes = ObjectFactory.GetInstance<IRepositoryRestaurante>().List().Take(3).ToList();
            votacao.Restaurantes = restaurantes;
            Random r = new Random();
            for (int i = 0; i <= 29; i++)
            {                
                var rep = ObjectFactory.GetInstance<IRepositoryVoto>();
                var v = new Voto() { Id = i, Profissional = BaseTestClass.ProfissionalFaminto };
                v.Votacao = votacao;
                var id = r.Next(0, 3);
                v.Restaurante = restaurantes[id];
                rep.Save(v);
                votacao.Votos.Add(v);
            }
            var votos = votacao.Votos;         
            var counts = votos.GroupBy(x => x.Restaurante)
                                      .ToDictionary(g => g.Key,
                                                    g => g.Count()).OrderByDescending(x=> x.Value);            
            var maisvotado = counts.OrderByDescending(x=> x.Value).FirstOrDefault().Key;
            Console.Write("Resultado da Votação: \r\n");
            Console.Write("Restaurante          Votos\r\n");
            foreach (var item in counts)
                Console.Write(item.Key.Descricao + "            " + item.Value + "\r\n");
            
            Console.Write("Mais Votado = " + maisvotado.Descricao);
            Assert.IsNotNull(maisvotado);
        }
    }
}
