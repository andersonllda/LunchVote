using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain.Repository
{
    public interface IRepositoryVotacao : IRepositoryBase<Votacao>
    {
        IRepositoryVotacao OndeIdIgual(int id);
        IRepositoryVotacao OndeDataIgual(DateTime data);
        IRepositoryVotacao OndePeriodoIgual(DateTime datainicial, DateTime datafinal);
    }
}
