using LunchVote.Core.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain
{
    public class Voto : DomainBase, IAggregateRoot
    {
        public Voto() 
        { 
            
        }
        
        public int Id { get; set; }
        public Votacao Votacao { get; set; }
        public Profissional Profissional { get; set; }
        public Restaurante Restaurante { get; set; }
    }
}
