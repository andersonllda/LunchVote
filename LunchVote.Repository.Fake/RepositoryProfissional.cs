using LunchVote.Domain;
using LunchVote.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Repository.Fake
{
    public class RepositoryProfissional : RepositoryFilter<Profissional>, IRepositoryProfissional
    {
        public RepositoryProfissional()
            : base()
        {
        }      

        public IRepositoryProfissional OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.Id == id).ToList();
            return this;
        }

        public IRepositoryProfissional OndeIsFacilitador()
        {
            _list = _list.Where(x => x.IsFacilitador == true).ToList();
            return this;
        }
    }
}
