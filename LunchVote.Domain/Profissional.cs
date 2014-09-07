using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain
{
    public class Profissional : DomainBase
    {
        public Profissional() { }
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool IsFacilitador { get; set; }
    }
}
