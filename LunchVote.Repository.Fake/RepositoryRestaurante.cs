using LunchVote.Domain;
using LunchVote.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Repository.Fake
{
    public class RepositoryRestaurante : RepositoryFilter<Restaurante>, IRepositoryRestaurante
    {
        public RepositoryRestaurante()
            : base()
        {
        }

        public IRepositoryRestaurante OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.Id == id).ToList();
            return this;
        }      
    }
}
