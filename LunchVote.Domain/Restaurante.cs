using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain
{
    public class Restaurante : DomainBase
    {
        public Restaurante() { }

        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}
