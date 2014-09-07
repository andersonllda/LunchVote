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
        public Votacao() { }

        public int Id { get; set; }
        public DateTime Data { get; set; }
        public IList<Voto> Votos { get; }
    }
}
