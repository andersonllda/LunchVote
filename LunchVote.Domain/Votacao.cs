using LunchVote.Core.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain
{
    public class Votacao : DomainBase, IAggregateRoot
    {
        public Votacao() 
        {
            this.Restaurantes = new List<Restaurante>();
            this.Votos = new List<Voto>();
        }

        public int Id { get; set; }
        public DateTime Data { get; set; }        
        public IList<Restaurante> Restaurantes { get; set; }
        public IList<Voto> Votos { get; set; }
        public Profissional Profissional { get; set; }

        public Restaurante RestauranteMaisVotado { get; set; }        
    }
}
