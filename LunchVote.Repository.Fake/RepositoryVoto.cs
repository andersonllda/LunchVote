using LunchVote.Domain;
using LunchVote.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Repository.Fake
{
    public class RepositoryVoto : RepositoryBase<Voto>, IRepositoryVoto
    {
        public RepositoryVoto()
            : base()
        {
        }

        public IRepositoryVoto OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.Id == id).ToList();
            return this;
        }

        public IRepositoryVoto OndeIdVotacaoIgual(int votacaoid)
        {
            _list = _list.Where(x => x.Votacao.Id == votacaoid).ToList();
            return this;
        }
    }
}
