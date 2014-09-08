using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain.Repository
{
    public interface IRepositoryRestaurante : IRepositoryFilter<Restaurante>
    {
        IRepositoryRestaurante OndeIdIgual(int id);
    }
}
