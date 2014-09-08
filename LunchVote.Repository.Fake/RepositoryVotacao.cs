using LunchVote.Domain;
using LunchVote.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Repository.Fake
{
    public class RepositoryVotacao : RepositoryBase<Votacao>, IRepositoryVotacao
    {
        public RepositoryVotacao()
            : base()
        {
        }

        public IRepositoryVotacao OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.Id == id).ToList();
            return this;
        }

        public IRepositoryVotacao OndeDataIgual(DateTime data)
        {
            _list = _list.Where(x => x.Data == data).ToList();
            return this;
        }

        public IRepositoryVotacao OndePeriodoIgual(DateTime datainicial, DateTime datafinal)
        {
            _list = _list.Where(x => x.Data >= datainicial && x.Data <= datafinal).ToList();
            return this;
        }
    }
}
