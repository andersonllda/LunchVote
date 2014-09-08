using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain.Repository
{
    public interface IRepositoryVoto : IRepositoryBase<Voto>
    {
        IRepositoryVoto OndeIdIgual(int id);
        IRepositoryVoto OndeIdVotacaoIgual(int votacaoid);
    }
}
